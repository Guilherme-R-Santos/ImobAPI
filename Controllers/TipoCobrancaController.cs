using ImobAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TipoCobrancaController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoCobrancaController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("CriarTipo")]
        public IActionResult Create(Entities.TipoCobranca TipoCobranca)
        {
            TipoCobranca.Cadastrador = _context.Usuarios.Find(TipoCobranca.Cadastrador.Id);
            if (TipoCobranca.Cadastrador == null)
            {
                return BadRequest("Usuário cadastrador não encontrado.");
            }
            TipoCobranca.DataCadastro = DateTime.Now;
            TipoCobranca.Ativo = true;
            _context.TiposCobranca.Add(TipoCobranca);
            _context.SaveChanges();
            return Ok(TipoCobranca);
        }
        [HttpGet("ListarTipos")]
        public IActionResult GetAll()
        {
            var tiposCobranca = _context.TiposCobranca.Where(tc => tc.Ativo).ToList();
            return Ok(tiposCobranca);
        }
        [HttpGet("ObterTipo/{id}")]
        public IActionResult GetById(int id)
        {
            var existingTipoCobranca = _context.TiposCobranca.Find(id);
            if (existingTipoCobranca == null || existingTipoCobranca.Ativo == false)
            {
                return NotFound("Tipo de cobrança não encontrado.");
            }
            return Ok(existingTipoCobranca);
        }
        [HttpPut("AtualizarTipo/{id}")]
        public IActionResult Update(int id, Entities.TipoCobranca updatedTipoCobranca)
        {
            var existingTipoCobranca = _context.TiposCobranca.Find(id);
            if (existingTipoCobranca == null)
            {
                return NotFound("Tipo de cobrança não encontrado.");
            }
            existingTipoCobranca.Nome = updatedTipoCobranca.Nome;
            existingTipoCobranca.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoCobranca);
        }
        [HttpPut("InativarTipo/{id}")]
        public IActionResult Update(int id)
        {
            var existingTipoCobranca = _context.TiposCobranca.Find(id);
            if (existingTipoCobranca == null)
            {
                return NotFound("Tipo de cliente não encontrado.");
            }
            existingTipoCobranca.Ativo = false;
            existingTipoCobranca.DataInativacao = DateTime.Now;
            existingTipoCobranca.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoCobranca);
        }
        [HttpPut("AtivarTipo/{id}")]
        public IActionResult UpdateAtivacao(int id)
        {
            var existingTipoCobranca = _context.TiposCobranca.Find(id);
            if (existingTipoCobranca == null)
            {
                return NotFound("Tipo de cobrança não encontrado.");
            }
            existingTipoCobranca.Ativo = true;
            existingTipoCobranca.DataAtualizacao = DateTime.Now;
            existingTipoCobranca.DataInativacao = null;
            _context.SaveChanges();
            return Ok(existingTipoCobranca);
        }
    }
}
