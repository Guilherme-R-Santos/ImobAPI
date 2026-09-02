using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Integrations.Asaas
{
    public class AsaasIntegrationException : Exception
    {
        public int? StatusCode { get; }
        public List<AsaasError> Errors { get; }

        public AsaasIntegrationException(string message, int? statusCode = null, List<AsaasError> errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors ?? new List<AsaasError>();
        }
    }
}
