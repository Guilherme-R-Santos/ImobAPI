using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TipoFotoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoFotoController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.TipoFoto tipoFoto)
        {
            if (tipoFoto.Cadastrador == null)
            {
                return BadRequest("Cadastrador inválido.");
            }
            tipoFoto.DataCadastro = DateTime.Now;
            tipoFoto.Ativo = true;
            tipoFoto.Cadastrador = _context.Usuarios.Find(tipoFoto.Cadastrador.Id);
            if (tipoFoto.Cadastrador == null)
            {
                return BadRequest("Cadastrador inválido.");
            }
            _context.TiposFoto.Add(tipoFoto);
            _context.SaveChanges();
            return Ok(tipoFoto);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var tiposFoto = _context.TiposFoto
                .Where(tf => tf.Ativo)
                .ToList();
            return Ok(tiposFoto);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var tipoFoto = _context.TiposFoto.Find(id);
            if (tipoFoto == null || !tipoFoto.Ativo)
            {
                return NotFound("Tipo de foto não encontrado.");
            }
            return Ok(tipoFoto);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetByName(string nome)
        {
            var tiposFoto = _context.TiposFoto
                .Where(tf => tf.Nome.ToLower().Contains(nome.ToLower()) && tf.Ativo)
                .ToList();
            return Ok(tiposFoto);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.TipoFoto updatedTipoFoto)
        {
            var tipoFoto = _context.TiposFoto.Find(id);
            if (tipoFoto == null)
            {
                return NotFound("Tipo de foto não encontrado.");
            }
            tipoFoto.Nome = updatedTipoFoto.Nome;
            tipoFoto.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(tipoFoto);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var tipoFoto = _context.TiposFoto.Find(id);
            if (tipoFoto == null)
            {
                return NotFound("Tipo de foto não encontrado.");
            }
            var agora = DateTime.Now;
            tipoFoto.Ativo = false;
            tipoFoto.DataInativacao = agora;
            tipoFoto.DataAtualizacao = agora;
            _context.SaveChanges();
            return Ok(tipoFoto);
        }
        [HttpPut("Ativar/{id}")]
        public IActionResult Activate(int id)
        {
            var tipoFoto = _context.TiposFoto.Find(id);
            if (tipoFoto == null)
            {
                return NotFound("Tipo de foto não encontrado.");
            }
            tipoFoto.Ativo = true;
            tipoFoto.DataAtualizacao = DateTime.Now;
            tipoFoto.DataInativacao = null;
            _context.SaveChanges();
            return Ok(tipoFoto);
        }
    }
}
