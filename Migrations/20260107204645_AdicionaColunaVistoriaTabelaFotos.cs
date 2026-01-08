using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaColunaVistoriaTabelaFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VistoriaId",
                table: "Fotos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_VistoriaId",
                table: "Fotos",
                column: "VistoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fotos_Vistorias_VistoriaId",
                table: "Fotos",
                column: "VistoriaId",
                principalTable: "Vistorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fotos_Vistorias_VistoriaId",
                table: "Fotos");

            migrationBuilder.DropIndex(
                name: "IX_Fotos_VistoriaId",
                table: "Fotos");

            migrationBuilder.DropColumn(
                name: "VistoriaId",
                table: "Fotos");
        }
    }
}
