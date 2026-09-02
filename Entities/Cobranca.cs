namespace ImobAPI.Entities
{
    public class Cobranca
    {
        public int Id { get; set; }
        public string IdCobrancaAsaas { get; set; }
        public Usuario Cadastrador { get; set; }
        public Contrato Contrato { get; set; }
        public TipoCobranca TipoCobranca { get; set; }
        public string Status { get; set; }
        public string LinkBoleto { get; set; }
        public string NossoNumero { get; set; }
        public bool Pago { get; set; }
        public DateTime? DataPagamento { get; set; }
        public bool ComprovanteEnviado { get; set; }
        public double Valor { get; set; }
        public double valorLiquido { get; set; }
        public string Nome { get; set; }
        public DateTime? Vencimento { get; set; }
        public bool PartilhaAutomatica { get; set; }
        public int ContaPartilha { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public bool Ativo { get; set; }
        public bool SincronizadoAsaas { get; set; } = true;
        public string ErroSincronizacaoAsaas { get; set; }

    }
}
