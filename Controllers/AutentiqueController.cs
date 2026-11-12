using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ImobAPI.Context;
using ImobAPI.Entities;
using ImobAPI.Integrations.Autentique;
using ImobAPI.Integrations.Autentique.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutentiqueController : ControllerBase
    {
        private const int TamanhoMaximoPdf = 20 * 1024 * 1024;
        private static readonly HashSet<string> Acoes = new(StringComparer.Ordinal)
        {
            "SIGN", "APPROVE", "RECOGNIZE", "SIGN_AS_A_WITNESS",
            "ACKNOWLEDGE_RECEIPT", "ENDORSE_IN_BLACK", "ENDORSE_IN_WHITE"
        };
        private readonly AutentiqueService _service;
        private readonly ImobContext _context;
        private readonly ILogger<AutentiqueController> _logger;

        public AutentiqueController(AutentiqueService service, ImobContext context, ILogger<AutentiqueController> logger)
        {
            _service = service;
            _context = context;
            _logger = logger;
        }

        [HttpPost("CriarDocumento")]
        [RequestSizeLimit(29_000_000)]
        public async Task<IActionResult> CriarDocumento([FromBody] CriarDocumentoRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nome) || request.Arquivo == null ||
                request.Arquivo.Length < 5 || request.Arquivo.Length > TamanhoMaximoPdf ||
                !request.Arquivo.AsSpan(0, 5).SequenceEqual("%PDF-"u8) ||
                request.Signatarios == null || request.Signatarios.Count == 0)
                return BadRequest("Informe o nome, um PDF válido de até 20 MiB e ao menos um signatário.");

            foreach (var signatario in request.Signatarios)
            {
                if (signatario == null || !Acoes.Contains(signatario.Acao ?? string.Empty))
                    return BadRequest("Informe uma ação válida para cada signatário.");

                var temNome = !string.IsNullOrWhiteSpace(signatario.Nome);
                var temEmail = !string.IsNullOrWhiteSpace(signatario.Email);
                var temTelefone = !string.IsNullOrWhiteSpace(signatario.Telefone);
                if ((temNome ? 1 : 0) + (temEmail ? 1 : 0) + (temTelefone ? 1 : 0) != 1)
                    return BadRequest("Informe exatamente um identificador por signatário: nome, e-mail ou telefone.");
                if (temEmail && !new EmailAddressAttribute().IsValid(signatario.Email.Trim()))
                    return BadRequest("E-mail do signatário inválido.");
                if (temTelefone && !Regex.IsMatch(signatario.Telefone.Trim(), @"^\+[1-9][0-9]{7,14}$"))
                    return BadRequest("Informe o telefone no formato internacional, por exemplo +5521999999999.");
                if (temTelefone && signatario.MetodoEntrega != "DELIVERY_METHOD_SMS" && signatario.MetodoEntrega != "DELIVERY_METHOD_WHATSAPP")
                    return BadRequest("Para telefone, informe DELIVERY_METHOD_SMS ou DELIVERY_METHOD_WHATSAPP.");
                if (!temTelefone && !string.IsNullOrWhiteSpace(signatario.MetodoEntrega))
                    return BadRequest("O método de entrega só pode ser informado com telefone.");
            }

            var emails = request.Signatarios.Where(s => !string.IsNullOrWhiteSpace(s.Email)).Select(s => s.Email.Trim()).ToArray();
            if (emails.Distinct(StringComparer.OrdinalIgnoreCase).Count() != emails.Length)
                return BadRequest("Não repita o mesmo e-mail na lista de signatários.");

            (string apiKey, bool sandbox) configuracao;
            try
            {
                configuracao = _service.ObterConfiguracao();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Configuração da Autentique inválida: {Erro}", ex.Message);
                return StatusCode(503, new { mensagem = "Integração Autentique não configurada corretamente." });
            }

            var reservado = await _context.Contratos
                .Where(c => c.Id == request.ContratoId && c.Ativo && c.IdDocumentoAutentique == null && !c.EnvioAutentiqueEmAndamento)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.EnvioAutentiqueEmAndamento, true), cancellationToken);
            if (reservado == 0)
            {
                if (!await _context.Contratos.AnyAsync(c => c.Id == request.ContratoId && c.Ativo, cancellationToken))
                    return NotFound("Contrato ativo não encontrado.");
                return Conflict("O contrato já possui documento ou há um envio pendente de conciliação.");
            }

            var contrato = await _context.Contratos.SingleAsync(c => c.Id == request.ContratoId, CancellationToken.None);
            var testemunhas = new[] { contrato.Testemunha1, contrato.Testemunha2, contrato.Testemunha3 }
                .Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.Trim()).ToArray();
            if (testemunhas.Any(e => request.Signatarios.Any(s => string.Equals(s.Email?.Trim(), e, StringComparison.OrdinalIgnoreCase))))
            {
                contrato.EnvioAutentiqueEmAndamento = false;
                await _context.SaveChangesAsync(CancellationToken.None);
                return BadRequest("As testemunhas cadastradas no contrato são incluídas automaticamente e não devem ser repetidas nos signatários.");
            }
            request.Signatarios.AddRange(testemunhas.Select(email => new SignatarioRequest { Email = email, Acao = "SIGN_AS_A_WITNESS" }));

            CriarDocumentoResponse documento = null;
            try
            {
                documento = await _service.CriarDocumentoAsync(request, configuracao.apiKey, configuracao.sandbox, cancellationToken);
                if (documento.Id.Length > 128 || documento.Assinaturas == null || documento.Assinaturas.Count < request.Signatarios.Count || documento.Assinaturas.Any(a => a == null || string.IsNullOrWhiteSpace(a.IdPublico) || a.IdPublico.Length > 128))
                    throw new AutentiqueIntegrationException("Resposta da Autentique sem identificadores válidos de assinatura.", documento.Id);

                var acoesPorEmail = request.Signatarios.Where(s => !string.IsNullOrWhiteSpace(s.Email))
                    .ToDictionary(s => s.Email.Trim(), s => s.Acao, StringComparer.OrdinalIgnoreCase);
                var registro = new DocumentoAutentique
                {
                    IdDocumentoAutentique = documento.Id,
                    ContratoId = contrato.Id,
                    Nome = request.Nome.Trim(),
                    Sandbox = configuracao.sandbox,
                    CriadoEmUtc = LerData(documento.CriadoEm),
                    UltimaSincronizacaoEmUtc = DateTimeOffset.UtcNow,
                    Signatarios = documento.Assinaturas.Select(a => new SignatarioAutentique
                    {
                        IdDocumentoAutentique = documento.Id,
                        PublicIdAutentique = a.IdPublico,
                        Nome = a.Nome,
                        Email = a.Email,
                        Acao = !string.IsNullOrWhiteSpace(a.Email) && acoesPorEmail.TryGetValue(a.Email.Trim(), out var acao) ? acao : a.Acao?.Nome,
                        CriadoEmUtc = LerData(a.CriadoEm)
                    }).ToList()
                };
                _context.DocumentosAutentique.Add(registro);
                contrato.IdDocumentoAutentique = documento.Id;
                contrato.EnvioAutentiqueEmAndamento = false;
                await _context.SaveChangesAsync(CancellationToken.None);
                return Ok(new { sandbox = configuracao.sandbox, documento });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Não foi possível vincular o documento Autentique {DocumentoId} ao contrato {ContratoId}.", documento?.Id, request.ContratoId);
                return StatusCode(500, new { mensagem = "Documento criado, mas não foi possível salvar o vínculo. Concilie antes de reenviar.", documentoId = documento?.Id });
            }
            catch (AutentiqueIntegrationException ex)
            {
                _logger.LogWarning("Falha na criação de documento na Autentique: {Erro}; documento: {DocumentoId}", ex.Message, ex.DocumentoId ?? documento?.Id);
                return StatusCode(502, new { mensagem = ex.Message, detalhes = ex.Detalhes, documentoId = ex.DocumentoId ?? documento?.Id, aviso = "Confira o resultado na Autentique antes de reenviar; o documento pode ter sido criado." });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Falha de comunicação com a Autentique.");
                return StatusCode(502, new { mensagem = "Falha de comunicação com a Autentique. Confira se o documento foi criado antes de reenviar." });
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Tempo limite ao enviar documento à Autentique.");
                return StatusCode(504, new { mensagem = "Tempo limite ao chamar a Autentique. Confira se o documento foi criado antes de reenviar." });
            }
        }

        [HttpGet("DocumentosDoContrato/{contratoId:int}")]
        public async Task<IActionResult> DocumentosDoContrato(int contratoId, CancellationToken cancellationToken)
        {
            if (!await _context.Contratos.AnyAsync(c => c.Id == contratoId, cancellationToken))
                return NotFound("Contrato não encontrado.");
            var documentos = await _context.DocumentosAutentique.AsNoTracking().Include(d => d.Signatarios)
                .Where(d => d.ContratoId == contratoId).OrderByDescending(d => d.CriadoEmUtc).ToListAsync(cancellationToken);
            return Ok(documentos);
        }

        private static DateTimeOffset? LerData(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (!DateTimeOffset.TryParse(valor, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal, out var data))
                throw new AutentiqueIntegrationException("A Autentique retornou uma data inválida.");
            return data.ToUniversalTime();
        }
    }
}
