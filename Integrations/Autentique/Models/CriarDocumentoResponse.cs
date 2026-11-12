using System.Text.Json.Serialization;

namespace ImobAPI.Integrations.Autentique.Models
{
    public class CriarDocumentoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Nome { get; set; }

        [JsonPropertyName("created_at")]
        public string CriadoEm { get; set; }

        [JsonPropertyName("signatures")]
        public List<AssinaturaResponse> Assinaturas { get; set; }
    }

    public class AssinaturaResponse
    {
        [JsonPropertyName("public_id")]
        public string IdPublico { get; set; }

        [JsonPropertyName("name")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("created_at")]
        public string CriadoEm { get; set; }

        [JsonPropertyName("action")]
        public AcaoAssinaturaResponse Acao { get; set; }

        [JsonPropertyName("link")]
        public LinkAssinaturaResponse Link { get; set; }
    }

    public class AcaoAssinaturaResponse
    {
        [JsonPropertyName("name")]
        public string Nome { get; set; }
    }

    public class LinkAssinaturaResponse
    {
        [JsonPropertyName("short_link")]
        public string LinkCurto { get; set; }
    }
}
