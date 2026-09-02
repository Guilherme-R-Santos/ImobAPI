using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Integrations.Asaas
{
    public interface IAsaasService
    {
        Task<AsaasCustomerResponse> CriarClienteAsync(AsaasCustomerRequest request);
        Task<AsaasCustomerResponse> AtualizarClienteAsync(string idClienteAsaas, AsaasCustomerRequest request);
        Task<AsaasPaymentResponse> CriarCobrancaAsync(AsaasPaymentRequest request);
    }
}
