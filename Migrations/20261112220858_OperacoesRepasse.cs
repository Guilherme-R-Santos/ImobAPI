using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class OperacoesRepasse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repasses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CobrancaId = table.Column<int>(type: "int", nullable: false),
                    Destinatario = table.Column<int>(type: "int", nullable: false),
                    Origem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DadosDestino = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenciaExterna = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IdTransferenciaAsaas = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Erro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComprovanteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SolicitanteId = table.Column<int>(type: "int", nullable: true),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repasses_Cobrancas_CobrancaId",
                        column: x => x.CobrancaId,
                        principalTable: "Cobrancas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repasses_CobrancaId_Destinatario",
                table: "Repasses",
                columns: new[] { "CobrancaId", "Destinatario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repasses_IdTransferenciaAsaas",
                table: "Repasses",
                column: "IdTransferenciaAsaas",
                unique: true,
                filter: "[IdTransferenciaAsaas] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Repasses_ReferenciaExterna",
                table: "Repasses",
                column: "ReferenciaExterna",
                unique: true,
                filter: "[ReferenciaExterna] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repasses");
        }
    }
}
