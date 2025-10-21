// Importa atributos de autorização
using Microsoft.AspNetCore.Authorization;
// Importa tipos MVC (ControllerBase, ActionResult)
using Microsoft.AspNetCore.Mvc;
// Importa nosso TokenService para gerar tokens
using RestClientesJwt.Services;


namespace RestClientesJwt.Controllers.v1
{

    /// <summary>
    /// Controlador de autenticação (v1) responsável por emitir tokens JWT (Bearer).
    /// Exige DI de <see cref="RestClientesJwt.Services.TokenService"/> para validar credenciais 
    /// e gerar o token.
    /// Rota base: /api/v1/auth. Endpoints documentados no Swagger com respostas 200 e 401.
    /// </summary>
    // Marca como controlador de API (binding/validação automáticos)
    [ApiController]
    // Define a rota base para este controlador (prefixo do endpoint)
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        // Mantém referência ao serviço de tokens
        private readonly TokenService _tokens;

        // Injeta o TokenService pelo construtor
        public AuthController(TokenService tokens) => _tokens = tokens;



        /// <summary>
        /// Realiza login e retorna um JWT assinado (Bearer) em caso de sucesso.
        /// </summary>
        // Permite acesso sem autenticação a este endpoint específico
        [AllowAnonymous]
        // Indica que é um POST em /api/v1/auth/login
        [HttpPost("login")]
        // Define explicitamente os códigos de resposta esperados (opcional)
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginDto cred)
        {
            // Verifica se as credenciais são válidas (apenas DEMO)
            if (!_tokens.ValidateDemoCredentials(cred.Username, cred.Password))
                // Se não forem, retorna 401 Unauthorized
                return Unauthorized(new { error = "Credenciais inválidas." });

            // Gera o token JWT para o usuário (expiração padrão 1h)
            var jwt = _tokens.GenerateToken(cred.Username);

            // Devolve o token, o tipo e a validade (em segundos)
            return Ok(new { access_token = jwt, token_type = "Bearer", expires_in = 3600 });
        }

        // DTO mínimo para receber Username/Password no body do login
        public record LoginDto(string Username, string Password);
    }
}
