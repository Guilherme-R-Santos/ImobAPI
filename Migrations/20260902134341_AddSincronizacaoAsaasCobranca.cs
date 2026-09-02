using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSincronizacaoAsaasCobranca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErroSincronizacaoAsaas",
                table: "Cobrancas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SincronizadoAsaas",
                table: "Cobrancas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErroSincronizacaoAsaas",
                table: "Cobrancas");

            migrationBuilder.DropColumn(
                name: "SincronizadoAsaas",
                table: "Cobrancas");
        }
    }
}
