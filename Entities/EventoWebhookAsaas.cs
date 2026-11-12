using System.ComponentModel.DataAnnotations;

namespace ImobAPI.Entities
{
    public class EventoWebhookAsaas
    {
        public long Id { get; set; }

        [Required, MaxLength(256)]
        public string IdEventoAsaas { get; set; }

        [Required, MaxLength(128)]
        public string Tipo { get; set; }

        [MaxLength(128)]
        public string IdCobrancaAsaas { get; set; }

        [Required]
        public string Payload { get; set; }

        public DateTime RecebidoEmUtc { get; set; }
        public DateTime? ProcessadoEmUtc { get; set; }
        public DateTime? ProximaTentativaEmUtc { get; set; }
    }
}
