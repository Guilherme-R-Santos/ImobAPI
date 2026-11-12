using System.ComponentModel.DataAnnotations;

namespace ImobAPI.Integrations.Autentique.Models
{
    public class CriarDocumentoRequest
    {
        [Range(1, int.MaxValue)]
        public int ContratoId { get; set; }

        [Required, StringLength(255, MinimumLength = 1)]
        public string Nome { get; set; }

        [Required]
        public byte[] Arquivo { get; set; }

        [Required, MinLength(1)]
        public List<SignatarioRequest> Signatarios { get; set; }
    }

    public class SignatarioRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string MetodoEntrega { get; set; }
        public string Acao { get; set; } = "SIGN";
    }
}
