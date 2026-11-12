using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace ImobAPI.Integrations.Email
{
    public abstract class CobrancaEmailJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmailCobrancaOptions _options;
        private readonly ILogger _logger;
        protected abstract string Tipo { get; }

        protected CobrancaEmailJob(IServiceScopeFactory scopeFactory, IOptions<EmailCobrancaOptions> options, ILogger logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Habilitado)
            {
                _logger.LogInformation("Job de e-mail {Tipo} desabilitado.", Tipo);
                return;
            }

            var assunto = Tipo == CobrancaEmailJobRunner.Mensal ? _options.AssuntoMensal : _options.AssuntoVencimento;
            var texto = Tipo == CobrancaEmailJobRunner.Mensal ? _options.TextoMensal : _options.TextoVencimento;
            if (_options.HoraEnvio < 0 || _options.HoraEnvio > 23 ||
                !MailAddress.TryCreate(_options.ContaGoogle, out _) || string.IsNullOrWhiteSpace(_options.SenhaAplicativo) ||
                string.IsNullOrWhiteSpace(assunto) || string.IsNullOrWhiteSpace(texto) ||
                (Tipo == CobrancaEmailJobRunner.Mensal && !texto.Contains("{Link}", StringComparison.Ordinal)))
            {
                _logger.LogError("Job de e-mail {Tipo} não iniciado: configure conta Google, senha de aplicativo, horário, assunto e texto válidos; o texto mensal deve incluir {Marcador}.", Tipo, "{Link}");
                return;
            }

            TimeZoneInfo fuso;
            try
            {
                fuso = TimeZoneInfo.FindSystemTimeZoneById(_options.FusoHorario);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException || ex is ArgumentException)
            {
                _logger.LogError(ex, "Job de e-mail {Tipo} não iniciado: fuso horário inválido.", Tipo);
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var agora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, fuso);
                    if (DeveExecutar(Tipo, agora, _options.HoraEnvio))
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var runner = scope.ServiceProvider.GetRequiredService<CobrancaEmailJobRunner>();
                        await runner.ExecutarAsync(Tipo, agora.Date, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no job de e-mail {Tipo}; nova verificação em um minuto.", Tipo);
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public static bool DeveExecutar(string tipo, DateTime agora, int horaEnvio)
        {
            return agora.Hour >= horaEnvio && (tipo != CobrancaEmailJobRunner.Mensal || agora.Day == 1);
        }
    }

    public sealed class CobrancaMensalEmailJob : CobrancaEmailJob
    {
        protected override string Tipo => CobrancaEmailJobRunner.Mensal;
        public CobrancaMensalEmailJob(IServiceScopeFactory scopeFactory, IOptions<EmailCobrancaOptions> options, ILogger<CobrancaMensalEmailJob> logger)
            : base(scopeFactory, options, logger) { }
    }

    public sealed class CobrancaVencimentoEmailJob : CobrancaEmailJob
    {
        protected override string Tipo => CobrancaEmailJobRunner.Vencimento;
        public CobrancaVencimentoEmailJob(IServiceScopeFactory scopeFactory, IOptions<EmailCobrancaOptions> options, ILogger<CobrancaVencimentoEmailJob> logger)
            : base(scopeFactory, options, logger) { }
    }
}
