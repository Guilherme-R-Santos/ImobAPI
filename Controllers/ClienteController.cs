using Microsoft.AspNetCore.Mvc;
using ImobAPI.Integrations.Asaas;
using ImobAPI.Integrations.Asaas.Models;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        private readonly IAsaasService _asaasService;
        public ClienteController(Context.ImobContext context, IAsaasService asaasService)
        {
            _context = context;
            _asaasService = asaasService;
        }
        [HttpPost("Criar")]
        public async Task<IActionResult> Create(Entities.Cliente cliente)
        {
            cliente.DataCadastro = DateTime.Now;
            cliente.Ativo = true;
            cliente.TipoCliente = _context.TiposCliente.Find(cliente.TipoCliente.Id) ?? throw new Exception("Tipo de cliente não encontrado");
            cliente.Cadastrador = _context.Usuarios.Find(cliente.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");

            if (string.IsNullOrWhiteSpace(cliente.IdClienteAsaas))
            {
                try
                {
                    var asaasRequest = new AsaasCustomerRequest
                    {
                        Name = cliente.Nome,
                        CpfCnpj = cliente.CpfCnpj,
                        Email = cliente.Email,
                        Phone = cliente.Telefone,
                        MobilePhone = cliente.Telefone,
                        Address = cliente.Endereco,
                        AddressNumber = cliente.NumeroEndereco,
                        Complement = cliente.Complemento,
                        Province = cliente.Bairro,
                        PostalCode = cliente.Cep,
                        ExternalReference = cliente.CpfCnpj
                    };
                    var asaasResponse = await _asaasService.CriarClienteAsync(asaasRequest);
                    cliente.IdClienteAsaas = asaasResponse.Id;
                }
                catch (AsaasIntegrationException ex)
                {
                    return StatusCode(502, new { mensagem = "Falha ao criar cliente no Asaas.", detalhes = ex.Message });
                }
            }

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

        [HttpGet("ObterPorTipo/{tipoId}")]
        public IActionResult GetByTipo(int tipoId)
        {
            var clientes = _context.Clientes.Where(c => c.TipoCliente.Id == tipoId && c.Ativo).ToList();
            return Ok(clientes);
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
        public async Task<IActionResult> Update(int id, Entities.Cliente updatedCliente)
        {
            var existingCliente = _context.Clientes.Find(id);
            if (existingCliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            existingCliente.TipoCliente = _context.TiposCliente.Find(updatedCliente.TipoCliente.Id) ?? throw new Exception("Tipo de cliente não encontrado");
            existingCliente.Nome = updatedCliente.Nome;
            existingCliente.CpfCnpj = updatedCliente.CpfCnpj;
            existingCliente.Identidade = updatedCliente.Identidade;
            existingCliente.OrgaoExpedidor = updatedCliente.OrgaoExpedidor;
            existingCliente.Nacionalidade = updatedCliente.Nacionalidade;
            existingCliente.Naturalidade = updatedCliente.Naturalidade;
            existingCliente.EstadoCivil = updatedCliente.EstadoCivil;
            existingCliente.Profissao = updatedCliente.Profissao;
            existingCliente.Endereco = updatedCliente.Endereco;
            existingCliente.NumeroEndereco = updatedCliente.NumeroEndereco;
            existingCliente.Complemento = updatedCliente.Complemento;
            existingCliente.Bairro = updatedCliente.Bairro;
            existingCliente.Cep = updatedCliente.Cep;
            existingCliente.Agencia = updatedCliente.Agencia;
            existingCliente.Conta = updatedCliente.Conta;
            existingCliente.ChavePix = updatedCliente.ChavePix;
            existingCliente.CodBanco = updatedCliente.CodBanco;
            existingCliente.Banco = updatedCliente.Banco;
            existingCliente.Email = updatedCliente.Email;
            existingCliente.Telefone = updatedCliente.Telefone;
            existingCliente.DataNascimento = updatedCliente.DataNascimento;
            existingCliente.DataAtualizacao = DateTime.Now;

            var asaasRequest = new AsaasCustomerRequest
            {
                Name = existingCliente.Nome,
                CpfCnpj = existingCliente.CpfCnpj,
                Email = existingCliente.Email,
                Phone = existingCliente.Telefone,
                MobilePhone = existingCliente.Telefone,
                Address = existingCliente.Endereco,
                AddressNumber = existingCliente.NumeroEndereco,
                Complement = existingCliente.Complemento,
                Province = existingCliente.Bairro,
                PostalCode = existingCliente.Cep,
                ExternalReference = existingCliente.CpfCnpj
            };

            try
            {
                if (string.IsNullOrWhiteSpace(existingCliente.IdClienteAsaas))
                {
                    var asaasResponse = await _asaasService.CriarClienteAsync(asaasRequest);
                    existingCliente.IdClienteAsaas = asaasResponse.Id;
                }
                else
                {
                    await _asaasService.AtualizarClienteAsync(existingCliente.IdClienteAsaas, asaasRequest);
                }
            }
            catch (AsaasIntegrationException ex)
            {
                return StatusCode(502, new { mensagem = "Falha ao atualizar cliente no Asaas.", detalhes = ex.Message });
            }

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
