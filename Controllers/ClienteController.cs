using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public ClienteController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Cliente cliente)
        {
            cliente.DataCadastro = DateTime.Now;
            cliente.Ativo = true;
            cliente.TipoCliente = _context.TiposCliente.Find(cliente.TipoCliente.Id) ?? throw new Exception("Tipo de cliente não encontrado");
            cliente.Cadastrador = _context.Usuarios.Find(cliente.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return Ok(cliente);

        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var clientes = _context.Clientes.Where(c => c.Ativo).ToList();
            return Ok(clientes);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var existingCliente = _context.Clientes.Find(id);
            if (existingCliente == null || existingCliente.Ativo == false)
            {
                return NotFound("Cliente não encontrado.");
            }
            return Ok(existingCliente);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetByName(string nome)
        {
            var clientes = _context.Clientes.Where(c => c.Nome.Contains(nome) && c.Ativo).ToList();
            return Ok(clientes);
        }
        [HttpGet("ObterPorCpfCnpj/{cpfCnpj}")]
        public IActionResult GetByCpfCnpj(string cpfCnpj)
        {
            var existingCliente = _context.Clientes.FirstOrDefault(c => c.CpfCnpj.Replace(".", "").Replace("-", "") == cpfCnpj.Replace(".", "").Replace("-", "") && c.Ativo);
            if (existingCliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            return Ok(existingCliente);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingCliente = _context.Clientes.Find(id);
            if (existingCliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            existingCliente.Ativo = false;
            existingCliente.DataInativacao = DateTime.Now;
            existingCliente.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingCliente);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.Cliente updatedCliente)
        {
            var existingCliente = _context.Clientes.Find(id);
            if (existingCliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            existingCliente.Nome = updatedCliente.Nome;
            existingCliente.Email = updatedCliente.Email;
            existingCliente.Telefone = updatedCliente.Telefone;
            existingCliente.TipoCliente = _context.TiposCliente.Find(updatedCliente.TipoCliente.Id) ?? throw new Exception("Tipo de cliente não encontrado");
            existingCliente.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingCliente);
        }
        [HttpPut("Ativar/{id}")]
        public IActionResult Activate(int id)
        {
            var existingCliente = _context.Clientes.Find(id);
            if (existingCliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            existingCliente.Ativo = true;
            existingCliente.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingCliente);
        }

    }
}
