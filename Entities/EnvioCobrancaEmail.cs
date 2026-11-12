using System.ComponentModel.DataAnnotations;

namespace ImobAPI.Entities
{
    public class EnvioCobrancaEmail
    {
        public long Id { get; set; }
        public int CobrancaId { get; set; }
        [Required, MaxLength(32)]
        public string Tipo { get; set; }
        public DateTime Referencia { get; set; }
        [Required, MaxLength(32)]
        public string Situacao { get; set; }
        public DateTime IniciadoEmUtc { get; set; }
        public DateTime? ConcluidoEmUtc { get; set; }
    }
}
