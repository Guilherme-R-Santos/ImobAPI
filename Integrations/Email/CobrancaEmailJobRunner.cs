using System.Globalization;
using System.Net.Mail;
using ImobAPI.Context;
using ImobAPI.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ImobAPI.Integrations.Email
{
    public class CobrancaEmailJobRunner
    {
        public const string Mensal = "Mensal";
        public const string Vencimento = "Vencimento";
        private readonly ImobContext _context;
        private readonly IEmailCobrancaSender _sender;
        private readonly EmailCobrancaOptions _options;
        private readonly ILogger<CobrancaEmailJobRunner> _logger;

        public CobrancaEmailJobRunner(ImobContext context, IEmailCobrancaSender sender, IOptions<EmailCobrancaOptions> options, ILogger<CobrancaEmailJobRunner> logger)
        {
            _context = context;
            _sender = sender;
            _options = options.Value;
            _logger = logger;
        }

        public async Task ExecutarAsync(string tipo, DateTime hoje, CancellationToken cancellationToken)
        {
            var mensal = tipo == Mensal;
            var inicio = mensal ? new DateTime(hoje.Year, hoje.Month, 1) : hoje.Date;
            var fim = mensal ? inicio.AddMonths(1) : inicio.AddDays(1);
            var assunto = mensal ? _options.AssuntoMensal : _options.AssuntoVencimento;
            var texto = mensal ? _options.TextoMensal : _options.TextoVencimento;
            var ids = await _context.Cobrancas.AsNoTracking()
                .Where(c => c.Ativo && !c.Pago && c.Status == "PENDING" && c.Vencimento >= inicio && c.Vencimento < fim)
                .Where(c => !_context.EnviosCobrancaEmail.Any(e => e.CobrancaId == c.Id && e.Tipo == tipo && e.Referencia == inicio))
                .Select(c => c.Id).ToListAsync(cancellationToken);

            foreach (var id in ids)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var cobranca = await _context.Cobrancas.AsNoTracking()
                    .Include(c => c.Contrato).ThenInclude(c => c.Contratante1)
                    .FirstOrDefaultAsync(c => c.Id == id && c.Ativo && !c.Pago && c.Status == "PENDING" && c.Vencimento >= inicio && c.Vencimento < fim, cancellationToken);
                if (cobranca == null) continue;
                var cliente = cobranca.Contrato?.Contratante1;
                if (cliente == null || !MailAddress.TryCreate(cliente.Email, out _) ||
                    !Uri.TryCreate(cobranca.LinkBoleto, UriKind.Absolute, out var link) || link.Scheme != Uri.UriSchemeHttps)
                {
                    _logger.LogWarning("Cobrança {CobrancaId} não enviada no job {Tipo}: cliente, e-mail ou link de pagamento ausente ou inválido.", id, tipo);
                    continue;
                }

                var envio = new EnvioCobrancaEmail
                {
                    CobrancaId = id,
                    Tipo = tipo,
                    Referencia = inicio,
                    Situacao = "EmEnvio",
                    IniciadoEmUtc = DateTime.UtcNow
                };
                _context.EnviosCobrancaEmail.Add(envio);
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
                {
                    _context.Entry(envio).State = EntityState.Detached;
                    if (!await _context.EnviosCobrancaEmail.AnyAsync(e => e.CobrancaId == id && e.Tipo == tipo && e.Referencia == inicio, cancellationToken)) throw;
                    continue;
                }

                try
                {
                    await _sender.EnviarAsync(cliente.Email, Renderizar(assunto, cobranca, cliente), Renderizar(texto, cobranca, cliente), cancellationToken);
                    envio.Situacao = "Enviado";
                    envio.ConcluidoEmUtc = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex, "Falha ou resultado incerto no envio da cobrança {CobrancaId}, job {Tipo}. Verifique antes de reenviar.", id, tipo);
                    envio.Situacao = "FalhaOuIncerto";
                    envio.ConcluidoEmUtc = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        private static string Renderizar(string modelo, Cobranca cobranca, Cliente cliente)
        {
            return modelo.Replace("{Cliente}", cliente.Nome ?? "", StringComparison.Ordinal)
                .Replace("{Cobranca}", cobranca.Nome ?? "", StringComparison.Ordinal)
                .Replace("{Vencimento}", cobranca.Vencimento.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal)
                .Replace("{Valor}", cobranca.Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR")), StringComparison.Ordinal)
                .Replace("{Link}", cobranca.LinkBoleto, StringComparison.Ordinal);
        }
    }
}
