using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaColunasInscricaoIptuENumeroCbmerjImoveis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InscricaoIptu",
                table: "Imoveis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCbmerj",
                table: "Imoveis",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InscricaoIptu",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "NumeroCbmerj",
                table: "Imoveis");
        }
    }
}
