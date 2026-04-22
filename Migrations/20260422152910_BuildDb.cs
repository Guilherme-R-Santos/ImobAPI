using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImobAPI.Migrations
{
    /// <inheritdoc />
    public partial class BuildDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

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
                name: "ModalidadesContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalidadesContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModalidadesContrato_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObjetosContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetosContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjetosContrato_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TiposCliente",
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
                    table.PrimaryKey("PK_TiposCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposCliente_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TiposContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposContrato_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TiposFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposFoto_Usuarios_CadastradorId",
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
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoClienteId = table.Column<int>(type: "int", nullable: true),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CpfCnpj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Identidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgaoExpedidor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nacionalidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Naturalidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Profissao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Agencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodBanco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Banco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_TiposCliente_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalTable: "TiposCliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Clientes_Usuarios_CadastradorId",
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
                    InscricaoIptu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroCbmerj = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metragem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorLocacao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorVenda = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    ValorContrato = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    Vencimento = table.Column<int>(type: "int", nullable: false),
                    DataFimVigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Vistorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContratoId = table.Column<int>(type: "int", nullable: true),
                    ImovelId = table.Column<int>(type: "int", nullable: true),
                    DataVistoria = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEntregaChaves = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vistorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vistorias_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vistorias_Imoveis_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imoveis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vistorias_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Fotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImovelId = table.Column<int>(type: "int", nullable: true),
                    NomeArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoFotoId = table.Column<int>(type: "int", nullable: true),
                    Bin = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CadastradorId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataInativacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Principal = table.Column<bool>(type: "bit", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    VistoriaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fotos_Imoveis_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imoveis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fotos_TiposFoto_TipoFotoId",
                        column: x => x.TipoFotoId,
                        principalTable: "TiposFoto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fotos_Usuarios_CadastradorId",
                        column: x => x.CadastradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fotos_Vistorias_VistoriaId",
                        column: x => x.VistoriaId,
                        principalTable: "Vistorias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CadastradorId",
                table: "Clientes",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_CadastradorId",
                table: "Fotos",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_ImovelId",
                table: "Fotos",
                column: "ImovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_TipoFotoId",
                table: "Fotos",
                column: "TipoFotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_VistoriaId",
                table: "Fotos",
                column: "VistoriaId");

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
                name: "IX_ModalidadesContrato_CadastradorId",
                table: "ModalidadesContrato",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetosContrato_CadastradorId",
                table: "ObjetosContrato",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposCliente_CadastradorId",
                table: "TiposCliente",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposContrato_CadastradorId",
                table: "TiposContrato",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposFoto_CadastradorId",
                table: "TiposFoto",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposImovel_CadastradorId",
                table: "TiposImovel",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vistorias_CadastradorId",
                table: "Vistorias",
                column: "CadastradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vistorias_ContratoId",
                table: "Vistorias",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vistorias_ImovelId",
                table: "Vistorias",
                column: "ImovelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fotos");

            migrationBuilder.DropTable(
                name: "TiposFoto");

            migrationBuilder.DropTable(
                name: "Vistorias");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "Imoveis");

            migrationBuilder.DropTable(
                name: "ModalidadesContrato");

            migrationBuilder.DropTable(
                name: "ObjetosContrato");

            migrationBuilder.DropTable(
                name: "TiposContrato");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Intencoes");

            migrationBuilder.DropTable(
                name: "TiposImovel");

            migrationBuilder.DropTable(
                name: "TiposCliente");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
