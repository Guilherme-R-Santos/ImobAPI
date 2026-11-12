using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Integrations.Asaas
{
    public interface IAsaasService
    {
        Task<AsaasCustomerResponse> CriarClienteAsync(AsaasCustomerRequest request);
        Task<AsaasCustomerResponse> AtualizarClienteAsync(string idClienteAsaas, AsaasCustomerRequest request);
        Task DesativarNotificacoesClienteAsync(string idClienteAsaas);
        Task<AsaasPaymentResponse> CriarCobrancaAsync(AsaasPaymentRequest request);
        Task<AsaasPaymentResponse> AtualizarCobrancaAsync(string idCobrancaAsaas, AsaasPaymentRequest request);
        Task<AsaasPaymentResponse> CancelarCobrancaAsync(string idCobrancaAsaas);
    }
}
