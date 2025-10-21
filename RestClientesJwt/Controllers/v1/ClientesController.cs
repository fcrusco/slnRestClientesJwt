// Importa atributos de autorização (para proteger os endpoints)
using Microsoft.AspNetCore.Authorization;
// Importa tipos MVC e resultados de ação
using Microsoft.AspNetCore.Mvc;
// Importa o modelo Cliente
using RestClientesJwt.Models;
// Importa nosso "banco" em memória
using RestClientesJwt.Services;

namespace RestClientesJwt.Controllers.v1
{
    // Exige autenticação Bearer JWT em TODAS as ações deste controller
    [Authorize]
    // Marca como controlador de API
    [ApiController]
    // Define a rota base: /api/v1/clientes
    [Route("api/v1/[controller]")]
    public class ClientesController : ControllerBase
    {
        // Mantém referência ao "banco" em memória
        private readonly InMemoryStore _store;

        // Injeta o InMemoryStore via construtor
        public ClientesController(InMemoryStore store) => _store = store;


        // Mapeia GET /api/v1/clientes para listar todos os clientes
        [HttpGet]
        // Indica que retorna 200 OK com uma lista de Cliente (opcional para documentação)
        [ProducesResponseType(typeof(IEnumerable<Cliente>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            // Retorna 200 OK com o conteúdo da lista em memória
            return Ok(_store.Clientes);
        }


        // Mapeia GET /api/v1/clientes/{id} para obter um cliente específico
        [HttpGet("{id:int}")]
        // Documenta 200 e 404 na saída (opcional)
        [ProducesResponseType(typeof(Cliente), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            // Busca o cliente no "banco" em memória
            var c = _store.Get(id);

            // Se não existir, retorna 404 Not Found
            if (c is null) return NotFound();

            // Se existir, retorna 200 OK com o cliente
            return Ok(c);
        }


        // Mapeia POST /api/v1/clientes para criar um novo cliente
        [HttpPost]
        // Documenta 201 e 400 na saída (opcional)
        [ProducesResponseType(typeof(Cliente), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] Cliente body)
        {
            // Validações mínimas: Nome e Sobrenome obrigatórios
            if (string.IsNullOrWhiteSpace(body.Nome) || string.IsNullOrWhiteSpace(body.Sobrenome))
                // 400 BadRequest quando dados são inválidos
                return BadRequest(new { error = "Nome e Sobrenome são obrigatórios." });

            // Cria o cliente (Id gerado pelo servidor) ignorando qualquer Id enviado
            var novo = _store.Add(body.Nome, body.Sobrenome);

            // Retorna 201 Created + Location do novo recurso
            return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
        }


        // Mapeia PUT /api/v1/clientes/{id} para substituir totalmente os dados do cliente
        [HttpPut("{id:int}")]
        // Documenta 204 e 404 (opcional)
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, [FromBody] Cliente body)
        {
            // Tenta atualizar (substituir) o recurso; retorna bool
            var ok = _store.Update(id, body.Nome, body.Sobrenome);

            // Se não encontrou, 404 Not Found
            if (!ok) return NotFound();

            // Se deu certo, 204 No Content (sem corpo)
            return NoContent();
        }


        // Mapeia DELETE /api/v1/clientes/{id} para remover o cliente
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var exists = _store.Get(id) is not null;

            if (!exists) return NotFound();   // 404 se não existir

            _store.Delete(id);

            return NoContent();               // 204 se removeu

            //Após um DELETE bem-sucedido, não há representação do recurso para enviar.
            //'204 No Content' expressa isso melhor que 200, que normalmente retorna body.
        }
    }
}
