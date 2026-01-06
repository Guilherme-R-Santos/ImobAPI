using ImobAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TipoClienteController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoClienteController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("CriarTipo")]
        public IActionResult Create(Entities.TipoCliente tipoCliente)
        {
            tipoCliente.Cadastrador = _context.Usuarios.Find(tipoCliente.Cadastrador.Id);
            if (tipoCliente.Cadastrador == null)
            {
                return BadRequest("Usuário cadastrador não encontrado.");
            }
            tipoCliente.DataCadastro = DateTime.Now;
            tipoCliente.Ativo = true;
            _context.TiposCliente.Add(tipoCliente);
            _context.SaveChanges();
            return Ok(tipoCliente);
        }
        [HttpGet("ListarTipos")]
        public IActionResult GetAll()
        {
            var tiposCliente = _context.TiposCliente.ToList();
            return Ok(tiposCliente);
        }
        [HttpGet("ObterTipo/{id}")]
        public IActionResult GetById(int id)
        {
            var existingTipoCliente = _context.TiposCliente.Find(id);
            if (existingTipoCliente == null || existingTipoCliente.Ativo == false)
            {
                return NotFound("Tipo de cliente não encontrado.");
            }
            return Ok(existingTipoCliente);
        }
        [HttpPut("AtualizarTipo/{id}")]
        public IActionResult Update(int id, Entities.TipoCliente updatedTipoCliente)
        {
            var existingTipoCliente = _context.TiposCliente.Find(id);
            if (existingTipoCliente == null)
            {
                return NotFound("Tipo de cliente não encontrado.");
            }
            existingTipoCliente.Nome = updatedTipoCliente.Nome;
            existingTipoCliente.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoCliente);
        }
        [HttpPut("InativarTipo/{id}")]
        public IActionResult Update(int id)
        {
            var existingTipoCliente = _context.TiposCliente.Find(id);
            if (existingTipoCliente == null)
            {
                return NotFound("Tipo de cliente não encontrado.");
            }
            existingTipoCliente.Ativo = false;
            existingTipoCliente.DataInativacao = DateTime.Now;
            existingTipoCliente.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoCliente);
        }
        [HttpPut("AtivarTipo/{id}")]
        public IActionResult UpdateAtivacao(int id)
        {
            var existingTipoCliente = _context.TiposCliente.Find(id);
            if (existingTipoCliente == null)
            {
                return NotFound("Tipo de cliente não encontrado.");
            }
            existingTipoCliente.Ativo = true;
            existingTipoCliente.DataAtualizacao = DateTime.Now;
            existingTipoCliente.DataInativacao = null;
            _context.SaveChanges();
            return Ok(existingTipoCliente);
        }
    }
}
