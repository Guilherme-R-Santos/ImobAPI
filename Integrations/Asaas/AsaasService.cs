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
            request.NotificationDisabled = true;
            var response = await _httpClient.PostAsJsonAsync("customers", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasCustomerResponse>(response);
        }

        public async Task<AsaasCustomerResponse> AtualizarClienteAsync(string idClienteAsaas, AsaasCustomerRequest request)
        {
            request.NotificationDisabled = true;
            var response = await _httpClient.PutAsJsonAsync($"customers/{idClienteAsaas}", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasCustomerResponse>(response);
        }

        public async Task DesativarNotificacoesClienteAsync(string idClienteAsaas)
        {
            if (string.IsNullOrWhiteSpace(idClienteAsaas))
            {
                throw new AsaasIntegrationException("Não é possível desativar notificações sem o ID do cliente no Asaas.");
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"customers/{idClienteAsaas}", new { notificationDisabled = true }, SerializerOptions);
                var cliente = await ProcessResponseAsync<AsaasCustomerResponse>(response);
                if (!cliente.NotificationDisabled)
                {
                    throw new AsaasIntegrationException("O Asaas não confirmou a desativação das notificações do cliente.");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new AsaasIntegrationException($"Falha na comunicação com o Asaas ao desativar notificações: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                throw new AsaasIntegrationException($"Tempo limite excedido ao desativar notificações no Asaas: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new AsaasIntegrationException($"Resposta inválida do Asaas ao desativar notificações: {ex.Message}");
            }
        }

        public async Task<AsaasPaymentResponse> CriarCobrancaAsync(AsaasPaymentRequest request)
        {
            await DesativarNotificacoesClienteAsync(request.Customer);
            var response = await _httpClient.PostAsJsonAsync("payments", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasPaymentResponse>(response);
        }

        public async Task<AsaasPaymentResponse> AtualizarCobrancaAsync(string idCobrancaAsaas, AsaasPaymentRequest request)
        {
            AsaasPaymentResponse pagamento;
            try
            {
                var consulta = await _httpClient.GetAsync($"payments/{idCobrancaAsaas}");
                pagamento = await ProcessResponseAsync<AsaasPaymentResponse>(consulta);
            }
            catch (HttpRequestException ex)
            {
                throw new AsaasIntegrationException($"Falha ao consultar a cobrança no Asaas antes de desativar notificações: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                throw new AsaasIntegrationException($"Tempo limite excedido ao consultar a cobrança no Asaas: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new AsaasIntegrationException($"Resposta inválida ao consultar a cobrança no Asaas: {ex.Message}");
            }

            await DesativarNotificacoesClienteAsync(pagamento.Customer);
            var response = await _httpClient.PutAsJsonAsync($"payments/{idCobrancaAsaas}", request, SerializerOptions);
            return await ProcessResponseAsync<AsaasPaymentResponse>(response);
        }

        public async Task<AsaasPaymentResponse> CancelarCobrancaAsync(string idCobrancaAsaas)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"payments/{idCobrancaAsaas}");
                return await ProcessResponseAsync<AsaasPaymentResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new AsaasIntegrationException($"Falha na comunicação com o Asaas ao cancelar a cobrança: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                throw new AsaasIntegrationException($"Tempo limite excedido ao cancelar a cobrança no Asaas: {ex.Message}");
            }
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
