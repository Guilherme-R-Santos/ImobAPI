using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEnviosCobrancaEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnviosCobrancaEmail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CobrancaId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Referencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Situacao = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IniciadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConcluidoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnviosCobrancaEmail", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnviosCobrancaEmail_CobrancaId_Tipo_Referencia",
                table: "EnviosCobrancaEmail",
                columns: new[] { "CobrancaId", "Tipo", "Referencia" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnviosCobrancaEmail");
        }
    }
}
