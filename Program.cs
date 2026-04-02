using ImobAPI.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using BCrypt.Net;
using static BCrypt.Net.BCrypt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ImobContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao"))
    .UseSeeding((Context, _) =>
     {
         var imobContext = (ImobContext)Context;
         if (!imobContext.Usuarios.Any())
         {
             imobContext.Usuarios.Add(new ImobAPI.Entities.Usuario
             {
                 Nome = "Admin",
                 Login = "admin",
                 Email = "",
                 Senha = HashPassword("admin", 12),
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
         if (!imobContext.TiposCliente.Any())
         {
             imobContext.TiposCliente.Add(new ImobAPI.Entities.TipoCliente
             {
                 Nome = "PF",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.TiposCliente.Add(new ImobAPI.Entities.TipoCliente
             {
                 Nome = "PJ",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
         if (!imobContext.TiposImovel.Any())
         {
             imobContext.TiposImovel.Add(new ImobAPI.Entities.TipoImovel
             {
                 Nome = "Comercial",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.TiposImovel.Add(new ImobAPI.Entities.TipoImovel
             {
                 Nome = "Residencial",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.TiposImovel.Add(new ImobAPI.Entities.TipoImovel
             {
                 Nome = "Misto",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
         if (!imobContext.Intencoes.Any())
         {
             imobContext.Intencoes.Add(new ImobAPI.Entities.Intencao
             {
                 Nome = "Venda",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.Intencoes.Add(new ImobAPI.Entities.Intencao
             {
                 Nome = "Locação",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.Intencoes.Add(new ImobAPI.Entities.Intencao
             {
                 Nome = "Venda/Locação",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
         if(!imobContext.TiposFoto.Any())
         {
             imobContext.TiposFoto.Add(new ImobAPI.Entities.TipoFoto
             {
                 Nome = "Anúncio",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.TiposFoto.Add(new ImobAPI.Entities.TipoFoto
             {
                 Nome = "Vistoria",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
         }
         if(!imobContext.TiposContrato.Any())
         {
             imobContext.TiposContrato.Add(new ImobAPI.Entities.TipoContrato
             {
                 Nome = "Compra/Venda",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.TiposContrato.Add(new ImobAPI.Entities.TipoContrato
             {
                 Nome = "Locação",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
         if(!imobContext.ObjetosContrato.Any())
         {
             imobContext.ObjetosContrato.Add(new ImobAPI.Entities.ObjetoContrato
             {
                 Nome = "Comercial",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.ObjetosContrato.Add(new ImobAPI.Entities.ObjetoContrato
             {
                 Nome = "Residencial",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
         if(!imobContext.ModalidadesContrato.Any())
         {
             imobContext.ModalidadesContrato.Add(new ImobAPI.Entities.ModalidadeContrato
             {
                 Nome = "Caução",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.ModalidadesContrato.Add(new ImobAPI.Entities.ModalidadeContrato
             {
                 Nome = "Depósito",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.ModalidadesContrato.Add(new ImobAPI.Entities.ModalidadeContrato
             {
                 Nome = "Fiador",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.ModalidadesContrato.Add(new ImobAPI.Entities.ModalidadeContrato
             {
                 Nome = "Seguro Fiança",
                 Ativo = true,
                 DataCadastro = DateTime.Now
             });
             imobContext.SaveChanges();
         }
     }));

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT. Exemplo: eyJhbGciOi..."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
