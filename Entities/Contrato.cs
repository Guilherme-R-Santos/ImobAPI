namespace ImobAPI.Entities
{
    public class Contrato
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public decimal ValorContrato { get; set; }
        public Usuario Cadastrador { get; set; }
        public TipoContrato TipoContrato { get; set; }
        public Cliente Proprietario { get; set; }
        public Cliente Contratante1 { get; set; }
        public Cliente Contratante2 { get; set; }
        public Cliente Contratante3 { get; set; }
        public Cliente Contratante4 { get; set; }
        public Cliente Fiador { get; set; }
        public Imovel Imovel { get; set; }
        public ObjetoContrato ObjetoContrato { get; set; }
        public ModalidadeContrato ModalidadeContrato { get; set; }
        public DateTime ? DataCadastro { get; set; }
        public DateTime ? DataInicioVigencia { get; set; }
        public int PrazoMeses { get; set; }
        public int Vencimento { get; set; }
        public DateTime ? DataFimVigencia { get; set; }
        public DateTime ? DataAtualizacao { get; set; }
        public DateTime ? DataInativacao { get; set; }
        public string PropostaSegFianca { get; set; }
        public string ApoliceSegFianca { get; set; }
    }
}
