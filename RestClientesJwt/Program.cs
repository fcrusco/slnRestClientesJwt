// Importa recursos de autenticação JWT do ASP.NET Core
using Microsoft.AspNetCore.Authentication.JwtBearer;
// Importa atributos e helpers MVC (ex.: ApiController)
using Microsoft.AspNetCore.Mvc;
// Importa tipos para montar/validar tokens
using Microsoft.IdentityModel.Tokens;
// Importa tipos para configurar Swagger/OpenAPI com segurança
using Microsoft.OpenApi.Models;
// Importa System.Text para converter string da chave em bytes
using System.Text;
// Importa nossos serviços (TokenService, InMemoryStore)
using RestClientesJwt.Services;

// Cria o builder da aplicação (configura DI, logging, config)
var builder = WebApplication.CreateBuilder(args);

// Adiciona o suporte a Controllers (padrão Web API com atributos)
builder.Services.AddControllers();

// Adiciona o suporte ao Swagger (documentação/teste)
builder.Services.AddEndpointsApiExplorer();
// Configura o Swagger para reconhecer autenticação Bearer JWT
builder.Services.AddSwaggerGen(c =>
{
    // Cria o documento v1
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "REST Clientes + JWT", Version = "v1" });

    // Define o esquema de segurança do tipo HTTP Bearer (JWT)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Esquema: Bearer {seu_token_jwt}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
    };

    // Registra a definição Bearer
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Exige o esquema Bearer nas operações do Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// Lê a seção Jwt do appsettings.json
var jwtSection = builder.Configuration.GetSection("Jwt");
// Converte a chave secreta (string) em bytes
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

// Configura a AUTENTICAÇÃO para usar Bearer Tokens (JWT)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Define as regras de validação do token recebido
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Valida quem emitiu o token
            ValidateIssuer = true,
            // Valida para quem o token é destinado
            ValidateAudience = true,
            // Valida a data de expiração do token
            ValidateLifetime = true,
            // Valida a assinatura (chave) do token
            ValidateIssuerSigningKey = true,
            // Emissor esperado (appsettings)
            ValidIssuer = jwtSection["Issuer"],
            // Audiência esperada (appsettings)
            ValidAudience = jwtSection["Audience"],
            // Chave usada para verificar a assinatura HMAC-SHA256
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            // Tolerância para diferenças de relógio entre máquinas
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// Adiciona AUTORIZAÇÃO (políticas/roles se necessário)
builder.Services.AddAuthorization();

// Registra o "banco" em memória como Singleton (vive toda a app)
builder.Services.AddSingleton<InMemoryStore>();
// Registra o serviço que gera tokens
builder.Services.AddSingleton<TokenService>();

// Constrói o app com tudo configurado
var app = builder.Build();

// Se o ambiente é Desenvolvimento, habilita Swagger UI
if (app.Environment.IsDevelopment())
{
    // Exibe o endpoint do Swagger JSON
    app.UseSwagger();
    // Exibe a UI interativa do Swagger
    app.UseSwaggerUI();
}

// Redireciona HTTP → HTTPS (boa prática)
app.UseHttpsRedirection();

// Habilita o middleware de AUTENTICAÇÃO (lê/valida JWT do header)
app.UseAuthentication();
// Habilita o middleware de AUTORIZAÇÃO (checa [Authorize] nos endpoints)
app.UseAuthorization();

// Mapeia os Controllers (atributos [Route], [HttpGet], etc.)
app.MapControllers();

// Inicia o servidor Kestrel e começa a atender requisições
app.Run();
