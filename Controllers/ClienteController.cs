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
            cliente.TipoCliente = _context.TiposCliente.Find(cliente.TipoCliente.Id) ?? throw new Exception("Tipo de cliente não encontrado");
            cliente.Cadastrador = _context.Usuarios.Find(cliente.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return Ok(cliente);

        }

    }
}
