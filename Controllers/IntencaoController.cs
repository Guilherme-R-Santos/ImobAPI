using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IntencaoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public IntencaoController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Intencao intencao)
        {
            intencao.DataCadastro = DateTime.Now;
            intencao.Ativo = true;
            intencao.Cadastrador = _context.Usuarios.Find(intencao.Cadastrador.Id);
            if (intencao.Cadastrador == null)
            {
                return BadRequest("Usuário cadastrador não encontrado.");
            }
            _context.Intencoes.Add(intencao);
            _context.SaveChanges();
            return Ok(intencao);
        }
        [HttpGet("ObterTodas")]
        public IActionResult GetAll()
        {
            var intencoes = _context.Intencoes.Where(i => i.Ativo).ToList();
            return Ok(intencoes);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var existingIntencao = _context.Intencoes.Find(id);
            if (existingIntencao == null || existingIntencao.Ativo == false)
            {
                return NotFound("Intenção não encontrada.");
            }
            return Ok(existingIntencao);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingIntencao = _context.Intencoes.Find(id);
            if (existingIntencao == null)
            {
                return NotFound("Intenção não encontrada.");
            }
            existingIntencao.Ativo = false;
            existingIntencao.DataInativacao = DateTime.Now;
            existingIntencao.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingIntencao);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.Intencao updatedIntencao)
        {
            var existingIntencao = _context.Intencoes.Find(id);
            if (existingIntencao == null)
            {
                return NotFound("Intenção não encontrada.");
            }
            existingIntencao.Nome = updatedIntencao.Nome;
            existingIntencao.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingIntencao);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetPorNome(string nome)
        {
            var existingIntencao = _context.Intencoes.FirstOrDefault(i => i.Nome == nome && i.Ativo);
            if (existingIntencao == null)
            {
                return NotFound("Intenção não encontrada.");
            }
            return Ok(existingIntencao);
        }
    }
}
