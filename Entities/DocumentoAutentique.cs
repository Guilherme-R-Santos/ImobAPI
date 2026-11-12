using System.ComponentModel.DataAnnotations;

namespace ImobAPI.Entities
{
    public class DocumentoAutentique
    {
        [Key, MaxLength(128)]
        public string IdDocumentoAutentique { get; set; }
        public int ContratoId { get; set; }
        [MaxLength(255)]
        public string Nome { get; set; }
        public bool Sandbox { get; set; }
        [MaxLength(32)]
        public string Status { get; set; }
        public DateTimeOffset? CriadoEmUtc { get; set; }
        public DateTimeOffset? ConcluidoEmUtc { get; set; }
        public DateTimeOffset? UltimaSincronizacaoEmUtc { get; set; }
        public List<SignatarioAutentique> Signatarios { get; set; } = new();
    }
}
