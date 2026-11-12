using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEventosWebhookAsaas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventosWebhookAsaas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEventoAsaas = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, collation: "Latin1_General_100_BIN2"),
                    Tipo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IdCobrancaAsaas = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecebidoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProximaTentativaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosWebhookAsaas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosWebhookAsaas_IdEventoAsaas",
                table: "EventosWebhookAsaas",
                column: "IdEventoAsaas",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosWebhookAsaas");
        }
    }
}
