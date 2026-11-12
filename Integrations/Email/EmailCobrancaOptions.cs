namespace ImobAPI.Integrations.Email
{
    public class EmailCobrancaOptions
    {
        public bool Habilitado { get; set; }
        public string FusoHorario { get; set; } = "America/Sao_Paulo";
        public int HoraEnvio { get; set; } = 8;
        public string ContaGoogle { get; set; }
        public string SenhaAplicativo { get; set; }
        public string AssuntoMensal { get; set; }
        public string TextoMensal { get; set; }
        public string AssuntoVencimento { get; set; }
        public string TextoVencimento { get; set; }
    }
}
