using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImovelController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public ImovelController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Imovel imovel)
        {
            imovel.Proprietario = _context.Clientes.Find(imovel.Proprietario.Id) ?? throw new Exception("Cliente proprietário não encontrado");
            imovel.Cadastrador = _context.Usuarios.Find(imovel.Cadastrador.Id) ?? throw new Exception("Usuário cadastrador não encontrado");
            imovel.TipoImovel = _context.TiposImovel.Find(imovel.TipoImovel.Id) ?? throw new Exception("Tipo de imóvel não encontrado");
            imovel.Intencao = _context.Intencoes.Find(imovel.Intencao.Id) ?? throw new Exception("Intenção não encontrada");
            imovel.Nome = $"{imovel.TipoImovel.Nome} em {imovel.Bairro} - {imovel.Proprietario.Nome}";
            imovel.DataCadastro = DateTime.Now;
            imovel.Ativo = true;
            _context.Imoveis.Add(imovel);
            _context.SaveChanges();
            return Ok(imovel);
        }

        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var imoveis = _context.Imoveis
                .Include(i => i.Proprietario)
                .Include(i => i.TipoImovel)
                .Include(i => i.Intencao)
                .Include(i => i.Cadastrador)
                .Where(i => i.Ativo)
                .ToList();
            return Ok(imoveis);
        }

        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var imovel = _context.Imoveis
                .FirstOrDefault(i => i.Id == id && i.Ativo);
            if (imovel == null)
            {
                return NotFound("Imóvel não encontrado");
            }
            return Ok(imovel);
        }

        [HttpPost("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var imovel = _context.Imoveis
                .FirstOrDefault(i => i.Id == id && i.Ativo);
            if (imovel == null)
            {
                return NotFound("Imóvel não encontrado");
            }
            imovel.Ativo = false;
            imovel.DataInativacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(imovel);
        }

        [HttpGet("ObterPorProprietario/{proprietarioId}")]
        public IActionResult GetByProprietario(int proprietarioId)
        {
            var imoveis = _context.Imoveis
                .Where(i => i.Proprietario.Id == proprietarioId && i.Ativo)
                .ToList();
            return Ok(imoveis);
        }

        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.Imovel updatedImovel)
        {
            var imovel = _context.Imoveis
                .FirstOrDefault(i => i.Id == id && i.Ativo);
            if (imovel == null)
            {
                return NotFound("Imóvel não encontrado");
            }

            imovel.Descricao = updatedImovel.Descricao;
            imovel.Observacao = updatedImovel.Observacao;
            imovel.Cep = updatedImovel.Cep;
            imovel.Logradouro = updatedImovel.Logradouro;
            imovel.Numero = updatedImovel.Numero;
            imovel.Bairro = updatedImovel.Bairro;
            imovel.Cidade = updatedImovel.Cidade;
            imovel.Estado = updatedImovel.Estado;
            imovel.Pais = updatedImovel.Pais;
            imovel.Complemento = updatedImovel.Complemento;
            imovel.Metragem = updatedImovel.Metragem;
            imovel.Valor = updatedImovel.Valor;
            imovel.Condominio = updatedImovel.Condominio;
            imovel.Iptu = updatedImovel.Iptu;
            imovel.TaxaIncendio = updatedImovel.TaxaIncendio;
            imovel.Foro = updatedImovel.Foro;
            imovel.DataAtualizacao = DateTime.Now;
            imovel.InscricaoIptu = updatedImovel.InscricaoIptu;
            imovel.NumeroCbmerj = updatedImovel.NumeroCbmerj;
            _context.SaveChanges();
            return Ok(imovel);
        }
    }
}
