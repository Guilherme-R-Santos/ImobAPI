using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImobAPI.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public TipoUsuario Tipo {  get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Senha { get; set; }
        [NotMapped, JsonPropertyName("senha"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SenhaEntrada { get => null; set => Senha = value; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
    }
}
