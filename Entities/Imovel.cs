namespace ImobAPI.Entities
{
    public class Imovel
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public Cliente Proprietario { get; set; }
        public TipoImovel TipoImovel { get; set; }
        public Intencao Intencao { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string ? Complemento { get; set; }
        public string ? InscricaoIptu { get; set; }
        public string ? NumeroCbmerj { get; set; }
        public decimal Metragem { get; set; }
        public decimal ValorLocacao { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal ? Condominio { get; set; }
        public decimal ? Iptu { get; set; }
        public decimal ? TaxaIncendio { get; set; }
        public decimal ? Foro { get; set; }
        public Usuario Cadastrador { get; set; }
        public DateTime ? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
    }
}
