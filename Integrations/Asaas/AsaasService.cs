using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Integrations.Asaas
{
    public class AsaasService : IAsaasService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public AsaasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AsaasCustomerResponse> CriarClienteAsync(AsaasCustomerRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("customers", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasCustomerResponse>(response);
        }

        public async Task<AsaasCustomerResponse> AtualizarClienteAsync(string idClienteAsaas, AsaasCustomerRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"customers/{idClienteAsaas}", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasCustomerResponse>(response);
        }

        public async Task<AsaasPaymentResponse> CriarCobrancaAsync(AsaasPaymentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("payments", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasPaymentResponse>(response);
        }

        public async Task<AsaasPaymentResponse> AtualizarCobrancaAsync(string idCobrancaAsaas, AsaasPaymentRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"payments/{idCobrancaAsaas}", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasPaymentResponse>(response);
        }

        private static async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();
                return result ?? throw new AsaasIntegrationException("Resposta vazia recebida do Asaas.", (int)response.StatusCode);
            }

            AsaasErrorResponse? errorBody = null;
            var content = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    errorBody = System.Text.Json.JsonSerializer.Deserialize<AsaasErrorResponse>(content);
                }
                catch (System.Text.Json.JsonException)
                {
                    // Corpo de erro não é um JSON válido; será usada a mensagem padrão abaixo.
                }
            }

            var mensagem = errorBody?.Errors != null && errorBody.Errors.Count > 0
                ? string.Join(" | ", errorBody.Errors.Select(e => e.Description))
                : $"Falha na comunicação com o Asaas ({(int)response.StatusCode}).";

            throw new AsaasIntegrationException(mensagem, (int)response.StatusCode, errorBody?.Errors);
        }
    }
}
