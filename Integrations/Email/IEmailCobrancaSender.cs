namespace ImobAPI.Integrations.Email
{
    public interface IEmailCobrancaSender
    {
        Task EnviarAsync(string destinatario, string assunto, string texto, CancellationToken cancellationToken);
    }
}
