using System.Text.Json.Serialization;

namespace ImobAPI.Integrations.Asaas.Models
{
    public class AsaasPaymentResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("dateCreated")]
        public string DateCreated { get; set; }

        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("subscription")]
        public string Subscription { get; set; }

        [JsonPropertyName("installment")]
        public string Installment { get; set; }

        [JsonPropertyName("paymentLink")]
        public string PaymentLink { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("netValue")]
        public double NetValue { get; set; }

        [JsonPropertyName("originalValue")]
        public double? OriginalValue { get; set; }

        [JsonPropertyName("interestValue")]
        public double? InterestValue { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("billingType")]
        public string BillingType { get; set; }

        [JsonPropertyName("canBePaidAfterDueDate")]
        public bool CanBePaidAfterDueDate { get; set; }

        [JsonPropertyName("pixTransaction")]
        public string PixTransaction { get; set; }

        [JsonPropertyName("pixQrCodeId")]
        public string PixQrCodeId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dueDate")]
        public string DueDate { get; set; }

        [JsonPropertyName("originalDueDate")]
        public string OriginalDueDate { get; set; }

        [JsonPropertyName("paymentDate")]
        public string PaymentDate { get; set; }

        [JsonPropertyName("clientPaymentDate")]
        public string ClientPaymentDate { get; set; }

        [JsonPropertyName("installmentNumber")]
        public int? InstallmentNumber { get; set; }

        [JsonPropertyName("invoiceUrl")]
        public string InvoiceUrl { get; set; }

        [JsonPropertyName("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("anticipated")]
        public bool Anticipated { get; set; }

        [JsonPropertyName("anticipable")]
        public bool Anticipable { get; set; }

        [JsonPropertyName("creditDate")]
        public string CreditDate { get; set; }

        [JsonPropertyName("estimatedCreditDate")]
        public string EstimatedCreditDate { get; set; }

        [JsonPropertyName("transactionReceiptUrl")]
        public string TransactionReceiptUrl { get; set; }

        [JsonPropertyName("nossoNumero")]
        public string NossoNumero { get; set; }

        [JsonPropertyName("bankSlipUrl")]
        public string BankSlipUrl { get; set; }

        [JsonPropertyName("errors")]
        public List<AsaasError> Errors { get; set; }
    }
}
