using ImobAPI.Context;
using ImobAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ImobAPI.Integrations.Asaas
{
    public static class RepasseConfiguracao
    {
        public static bool Administrador(ImobContext context, ClaimsPrincipal user)
        {
            return int.TryParse(user.FindFirstValue("id"), out var id) &&
                context.Usuarios.AsNoTracking().Any(u => u.Id == id && u.Ativo && u.Tipo != null && u.Tipo.Ativo && u.Tipo.Nome == "Administrador");
        }

        public static Task<bool> TemEstornoOuContestacaoAsync(ImobContext context, string idCobranca, CancellationToken cancellationToken)
        {
            return context.EventosWebhookAsaas.AnyAsync(e => e.IdCobrancaAsaas == idCobranca &&
                (e.Tipo.StartsWith("PAYMENT_REFUND") || e.Tipo.StartsWith("PAYMENT_CHARGEBACK") ||
                 e.Tipo == "PAYMENT_PARTIALLY_REFUNDED" || e.Tipo == "PAYMENT_RECEIVED_IN_CASH_UNDONE" ||
                 e.Tipo == "PAYMENT_AWAITING_CHARGEBACK_REVERSAL"), cancellationToken);
        }

        public static string Validar(ContaRepasse conta)
        {
            if (conta == null) return null;
            if (conta.TipoValor != "FIXO" && conta.TipoValor != "PERCENTUAL") return "TipoValor deve ser FIXO ou PERCENTUAL.";
            if (conta.Valor <= 0 || conta.Valor > 99999999999999.9999m || (conta.TipoValor == "PERCENTUAL" && conta.Valor > 100) ||
                (conta.TipoValor == "FIXO" && decimal.Round(conta.Valor, 2) != conta.Valor) || decimal.Round(conta.Valor, 4) != conta.Valor)
                return "O valor de repasse deve ser positivo e caber em decimal(18,4), com no máximo duas casas para valor fixo ou quatro para percentual entre 0 e 100.";
            if (conta.Modalidade == "PIX_CHAVE")
            {
                if (string.IsNullOrWhiteSpace(conta.ChavePix) || !(new[] { "CPF", "CNPJ", "EMAIL", "PHONE", "EVP" }).Contains(conta.TipoChavePix))
                    return "Informe uma chave Pix e um tipo válido (CPF, CNPJ, EMAIL, PHONE ou EVP).";
                var chave = conta.ChavePix;
                if ((conta.TipoChavePix == "CPF" && !Digitos(chave, 11)) || (conta.TipoChavePix == "CNPJ" && !Digitos(chave, 14)) ||
                    (conta.TipoChavePix == "PHONE" && !Digitos(chave, 11)) ||
                    (conta.TipoChavePix == "EMAIL" && (chave.Length > 254 || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(chave))) ||
                    (conta.TipoChavePix == "EVP" && !Guid.TryParseExact(chave, "D", out _)))
                    return "Formato de chave Pix inválido; CPF, CNPJ e telefone devem conter apenas dígitos.";
            }
            else if (conta.Modalidade == "PIX_CONTA" || conta.Modalidade == "TED")
            {
                if (string.IsNullOrWhiteSpace(conta.CodigoBanco) || string.IsNullOrWhiteSpace(conta.NomeTitular) ||
                    string.IsNullOrWhiteSpace(conta.CpfCnpjTitular) || string.IsNullOrWhiteSpace(conta.Agencia) ||
                    string.IsNullOrWhiteSpace(conta.Conta) || string.IsNullOrWhiteSpace(conta.DigitoConta) ||
                    !(new[] { "CONTA_CORRENTE", "CONTA_POUPANCA" }).Contains(conta.TipoConta))
                    return "Informe os dados bancários completos do destinatário.";
                if (!Digitos(conta.CpfCnpjTitular, 11) && !Digitos(conta.CpfCnpjTitular, 14))
                    return "O CPF ou CNPJ do titular deve conter apenas dígitos.";
            }
            else return "Modalidade deve ser PIX_CHAVE, PIX_CONTA ou TED.";
            return null;
        }

        private static bool Digitos(string valor, int tamanho) => valor != null && valor.Length == tamanho && valor.All(c => c >= '0' && c <= '9');

        public static string ValidarContas(ContaRepasse primeira, ContaRepasse segunda)
        {
            var erro = Validar(primeira) ?? Validar(segunda);
            if (erro != null) return erro;
            var percentual = (primeira?.TipoValor == "PERCENTUAL" ? primeira.Valor : 0m) +
                (segunda?.TipoValor == "PERCENTUAL" ? segunda.Valor : 0m);
            return percentual > 100m ? "A soma dos percentuais de repasse não pode ultrapassar 100%." : null;
        }
    }
}
