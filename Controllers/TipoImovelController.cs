using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TipoImovelController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoImovelController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.TipoImovel tipoImovel)
        {
            tipoImovel.DataCadastro = DateTime.Now;
            tipoImovel.Ativo = true;
            tipoImovel.Cadastrador = _context.Usuarios.Find(tipoImovel.Cadastrador.Id);
            if (tipoImovel.Cadastrador == null)
            {
                return BadRequest("Usuário cadastrador não encontrado.");
            }
            _context.TiposImovel.Add(tipoImovel);
            _context.SaveChanges();
            return Ok(tipoImovel);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var tiposImovel = _context.TiposImovel.Where(t => t.Ativo).ToList();
            return Ok(tiposImovel);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var existingTipoImovel = _context.TiposImovel.Find(id);
            if (existingTipoImovel == null || existingTipoImovel.Ativo == false)
            {
                return NotFound("Tipo de imóvel não encontrado.");
            }
            return Ok(existingTipoImovel);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingTipoImovel = _context.TiposImovel.Find(id);
            if (existingTipoImovel == null)
            {
                return NotFound("Tipo de imóvel não encontrado.");
            }
            existingTipoImovel.Ativo = false;
            existingTipoImovel.DataInativacao = DateTime.Now;
            existingTipoImovel.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoImovel);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.TipoImovel updatedTipoImovel)
        {
            var existingTipoImovel = _context.TiposImovel.Find(id);
            if (existingTipoImovel == null)
            {
                return NotFound("Tipo de imóvel não encontrado.");
            }
            existingTipoImovel.Nome = updatedTipoImovel.Nome;
            existingTipoImovel.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoImovel);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetByName(string nome)
        {
            var tiposImovel = _context.TiposImovel
                .Where(t => t.Nome.ToLower().Contains(nome.ToLower()) && t.Ativo)
                .ToList();
            return Ok(tiposImovel);
        }
    }
}
