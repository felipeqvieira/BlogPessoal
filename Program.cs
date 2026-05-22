using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BlogPessoal.Config;
using BlogPessoal.Data;
using BlogPessoal.Middlewares;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using BlogPessoal.Services;     
using BlogPessoal.Services.IA; 

var builder = WebApplication.CreateBuilder(args);

// INJEÇÃO DE DEPENDÊNCIAS E CONFIGURAÇÃO DE SERVIÇOS

// Configuração do ORM com o provedor MySQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    )
);

// Configuração do Identity
// Integra as regras de usuário e roles com o contexto de base de dados.
builder.Services
    .AddIdentity<Usuario, IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>();

// Autenticação JWT
// Força o ASP.NET a procurar e validar exclusivamente Tokens JWT no cabeçalho 
// das requisições HTTP.
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme   = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme            = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer      = builder.Configuration["Jwt:Issuer"],
        ValidAudience    = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});

// Políticas de Autorização
// Cria rótulos de acesso que os Controladores podem usar via [Authorize(Policy = "AdminOnly")]
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    opt.AddPolicy("UserPolicy", p => p.RequireRole("User", "Admin"));
});

// Configuração do Cliente HTTP para integrações externas
// Estabelece um timeout global de 30 segundos para evitar que falhas de rede no Groq bloqueiem threads do servidor.
builder.Services.AddHttpClient("Groq", c =>
{
    c.Timeout = TimeSpan.FromSeconds(30);
});

// Registo dos Repositórios
builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
builder.Services.AddScoped<IRepository<Tema>, TemaRepository>();

// Registo dos Serviços de Domínio e Infraestrutura
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IIAService, GroqService>();
builder.Services.AddScoped<IPostagemService, PostagemService>();
builder.Services.AddScoped<ITemaService, TemaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger com suporte avançado a JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogPessoal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Insira apenas o token JWT, sem o prefixo Bearer."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// PIPELINE DE MIDDLEWARES

// Interceta e formata qualquer exceção lançada pelas camadas inferiores.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona tráfego HTTP para HTTPS.
app.UseHttpsRedirection();

// Identifica QUEM é o usuário.
app.UseAuthentication();
// Verifica O QUE o utilizador pode fazer.
app.UseAuthorization();

// Encaminha a requisição validada para o Controller apropriado.
app.MapControllers();

// INICIALIZAÇÃO DE DADOS (Seeding)
// Abre um escopo temporário para injetar as roles básicas no bd
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<long>(role));
        }
    }
}

// Inicia a escuta de tráfego de rede no servidor web.
await app.RunAsync();