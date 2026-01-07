using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FotoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public FotoController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Foto foto)
        {
            foto.DataCadastro = DateTime.Now;
            foto.Imovel = _context.Imoveis.Find(foto.Imovel.Id);
            if (foto.Imovel == null)
                return BadRequest("Imóvel não encontrado");
            foto.Cadastrador = _context.Usuarios.Find(foto.Cadastrador.Id);
            if (foto.Cadastrador == null)
                return BadRequest("Usuário cadastrador não encontrado");
            foto.Ativo = true;
            foto.TipoFoto = _context.TiposFoto.Find(foto.TipoFoto.Id);
            if (foto.TipoFoto == null)
                return BadRequest("Tipo de foto não encontrado");
            _context.Fotos.Add(foto);
            _context.SaveChanges();
            return Ok(foto);
        }
        [HttpGet("ObterPorImovel/{imovelId}")]
        public IActionResult GetByImovel(int imovelId)
        {
            var fotos = _context.Fotos.Where(f => f.Imovel.Id == imovelId && f.Ativo).ToList();
            return Ok(fotos);
        }
        [HttpPut("Inativar/{fotoId}")]
        public IActionResult GetById(int id) {
            var foto = _context.Fotos.Find(id) ?? throw new Exception("Foto não encontrada");
            foto.Ativo = false;
            foto.DataInativacao = DateTime.Now;
            foto.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(foto);
        }
        [HttpGet("ObterTodas")]
        public IActionResult GetAll()
        {
            var fotos = _context.Fotos.Where(f => f.Ativo).ToList();
            return Ok(fotos);
        }
    }
}
