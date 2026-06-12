using ImobAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TipoUsuarioController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoUsuarioController(Context.ImobContext context)
        {
            _context = context;
        }
        [HttpPost("Criar")]
        public IActionResult Create(Entities.TipoUsuario tipoUsuario)
        {
            tipoUsuario.DataCadastro = DateTime.Now;
            tipoUsuario.Ativo = true;
            _context.TiposUsuario.Add(tipoUsuario);
            _context.SaveChanges();
            return Ok(tipoUsuario);

        }
        [HttpGet("ObterTodos")]
        public IActionResult GetAll()
        {
            var tiposUsuario = _context.TiposUsuario.Where(t => t.Ativo).ToList();
            return Ok(tiposUsuario);
        }
        [HttpGet("ObterPorId/{id}")]
        public IActionResult GetById(int id)
        {
            var existingTipoUsuario = _context.TiposUsuario.Find(id);
            if (existingTipoUsuario == null || existingTipoUsuario.Ativo == false)
            {
                return NotFound("Tipo de usuário não encontrado.");
            }
            return Ok(existingTipoUsuario);
        }
        [HttpGet("ObterPorNome/{nome}")]
        public IActionResult GetByName(string nome)
        {
            var tiposUsuario = _context.TiposUsuario.Where(c => c.Nome.Contains(nome) && c.Ativo).ToList();
            return Ok(tiposUsuario);
        }
        [HttpPut("Inativar/{id}")]
        public IActionResult Inactivate(int id)
        {
            var existingTiposUsuario = _context.TiposUsuario.Find(id);
            if (existingTiposUsuario == null)
            {
                return NotFound("Tipo de usuário não encontrado.");
            }
            existingTiposUsuario.Ativo = false;
            existingTiposUsuario.DataInativacao = DateTime.Now;
            existingTiposUsuario.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTiposUsuario);
        }
        [HttpPut("Atualizar/{id}")]
        public IActionResult Update(int id, Entities.TipoUsuario updatedTipoUsuario)
        {
            var existingTipoUsuario = _context.TiposUsuario.Find(id);
            if (existingTipoUsuario == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            existingTipoUsuario.DataAtualizacao = DateTime.Now;
            existingTipoUsuario.Nome = updatedTipoUsuario.Nome;
            existingTipoUsuario.Ativo = updatedTipoUsuario.Ativo;
            _context.SaveChanges();
            return Ok(existingTipoUsuario);
        }
        [HttpPut("Ativar/{id}")]
        public IActionResult Activate(int id)
        {
            var existingTipoUsuario = _context.TiposUsuario.Find(id);
            if (existingTipoUsuario == null)
            {
                return NotFound("Tipo de usuário não encontrado.");
            }
            existingTipoUsuario.Ativo = true;
            existingTipoUsuario.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return Ok(existingTipoUsuario);
        }
    }
}
