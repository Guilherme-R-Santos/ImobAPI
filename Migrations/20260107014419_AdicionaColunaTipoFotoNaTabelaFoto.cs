using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaColunaTipoFotoNaTabelaFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoFotoId",
                table: "Fotos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_TipoFotoId",
                table: "Fotos",
                column: "TipoFotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fotos_TiposFoto_TipoFotoId",
                table: "Fotos",
                column: "TipoFotoId",
                principalTable: "TiposFoto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fotos_TiposFoto_TipoFotoId",
                table: "Fotos");

            migrationBuilder.DropIndex(
                name: "IX_Fotos_TipoFotoId",
                table: "Fotos");

            migrationBuilder.DropColumn(
                name: "TipoFotoId",
                table: "Fotos");
        }
    }
}
