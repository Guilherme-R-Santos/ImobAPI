using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class alteraTipoColunaVencimentoParaInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VencimentoNovo",
                table: "Contratos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("UPDATE Contratos SET VencimentoNovo = DAY(Vencimento)");

            migrationBuilder.DropColumn(
                name: "Vencimento",
                table: "Contratos");

            migrationBuilder.RenameColumn(
                name: "VencimentoNovo",
                table: "Contratos",
                newName: "Vencimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VencimentoData",
                table: "Contratos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1));

            migrationBuilder.Sql("UPDATE Contratos SET VencimentoData = DATEFROMPARTS(2000, 1, CASE WHEN Vencimento BETWEEN 1 AND 31 THEN Vencimento ELSE 1 END)");

            migrationBuilder.DropColumn(
                name: "Vencimento",
                table: "Contratos");

            migrationBuilder.RenameColumn(
                name: "VencimentoData",
                table: "Contratos",
                newName: "Vencimento");
        }
    }
}
