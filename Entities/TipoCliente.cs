namespace ImobAPI.Entities
{
    public class TipoCliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public Usuario Cadastrador { get; set; }
    }
}
