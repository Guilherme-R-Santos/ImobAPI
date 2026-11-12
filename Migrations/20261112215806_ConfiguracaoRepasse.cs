using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracaoRepasse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContaPartilha",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaPartilha",
                table: "Cobrancas");

            migrationBuilder.RenameColumn(
                name: "PartilhaAutomatica",
                table: "Contratos",
                newName: "RepasseAutomatico");

            migrationBuilder.RenameColumn(
                name: "PartilhaAutomatica",
                table: "Cobrancas",
                newName: "RepasseAutomatico");

            migrationBuilder.Sql("UPDATE Contratos SET RepasseAutomatico = 0; UPDATE Cobrancas SET RepasseAutomatico = 0;");

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_Agencia",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_ChavePix",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_CodigoBanco",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_Conta",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_CpfCnpjTitular",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContaRepasse1_DataNascimentoTitular",
                table: "Contratos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_DigitoConta",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_Modalidade",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_NomeTitular",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_TipoChavePix",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_TipoConta",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_TipoValor",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContaRepasse1_Valor",
                table: "Contratos",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_Agencia",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_ChavePix",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_CodigoBanco",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_Conta",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_CpfCnpjTitular",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContaRepasse2_DataNascimentoTitular",
                table: "Contratos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_DigitoConta",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_Modalidade",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_NomeTitular",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_TipoChavePix",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_TipoConta",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_TipoValor",
                table: "Contratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContaRepasse2_Valor",
                table: "Contratos",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_Agencia",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_ChavePix",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_CodigoBanco",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_Conta",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_CpfCnpjTitular",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContaRepasse1_DataNascimentoTitular",
                table: "Cobrancas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_DigitoConta",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_Modalidade",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_NomeTitular",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_TipoChavePix",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_TipoConta",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse1_TipoValor",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContaRepasse1_Valor",
                table: "Cobrancas",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_Agencia",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_ChavePix",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_CodigoBanco",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_Conta",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_CpfCnpjTitular",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContaRepasse2_DataNascimentoTitular",
                table: "Cobrancas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_DigitoConta",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_Modalidade",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_NomeTitular",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_TipoChavePix",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_TipoConta",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContaRepasse2_TipoValor",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContaRepasse2_Valor",
                table: "Cobrancas",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Agencia",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_ChavePix",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_CodigoBanco",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Conta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_CpfCnpjTitular",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_DataNascimentoTitular",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_DigitoConta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Modalidade",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_NomeTitular",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_TipoChavePix",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_TipoConta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_TipoValor",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Valor",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Agencia",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_ChavePix",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_CodigoBanco",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Conta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_CpfCnpjTitular",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_DataNascimentoTitular",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_DigitoConta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Modalidade",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_NomeTitular",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_TipoChavePix",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_TipoConta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_TipoValor",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Valor",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Agencia",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_ChavePix",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_CodigoBanco",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Conta",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_CpfCnpjTitular",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_DataNascimentoTitular",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_DigitoConta",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Modalidade",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_NomeTitular",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_TipoChavePix",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_TipoConta",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_TipoValor",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse1_Valor",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Agencia",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_ChavePix",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_CodigoBanco",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Conta",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_CpfCnpjTitular",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_DataNascimentoTitular",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_DigitoConta",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Modalidade",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_NomeTitular",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_TipoChavePix",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_TipoConta",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_TipoValor",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "ContaRepasse2_Valor",
                table: "Cobrancas");

            migrationBuilder.RenameColumn(
                name: "RepasseAutomatico",
                table: "Contratos",
                newName: "PartilhaAutomatica");

            migrationBuilder.RenameColumn(
                name: "RepasseAutomatico",
                table: "Cobrancas",
                newName: "PartilhaAutomatica");

            migrationBuilder.AddColumn<int>(
                name: "ContaPartilha",
                table: "Contratos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContaPartilha",
                table: "Cobrancas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
