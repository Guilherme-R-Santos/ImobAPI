using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ImobAPI.Context;
using ImobAPI.Entities;
using ImobAPI.Integrations.Asaas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AsaasWebhookController : ControllerBase
    {
        private readonly ImobContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AsaasWebhookController> _logger;

        public AsaasWebhookController(ImobContext context, IConfiguration configuration, ILogger<AsaasWebhookController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Receber")]
        public async Task<IActionResult> Receber([FromBody] AsaasWebhookRequest webhookRequest, CancellationToken cancellationToken)
        {
            var tokenEsperado = _configuration["Asaas:WebhookToken"];
            if (string.IsNullOrWhiteSpace(tokenEsperado))
            {
                _logger.LogError("Asaas:WebhookToken não configurado; webhook indisponível.");
                return StatusCode(503);
            }

            var tokens = Request.Headers["asaas-access-token"];
            if (tokens.Count != 1 || string.IsNullOrWhiteSpace(tokens[0]) ||
                !CryptographicOperations.FixedTimeEquals(
                    SHA256.HashData(Encoding.UTF8.GetBytes(tokenEsperado)),
                    SHA256.HashData(Encoding.UTF8.GetBytes(tokens[0]!))))
            {
                _logger.LogWarning("Webhook Asaas recebido com token inválido.");
                return Unauthorized();
            }

            if (webhookRequest == null || string.IsNullOrWhiteSpace(webhookRequest.Id) || webhookRequest.Id.Length > 256 ||
                string.IsNullOrWhiteSpace(webhookRequest.Event) || webhookRequest.Event.Length > 128 ||
                webhookRequest.Payment?.Id?.Length > 128 ||
                (webhookRequest.Event.StartsWith("PAYMENT_", StringComparison.Ordinal) &&
                 string.IsNullOrWhiteSpace(webhookRequest.Payment?.Id)))
            {
                _logger.LogWarning("Webhook Asaas recebido com identificador ou dados de cobrança inválidos.");
                return BadRequest("Evento ou cobrança inválidos.");
            }

            _context.EventosWebhookAsaas.Add(new EventoWebhookAsaas
            {
                IdEventoAsaas = webhookRequest.Id,
                Tipo = webhookRequest.Event,
                IdCobrancaAsaas = webhookRequest.Payment?.Id,
                Payload = JsonSerializer.Serialize(webhookRequest),
                RecebidoEmUtc = DateTime.UtcNow
            });

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                if (!await _context.EventosWebhookAsaas.AsNoTracking().AnyAsync(e => e.IdEventoAsaas == webhookRequest.Id, cancellationToken))
                {
                    throw;
                }
                _logger.LogInformation("Evento Asaas duplicado recebido: {IdEvento}", webhookRequest.Id);
            }

            return Ok();
        }
    }
}
