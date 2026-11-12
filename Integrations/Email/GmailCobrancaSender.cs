using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace ImobAPI.Integrations.Email
{
    public class GmailCobrancaSender : IEmailCobrancaSender
    {
        private readonly EmailCobrancaOptions _options;

        public GmailCobrancaSender(IOptions<EmailCobrancaOptions> options)
        {
            _options = options.Value;
        }

        public async Task EnviarAsync(string destinatario, string assunto, string texto, CancellationToken cancellationToken)
        {
            using var mensagem = new MailMessage(new MailAddress(_options.ContaGoogle), new MailAddress(destinatario))
            {
                Subject = assunto,
                SubjectEncoding = Encoding.UTF8,
                Body = texto,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = false
            };
            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_options.ContaGoogle, _options.SenhaAplicativo),
                Timeout = 30000
            };
            await smtp.SendMailAsync(mensagem, cancellationToken);
        }
    }
}
