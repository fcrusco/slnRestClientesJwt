using RestClientesJwt.Models;

namespace RestClientesJwt.Services
{
    // Define um "banco" em memória para a DEMO (sem BD real)
    public class InMemoryStore
    {
        // Lista de clientes em memória (persistem enquanto a app estiver no ar)
        public List<Cliente> Clientes { get; } = new()
        {
            // Pré-carrega dois registros só para facilitar testes
            new Cliente { Id = 1, Nome = "Ana",   Sobrenome = "Silva" },
            new Cliente { Id = 2, Nome = "Bruno", Sobrenome = "Souza" }
        };

        // Próximo Id disponível (auto-incremento simples)
        private int _nextId = 3;

        // Cria um novo cliente a partir de nome/sobrenome
        public Cliente Add(string nome, string sobrenome)
        {
            // Monta o objeto cliente com novo Id
            var cli = new Cliente { Id = _nextId++, Nome = nome.Trim(), Sobrenome = sobrenome.Trim() };
            // Adiciona na lista
            Clientes.Add(cli);
            // Retorna o objeto criado
            return cli;
        }

        // Busca um cliente pelo Id
        public Cliente? Get(int id) => Clientes.FirstOrDefault(c => c.Id == id);

        // Atualiza por inteiro (PUT) um cliente existente
        public bool Update(int id, string nome, string sobrenome)
        {
            // Localiza o índice do cliente na lista
            var idx = Clientes.FindIndex(c => c.Id == id);
            // Se não encontrou, retorna falso
            if (idx < 0) return false;
            // Substitui o objeto no índice pela nova versão
            Clientes[idx] = new Cliente { Id = id, Nome = nome.Trim(), Sobrenome = sobrenome.Trim() };
            // Indica sucesso
            return true;
        }

        // Remove um cliente pelo Id
        public void Delete(int id)
        {
            // Remove todos com o Id informado (idempotente)
            Clientes.RemoveAll(c => c.Id == id);
        }
    }
}
