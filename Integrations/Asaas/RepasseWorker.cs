using ImobAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Integrations.Asaas
{
    public class RepasseWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<RepasseWorker> _logger;
        public RepasseWorker(IServiceScopeFactory scopes, ILogger<RepasseWorker> logger) { _scopes = scopes; _logger = logger; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!await ExecutarProximoAsync(stoppingToken)) await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no processador de repasses; nova tentativa em 10 segundos.");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task<bool> ExecutarProximoAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopes.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ImobContext>();
            var service = scope.ServiceProvider.GetRequiredService<RepasseService>();
            await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
            await context.Database.ExecuteSqlRawAsync("DECLARE @result int; EXEC @result = sp_getapplock @Resource = 'ImobAPI.AsaasWebhookProcessor', @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; IF @result < 0 THROW 51000, 'Nao foi possivel obter lock de repasse.', 1;", cancellationToken);
            var repasse = await context.Repasses.Where(r => r.Status == "NA_FILA")
                .Where(r => !context.Repasses.Any(a => a.CobrancaId == r.CobrancaId && a.Id < r.Id && a.Status == "ENVIANDO"))
                .Where(r => !context.Cobrancas.Any(c => c.Id == r.CobrancaId && context.EventosWebhookAsaas.Any(e => e.IdCobrancaAsaas == c.IdCobrancaAsaas && e.ProcessadoEmUtc == null)))
                .OrderBy(r => r.Id).FirstOrDefaultAsync(cancellationToken);
            if (repasse == null) { await tx.CommitAsync(cancellationToken); return false; }
            var cobranca = await context.Cobrancas.FirstAsync(c => c.Id == repasse.CobrancaId, cancellationToken);
            var eventosPendentes = await context.EventosWebhookAsaas.AnyAsync(e => e.IdCobrancaAsaas == cobranca.IdCobrancaAsaas && e.ProcessadoEmUtc == null, cancellationToken);
            if (eventosPendentes) { await tx.CommitAsync(cancellationToken); await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken); return false; }
            var anteriorEmEnvio = await context.Repasses.AnyAsync(r => r.CobrancaId == repasse.CobrancaId && r.Id < repasse.Id && r.Status == "ENVIANDO", cancellationToken);
            if (anteriorEmEnvio) { await tx.CommitAsync(cancellationToken); return false; }
            var anteriorProblematico = await context.Repasses.AnyAsync(r => r.CobrancaId == repasse.CobrancaId && r.Id < repasse.Id &&
                (r.Status == "INCERTO" || r.Status == "BLOQUEADO" || r.Status == "REJEITADO" || r.Status == "FAILED" || r.Status == "CANCELLED"), cancellationToken);
            var estorno = await RepasseConfiguracao.TemEstornoOuContestacaoAsync(context, cobranca.IdCobrancaAsaas, cancellationToken);
            if (!cobranca.Ativo || !cobranca.Pago || cobranca.Status != "RECEIVED" || anteriorProblematico || estorno)
            {
                repasse.Status = "BLOQUEADO";
                repasse.Erro = anteriorProblematico ? "Repasse anterior precisa de conciliação; transferência não enviada." : estorno ? "Cobrança possui evento de estorno ou contestação; transferência não enviada." : "Cobrança não está ativa e recebida; transferência não enviada.";
            }
            else repasse.Status = "ENVIANDO";
            repasse.AtualizadoEmUtc = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            if (repasse.Status == "ENVIANDO") await service.ProcessarAsync(repasse, cancellationToken);
            return true;
        }
    }
}
