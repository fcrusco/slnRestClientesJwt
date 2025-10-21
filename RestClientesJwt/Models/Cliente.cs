namespace RestClientesJwt.Models
{
    // Declara a classe que representa o recurso Cliente
    public class Cliente
    {
        // Identificador interno (chave) gerado pelo servidor
        public int Id { get; set; }

        // Nome do cliente
        public string Nome { get; set; }

        // Sobrenome do cliente
        public string Sobrenome { get; set; }
    }
}
