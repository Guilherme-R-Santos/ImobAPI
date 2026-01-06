using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoTabelasImoveisEComplementares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Intencoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CadastradorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intencoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intencoes_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TiposImovel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CadastradorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposImovel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposImovel_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Imoveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProprietarioId = table.Column<int>(type: "int", nullable: true),
                    TipoImovelId = table.Column<int>(type: "int", nullable: true),
                    IntencaoId = table.Column<int>(type: "int", nullable: true),
                    Cep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logradouro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metragem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Condominio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Iptu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxaIncendio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Foro = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imoveis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imoveis_Clientes_ProprietarioId",
                        column: x => x.ProprietarioId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imoveis_Intencoes_IntencaoId",
                        column: x => x.IntencaoId,
                        principalTable: "Intencoes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imoveis_TiposImovel_TipoImovelId",
                        column: x => x.TipoImovelId,
                        principalTable: "TiposImovel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imoveis_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imoveis_CadastradorId",
                table: "Imoveis",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Imoveis_IntencaoId",
                table: "Imoveis",
                column: "IntencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Imoveis_ProprietarioId",
                table: "Imoveis",
                column: "ProprietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Imoveis_TipoImovelId",
                table: "Imoveis",
                column: "TipoImovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Intencoes_CadastradorId",
                table: "Intencoes",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposImovel_CadastradorId",
                table: "TiposImovel",
                column: "CadastradorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imoveis");

            migrationBuilder.DropTable(
                name: "Intencoes");

            migrationBuilder.DropTable(
                name: "TiposImovel");
        }
    }
}
