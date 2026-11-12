using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAutentiqueContratos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnvioAutentiqueEmAndamento",
                table: "Contratos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IdDocumentoAutentique",
                table: "Contratos",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                collation: "Latin1_General_100_BIN2");

            migrationBuilder.AddColumn<string>(
                name: "Testemunha1",
                table: "Contratos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Testemunha2",
                table: "Contratos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Testemunha3",
                table: "Contratos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentosAutentique",
                columns: table => new
                {
                    IdDocumentoAutentique = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, collation: "Latin1_General_100_BIN2"),
                    ContratoId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Sandbox = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CriadoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ConcluidoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UltimaSincronizacaoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosAutentique", x => x.IdDocumentoAutentique);
                    table.UniqueConstraint("AK_DocumentosAutentique_ContratoId_IdDocumentoAutentique", x => new { x.ContratoId, x.IdDocumentoAutentique });
                    table.ForeignKey(
                        name: "FK_DocumentosAutentique_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignatariosAutentique",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDocumentoAutentique = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, collation: "Latin1_General_100_BIN2"),
                    PublicIdAutentique = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, collation: "Latin1_General_100_BIN2"),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Acao = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CriadoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    VisualizadoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AssinadoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RecusadoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatariosAutentique", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignatariosAutentique_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignatariosAutentique_DocumentosAutentique_IdDocumentoAutentique",
                        column: x => x.IdDocumentoAutentique,
                        principalTable: "DocumentosAutentique",
                        principalColumn: "IdDocumentoAutentique",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_Id_IdDocumentoAutentique",
                table: "Contratos",
                columns: new[] { "Id", "IdDocumentoAutentique" });

            migrationBuilder.CreateIndex(
                name: "IX_SignatariosAutentique_ClienteId",
                table: "SignatariosAutentique",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_SignatariosAutentique_IdDocumentoAutentique_PublicIdAutentique",
                table: "SignatariosAutentique",
                columns: new[] { "IdDocumentoAutentique", "PublicIdAutentique" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_DocumentosAutentique_Id_IdDocumentoAutentique",
                table: "Contratos",
                columns: new[] { "Id", "IdDocumentoAutentique" },
                principalTable: "DocumentosAutentique",
                principalColumns: new[] { "ContratoId", "IdDocumentoAutentique" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_DocumentosAutentique_Id_IdDocumentoAutentique",
                table: "Contratos");

            migrationBuilder.DropTable(
                name: "SignatariosAutentique");

            migrationBuilder.DropTable(
                name: "DocumentosAutentique");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_Id_IdDocumentoAutentique",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "EnvioAutentiqueEmAndamento",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "IdDocumentoAutentique",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Testemunha1",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Testemunha2",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Testemunha3",
                table: "Contratos");
        }
    }
}
