using OttawaTech.BackEnd.Enums;

namespace OttawaTech.BackEnd.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateOnly? DataNascimento { get; set; }
    public string SenhaHash { get; set; } = string.Empty;
    public StatusCliente Status { get; set; } = StatusCliente.Ativo;
    public ICollection<Reserva> Reservas { get; set; } = [];
}
