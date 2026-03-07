using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using static BCrypt.Net.BCrypt;
using System.Text;

namespace ImobAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        private readonly IConfiguration _configuration;

        private const int WorkFactor = 12;
        public UsuarioController(Context.ImobContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("Connect")]
        public IActionResult Get()
        {
            return Ok("Conexão estabelecida com sucesso!");
        }

        [AllowAnonymous]
        [HttpPost("Criar")]
        public IActionResult Create(Entities.Usuario usuario)
        {
            var existingUser = _context.Usuarios.FirstOrDefault(u => u.Login == usuario.Login);
            if (existingUser != null)
            {
                return Conflict("Já existe um usuário com este login.");
            }

            string senhaImput = usuario.Senha;
            if (string.IsNullOrEmpty(senhaImput) || senhaImput.Length < 8)
            {
                return BadRequest("A senha deve ter pelo menos 8 caracteres.");
            }

            if (senhaImput.Length > 20)
            {
                return BadRequest("A senha deve ter no máximo 20 caracteres.");
            }

            if (!senhaImput.Any(char.IsUpper))
            {
                return BadRequest("A senha deve conter pelo menos uma letra maiúscula.");
            }

            if (!senhaImput.Any(char.IsLower))
            {
                return BadRequest("A senha deve conter pelo menos uma letra minúscula.");
            }

            if (!senhaImput.Any(char.IsDigit))
            {
                return BadRequest("A senha deve conter pelo menos um número.");
            }

            if(!senhaImput.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return BadRequest("A senha deve conter pelo menos um caractere especial.");
            }

            usuario.DataCadastro = DateTime.Now;
            usuario.Ativo = true;
            usuario.Senha = HashPassword(usuario.Senha, WorkFactor);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }

        [HttpGet("ObterPorId/{id}")]
        public IActionResult ObterPorId(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id && u.Ativo == true);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpGet("ObterPorLogin/{login}")]
        public IActionResult ObterPorLogin(string login)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Login == login && u.Ativo == true);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var usuarios = _context.Usuarios.ToList();
            return Ok(usuarios);
        }

        [HttpGet("ObterTodosAtivos")]
        public IActionResult ObterTodosAtivos()
        {
            var usuarios = _context.Usuarios.Where(u => u.Ativo == true).ToList();
            return Ok(usuarios);
        }

        [HttpPut("Atualizar/{id}")]
        public IActionResult Atualizar(int id, Entities.Usuario usuarioAtualizado)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.Nome = usuarioAtualizado.Nome;
            usuario.Login = usuarioAtualizado.Login;
            usuario.Senha = usuarioAtualizado.Senha;
            usuario.Ativo = usuarioAtualizado.Ativo;
            usuario.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("InativarUsuario/{id}")]
        public IActionResult InativarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Ativo = false;
            usuario.DataAtualizacao = DateTime.Now;
            usuario.DataInativacao = DateTime.Now;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("Deletar/{id}")]
        public IActionResult Deletar(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Logar(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
            {
                return BadRequest("Informe login e senha.");
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Login == login && u.Ativo == true);
            if (usuario == null)
            {
                return Unauthorized("Usuário ou senha Incorretos. Tente novamente!");
            }

            var senhaCrypto = usuario.Senha;
            var deuCerto = Verify(senha, senhaCrypto);

            if (!deuCerto)
            {
                return Unauthorized("Usuário ou senha Incorretos. Tente novamente!");
            }

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expirationInMinutes = int.TryParse(_configuration["Jwt:ExpirationInMinutes"], out var configuredExpiration)
                ? configuredExpiration
                : 60;

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, "Configuração JWT inválida.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", usuario.Id.ToString()),
                new Claim("nome", usuario.Nome ?? string.Empty)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                expiration = expires,
                tipo = "Bearer"
            });
        }
    }
}
