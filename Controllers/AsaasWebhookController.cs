using Microsoft.AspNetCore.Mvc;
using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AsaasWebhookController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AsaasWebhookController> _logger;

        private static readonly HashSet<string> EventosPago = new()
        {
            "PAYMENT_CONFIRMED",
            "PAYMENT_RECEIVED"
        };

        public AsaasWebhookController(Context.ImobContext context, IConfiguration configuration, ILogger<AsaasWebhookController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("Receber")]
        public IActionResult Receber([FromBody] AsaasWebhookRequest webhookRequest, [FromHeader(Name = "asaas-access-token")] string asaasAccessToken)
        {
            var tokenEsperado = _configuration["Asaas:WebhookToken"];
            if (!string.IsNullOrWhiteSpace(tokenEsperado) && tokenEsperado != asaasAccessToken)
            {
                _logger.LogWarning("Webhook Asaas recebido com token inválido.");
                return Unauthorized();
            }

            if (webhookRequest?.Payment == null || string.IsNullOrWhiteSpace(webhookRequest.Payment.Id))
            {
                _logger.LogWarning("Webhook Asaas recebido sem dados de cobrança (payment).");
                return Ok();
            }

            var cobranca = _context.Cobrancas.FirstOrDefault(c => c.IdCobrancaAsaas == webhookRequest.Payment.Id);
            if (cobranca == null)
            {
                _logger.LogWarning("Webhook Asaas recebido para cobrança não encontrada localmente. IdCobrancaAsaas: {IdCobrancaAsaas}, Evento: {Evento}", webhookRequest.Payment.Id, webhookRequest.Event);
                return Ok();
            }

            AtualizarCobranca(cobranca, webhookRequest);

            _context.SaveChanges();
            return Ok();
        }

        private void AtualizarCobranca(Entities.Cobranca cobranca, AsaasWebhookRequest webhookRequest)
        {
            var payment = webhookRequest.Payment;

            switch (webhookRequest.Event)
            {
                case "PAYMENT_CREATED":
                case "PAYMENT_UPDATED":
                case "PAYMENT_CONFIRMED":
                case "PAYMENT_RECEIVED":
                case "PAYMENT_OVERDUE":
                case "PAYMENT_RESTORED":
                case "PAYMENT_REFUNDED":
                    cobranca.Status = payment.Status ?? cobranca.Status;
                    cobranca.LinkBoleto = payment.BankSlipUrl ?? payment.InvoiceUrl ?? cobranca.LinkBoleto;
                    cobranca.NossoNumero = payment.NossoNumero ?? cobranca.NossoNumero;
                    if (payment.NetValue.HasValue)
                    {
                        cobranca.valorLiquido = payment.NetValue.Value;
                    }

                    if (EventosPago.Contains(webhookRequest.Event))
                    {
                        cobranca.Pago = true;
                        var dataPagamento = payment.PaymentDate ?? payment.ClientPaymentDate ?? payment.ConfirmedDate;
                        if (DateTime.TryParse(dataPagamento, out var dataPagamentoConvertida))
                        {
                            cobranca.DataPagamento = dataPagamentoConvertida;
                        }
                    }
                    else if (webhookRequest.Event == "PAYMENT_REFUNDED")
                    {
                        cobranca.Pago = false;
                        cobranca.DataPagamento = null;
                    }
                    break;

                case "PAYMENT_DELETED":
                    cobranca.Status = payment.Status ?? cobranca.Status;
                    cobranca.Ativo = false;
                    cobranca.DataInativacao = DateTime.Now;
                    break;

                default:
                    _logger.LogInformation("Evento Asaas não tratado recebido: {Evento} para cobrança {IdCobrancaAsaas}", webhookRequest.Event, payment.Id);
                    return;
            }

            cobranca.DataAtualizacao = DateTime.Now;
        }
    }
}
