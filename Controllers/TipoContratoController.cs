using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TipoContratoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoContratoController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.TipoContrato tipoContrato)
        {
            tipoContrato.Cadastrador = _context.Usuarios.Find(tipoContrato.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            tipoContrato.DataCadastro = DateTime.Now;
            tipoContrato.Ativo = true;
            _context.TiposContrato.Add(tipoContrato);
            _context.SaveChanges();
            return Ok(tipoContrato);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var tiposContrato = _context.TiposContrato.Where(tc => tc.Ativo).ToList();
            return Ok(tiposContrato);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetPorId(int id)
        {
            var existingTiposContrato = _context.TiposContrato.Find(id);
            if (existingTiposContrato == null || existingTiposContrato.Ativo == false)
            {
                return NotFound("Tipo de contrato não encontrado.");
            }
            return Ok(existingTiposContrato);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingTipoContrato = _context.TiposContrato.Find(id);
            if (existingTipoContrato == null || existingTipoContrato.Ativo == false)
            {
                return NotFound("Tipo de contrato não encontrado.");
            }
            existingTipoContrato.Ativo = false;
            existingTipoContrato.DataInativacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoContrato);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.TipoContrato tipoContrato)
        {
            var existingTipoContrato = _context.TiposContrato.Find(id);
            if (existingTipoContrato == null || existingTipoContrato.Ativo == false)
            {
                return NotFound("Tipo de contrato não encontrado.");
            }
            existingTipoContrato.Nome = tipoContrato.Nome;
            existingTipoContrato.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoContrato);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetPorNome(string nome) {
            var existingTipoContrato = _context.TiposContrato.Where(tc => tc.Nome.Contains(nome) && tc.Ativo).ToList();
            return Ok(existingTipoContrato);
        }
    }
}
