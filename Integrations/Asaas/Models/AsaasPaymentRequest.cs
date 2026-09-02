using System.Text.Json.Serialization;

namespace ImobAPI.Integrations.Asaas.Models
{
    public class AsaasPaymentRequest
    {
        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("billingType")]
        public string BillingType { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("dueDate")]
        public string DueDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("daysAfterDueDateToRegistrationCancellation")]
        public int? DaysAfterDueDateToRegistrationCancellation { get; set; }

        [JsonPropertyName("discount")]
        public AsaasDiscount Discount { get; set; }

        [JsonPropertyName("interest")]
        public AsaasInterest Interest { get; set; }

        [JsonPropertyName("fine")]
        public AsaasFine Fine { get; set; }

        [JsonPropertyName("postalService")]
        public bool PostalService { get; set; }

        [JsonPropertyName("split")]
        public List<AsaasSplit> Split { get; set; }

        [JsonPropertyName("callback")]
        public AsaasCallback Callback { get; set; }
    }

    public class AsaasDiscount
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("dueDateLimitDays")]
        public int DueDateLimitDays { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class AsaasInterest
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }
    }

    public class AsaasFine
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class AsaasSplit
    {
        [JsonPropertyName("walletId")]
        public string WalletId { get; set; }

        [JsonPropertyName("fixedValue")]
        public double? FixedValue { get; set; }

        [JsonPropertyName("percentualValue")]
        public double? PercentualValue { get; set; }

        [JsonPropertyName("totalFixedValue")]
        public double? TotalFixedValue { get; set; }

        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class AsaasCallback
    {
        [JsonPropertyName("successUrl")]
        public string SuccessUrl { get; set; }

        [JsonPropertyName("autoRedirect")]
        public bool AutoRedirect { get; set; }
    }
}
