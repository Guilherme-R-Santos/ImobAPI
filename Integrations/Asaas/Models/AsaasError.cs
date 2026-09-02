using System.Text.Json.Serialization;

namespace ImobAPI.Integrations.Asaas.Models
{
    public class AsaasError
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class AsaasErrorResponse
    {
        [JsonPropertyName("errors")]
        public List<AsaasError> Errors { get; set; }
    }
}
