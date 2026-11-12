using System.Security.Claims;
using System.Net.Http.Json;
using System.Text.Json;
using ImobAPI.Context;
using ImobAPI.Entities;
using ImobAPI.Integrations.Asaas;
using ImobAPI.Integrations.Asaas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepasseController : ControllerBase
    {
        private readonly ImobContext _context;
        private readonly RepasseService _service;
        private readonly IHttpClientFactory _clients;
        public RepasseController(ImobContext context, RepasseService service, IHttpClientFactory clients) { _context = context; _service = service; _clients = clients; }

        public class SolicitacaoRepasse
        {
            public int CobrancaId { get; set; }
            public ContaRepasse ContaRepasse1 { get; set; }
            public ContaRepasse ContaRepasse2 { get; set; }
        }

        [HttpPost("Manual")]
        public async Task<IActionResult> Manual([FromBody] SolicitacaoRepasse pedido, CancellationToken cancellationToken)
        {
            if (!RepasseConfiguracao.Administrador(_context, User)) return Forbid();
            if (!_service.Habilitado()) return StatusCode(503, "Repasses desabilitados ou ambiente Asaas não é sandbox.");
            if (pedido == null) return BadRequest("Informe a solicitação.");
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
            await _context.Database.ExecuteSqlRawAsync("DECLARE @result int; EXEC @result = sp_getapplock @Resource = 'ImobAPI.AsaasWebhookProcessor', @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; IF @result < 0 THROW 51000, 'Nao foi possivel obter lock de repasse.', 1;", cancellationToken);
            var cobranca = await _context.Cobrancas.FirstOrDefaultAsync(c => c.Id == pedido.CobrancaId, cancellationToken);
            if (cobranca == null) return NotFound("Cobrança não encontrada.");
            if (!cobranca.Ativo || !cobranca.Pago || cobranca.Status != "RECEIVED" || string.IsNullOrWhiteSpace(cobranca.IdCobrancaAsaas)) return Conflict("A cobrança deve estar ativa e recebida no Asaas.");
            if (await _context.EventosWebhookAsaas.AnyAsync(e => e.IdCobrancaAsaas == cobranca.IdCobrancaAsaas && e.ProcessadoEmUtc == null, cancellationToken)) return Conflict("Há eventos pendentes para a cobrança; aguarde o processamento.");
            if (await RepasseConfiguracao.TemEstornoOuContestacaoAsync(_context, cobranca.IdCobrancaAsaas, cancellationToken))
                return Conflict("A cobrança possui evento de estorno ou contestação e exige análise manual.");
            if (await _context.Repasses.AnyAsync(r => _context.Cobrancas.Any(c => c.Id == r.CobrancaId && c.IdCobrancaAsaas == cobranca.IdCobrancaAsaas), cancellationToken)) return Conflict("Já existe solicitação de repasse para esta cobrança Asaas; consulte e concilie os registros existentes.");
            var evento = await _context.EventosWebhookAsaas.Where(e => e.IdCobrancaAsaas == cobranca.IdCobrancaAsaas && e.Tipo == "PAYMENT_RECEIVED" && e.ProcessadoEmUtc != null).OrderByDescending(e => e.Id).FirstOrDefaultAsync(cancellationToken);
            var payment = evento == null ? null : JsonSerializer.Deserialize<AsaasWebhookRequest>(evento.Payload)?.Payment;
            if (payment?.Status != "RECEIVED" || !payment.NetValue.HasValue) return Conflict("Webhook de recebimento com valor líquido não encontrado.");
            var erro = RepasseService.ValidarValores(pedido.ContaRepasse1, pedido.ContaRepasse2, payment.NetValue.Value, out _, out _);
            if (erro != null) return BadRequest(erro);
            _service.Reservar(cobranca, pedido.ContaRepasse1, pedido.ContaRepasse2, payment.NetValue.Value, "MANUAL", int.Parse(User.FindFirstValue("id")));
            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return Accepted(new { mensagem = "Repasse registrado para processamento; acompanhe o status antes de reenviar.", cobrancaId = cobranca.Id });
        }

        [HttpGet("PorCobranca/{id}")]
        public async Task<IActionResult> PorCobranca(int id, CancellationToken cancellationToken)
        {
            if (!RepasseConfiguracao.Administrador(_context, User)) return Forbid();
            var registros = await _context.Repasses.AsNoTracking().Where(r => r.CobrancaId == id).OrderBy(r => r.Destinatario)
                .Select(r => new { r.Id, r.CobrancaId, r.Destinatario, r.Origem, r.Valor, r.ReferenciaExterna, r.IdTransferenciaAsaas, r.Status, r.Erro, r.ComprovanteUrl, r.CriadoEmUtc, r.AtualizadoEmUtc }).ToListAsync(cancellationToken);
            return Ok(registros);
        }

        [HttpPost("RetomarNaoEnviados/{cobrancaId}")]
        public async Task<IActionResult> RetomarNaoEnviados(int cobrancaId, CancellationToken cancellationToken)
        {
            if (!RepasseConfiguracao.Administrador(_context, User)) return Forbid();
            if (!_service.Habilitado()) return StatusCode(503, "Repasses desabilitados ou ambiente Asaas não é sandbox.");
            var alterados = await _context.Repasses.Where(r => r.CobrancaId == cobrancaId && r.Status == "BLOQUEADO")
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, "NA_FILA").SetProperty(r => r.Erro, (string)null)
                    .SetProperty(r => r.AtualizadoEmUtc, DateTime.UtcNow), cancellationToken);
            return alterados == 0 ? Conflict("Não há repasses comprovadamente não enviados para retomar.") : Accepted(new { retomados = alterados });
        }

        public class Conciliacao { public string IdTransferenciaAsaas { get; set; } }

        [HttpPost("Conciliar/{id}")]
        public async Task<IActionResult> Conciliar(long id, [FromBody] Conciliacao pedido, CancellationToken cancellationToken)
        {
            if (!RepasseConfiguracao.Administrador(_context, User)) return Forbid();
            if (!_service.Habilitado()) return StatusCode(503, "Repasses desabilitados ou ambiente Asaas não é sandbox.");
            if (string.IsNullOrWhiteSpace(pedido?.IdTransferenciaAsaas) || pedido.IdTransferenciaAsaas.Length > 128) return BadRequest("Informe o ID da transferência no Asaas.");
            var repasse = await _context.Repasses.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (repasse == null) return NotFound();
            if (repasse.Status != "INCERTO" && repasse.Status != "ENVIANDO") return Conflict("Somente envios incertos ou interrompidos podem ser conciliados.");
            AsaasWebhookTransfer transferencia;
            try
            {
                using var resposta = await _clients.CreateClient("AsaasRepasse").GetAsync("transfers/" + Uri.EscapeDataString(pedido.IdTransferenciaAsaas), cancellationToken);
                if (!resposta.IsSuccessStatusCode) return StatusCode(502, $"Consulta ao Asaas falhou: HTTP {(int)resposta.StatusCode}.");
                transferencia = await resposta.Content.ReadFromJsonAsync<AsaasWebhookTransfer>(cancellationToken: cancellationToken);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                return StatusCode(502, "Falha ao consultar transferência no Asaas; tente conciliar novamente.");
            }
            if (transferencia == null || transferencia.Id != pedido.IdTransferenciaAsaas || transferencia.ExternalReference != repasse.ReferenciaExterna || transferencia.Value != repasse.Valor ||
                !(new[] { "PENDING", "SCHEDULED", "BANK_PROCESSING", "BLOCKED", "DONE", "FAILED", "CANCELLED" }).Contains(transferencia.Status))
                return Conflict("A transferência consultada não corresponde à referência e valor do repasse ou tem status inválido.");
            var atualizados = await _context.Repasses.Where(r => r.Id == id && (r.Status == "INCERTO" || r.Status == "ENVIANDO") && (r.IdTransferenciaAsaas == null || r.IdTransferenciaAsaas == transferencia.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.IdTransferenciaAsaas, transferencia.Id).SetProperty(r => r.Status, transferencia.Status)
                    .SetProperty(r => r.Erro, transferencia.FailReason).SetProperty(r => r.ComprovanteUrl, transferencia.TransactionReceiptUrl)
                    .SetProperty(r => r.AtualizadoEmUtc, DateTime.UtcNow), cancellationToken);
            return atualizados == 0 ? Conflict("O repasse foi atualizado durante a conciliação; consulte novamente.") : Ok(new { id, transferencia.Id, transferencia.Status });
        }
    }
}
