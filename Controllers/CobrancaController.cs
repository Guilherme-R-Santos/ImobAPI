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
            var cobrancas = _context.Cobrancas.Where(c => c.Ativo).ToList();
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
        public IActionResult Inactivate(int id)
        {
            var existingCobranca = _context.Cobrancas.Find(id);
            if (existingCobranca == null)
            {
                return NotFound("Cobrança não encontrada.");
            }
            existingCobranca.Ativo = false;
            existingCobranca.DataInativacao = DateTime.Now;
            existingCobranca.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingCobranca);
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
            existingCobranca.TipoCobranca = _context.TiposCobranca.Find(updatedCobranca.TipoCobranca.Id) ?? throw new Exception("Tipo de cobrança não encontrado");
            existingCobranca.Contrato = _context.Contratos.Include(c => c.Contratante1).FirstOrDefault(c => c.Id == updatedCobranca.Contrato.Id) ?? throw new Exception("Contrato não encontrado");
            existingCobranca.Nome = updatedCobranca.Nome;
            existingCobranca.Valor = updatedCobranca.Valor;
            existingCobranca.Vencimento = updatedCobranca.Vencimento;
            existingCobranca.PartilhaAutomatica = updatedCobranca.PartilhaAutomatica;
            existingCobranca.ContaPartilha = updatedCobranca.ContaPartilha;
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
