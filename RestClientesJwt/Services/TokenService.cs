// Importa tipos para gerar o token JWT
using Microsoft.IdentityModel.Tokens;
// Importa gerenciador/escritor do JWT
using System.IdentityModel.Tokens.Jwt;
// Importa tipos de "claims" (informações sobre o usuário)
using System.Security.Claims;
// Importa System.Text para converter a chave em bytes
using System.Text;

namespace RestClientesJwt.Services;

// Serviço que gera tokens JWT e valida credenciais (demo)
public class TokenService
{
    // Mantém acesso às configurações (appsettings.json)
    private readonly IConfiguration _cfg;

    // Construtor
    // O ASP.NET injeta IConfiguration
    // Injeção de dependência (DI) - Padrão de design que desacopla classes de suas dependências,
    //   permitindo que elas recebam as dependências de fora, em vez de criá-las internamente.
    //   Essa técnica resulta em um código modular, flexível, fácil de testar e manter, pois as
    //   classes passam a depender de abstrações (interfaces) em vez de implementações concretas. 
    public TokenService(IConfiguration cfg)
    {
        // Salva a referência; 'cfg' acessa appsettings.json
        _cfg = cfg;
    }

    // Valida credenciais de DEMO (sem banco): dois usuários fixos
    public bool ValidateDemoCredentials(string username, string password)
    {
        // Retorna true para pares conhecidos; false caso contrário
        return (username == "admin" && password == "admin")
            || (username == "teste" && password == "123456");
    }




    /// <summary>
    /// Gera um JWT assinado para o usuário informado.
    /// Lê Issuer/Audience/Key do appsettings.json, cria as credenciais de assinatura,
    /// define "hora" e a expiração (padrão 1h ou valor em ttl),
    /// adiciona claims básicas (sub, jti, name),
    /// monta o JwtSecurityToken com emissor, audiência, claims e validade,
    /// serializa e retorna o token no formato compactado (string).
    /// </summary>
    /// <param name="username">Identidade do usuário (vai em 'sub' e 'name').</param>
    /// <param name="ttl">Tempo de vida opcional do token; se nulo, usa 1 hora.</param>
    /// <returns>Token JWT compactado assinado (string).</returns>
    public string GenerateToken(string username, TimeSpan? ttl = null)
    {
        // Lê Issuer/Audience/Key do appsettings.json
        var issuer = _cfg["Jwt:Issuer"];
        var audience = _cfg["Jwt:Audience"];
        var key = _cfg["Jwt:Key"]!;

        // Cria credenciais de assinatura HMAC-SHA256 com a chave
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        // Define instante atual e expiração (default 1h)
        var now = DateTime.UtcNow;
        var expires = now.Add(ttl ?? TimeSpan.FromHours(1));

        // Constrói a lista de "claims" (informações dentro do token)
        var claims = new List<Claim>
        {
            // "sub" = subject (quem é o dono do token)
            new(JwtRegisteredClaimNames.Sub, username),
            // "jti" = id único do token
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // Nome amistoso
            new(ClaimTypes.Name, username)
        };

        // Monta o objeto JWT com emissor, audiência, claims, validade e assinatura
        var token = new JwtSecurityToken(issuer, audience, claims, now, expires, signingCredentials);

        // Serializa o JWT para string (compact token)
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
