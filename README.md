# 🏡 ImobAPI

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/C%23-14.0-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C# 14" />
  <img src="https://img.shields.io/badge/ASP.NET%20Core-Web%20API-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/EF%20Core-10.0-6DB33F?style=for-the-badge&logo=databricks&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Auth-JWT%20Bearer-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT" />
  <img src="https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger" />
</p>

<p align="center">
  <img src="https://skillicons.dev/icons?i=dotnet,cs,visualstudio,docker" alt="Tecnologias" />
</p>

API REST para gestão imobiliária com foco em cadastro de usuários, clientes, imóveis, contratos, fotos, vistorias e tabelas auxiliares do domínio.

---

## 📚 Sumário

- [Visão geral](#-visão-geral)
- [Arquitetura do projeto](#-arquitetura-do-projeto)
- [Modelagem do domínio](#-modelagem-do-domínio)
- [Como a API funciona](#-como-a-api-funciona)
- [Autenticação e autorização (JWT)](#-autenticação-e-autorização-jwt)
- [Seeders (dados iniciais)](#-seeders-dados-iniciais)
- [Configuração e execução local](#-configuração-e-execução-local)
- [Execução com Docker](#-execução-com-docker)
- [Swagger / OpenAPI](#-swagger--openapi)
- [Rotas disponíveis](#-rotas-disponíveis)
- [Fluxo sugerido de uso](#-fluxo-sugerido-de-uso)

---

## 🎯 Visão geral

A `ImobAPI` foi construída como uma Web API em `ASP.NET Core` usando `Entity Framework Core` com `SQL Server`.

Ela permite:

- 👤 Gerenciar usuários do sistema
- 🧾 Gerenciar clientes e tipos de cliente
- 🏠 Gerenciar imóveis, tipos de imóvel e intenção (venda/locação)
- 📄 Gerenciar contratos, objetos e modalidades
- 🖼️ Gerenciar fotos e tipos de foto
- 🧰 Gerenciar vistorias vinculadas a contratos e imóveis

---

## 🧱 Arquitetura do projeto

Estrutura principal da solução:

- `Controllers/` → Endpoints HTTP (camada de entrada)
- `Entities/` → Entidades de domínio persistidas em banco
- `Context/ImobContext.cs` → `DbContext` com todos os `DbSet`
- `Migrations/` → Histórico de migrações do EF Core
- `Program.cs` → Configuração da aplicação (DI, JWT, Swagger, seed)
- `appsettings*.json` → Configurações por ambiente

Fluxo resumido:

```text
Cliente HTTP -> Controller -> ImobContext (EF Core) -> SQL Server
```

---

## 🧩 Modelagem do domínio

### Entidades principais

- `Usuario`
- `Cliente`
- `Imovel`
- `Contrato`
- `Foto`
- `Vistoria`

### Entidades de apoio (catálogos)

- `TipoCliente`
- `TipoImovel`
- `Intencao`
- `TipoContrato`
- `ObjetoContrato`
- `ModalidadeContrato`
- `TipoFoto`

### Relacionamentos importantes

- `Cliente` referencia `TipoCliente` e `Usuario` (cadastrador)
- `Imovel` referencia `Cliente` (proprietário), `TipoImovel`, `Intencao`, `Usuario`
- `Contrato` referencia múltiplos `Cliente` (proprietário/contratantes/fiador), além de `Imovel`, `TipoContrato`, `ObjetoContrato`, `ModalidadeContrato`, `Usuario`
- `Foto` referencia `Imovel`, `TipoFoto`, `Usuario` e opcionalmente `Vistoria`
- `Vistoria` referencia `Contrato`, `Imovel` e `Usuario`

---

## ⚙️ Como a API funciona

No `Program.cs`, a aplicação configura:

1. `DbContext` com SQL Server (`ConexaoPadrao`)
2. Seed inicial via `UseSeeding(...)`
3. Controllers (`AddControllers`)
4. Autenticação JWT (`AddAuthentication().AddJwtBearer(...)`)
5. Política global de autorização (`FallbackPolicy` exigindo usuário autenticado)
6. OpenAPI/Swagger (`AddOpenApi`, `AddSwaggerGen`)

Em ambiente de desenvolvimento:

- Swagger UI é habilitado
- OpenAPI JSON é mapeado

---

## 🔐 Autenticação e autorização (JWT)

### Regras atuais

- A API aplica autenticação por padrão em todas as rotas (`FallbackPolicy`)
- Rotas anônimas explícitas:
  - `GET /Usuario/Connect`
  - `GET /Usuario/Login`

### Emissão de token

O token é gerado em `UsuarioController` na rota `GET /Usuario/Login`, usando configurações:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpirationInMinutes`

### Uso do token

Enviar no header:

`Authorization: Bearer {seu_token}`

---

## 🌱 Seeders (dados iniciais)

Na inicialização, o sistema tenta popular dados básicos quando as tabelas estão vazias:

- Usuário padrão
  - `Login`: `admin`
  - `Senha`: `admin` (armazenada com hash BCrypt, fator 12)
- Tipos de cliente:
  - `Proprietário`, `Fiador`, `Locatário`
- Tipos de imóvel:
  - `Comercial`, `Residencial`, `Misto`
- Intenções:
  - `Venda`, `Locação`, `Venda/Locação`
- Tipos de foto:
  - `Anúncio`, `Vistoria`
- Tipos de contrato:
  - `Compra/Venda`, `Locação`
- Objetos de contrato:
  - `Comercial`, `Residencial`
- Modalidades de contrato:
  - `Caução`, `Depósito`, `Fiador`, `Seguro Fiança`

---

## 🚀 Configuração e execução local

### Pré-requisitos

- SDK `.NET 10`
- SQL Server (ou SQL Express)

### 1) Configurar conexão

No `appsettings.Development.json`:

- `ConnectionStrings:ConexaoPadrao`

Exemplo atual do projeto:

`Server=localhost\\sqlexpress;Initial Catalog=Imob;Integrated Security=True;TrustServerCertificate=True`

### 2) Configurar JWT

Em `appsettings.json`/`appsettings.Development.json`:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpirationInMinutes`

### 3) Executar

```bash
dotnet restore
dotnet build
dotnet run
```

### URLs padrão de desenvolvimento

Conforme `Properties/launchSettings.json`:

- `https://localhost:7251`
- `http://localhost:5067`

---

## 🐳 Execução com Docker

O projeto possui `Dockerfile` baseado em imagens oficiais `.NET 10`.

Build:

```bash
docker build -t imobapi .
```

Run (exemplo):

```bash
docker run -p 8080:8080 -p 8081:8081 imobapi
```

---

## 🧪 Swagger / OpenAPI

Em `Development`, a documentação interativa fica disponível via Swagger UI.

### Recursos habilitados

- Descrição automática das rotas dos controllers
- Botão `Authorize` com esquema `Bearer`
- Suporte para envio de JWT nas requisições protegidas

### Endpoints de documentação

- Swagger UI: `/swagger`
- OpenAPI JSON: `/openapi/v1.json`

---

## 🛣️ Rotas disponíveis

> Base route dos controllers: `/<NomeDoController>`

### `UsuarioController`

- `GET /Usuario/Connect` *(anônima)*
- `POST /Usuario/Criar`
- `GET /Usuario/ObterPorId/{id}`
- `GET /Usuario/ObterPorLogin/{login}`
- `GET /Usuario/ObterTodos`
- `GET /Usuario/ObterTodosAtivos`
- `PUT /Usuario/Atualizar/{id}`
- `PUT /Usuario/InativarUsuario/{id}`
- `DELETE /Usuario/Deletar/{id}`
- `GET /Usuario/Login` *(anônima)*

### `ClienteController`

- `POST /Cliente/Criar`
- `GET /Cliente/ObterTodos`
- `GET /Cliente/ObterPorId/{id}`
- `GET /Cliente/ObterPorNome/{nome}`
- `GET /Cliente/ObterPorCpfCnpj/{cpfCnpj}`
- `GET /Cliente/ObterPorTipo/{tipoId}`
- `PUT /Cliente/Inativar/{id}`
- `PUT /Cliente/Atualizar/{id}`
- `PUT /Cliente/Ativar/{id}`

### `TipoClienteController`

- `POST /TipoCliente/CriarTipo`
- `GET /TipoCliente/ListarTipos`
- `GET /TipoCliente/ObterTipo/{id}`
- `PUT /TipoCliente/AtualizarTipo/{id}`
- `PUT /TipoCliente/InativarTipo/{id}`
- `PUT /TipoCliente/AtivarTipo/{id}`

### `TipoImovelController`

- `POST /TipoImovel/Criar`
- `GET /TipoImovel/ObterTodos`
- `GET /TipoImovel/ObterPorId/{id}`
- `PUT /TipoImovel/Inativar/{id}`
- `PUT /TipoImovel/Atualizar/{id}`
- `GET /TipoImovel/ObterPorNome/{nome}`

### `IntencaoController`

- `POST /Intencao/Criar`
- `GET /Intencao/ObterTodas`
- `GET /Intencao/ObterPorId/{id}`
- `PUT /Intencao/Inativar/{id}`
- `PUT /Intencao/Atualizar/{id}`
- `GET /Intencao/ObterPorNome/{nome}`

### `ImovelController`

- `POST /Imovel/Criar`
- `GET /Imovel/ObterTodos`
- `GET /Imovel/ObterPorId/{id}`
- `POST /Imovel/Inativar/{id}`
- `GET /Imovel/ObterPorProprietario/{proprietarioId}`
- `PUT /Imovel/Atualizar/{id}`

### `TipoContratoController`

- `POST /TipoContrato/Criar`
- `GET /TipoContrato/ObterTodos`
- `GET /TipoContrato/ObterPorId/{id}`
- `PUT /TipoContrato/Inativar/{id}`
- `PUT /TipoContrato/Atualizar/{id}`
- `GET /TipoContrato/ObterPorNome/{nome}`

### `ObjetoContratoController`

- `POST /ObjetoContrato/Criar`
- `GET /ObjetoContrato/ObterTodos`
- `GET /ObjetoContrato/ObterPorId/{id}`
- `GET /ObjetoContrato/ObterPorNome/{nome}`
- `PUT /ObjetoContrato/Inativar/{id}`
- `PUT /ObjetoContrato/Atualizar/{id}`

### `ModalidadeContratoController`

- `POST /ModalidadeContrato/Criar`
- `GET /ModalidadeContrato/ObterTodos`
- `GET /ModalidadeContrato/ObterPorId/{id}`
- `GET /ModalidadeContrato/ObterPorNome/{nome}`
- `PUT /ModalidadeContrato/Atualizar/{id}`
- `PUT /ModalidadeContrato/Inativar/{id}`

### `ContratoController`

- `POST /Contrato/Criar`
- `GET /Contrato/ObterTodos`
- `GET /Contrato/ObterPorId/{id}`
- `GET /Contrato/ObterPorProprietario/{proprietarioId}`
- `GET /Contrato/ObterPorContratante/{contratanteId}`
- `PUT /Contrato/Inativar/{id}`
- `PUT /Contrato/Atualizar/{id}`

### `TipoFotoController`

- `POST /TipoFoto/Criar`
- `GET /TipoFoto/ObterTodos`

### `FotoController`

- `POST /Foto/Criar`
- `GET /Foto/ObterPorImovel/{imovelId}`
- `PUT /Foto/Inativar/{id}`
- `GET /Foto/ObterTodas`

### `VistoriaController`

- `POST /Vistoria/Criar`
- `GET /Vistoria/ObterTodos`
- `GET /Vistoria/ObterPorId/{id}`
- `GET /Vistoria/ObterPorImovel/{imovelId}`
- `POST /Vistoria/Inativar/{id}`
- `POST /Vistoria/Atualizar/{id}`
- `GET /Vistoria/ObterFotosDaVistoria/{vistoriaId}`
- `GET /Vistoria/ObterVistoriaPorContrato/{contratoId}`

---

## 🧭 Fluxo sugerido de uso

Para explorar a API de forma natural:

1. Criar/usar usuário (`/Usuario/Criar` ou seed `admin`)
2. Autenticar em `/Usuario/Login` e obter JWT
3. Autorizar no Swagger com `Bearer {token}`
4. Validar cadastros base (`TipoCliente`, `TipoImovel`, `Intencao`, `TipoContrato`, etc.)
5. Cadastrar `Cliente`
6. Cadastrar `Imovel`
7. Cadastrar `Contrato`
8. Cadastrar `Vistoria` e `Foto`

---

## ✅ Status

Projeto funcional com autenticação JWT, persistência relacional via EF Core e documentação de API via Swagger/OpenAPI.

### Projeto ainda em desenvolvimento, mas com funcionalidades principais implementadas e testadas.
