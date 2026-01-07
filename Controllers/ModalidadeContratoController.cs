using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ModalidadeContratoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public ModalidadeContratoController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.ModalidadeContrato modalidadeContrato)
        {
            modalidadeContrato.Cadastrador = _context.Usuarios.Find(modalidadeContrato.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            modalidadeContrato.DataCadastro = DateTime.Now;
            modalidadeContrato.Ativo = true;
            _context.ModalidadesContrato.Add(modalidadeContrato);
            _context.SaveChanges();
            return Ok(modalidadeContrato);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var modalidadesContrato = _context.ModalidadesContrato.Where(mc => mc.Ativo).ToList();
            return Ok(modalidadesContrato);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetPorId(int id)
        {
            var existingModalidadeContrato = _context.ModalidadesContrato.Find(id);
            if (existingModalidadeContrato == null || existingModalidadeContrato.Ativo == false)
            {
                return NotFound("Modalidade de contrato não encontrado.");
            }
            return Ok(existingModalidadeContrato);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetPorNome(string nome)
        {
            var modalidadesContrato = _context.ModalidadesContrato
                .Where(mc => mc.Nome.Contains(nome) && mc.Ativo)
                .ToList();
            return Ok(modalidadesContrato);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.ModalidadeContrato modalidadeContrato)
        {
            var existingModalidadeContrato = _context.ModalidadesContrato.Find(id);
            if (existingModalidadeContrato == null || existingModalidadeContrato.Ativo == false)
            {
                return NotFound("Modalidade de contrato não encontrado.");
            }
            existingModalidadeContrato.Nome = modalidadeContrato.Nome;
            existingModalidadeContrato.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingModalidadeContrato);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingModalidadeContrato = _context.ModalidadesContrato.Find(id);
            if (existingModalidadeContrato == null || existingModalidadeContrato.Ativo == false)
            {
                return NotFound("Modalidade de contrato não encontrado.");
            }
            existingModalidadeContrato.Ativo = false;
            existingModalidadeContrato.DataInativacao = DateTime.Now;
            existingModalidadeContrato.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingModalidadeContrato);
        }
    }
}
