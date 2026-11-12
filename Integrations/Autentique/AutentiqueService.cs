using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImobAPI.Integrations.Autentique.Models;

namespace ImobAPI.Integrations.Autentique
{
    public class AutentiqueService
    {
        private const string Mutation = """
            mutation CreateDocumentMutation($document: DocumentInput!, $signers: [SignerInput!]!, $file: Upload!, $sandbox: Boolean!) {
              createDocument(document: $document, signers: $signers, file: $file, sandbox: $sandbox) {
                id name created_at signatures { public_id name email created_at action { name } link { short_link } }
              }
            }
            """;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public AutentiqueService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public (string ApiKey, bool Sandbox) ObterConfiguracao()
        {
            var apiKey = _configuration["Autentique:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Autentique:ApiKey não configurada.");

            var sandboxValue = _configuration["Autentique:Sandbox"];
            if (sandboxValue != null && !bool.TryParse(sandboxValue, out _))
                throw new InvalidOperationException("Autentique:Sandbox deve ser true ou false.");

            return (apiKey, sandboxValue == null || bool.Parse(sandboxValue));
        }

        public async Task<CriarDocumentoResponse> CriarDocumentoAsync(CriarDocumentoRequest request, string apiKey, bool sandbox, CancellationToken cancellationToken)
        {
            var operations = new
            {
                query = Mutation,
                variables = new GraphQlVariables
                {
                    Document = new { name = request.Nome.Trim() },
                    Signers = request.Signatarios.Select(s => new
                    {
                        name = string.IsNullOrWhiteSpace(s.Nome) ? null : s.Nome.Trim(),
                        email = string.IsNullOrWhiteSpace(s.Email) ? null : s.Email.Trim(),
                        phone = string.IsNullOrWhiteSpace(s.Telefone) ? null : s.Telefone.Trim(),
                        delivery_method = string.IsNullOrWhiteSpace(s.MetodoEntrega) ? null : s.MetodoEntrega,
                        action = s.Acao
                    }),
                    Sandbox = sandbox
                }
            };

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(JsonSerializer.Serialize(operations, SerializerOptions), Encoding.UTF8, "application/json"), "operations");
            form.Add(new StringContent("{\"file\":[\"variables.file\"]}", Encoding.UTF8, "application/json"), "map");
            var file = new ByteArrayContent(request.Arquivo);
            file.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(file, "file", "contrato.pdf");

            using var message = new HttpRequestMessage(HttpMethod.Post, "https://api.autentique.com.br/v2/graphql");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            message.Content = form;
            using var response = await _client.SendAsync(message, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new AutentiqueIntegrationException($"A Autentique retornou HTTP {(int)response.StatusCode}.");

            GraphQlResponse result;
            try
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                result = await JsonSerializer.DeserializeAsync<GraphQlResponse>(stream, SerializerOptions, cancellationToken);
            }
            catch (JsonException ex)
            {
                throw new AutentiqueIntegrationException("Resposta inválida da Autentique.", innerException: ex);
            }

            var documento = result?.Data?.CreateDocument;
            if (result?.Errors?.Count > 0)
                throw new AutentiqueIntegrationException("A Autentique retornou erros GraphQL.", documento?.Id,
                    detalhes: result.Errors.Where(e => !string.IsNullOrWhiteSpace(e?.Message)).Select(e => e.Message).ToArray());
            if (string.IsNullOrWhiteSpace(documento?.Id))
                throw new AutentiqueIntegrationException("A Autentique não confirmou a criação do documento.");

            return documento;
        }

        private sealed class GraphQlVariables
        {
            [JsonPropertyName("document")]
            public object Document { get; set; }
            [JsonPropertyName("signers")]
            public object Signers { get; set; }
            [JsonPropertyName("file"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public object File { get; set; }
            [JsonPropertyName("sandbox")]
            public bool Sandbox { get; set; }
        }

        private sealed class GraphQlResponse
        {
            [JsonPropertyName("data")]
            public GraphQlData Data { get; set; }
            [JsonPropertyName("errors")]
            public List<GraphQlError> Errors { get; set; }
        }

        private sealed class GraphQlError
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        private sealed class GraphQlData
        {
            [JsonPropertyName("createDocument")]
            public CriarDocumentoResponse CreateDocument { get; set; }
        }
    }

    public class AutentiqueIntegrationException : Exception
    {
        public string DocumentoId { get; }
        public IReadOnlyList<string> Detalhes { get; }

        public AutentiqueIntegrationException(string message, string documentoId = null, Exception innerException = null, IReadOnlyList<string> detalhes = null)
            : base(message, innerException)
        {
            DocumentoId = documentoId;
            Detalhes = detalhes ?? Array.Empty<string>();
        }
    }
}
