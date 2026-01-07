using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTabelaContratos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    TipoContratoId = table.Column<int>(type: "int", nullable: true),
                    ProprietarioId = table.Column<int>(type: "int", nullable: true),
                    Contratante1Id = table.Column<int>(type: "int", nullable: true),
                    Contratante2Id = table.Column<int>(type: "int", nullable: true),
                    Contratante3Id = table.Column<int>(type: "int", nullable: true),
                    Contratante4Id = table.Column<int>(type: "int", nullable: true),
                    FiadorId = table.Column<int>(type: "int", nullable: true),
                    ImovelId = table.Column<int>(type: "int", nullable: true),
                    ObjetoContratoId = table.Column<int>(type: "int", nullable: true),
                    ModalidadeContratoId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInicioVigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrazoMeses = table.Column<int>(type: "int", nullable: false),
                    Vencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFimVigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PropostaSegFianca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApoliceSegFianca = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_Contratante1Id",
                        column: x => x.Contratante1Id,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_Contratante2Id",
                        column: x => x.Contratante2Id,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_Contratante3Id",
                        column: x => x.Contratante3Id,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_Contratante4Id",
                        column: x => x.Contratante4Id,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_FiadorId",
                        column: x => x.FiadorId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_ProprietarioId",
                        column: x => x.ProprietarioId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Imoveis_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imoveis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_ModalidadesContrato_ModalidadeContratoId",
                        column: x => x.ModalidadeContratoId,
                        principalTable: "ModalidadesContrato",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_ObjetosContrato_ObjetoContratoId",
                        column: x => x.ObjetoContratoId,
                        principalTable: "ObjetosContrato",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_TiposContrato_TipoContratoId",
                        column: x => x.TipoContratoId,
                        principalTable: "TiposContrato",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contratos_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_CadastradorId",
                table: "Contratos",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_Contratante1Id",
                table: "Contratos",
                column: "Contratante1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_Contratante2Id",
                table: "Contratos",
                column: "Contratante2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_Contratante3Id",
                table: "Contratos",
                column: "Contratante3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_Contratante4Id",
                table: "Contratos",
                column: "Contratante4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_FiadorId",
                table: "Contratos",
                column: "FiadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ImovelId",
                table: "Contratos",
                column: "ImovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ModalidadeContratoId",
                table: "Contratos",
                column: "ModalidadeContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ObjetoContratoId",
                table: "Contratos",
                column: "ObjetoContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ProprietarioId",
                table: "Contratos",
                column: "ProprietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_TipoContratoId",
                table: "Contratos",
                column: "TipoContratoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratos");
        }
    }
}
