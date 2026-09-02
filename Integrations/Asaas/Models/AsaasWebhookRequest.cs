using System.Text.Json.Serialization;

namespace ImobAPI.Integrations.Asaas.Models
{
    public class AsaasWebhookRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("dateCreated")]
        public string DateCreated { get; set; }

        [JsonPropertyName("payment")]
        public AsaasWebhookPayment Payment { get; set; }
    }

    public class AsaasWebhookPayment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }

        [JsonPropertyName("netValue")]
        public double? NetValue { get; set; }

        [JsonPropertyName("nossoNumero")]
        public string NossoNumero { get; set; }

        [JsonPropertyName("invoiceUrl")]
        public string InvoiceUrl { get; set; }

        [JsonPropertyName("bankSlipUrl")]
        public string BankSlipUrl { get; set; }

        [JsonPropertyName("confirmedDate")]
        public string ConfirmedDate { get; set; }

        [JsonPropertyName("paymentDate")]
        public string PaymentDate { get; set; }

        [JsonPropertyName("clientPaymentDate")]
        public string ClientPaymentDate { get; set; }

        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("deleted")]
        public bool? Deleted { get; set; }
    }
}
