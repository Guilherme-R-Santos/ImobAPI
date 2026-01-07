using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ObjetoContratoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public ObjetoContratoController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.ObjetoContrato objetoContrato)
        {
            objetoContrato.Cadastrador = _context.Usuarios.Find(objetoContrato.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            objetoContrato.DataCadastro = DateTime.Now;
            objetoContrato.Ativo = true;
            _context.ObjetosContrato.Add(objetoContrato);
            _context.SaveChanges();
            return Ok(objetoContrato);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var objetosContrato = _context.ObjetosContrato.Where(oc => oc.Ativo).ToList();
            return Ok(objetosContrato);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult Get(int id)
        {
            var existingObjetoContrato = _context.ObjetosContrato.Find(id);
            if (existingObjetoContrato == null || existingObjetoContrato.Ativo == false)
            {
                return NotFound("Objeto de contrato não encontrado.");
            }
            return Ok(existingObjetoContrato);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult Get(string nome) {
            var objetosContrato = _context.ObjetosContrato.Where(oc => oc.Nome.Contains(nome) && oc.Ativo).ToList();
            return Ok(objetosContrato);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingObjetoContrato = _context.ObjetosContrato.Find(id);
            if (existingObjetoContrato == null || existingObjetoContrato.Ativo == false)
            {
                return NotFound("Objeto de contrato não encontrado.");
            }
            existingObjetoContrato.Ativo = false;
            existingObjetoContrato.DataInativacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingObjetoContrato);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.ObjetoContrato objetoContrato)
        {
            var existingObjetoContrato = _context.ObjetosContrato.Find(id);
            if (existingObjetoContrato == null || existingObjetoContrato.Ativo == false)
            {
                return NotFound("Objeto de contrato não encontrado.");
            }
            existingObjetoContrato.Nome = objetoContrato.Nome;
            existingObjetoContrato.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingObjetoContrato);
        }
    }
}
