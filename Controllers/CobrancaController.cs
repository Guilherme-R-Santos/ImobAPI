using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CobrancaController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public CobrancaController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Cobranca cobranca)
        {
            cobranca.Cadastrador = _context.Usuarios.Find(cobranca.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            cobranca.Contrato = _context.Contratos.Find(cobranca.Contrato.Id) ?? throw new Exception("Contrato não encontrado");
            cobranca.TipoCobranca = _context.TiposCobranca.Find(cobranca.TipoCobranca.Id) ?? throw new Exception("Tipo de cobrança não encontrado");
            cobranca.DataCadastro = DateTime.Now;
            cobranca.Ativo = true;
            _context.Cobrancas.Add(cobranca);
            _context.SaveChanges();
            return Ok(cobranca);
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
        public IActionResult Update(int id, Entities.Cobranca updatedCobranca)
        {
            var existingCobranca = _context.Cobrancas.Find(id);
            if (existingCobranca == null)
            {
                return NotFound("Cobrança não encontrada.");
            }
            existingCobranca.TipoCobranca = _context.TiposCobranca.Find(updatedCobranca.TipoCobranca.Id) ?? throw new Exception("Tipo de cobrança não encontrado");
            existingCobranca.Contrato = _context.Contratos.Find(updatedCobranca.Contrato.Id) ?? throw new Exception("Contrato não encontrado");
            existingCobranca.Nome = updatedCobranca.Nome;
            existingCobranca.Valor = updatedCobranca.Valor;
            existingCobranca.Vencimento = updatedCobranca.Vencimento;
            existingCobranca.PartilhaAutomatica = updatedCobranca.PartilhaAutomatica;
            existingCobranca.ContaPartilha = updatedCobranca.ContaPartilha;
            existingCobranca.ComprovanteEnviado = updatedCobranca.ComprovanteEnviado;
            existingCobranca.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingCobranca);
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
