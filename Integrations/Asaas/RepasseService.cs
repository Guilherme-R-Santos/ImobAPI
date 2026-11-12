using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImobAPI.Context;
using ImobAPI.Entities;
using ImobAPI.Integrations.Asaas.Models;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Integrations.Asaas
{
    public class RepasseService
    {
        private readonly ImobContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clients;
        private readonly ILogger<RepasseService> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public RepasseService(ImobContext context, IConfiguration configuration, IHttpClientFactory clients, ILogger<RepasseService> logger)
        {
            _context = context;
            _configuration = configuration;
            _clients = clients;
            _logger = logger;
        }

        public bool Habilitado()
        {
            return _configuration.GetValue<bool>("Asaas:RepassesHabilitados") &&
                Uri.TryCreate(_configuration["Asaas:BaseUrl"], UriKind.Absolute, out var uri) &&
                uri.Scheme == Uri.UriSchemeHttps && uri.IsDefaultPort &&
                ((uri.Host.Equals("api-sandbox.asaas.com", StringComparison.OrdinalIgnoreCase) && uri.AbsolutePath.TrimEnd('/') == "/v3") ||
                 (uri.Host.Equals("sandbox.asaas.com", StringComparison.OrdinalIgnoreCase) && uri.AbsolutePath.TrimEnd('/') == "/api/v3")) &&
                string.IsNullOrEmpty(uri.UserInfo) && string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment) &&
                !string.IsNullOrWhiteSpace(_configuration["Asaas:ApiKey"]);
        }

        public static string ValidarValores(ContaRepasse primeira, ContaRepasse segunda, decimal liquido, out decimal valor1, out decimal valor2)
        {
            valor1 = valor2 = 0;
            var erro = RepasseConfiguracao.ValidarContas(primeira, segunda);
            if (erro != null) return erro;
            if (primeira == null && segunda == null) return "Informe ao menos um destinatário.";
            if (liquido <= 0 || decimal.Round(liquido, 2) != liquido || liquido > 99999999999999.99m) return "Valor líquido inválido ou indisponível.";
            valor1 = Calcular(primeira, liquido);
            valor2 = Calcular(segunda, liquido);
            if ((primeira != null && valor1 <= 0) || (segunda != null && valor2 <= 0)) return "O valor calculado deve ser de pelo menos um centavo por destinatário.";
            if (valor1 + valor2 > liquido) return "A soma dos repasses excede o valor líquido da cobrança.";
            return null;
        }

        private static decimal Calcular(ContaRepasse conta, decimal liquido) => conta == null ? 0m :
            conta.TipoValor == "FIXO" ? conta.Valor : decimal.Round(liquido * conta.Valor / 100m, 2, MidpointRounding.AwayFromZero);

        public void Reservar(Cobranca cobranca, ContaRepasse primeira, ContaRepasse segunda, decimal liquido, string origem, int? usuarioId)
        {
            var erro = ValidarValores(primeira, segunda, liquido, out var valor1, out var valor2);
            if (erro != null) throw new InvalidOperationException(erro);
            if (primeira != null) Adicionar(cobranca.Id, 1, primeira, valor1, origem, usuarioId);
            if (segunda != null) Adicionar(cobranca.Id, 2, segunda, valor2, origem, usuarioId);
        }

        private void Adicionar(int cobrancaId, int numero, ContaRepasse conta, decimal valor, string origem, int? usuarioId)
        {
            _context.Repasses.Add(new Repasse { CobrancaId = cobrancaId, Destinatario = numero, Origem = origem, Valor = valor,
                DadosDestino = JsonSerializer.Serialize(conta, JsonOptions), ReferenciaExterna = Guid.NewGuid().ToString("N"),
                Status = "NA_FILA", SolicitanteId = usuarioId, CriadoEmUtc = DateTime.UtcNow, AtualizadoEmUtc = DateTime.UtcNow });
        }

        public async Task ProcessarAsync(Repasse repasse, CancellationToken cancellationToken)
        {
            if (!Habilitado())
            {
                repasse.Status = "BLOQUEADO";
                repasse.Erro = "Repasses não habilitados ou URL não é do sandbox Asaas.";
                await PersistirResultadoAsync(repasse);
                return;
            }
            var client = _clients.CreateClient("AsaasRepasse");
            decimal saldo;
            try
            {
                using var response = await client.GetAsync("finance/balance", cancellationToken);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadFromJsonAsync<SaldoAsaas>(cancellationToken: cancellationToken);
                if (body?.Balance == null) throw new InvalidOperationException("Saldo ausente na resposta Asaas.");
                saldo = body.Balance.Value;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
            {
                repasse.Status = "BLOQUEADO";
                repasse.Erro = "Falha ao consultar saldo do Asaas; não foi enviada transferência.";
                _logger.LogWarning(ex, "Falha ao consultar saldo para repasse {Id}.", repasse.Id);
                await PersistirResultadoAsync(repasse);
                return;
            }
            var pendente = await _context.Repasses.Where(r => r.CobrancaId == repasse.CobrancaId && r.Status == "NA_FILA").SumAsync(r => (decimal?)r.Valor, cancellationToken) ?? 0m;
            if (saldo < repasse.Valor + pendente)
            {
                repasse.Status = "BLOQUEADO";
                repasse.Erro = "Saldo insuficiente para os repasses pendentes; não foi enviada transferência.";
                await PersistirResultadoAsync(repasse);
                return;
            }
            var cobranca = await _context.Cobrancas.AsNoTracking().FirstAsync(c => c.Id == repasse.CobrancaId, cancellationToken);
            var eventosPendentes = await _context.EventosWebhookAsaas.AnyAsync(e => e.IdCobrancaAsaas == cobranca.IdCobrancaAsaas && e.ProcessadoEmUtc == null, cancellationToken);
            var estorno = await RepasseConfiguracao.TemEstornoOuContestacaoAsync(_context, cobranca.IdCobrancaAsaas, cancellationToken);
            if (!cobranca.Ativo || !cobranca.Pago || cobranca.Status != "RECEIVED" || eventosPendentes || estorno)
            {
                repasse.Status = "BLOQUEADO";
                repasse.Erro = "Cobrança alterada, com eventos pendentes, estorno ou contestação; transferência não enviada.";
                await PersistirResultadoAsync(repasse);
                return;
            }
            var conta = JsonSerializer.Deserialize<ContaRepasse>(repasse.DadosDestino, JsonOptions) ?? throw new InvalidOperationException("Destino de repasse inválido.");
            var bank = conta.Modalidade == "PIX_CHAVE" ? null : new { bank = new { code = conta.CodigoBanco }, ownerName = conta.NomeTitular,
                cpfCnpj = conta.CpfCnpjTitular, ownerBirthDate = conta.DataNascimentoTitular?.ToString("yyyy-MM-dd"), agency = conta.Agencia,
                account = conta.Conta, accountDigit = conta.DigitoConta, bankAccountType = conta.TipoConta };
            var payload = new { value = repasse.Valor, externalReference = repasse.ReferenciaExterna,
                operationType = conta.Modalidade == "TED" ? "TED" : "PIX", pixAddressKey = conta.Modalidade == "PIX_CHAVE" ? conta.ChavePix : null,
                pixAddressKeyType = conta.Modalidade == "PIX_CHAVE" ? conta.TipoChavePix : null, bankAccount = bank };
            try
            {
                using var response = await client.PostAsJsonAsync("transfers", payload, JsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    repasse.Status = (int)response.StatusCode >= 400 && (int)response.StatusCode < 500 && (int)response.StatusCode != 408 && (int)response.StatusCode != 429 ? "REJEITADO" : "INCERTO";
                    var detalhes = await response.Content.ReadAsStringAsync(cancellationToken);
                    try
                    {
                        var erro = JsonSerializer.Deserialize<AsaasErrorResponse>(detalhes, JsonOptions);
                        detalhes = erro?.Errors == null ? null : string.Join(" | ", erro.Errors.Select(e => e.Description));
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Resposta de erro inválida do Asaas para repasse {Id}.", repasse.Id);
                        detalhes = null;
                    }
                    repasse.Erro = $"Asaas retornou HTTP {(int)response.StatusCode}. {detalhes} Consulte o Asaas antes de qualquer novo envio.";
                }
                else
                {
                    var result = await response.Content.ReadFromJsonAsync<AsaasWebhookTransfer>(cancellationToken: cancellationToken);
                    if (result == null || string.IsNullOrWhiteSpace(result.Id) || string.IsNullOrWhiteSpace(result.Status))
                        throw new InvalidOperationException("Resposta de transferência sem ID ou status.");
                    repasse.IdTransferenciaAsaas = result.Id;
                    repasse.Status = result.Status;
                    repasse.Erro = result.FailReason;
                    repasse.ComprovanteUrl = result.TransactionReceiptUrl;
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
            {
                repasse.Status = "INCERTO";
                repasse.Erro = "Resultado do envio incerto; concilie no Asaas antes de qualquer novo envio.";
                _logger.LogError(ex, "Resultado incerto ao enviar repasse {Id}.", repasse.Id);
            }
            await PersistirResultadoAsync(repasse);
        }

        private async Task PersistirResultadoAsync(Repasse repasse)
        {
            var atualizados = await _context.Repasses.Where(r => r.Id == repasse.Id && r.Status == "ENVIANDO")
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, repasse.Status)
                    .SetProperty(r => r.IdTransferenciaAsaas, repasse.IdTransferenciaAsaas)
                    .SetProperty(r => r.Erro, repasse.Erro)
                    .SetProperty(r => r.ComprovanteUrl, repasse.ComprovanteUrl)
                    .SetProperty(r => r.AtualizadoEmUtc, DateTime.UtcNow), CancellationToken.None);
            if (atualizados == 0) _logger.LogInformation("Resultado do repasse {Id} já atualizado por webhook; resposta HTTP não sobrescrita.", repasse.Id);
        }

        private class SaldoAsaas { public decimal? Balance { get; set; } }
    }
}
