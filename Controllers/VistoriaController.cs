using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VistoriaController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public VistoriaController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Vistoria vistoria)
        {
            vistoria.DataCadastro = DateTime.Now;
            vistoria.Imovel = _context.Imoveis.Find(vistoria.Imovel.Id);
            if (vistoria.Imovel == null)
                return BadRequest("Imóvel não encontrado");
            vistoria.Contrato = _context.Contratos.Find(vistoria.Contrato.Id);
            if (vistoria.Contrato == null)
                return BadRequest("Contrato não encontrado");
            vistoria.Cadastrador = _context.Usuarios.Find(vistoria.Cadastrador.Id);
            if (vistoria.Cadastrador == null)
                return BadRequest("Usuário cadastrador não encontrado");
            vistoria.Ativo = true;
            vistoria.Nome = $"Vistoria - {vistoria.Imovel.Nome} - {DateTime.Now.ToString("dd/MM/yyyy")}";
            _context.Vistorias.Add(vistoria);
            _context.SaveChanges();
            return Ok(vistoria);
        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var vistorias = _context.Vistorias
                .Where(v => v.Ativo)
                .ToList();
            return Ok(vistorias);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var vistoria = _context.Vistorias
                .FirstOrDefault(v => v.Id == id && v.Ativo);
            if (vistoria == null)
            {
                return NotFound("Vistoria não encontrada");
            }
            return Ok(vistoria);
        }
        [HttpGet("ObterPorImovel/{imovelId}")]
        public IActionResult GetByImovel(int imovelId)
        {
            var vistorias = _context.Vistorias
                .Where(v => v.Imovel.Id == imovelId && v.Ativo)
                .ToList();
            return Ok(vistorias);
        }
        [HttpPost("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var vistoria = _context.Vistorias
                .FirstOrDefault(v => v.Id == id && v.Ativo);
            if (vistoria == null)
            {
                return NotFound("Vistoria não encontrada");
            }
            vistoria.Ativo = false;
            vistoria.DataInativacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(vistoria);
        }
        [HttpPost("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.Vistoria updatedVistoria)
        {
            var vistoria = _context.Vistorias
                .FirstOrDefault(v => v.Id == id && v.Ativo);
            if (vistoria == null)
            {
                return NotFound("Vistoria não encontrada");
            }
            vistoria.DataVistoria = updatedVistoria.DataVistoria;
            vistoria.DataEntregaChaves = updatedVistoria.DataEntregaChaves;
            vistoria.Observacoes = updatedVistoria.Observacoes;
            vistoria.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(vistoria);
        }
        [HttpGet("ObterFotosDaVistoria/{vistoriaId}")]
        public IActionResult GetFotosByVistoria(int vistoriaId)
        {
            var fotos = _context.Fotos
                .Where(f => f.Vistoria != null && f.Vistoria.Id == vistoriaId && f.Ativo)
                .ToList();
            return Ok(fotos);
        }
        [HttpGet("ObterVistoriaPorContrato/{contratoId}")]
        public IActionResult GetByContrato(int contratoId)
        {
            var vistorias = _context.Vistorias
                .Where(v => v.Contrato.Id == contratoId && v.Ativo)
                .ToList();
            return Ok(vistorias);
        }
    }
}
