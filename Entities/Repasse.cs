using System.ComponentModel.DataAnnotations;

namespace ImobAPI.Entities
{
    public class Repasse
    {
        public long Id { get; set; }
        public int CobrancaId { get; set; }
        public int Destinatario { get; set; }
        public string Origem { get; set; }
        public decimal Valor { get; set; }
        public string DadosDestino { get; set; }
        [MaxLength(128)] public string ReferenciaExterna { get; set; }
        [MaxLength(128)] public string IdTransferenciaAsaas { get; set; }
        [MaxLength(40)] public string Status { get; set; }
        public string Erro { get; set; }
        public string ComprovanteUrl { get; set; }
        public int? SolicitanteId { get; set; }
        public DateTime CriadoEmUtc { get; set; }
        public DateTime AtualizadoEmUtc { get; set; }
    }
}
