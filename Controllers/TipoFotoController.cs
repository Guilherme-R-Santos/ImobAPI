using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//TODO: Finalizar a implementação do TipoFotoController
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
            tipoFoto.DataCadastro = DateTime.Now;
            tipoFoto.Ativo = true;
            tipoFoto.Cadastrador = _context.Usuarios.Find(tipoFoto.Cadastrador.Id);
            if(tipoFoto.Cadastrador == null)
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
    }
}
