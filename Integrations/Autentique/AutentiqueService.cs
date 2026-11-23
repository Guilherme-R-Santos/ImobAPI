using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AutentiqueService(HttpClient client, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = client;
            _httpClientFactory = httpClientFactory;
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

        public async Task<byte[]> BaixarDocumentoAsync(string documentoId, string apiKey, bool original, CancellationToken cancellationToken)
        {
            var campo = original ? "original" : "signed";
            var query = $"query {{ document(id: {JsonSerializer.Serialize(documentoId)}) {{ id files {{ {campo} }} }} }}";
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.autentique.com.br/v2/graphql");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = JsonContent.Create(new { query });
            using var response = await _client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new AutentiqueIntegrationException($"A Autentique retornou HTTP {(int)response.StatusCode} ao consultar o documento.");

            ConsultaDocumentoResponse resultado;
            try
            {
                await using var body = await response.Content.ReadAsStreamAsync(cancellationToken);
                resultado = await JsonSerializer.DeserializeAsync<ConsultaDocumentoResponse>(body, cancellationToken: cancellationToken);
            }
            catch (JsonException ex)
            {
                throw new AutentiqueIntegrationException("Resposta inválida da Autentique ao consultar o documento.", innerException: ex);
            }

            if (resultado?.Errors?.Count > 0)
                throw new AutentiqueIntegrationException("A Autentique retornou erros ao consultar o documento.",
                    detalhes: resultado.Errors.Where(e => !string.IsNullOrWhiteSpace(e?.Message)).Select(e => e.Message).ToArray());
            if (resultado?.Data == null)
                throw new AutentiqueIntegrationException("A Autentique não retornou dados do documento.");
            if (resultado.Data.Document == null)
                throw new DocumentoAutentiqueIndisponivelException("Documento não encontrado ou inacessível na Autentique.");
            if (resultado.Data.Document.Id != documentoId || resultado.Data.Document.Files == null)
                throw new AutentiqueIntegrationException("A Autentique retornou dados inconsistentes do documento.");
            var url = original ? resultado.Data.Document.Files.Original : resultado.Data.Document.Files.Signed;
            if (string.IsNullOrWhiteSpace(url))
                throw new DocumentoAutentiqueIndisponivelException(original
                    ? "O PDF original não está disponível na Autentique."
                    : "O PDF assinado não está disponível na Autentique.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var endereco))
                throw new AutentiqueIntegrationException("A Autentique retornou um endereço de download inválido.");
            using var client = _httpClientFactory.CreateClient("AutentiqueDownload");
            using var limiteTempo = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            limiteTempo.CancelAfter(TimeSpan.FromMinutes(2));
            const int tamanhoMaximo = 100 * 1024 * 1024;
            for (var redirecionamentos = 0; redirecionamentos <= 5; redirecionamentos++)
            {
                if (!EnderecoDownloadPermitido(endereco))
                    throw new AutentiqueIntegrationException("O endereço de download retornado pela Autentique não é permitido.");
                using var download = await client.GetAsync(endereco, HttpCompletionOption.ResponseHeadersRead, limiteTempo.Token);
                if (download.StatusCode is HttpStatusCode.MovedPermanently or HttpStatusCode.Redirect or HttpStatusCode.SeeOther or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect)
                {
                    var destino = download.Headers.Location;
                    if (destino == null || !Uri.TryCreate(endereco, destino, out endereco))
                        throw new AutentiqueIntegrationException("Redirecionamento de download inválido.");
                    continue;
                }
                if ((int)download.StatusCode == 425)
                    throw new DocumentoAutentiqueIndisponivelException("A Autentique retornou HTTP 425: o PDF solicitado não está disponível no momento. Tente novamente mais tarde.");
                if (download.StatusCode != HttpStatusCode.OK)
                    throw new AutentiqueIntegrationException($"Falha ao baixar o PDF da Autentique (HTTP {(int)download.StatusCode}).");
                if (download.Content.Headers.ContentLength > tamanhoMaximo)
                    throw new AutentiqueIntegrationException("O PDF da Autentique excede o limite de 100 MiB.");

                await using var origem = await download.Content.ReadAsStreamAsync(limiteTempo.Token);
                using var conteudo = new MemoryStream();
                var buffer = new byte[81920];
                int lidos;
                while ((lidos = await origem.ReadAsync(buffer, limiteTempo.Token)) > 0)
                {
                    if (conteudo.Length + lidos > tamanhoMaximo)
                        throw new AutentiqueIntegrationException("O PDF da Autentique excede o limite de 100 MiB.");
                    conteudo.Write(buffer, 0, lidos);
                }
                var pdf = conteudo.ToArray();
                if (pdf.Length < 5 || !pdf.AsSpan(0, 5).SequenceEqual("%PDF-"u8))
                    throw new AutentiqueIntegrationException("O arquivo retornado pela Autentique não é um PDF válido.");
                return pdf;
            }
            throw new AutentiqueIntegrationException("Limite de redirecionamentos do download excedido.");
        }

        private static bool EnderecoDownloadPermitido(Uri uri)
        {
            return uri.Scheme == Uri.UriSchemeHttps && uri.IsDefaultPort && string.IsNullOrEmpty(uri.UserInfo) &&
                (uri.IdnHost.Equals("painel.autentique.com.br", StringComparison.OrdinalIgnoreCase) ||
                 uri.IdnHost.Equals("api.autentique.com.br", StringComparison.OrdinalIgnoreCase) ||
                 uri.IdnHost.Equals("storage.googleapis.com", StringComparison.OrdinalIgnoreCase));
        }

        private sealed class ConsultaDocumentoResponse
        {
            [JsonPropertyName("data")]
            public ConsultaDocumentoData Data { get; set; }
            [JsonPropertyName("errors")]
            public List<GraphQlError> Errors { get; set; }
        }

        private sealed class ConsultaDocumentoData
        {
            [JsonPropertyName("document")]
            public DocumentoConsultado Document { get; set; }
        }

        private sealed class DocumentoConsultado
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("files")]
            public DocumentoArquivos Files { get; set; }
        }

        private sealed class DocumentoArquivos
        {
            [JsonPropertyName("signed")]
            public string Signed { get; set; }
            [JsonPropertyName("original")]
            public string Original { get; set; }
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

    public class DocumentoAutentiqueIndisponivelException : Exception
    {
        public DocumentoAutentiqueIndisponivelException(string message) : base(message) { }
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
