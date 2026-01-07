namespace ImobAPI.Entities
{
    public class TipoFoto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Usuario Cadastrador { get; set; }
        public bool Ativo { get; set; }
        public DateTime ? DataCadastro { get; set; }
        public DateTime ? DataAtualizacao { get; set; }
        public DateTime ? DataInativacao { get; set; }
    }
}
