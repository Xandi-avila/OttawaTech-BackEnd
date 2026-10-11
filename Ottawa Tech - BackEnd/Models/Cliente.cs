using Ottawa_Tech___BackEnd.Enums;

namespace Ottawa_Tech___BackEnd.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string? Cpf { get; set; }
        public int? Telefone { get; set; }
        public string? Email { get; set; }
        public DateOnly? DataNascimento { get; set; }
        public string Senha { get; set; }
        public StatusCliente Situacao { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = [];
    }
}