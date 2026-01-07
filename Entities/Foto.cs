namespace ImobAPI.Entities
{
    public class Foto
    {
        public int Id { get; set; }
        public Imovel Imovel { get; set; }
        public string NomeArquivo { get; set; }
        public TipoFoto TipoFoto { get; set; }
        public byte[] Bin { get; set; }
        public Usuario Cadastrador { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime ? DataInativacao { get; set; }
        public DateTime ? DataAtualizacao { get; set; }
        public bool Principal { get; set; }
        public bool Ativo { get; set; }
    }
}
