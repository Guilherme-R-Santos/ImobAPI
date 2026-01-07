using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTabelaTiposContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipoContrato_Usuarios_CadastradorId",
                table: "TipoContrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoContrato",
                table: "TipoContrato");

            migrationBuilder.RenameTable(
                name: "TipoContrato",
                newName: "TiposContrato");

            migrationBuilder.RenameIndex(
                name: "IX_TipoContrato_CadastradorId",
                table: "TiposContrato",
                newName: "IX_TiposContrato_CadastradorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TiposContrato",
                table: "TiposContrato",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TiposContrato_Usuarios_CadastradorId",
                table: "TiposContrato",
                column: "CadastradorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiposContrato_Usuarios_CadastradorId",
                table: "TiposContrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TiposContrato",
                table: "TiposContrato");

            migrationBuilder.RenameTable(
                name: "TiposContrato",
                newName: "TipoContrato");

            migrationBuilder.RenameIndex(
                name: "IX_TiposContrato_CadastradorId",
                table: "TipoContrato",
                newName: "IX_TipoContrato_CadastradorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoContrato",
                table: "TipoContrato",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TipoContrato_Usuarios_CadastradorId",
                table: "TipoContrato",
                column: "CadastradorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
