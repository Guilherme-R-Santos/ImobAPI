using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImobAPI.Integrations.Asaas;
using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CobrancaController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        private readonly IAsaasService _asaasService;
        public CobrancaController(Context.ImobContext context, IAsaasService asaasService)
        {
            _context = context;
            _asaasService = asaasService;
        }
        [HttpPost("Criar")]
        public async Task<IActionResult> Create(Entities.Cobranca cobranca)
        {
            if ((cobranca.RepasseAutomatico || cobranca.ContaRepasse1 != null || cobranca.ContaRepasse2 != null) && !RepasseConfiguracao.Administrador(_context, User)) return Forbid();
            if (cobranca.RepasseAutomatico && cobranca.ContaRepasse1 == null && cobranca.ContaRepasse2 == null) return BadRequest("Informe ao menos um destinatário para o repasse automático.");
            var erroRepasse = RepasseConfiguracao.ValidarContas(cobranca.ContaRepasse1, cobranca.ContaRepasse2);
            if (erroRepasse != null) return BadRequest(erroRepasse);
            cobranca.Cadastrador = _context.Usuarios.Find(cobranca.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            cobranca.Contrato = _context.Contratos.Include(c => c.Contratante1).FirstOrDefault(c => c.Id == cobranca.Contrato.Id) ?? throw new Exception("Contrato não encontrado");
            cobranca.TipoCobranca = _context.TiposCobranca.Find(cobranca.TipoCobranca.Id) ?? throw new Exception("Tipo de cobrança não encontrado");

            var contratante = cobranca.Contrato.Contratante1 ?? throw new Exception("Contrato não possui Contratante1 definido");
            if (string.IsNullOrWhiteSpace(contratante.IdClienteAsaas))
            {
                return BadRequest("O Contratante1 deste contrato ainda não possui cadastro no Asaas (IdClienteAsaas vazio). Cadastre/atualize o cliente antes de criar a cobrança.");
            }

            await SincronizarComAsaasAsync(cobranca);

            cobranca.DataCadastro = DateTime.Now;
            cobranca.Ativo = true;
            _context.Cobrancas.Add(cobranca);
            _context.SaveChanges();

            if (!cobranca.SincronizadoAsaas)
            {
                return StatusCode(201, new { mensagem = "Cobrança criada localmente, mas houve falha ao sincronizar com o Asaas. Use o endpoint de retentativa quando possível.", detalhes = cobranca.ErroSincronizacaoAsaas, cobranca });
            }

            return Ok(cobranca);
        }

        public async Task<List<Entities.Cobranca>> GerarCobrancasDoContratoAsync(int contratoId)
        {
            var contrato = _context.Contratos
                .Include(c => c.Cadastrador)
                .Include(c => c.Contratante1)
                .FirstOrDefault(c => c.Id == contratoId) ?? throw new Exception("Contrato não encontrado");

            var tipoCobranca = _context.TiposCobranca.AsEnumerable()
                .FirstOrDefault(t => string.Equals(t.Nome?.Trim(), "boleto", StringComparison.OrdinalIgnoreCase))
                ?? _context.TiposCobranca.FirstOrDefault()
                ?? throw new Exception("Nenhum tipo de cobrança cadastrado");

            var cobrancasAntigas = _context.Cobrancas
                .Where(c => c.Contrato.Id == contrato.Id && c.Ativo && !c.Pago)
                .ToList();
            foreach (var antiga in cobrancasAntigas)
            {
                antiga.Ativo = false;
                antiga.DataInativacao = DateTime.Now;
                antiga.DataAtualizacao = DateTime.Now;
            }

            var inicio = contrato.DataInicioVigencia ?? DateTime.Today;
            var cobrancas = new List<Entities.Cobranca>();
            for (int i = 0; i < contrato.PrazoMeses; i++)
            {
                var mes = new DateTime(inicio.Year, inicio.Month, 1).AddMonths(i);
                var dia = Math.Clamp(contrato.Vencimento, 1, DateTime.DaysInMonth(mes.Year, mes.Month));
                var vencimento = new DateTime(mes.Year, mes.Month, dia);
                while (vencimento.DayOfWeek == DayOfWeek.Saturday || vencimento.DayOfWeek == DayOfWeek.Sunday)
                    vencimento = vencimento.AddDays(1);

                var cobranca = new Entities.Cobranca
                {
                    Cadastrador = contrato.Cadastrador,
                    Contrato = contrato,
                    TipoCobranca = tipoCobranca,
                    Nome = $"{contrato.Nome} - Parcela {i + 1}/{contrato.PrazoMeses}",
                    Valor = (double)contrato.ValorContrato,
                    Vencimento = vencimento,
                    RepasseAutomatico = contrato.RepasseAutomatico,
                    ContaRepasse1 = CopiarContaRepasse(contrato.ContaRepasse1),
                    ContaRepasse2 = CopiarContaRepasse(contrato.ContaRepasse2),
                    DataCadastro = DateTime.Now,
                    Ativo = true
                };

                if (string.IsNullOrWhiteSpace(contrato.Contratante1?.IdClienteAsaas))
                {
                    cobranca.SincronizadoAsaas = false;
                    cobranca.ErroSincronizacaoAsaas = "Contratante1 sem cadastro no Asaas (IdClienteAsaas vazio).";
                }
                else
                {
                    await SincronizarComAsaasAsync(cobranca);
                }

                _context.Cobrancas.Add(cobranca);
                cobrancas.Add(cobranca);
            }
            _context.SaveChanges();
            return cobrancas;
        }

        private static Entities.ContaRepasse CopiarContaRepasse(Entities.ContaRepasse conta)
        {
            if (conta == null) return null;
            return new Entities.ContaRepasse
            {
                Modalidade = conta.Modalidade,
                TipoValor = conta.TipoValor,
                Valor = conta.Valor,
                ChavePix = conta.ChavePix,
                TipoChavePix = conta.TipoChavePix,
                CodigoBanco = conta.CodigoBanco,
                NomeTitular = conta.NomeTitular,
                CpfCnpjTitular = conta.CpfCnpjTitular,
                DataNascimentoTitular = conta.DataNascimentoTitular,
                Agencia = conta.Agencia,
                Conta = conta.Conta,
                DigitoConta = conta.DigitoConta,
                TipoConta = conta.TipoConta
            };
        }

        private static AsaasPaymentRequest MontarAsaasRequest(Entities.Cobranca cobranca)
        {
            return new AsaasPaymentRequest
            {
                Customer = cobranca.Contrato.Contratante1.IdClienteAsaas,
                BillingType = MapBillingType(cobranca.TipoCobranca.Nome),
                Value = cobranca.Valor,
                DueDate = cobranca.Vencimento?.ToString("yyyy-MM-dd"),
                Description = cobranca.Nome,
                ExternalReference = cobranca.Contrato.Id.ToString()
            };
        }

        private async Task SincronizarComAsaasAsync(Entities.Cobranca cobranca)
        {
            var asaasRequest = MontarAsaasRequest(cobranca);

            try
            {
                if (string.IsNullOrWhiteSpace(cobranca.IdCobrancaAsaas))
                {
                    var asaasCriacao = await _asaasService.CriarCobrancaAsync(asaasRequest);
                    cobranca.IdCobrancaAsaas = asaasCriacao.Id;
                    cobranca.Status = asaasCriacao.Status;
                    cobranca.LinkBoleto = asaasCriacao.BankSlipUrl ?? asaasCriacao.InvoiceUrl;
                    cobranca.NossoNumero = asaasCriacao.NossoNumero;
                    cobranca.valorLiquido = asaasCriacao.NetValue;
                }
                else
                {
                    var asaasAtualizacao = await _asaasService.AtualizarCobrancaAsync(cobranca.IdCobrancaAsaas, asaasRequest);
                    cobranca.Status = asaasAtualizacao.Status;
                    cobranca.LinkBoleto = asaasAtualizacao.BankSlipUrl ?? asaasAtualizacao.InvoiceUrl;
                    cobranca.NossoNumero = asaasAtualizacao.NossoNumero;
                    cobranca.valorLiquido = asaasAtualizacao.NetValue;
                }

                cobranca.SincronizadoAsaas = true;
                cobranca.ErroSincronizacaoAsaas = null;
            }
            catch (AsaasIntegrationException ex)
            {
                cobranca.SincronizadoAsaas = false;
                cobranca.ErroSincronizacaoAsaas = ex.Message;
            }
        }

        private static string MapBillingType(string tipoCobrancaNome)
        {
            return tipoCobrancaNome?.Trim().ToLowerInvariant() switch
            {
                "boleto" => "BOLETO",
                "pix" => "PIX",
                "cartão" or "cartao" => "CREDIT_CARD",
                "ted" => "UNDEFINED",
                _ => "UNDEFINED"
            };
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var cobrancas = _context.Cobrancas.Where(c => c.Ativo)
                .Include(i => i.Contrato)
                .Include(i => i.TipoCobranca)
                .ToList();
            return Ok(cobrancas);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var existingCobranca = _context.Cobrancas.Find(id);
            if (existingCobranca == null || existingCobranca.Ativo == false)
            {
                return NotFound("Cobrança não encontrada.");
            }
            return Ok(existingCobranca);
        }
        [HttpGet("ObterPorContrato/{contratoId}")]
        public IActionResult GetByContrato(int contratoId)
        {
            var cobrancas = _context.Cobrancas.Where(c => c.Contrato.Id == contratoId && c.Ativo).ToList();
            return Ok(cobrancas);
        }
        [HttpGet("ObterPorTipo/{tipoId}")]
        public IActionResult GetByTipo(int tipoId)
        {
            var cobrancas = _context.Cobrancas.Where(c => c.TipoCobranca.Id == tipoId && c.Ativo).ToList();
            return Ok(cobrancas);
        }
        [HttpGet("ObterPorStatus/{status}")]
        public IActionResult GetByStatus(string status)
        {
            var cobrancas = _context.Cobrancas.Where(c => c.Status == status && c.Ativo).ToList();
            return Ok(cobrancas);
        }
        [HttpPut("Inativar/{id}")]
        public async Task<IActionResult> Inactivate(int id)
        {
            var existingCobranca = _context.Cobrancas.Find(id);
            if (existingCobranca == null)
            {
                return NotFound("Cobrança não encontrada.");
            }

            var erroCancelamentoAsaas = await InativarCobrancaAsync(existingCobranca);
            _context.SaveChanges();

            if (erroCancelamentoAsaas != null)
            {
                return StatusCode(502, new { mensagem = "Cobrança inativada localmente, mas houve falha ao cancelar no Asaas.", detalhes = erroCancelamentoAsaas.Message, cobranca = existingCobranca });
            }

            return Ok(existingCobranca);
        }

        [HttpPut("InativarLista")]
        public async Task<IActionResult> InactivateList([FromBody] List<int> cobrancaIds)
        {
            if (cobrancaIds == null || cobrancaIds.Count == 0)
            {
                return BadRequest("Informe ao menos um ID de cobrança.");
            }

            var resultados = new List<object>();
            var houveFalha = false;
            foreach (var id in cobrancaIds.Distinct())
            {
                var cobranca = _context.Cobrancas.Find(id);
                if (cobranca == null)
                {
                    houveFalha = true;
                    resultados.Add(new { id, inativada = false, mensagem = "Cobrança não encontrada." });
                    continue;
                }

                var erroCancelamentoAsaas = await InativarCobrancaAsync(cobranca);
                if (erroCancelamentoAsaas != null)
                {
                    houveFalha = true;
                    resultados.Add(new { id, inativada = true, sincronizadoAsaas = false, detalhes = erroCancelamentoAsaas.Message, cobranca });
                }
                else
                {
                    resultados.Add(new { id, inativada = true, sincronizadoAsaas = true, cobranca });
                }
            }

            _context.SaveChanges();
            return houveFalha ? StatusCode(207, resultados) : Ok(resultados);
        }

        private async Task<AsaasIntegrationException?> InativarCobrancaAsync(Entities.Cobranca cobranca)
        {
            AsaasIntegrationException? erroCancelamentoAsaas = null;
            if (!string.IsNullOrWhiteSpace(cobranca.IdCobrancaAsaas))
            {
                try
                {
                    var respostaAsaas = await _asaasService.CancelarCobrancaAsync(cobranca.IdCobrancaAsaas);
                    if (!respostaAsaas.Deleted)
                    {
                        throw new AsaasIntegrationException("O Asaas não confirmou o cancelamento da cobrança.", 502);
                    }

                    cobranca.Status = "DELETED";
                    cobranca.SincronizadoAsaas = true;
                    cobranca.ErroSincronizacaoAsaas = null;
                }
                catch (AsaasIntegrationException ex)
                {
                    erroCancelamentoAsaas = ex;
                    cobranca.SincronizadoAsaas = false;
                    cobranca.ErroSincronizacaoAsaas = ex.Message;
                }
            }

            cobranca.Ativo = false;
            cobranca.DataInativacao = DateTime.Now;
            cobranca.DataAtualizacao = DateTime.Now;
            return erroCancelamentoAsaas;
        }

        [HttpPut("Atualizar/{id}")]
        public async Task<IActionResult> Update(int id, Entities.Cobranca updatedCobranca)
        {
            var existingCobranca = _context.Cobrancas
                .Include(c => c.Contrato).ThenInclude(ct => ct.Contratante1)
                .Include(c => c.TipoCobranca)
                .FirstOrDefault(c => c.Id == id);
            if (existingCobranca == null)
            {
                return NotFound("Cobrança não encontrada.");
            }
            if ((updatedCobranca.RepasseAutomatico || updatedCobranca.ContaRepasse1 != null || updatedCobranca.ContaRepasse2 != null || existingCobranca.ContaRepasse1 != null || existingCobranca.ContaRepasse2 != null) && !RepasseConfiguracao.Administrador(_context, User)) return Forbid();
            if (updatedCobranca.RepasseAutomatico && updatedCobranca.ContaRepasse1 == null && updatedCobranca.ContaRepasse2 == null) return BadRequest("Informe ao menos um destinatário para o repasse automático.");
            var erroRepasse = RepasseConfiguracao.ValidarContas(updatedCobranca.ContaRepasse1, updatedCobranca.ContaRepasse2);
            if (erroRepasse != null) return BadRequest(erroRepasse);
            existingCobranca.TipoCobranca = _context.TiposCobranca.Find(updatedCobranca.TipoCobranca.Id) ?? throw new Exception("Tipo de cobrança não encontrado");
            existingCobranca.Contrato = _context.Contratos.Include(c => c.Contratante1).FirstOrDefault(c => c.Id == updatedCobranca.Contrato.Id) ?? throw new Exception("Contrato não encontrado");
            existingCobranca.Nome = updatedCobranca.Nome;
            existingCobranca.Valor = updatedCobranca.Valor;
            existingCobranca.Vencimento = updatedCobranca.Vencimento;
            existingCobranca.RepasseAutomatico = updatedCobranca.RepasseAutomatico;
            existingCobranca.ContaRepasse1 = updatedCobranca.ContaRepasse1;
            existingCobranca.ContaRepasse2 = updatedCobranca.ContaRepasse2;
            existingCobranca.ComprovanteEnviado = updatedCobranca.ComprovanteEnviado;

            var contratante = existingCobranca.Contrato.Contratante1 ?? throw new Exception("Contrato não possui Contratante1 definido");
            if (string.IsNullOrWhiteSpace(contratante.IdClienteAsaas))
            {
                return BadRequest("O Contratante1 deste contrato ainda não possui cadastro no Asaas (IdClienteAsaas vazio). Cadastre/atualize o cliente antes de atualizar a cobrança.");
            }

            await SincronizarComAsaasAsync(existingCobranca);

            existingCobranca.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();

            if (!existingCobranca.SincronizadoAsaas)
            {
                return Ok(new { mensagem = "Cobrança atualizada localmente, mas houve falha ao sincronizar com o Asaas. Use o endpoint de retentativa quando possível.", detalhes = existingCobranca.ErroSincronizacaoAsaas, cobranca = existingCobranca });
            }

            return Ok(existingCobranca);
        }
        [HttpPost("RetentarSincronizacaoAsaas/{id}")]
        public async Task<IActionResult> RetentarSincronizacaoAsaas(int id)
        {
            var cobranca = _context.Cobrancas
                .Include(c => c.Contrato).ThenInclude(ct => ct.Contratante1)
                .Include(c => c.TipoCobranca)
                .FirstOrDefault(c => c.Id == id);
            if (cobranca == null)
            {
                return NotFound("Cobrança não encontrada.");
            }

            var contratante = cobranca.Contrato.Contratante1 ?? throw new Exception("Contrato não possui Contratante1 definido");
            if (string.IsNullOrWhiteSpace(contratante.IdClienteAsaas))
            {
                return BadRequest("O Contratante1 deste contrato ainda não possui cadastro no Asaas (IdClienteAsaas vazio). Cadastre/atualize o cliente antes de retentar a sincronização.");
            }

            await SincronizarComAsaasAsync(cobranca);
            cobranca.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();

            if (!cobranca.SincronizadoAsaas)
            {
                return StatusCode(502, new { mensagem = "Falha ao sincronizar cobrança com o Asaas.", detalhes = cobranca.ErroSincronizacaoAsaas, cobranca });
            }

            return Ok(cobranca);
        }
        [HttpPut("Ativar/{id}")]
        public IActionResult Activate(int id)
        {
            var existingCobranca = _context.Cobrancas.Find(id);
            if (existingCobranca == null)
            {
                return NotFound("Cobrança não encontrada.");
            }
            existingCobranca.Ativo = true;
            existingCobranca.DataAtualizacao = DateTime.Now;
            existingCobranca.DataInativacao = null;
            _context.SaveChanges();
            return Ok(existingCobranca);
        }
    }
}
