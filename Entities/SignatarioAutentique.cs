using System.ComponentModel.DataAnnotations;

namespace ImobAPI.Entities
{
    public class SignatarioAutentique
    {
        public int Id { get; set; }
        [Required, MaxLength(128)]
        public string IdDocumentoAutentique { get; set; }
        [Required, MaxLength(128)]
        public string PublicIdAutentique { get; set; }
        public int? ClienteId { get; set; }
        [MaxLength(255)]
        public string Nome { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(32)]
        public string Telefone { get; set; }
        [MaxLength(64)]
        public string Acao { get; set; }
        [MaxLength(32)]
        public string Status { get; set; }
        public DateTimeOffset? CriadoEmUtc { get; set; }
        public DateTimeOffset? VisualizadoEmUtc { get; set; }
        public DateTimeOffset? AssinadoEmUtc { get; set; }
        public DateTimeOffset? RecusadoEmUtc { get; set; }
    }
}
