using System.Net.Http.Json;
using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Integrations.Asaas
{
    public class AsaasService : IAsaasService
    {
        private readonly HttpClient _httpClient;

        public AsaasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AsaasCustomerResponse> CriarClienteAsync(AsaasCustomerRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("customers", request);
            return await ProcessResponseAsync<AsaasCustomerResponse>(response);
        }

        public async Task<AsaasCustomerResponse> AtualizarClienteAsync(string idClienteAsaas, AsaasCustomerRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"customers/{idClienteAsaas}", request);
            return await ProcessResponseAsync<AsaasCustomerResponse>(response);
        }

        public async Task<AsaasPaymentResponse> CriarCobrancaAsync(AsaasPaymentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("payments", request);
            return await ProcessResponseAsync<AsaasPaymentResponse>(response);
        }

        private static async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();
                return result ?? throw new AsaasIntegrationException("Resposta vazia recebida do Asaas.", (int)response.StatusCode);
            }

            var errorBody = await response.Content.ReadFromJsonAsync<AsaasErrorResponse>();
            var mensagem = errorBody?.Errors != null && errorBody.Errors.Count > 0
                ? string.Join(" | ", errorBody.Errors.Select(e => e.Description))
                : $"Falha na comunicação com o Asaas ({(int)response.StatusCode}).";

            throw new AsaasIntegrationException(mensagem, (int)response.StatusCode, errorBody?.Errors);
        }
    }
}
