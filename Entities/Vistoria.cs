namespace ImobAPI.Entities
{
    public class Vistoria
    {
        public int Id { get; set; }
        public Usuario Cadastrador { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime ? DataCadastro { get; set; }
        public DateTime ? DataAtualizacao { get; set; }
        public DateTime ? DataInativacao { get; set; }
        public Contrato Contrato { get; set; }
        public Imovel Imovel { get; set; }
        public DateTime ? DataVistoria { get; set; }
        public DateTime ? DataEntregaChaves { get; set; }
        public string Observacoes { get; set; }
    }
}
