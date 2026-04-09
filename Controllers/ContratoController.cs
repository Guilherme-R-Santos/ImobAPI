using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public ContratoController(Context.ImobContext context)
        {
            _context = context;
        }

        private IQueryable<Entities.Contrato> ContratosComIncludes()
        {
            return _context.Contratos
                .Include(c => c.Cadastrador)
                .Include(c => c.TipoContrato)
                .Include(c => c.Proprietario)
                .Include(c => c.Proprietario)
                    .ThenInclude(p => p.TipoCliente)
                .Include(c => c.Contratante1)
                .Include(c => c.Contratante1)
                    .ThenInclude(c1 => c1.TipoCliente)
                .Include(c => c.Contratante2)
                .Include(c => c.Contratante2)
                    .ThenInclude(c2 => c2.TipoCliente)
                .Include(c => c.Contratante3)
                .Include(c => c.Contratante3)
                    .ThenInclude(c3 => c3.TipoCliente)
                .Include(c => c.Contratante4)
                .Include(c => c.Contratante4)
                    .ThenInclude(c4 => c4.TipoCliente)
                .Include(c => c.Fiador)
                .Include(c => c.Fiador)
                    .ThenInclude(f => f.TipoCliente)
                .Include(c => c.Imovel)
                .Include(c => c.Imovel)
                    .ThenInclude(i => i.TipoImovel)
                .Include(c => c.Imovel)
                    .ThenInclude(i => i.Intencao)
                .Include(c => c.Imovel)
                    .ThenInclude(i => i.Proprietario)
                .Include(c => c.Imovel)
                    .ThenInclude(i => i.Proprietario)
                        .ThenInclude(p => p.TipoCliente)
                .Include(c => c.ObjetoContrato)
                .Include(c => c.ModalidadeContrato);
        }

        [HttpPost("Criar")]
        public IActionResult Create(Entities.Contrato contrato)
        {
            contrato.Cadastrador = _context.Usuarios.Find(contrato.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            contrato.TipoContrato = _context.TiposContrato.Find(contrato.TipoContrato.Id) ?? throw new Exception("Tipo de contrato não encontrado");
            contrato.Proprietario = _context.Clientes.Find(contrato.Proprietario.Id) ?? throw new Exception("Proprietário não encontrado");
            contrato.Contratante1 = _context.Clientes.Find(contrato.Contratante1.Id) ?? throw new Exception("Contratante 1 não encontrado");
            if (contrato.Contratante2 != null)
                contrato.Contratante2 = _context.Clientes.Find(contrato.Contratante2.Id) ?? throw new Exception("Contratante 2 não encontrado");
            if (contrato.Contratante3 != null)
                contrato.Contratante3 = _context.Clientes.Find(contrato.Contratante3.Id) ?? throw new Exception("Contratante 3 não encontrado");
            if (contrato.Contratante4 != null)
                contrato.Contratante4 = _context.Clientes.Find(contrato.Contratante4.Id) ?? throw new Exception("Contratante 4 não encontrado");
            if (contrato.Fiador != null)
                contrato.Fiador = _context.Clientes.Find(contrato.Fiador.Id) ?? throw new Exception("Fiador não encontrado");
            contrato.Imovel = _context.Imoveis.Find(contrato.Imovel.Id) ?? throw new Exception("Imóvel não encontrado");
            contrato.ObjetoContrato = _context.ObjetosContrato.Find(contrato.ObjetoContrato.Id) ?? throw new Exception("Objeto do contrato não encontrado");
            contrato.ModalidadeContrato = _context.ModalidadesContrato.Find(contrato.ModalidadeContrato.Id) ?? throw new Exception("Modalidade do contrato não encontrada");
            contrato.DataCadastro = DateTime.Now;
            contrato.Ativo = true;
            _context.Contratos.Add(contrato);
            _context.SaveChanges();
            return Ok(contrato);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var contratos = ContratosComIncludes()
                .Where(c => c.Ativo).ToList();
            return Ok(contratos);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var existingContrato = ContratosComIncludes()
                .FirstOrDefault(c => c.Id == id && c.Ativo);

            if (existingContrato == null)
            {
                return NotFound("Contrato não encontrado.");
            }
            return Ok(existingContrato);
        }
        [HttpGet("ObterPorProprietario/{proprietarioId}")]
        public IActionResult GetByProprietario(int proprietarioId)
        {
            var contratos = ContratosComIncludes()
                .Where(c => c.Proprietario.Id == proprietarioId && c.Ativo)
                .ToList();
            return Ok(contratos);
        }
        [HttpGet("ObterPorContratante/{contratanteId}")]
        public IActionResult GetByContratante(int contratanteId)
        {
            var contratos = ContratosComIncludes()
                .Where(c => (c.Contratante1.Id == contratanteId ||
                             (c.Contratante2 != null && c.Contratante2.Id == contratanteId) ||
                             (c.Contratante3 != null && c.Contratante3.Id == contratanteId) ||
                             (c.Contratante4 != null && c.Contratante4.Id == contratanteId)) && c.Ativo)
                .ToList();
            return Ok(contratos);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingContrato = _context.Contratos.Find(id);
            if (existingContrato == null || existingContrato.Ativo == false)
            {
                return NotFound("Contrato não encontrado.");
            }
            existingContrato.Ativo = false;
            existingContrato.DataInativacao = DateTime.Now;
            existingContrato.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingContrato);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.Contrato contrato)
        {
            var existingContrato = _context.Contratos.Find(id);
            if (existingContrato == null || existingContrato.Ativo == false)
            {
                return NotFound("Contrato não encontrado.");
            }
            existingContrato.TipoContrato = _context.TiposContrato.Find(contrato.TipoContrato.Id) ?? throw new Exception("Tipo de contrato não encontrado");
            existingContrato.Proprietario = _context.Clientes.Find(contrato.Proprietario.Id) ?? throw new Exception("Proprietário não encontrado");
            existingContrato.Contratante1 = _context.Clientes.Find(contrato.Contratante1.Id) ?? throw new Exception("Contratante 1 não encontrado");
            if (contrato.Contratante2 != null)
                existingContrato.Contratante2 = _context.Clientes.Find(contrato.Contratante2.Id) ?? throw new Exception("Contratante 2 não encontrado");
            else
                existingContrato.Contratante2 = null;
            if (contrato.Contratante3 != null)
                existingContrato.Contratante3 = _context.Clientes.Find(contrato.Contratante3.Id) ?? throw new Exception("Contratante 3 não encontrado");
            else
                existingContrato.Contratante3 = null;
            if (contrato.Contratante4 != null)
                existingContrato.Contratante4 = _context.Clientes.Find(contrato.Contratante4.Id) ?? throw new Exception("Contratante 4 não encontrado");
            else
                existingContrato.Contratante4 = null;
            if (contrato.Fiador != null)
                existingContrato.Fiador = _context.Clientes.Find(contrato.Fiador.Id) ?? throw new Exception("Fiador não encontrado");
            else
                existingContrato.Fiador = null;
            existingContrato.Imovel = _context.Imoveis.Find(contrato.Imovel.Id) ?? throw new Exception("Imóvel não encontrado");
            existingContrato.ObjetoContrato = _context.ObjetosContrato.Find(contrato.ObjetoContrato.Id) ?? throw new Exception("Objeto do contrato não encontrado");
            existingContrato.ModalidadeContrato = _context.ModalidadesContrato.Find(contrato.ModalidadeContrato.Id) ?? throw new Exception("Modalidade do contrato não encontrada");
            existingContrato.PrazoMeses = contrato.PrazoMeses;
            existingContrato.Vencimento = contrato.Vencimento;
            existingContrato.DataFimVigencia = contrato.DataFimVigencia;
            existingContrato.PropostaSegFianca = contrato.PropostaSegFianca;
            existingContrato.ApoliceSegFianca = contrato.ApoliceSegFianca;
            existingContrato.DataInicioVigencia = contrato.DataInicioVigencia;
            existingContrato.Nome = contrato.Nome;
            existingContrato.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingContrato);
        }
    }
}
