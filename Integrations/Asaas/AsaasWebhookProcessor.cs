using System.Globalization;
using System.Text.Json;
using ImobAPI.Context;
using ImobAPI.Entities;
using ImobAPI.Integrations.Asaas.Models;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Integrations.Asaas
{
    public class AsaasWebhookProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AsaasWebhookProcessor> _logger;

        public AsaasWebhookProcessor(IServiceScopeFactory scopeFactory, ILogger<AsaasWebhookProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!await ProcessarProximoAsync(stoppingToken))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar eventos do webhook Asaas; nova tentativa em 10 segundos.");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task<bool> ProcessarProximoAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ImobContext>();
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            // Serialize workers across application instances; the lock is released with the transaction.
            await context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = 'ImobAPI.AsaasWebhookProcessor', @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; IF @result < 0 THROW 51000, 'Nao foi possivel obter o lock do processador Asaas.', 1;",
                cancellationToken);

            var agora = DateTime.UtcNow;
            var evento = await context.EventosWebhookAsaas
                .Where(e => e.ProcessadoEmUtc == null && (e.ProximaTentativaEmUtc == null || e.ProximaTentativaEmUtc <= agora))
                .Where(e => e.IdCobrancaAsaas == null || !context.EventosWebhookAsaas.Any(anterior =>
                    anterior.IdCobrancaAsaas == e.IdCobrancaAsaas && anterior.Id < e.Id && anterior.ProcessadoEmUtc == null))
                .OrderBy(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (evento == null)
            {
                await transaction.CommitAsync(cancellationToken);
                return false;
            }

            if (!evento.Tipo.StartsWith("PAYMENT_", StringComparison.Ordinal))
            {
                _logger.LogInformation("Evento Asaas ignorado: {Tipo}, ID {IdEvento}.", evento.Tipo, evento.IdEventoAsaas);
            }
            else
            {
                var cobranca = await context.Cobrancas.FirstOrDefaultAsync(c => c.IdCobrancaAsaas == evento.IdCobrancaAsaas, cancellationToken);
                if (cobranca == null)
                {
                    evento.ProximaTentativaEmUtc = agora.AddMinutes(1);
                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    _logger.LogWarning("Cobrança Asaas {IdCobranca} ainda não encontrada para o evento {IdEvento}; será feita nova tentativa.", evento.IdCobrancaAsaas, evento.IdEventoAsaas);
                    return true;
                }

                var request = JsonSerializer.Deserialize<AsaasWebhookRequest>(evento.Payload)
                    ?? throw new InvalidOperationException($"Payload inválido no evento Asaas {evento.IdEventoAsaas}.");
                AtualizarCobranca(cobranca, request);
            }

            evento.ProcessadoEmUtc = DateTime.UtcNow;
            evento.ProximaTentativaEmUtc = null;
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        private static void AtualizarCobranca(Cobranca cobranca, AsaasWebhookRequest request)
        {
            var payment = request.Payment ?? throw new InvalidOperationException("Evento de cobrança sem payment.");
            cobranca.Status = request.Event == "PAYMENT_DELETED" ? "DELETED" : payment.Status ?? cobranca.Status;
            cobranca.LinkBoleto = payment.BankSlipUrl ?? payment.InvoiceUrl ?? cobranca.LinkBoleto;
            cobranca.NossoNumero = payment.NossoNumero ?? cobranca.NossoNumero;
            if (payment.NetValue.HasValue)
            {
                cobranca.valorLiquido = payment.NetValue.Value;
            }

            if (request.Event == "PAYMENT_CONFIRMED" || request.Event == "PAYMENT_RECEIVED")
            {
                cobranca.Pago = true;
                var dataPagamento = payment.PaymentDate ?? payment.ClientPaymentDate ?? payment.ConfirmedDate;
                if (DateTime.TryParseExact(dataPagamento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
                {
                    cobranca.DataPagamento = data;
                }
            }
            else if (request.Event == "PAYMENT_REFUNDED" || request.Event == "PAYMENT_RECEIVED_IN_CASH_UNDONE")
            {
                cobranca.Pago = false;
                cobranca.DataPagamento = null;
            }

            if (request.Event == "PAYMENT_DELETED")
            {
                cobranca.Ativo = false;
                cobranca.DataInativacao ??= DateTime.Now;
            }
            else if (request.Event == "PAYMENT_RESTORED")
            {
                cobranca.Ativo = true;
                cobranca.DataInativacao = null;
            }

            cobranca.DataAtualizacao = DateTime.Now;
        }
    }
}
