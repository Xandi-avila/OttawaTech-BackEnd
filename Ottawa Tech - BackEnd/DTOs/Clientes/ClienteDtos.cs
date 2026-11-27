using OttawaTech.BackEnd.DTOs.Auth;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Models;

namespace OttawaTech.BackEnd.DTOs.Clientes;

public sealed record ClienteRequest : RegistroRequest
{
    public StatusCliente Status { get; init; }
}

public sealed record ClienteFiltro(string? Busca, StatusCliente? Status);

public sealed record ClienteResponse(
    int Id,
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    DateOnly? DataNascimento,
    StatusCliente Status)
{
    public static ClienteResponse De(Cliente cliente) => new(
        cliente.Id,
        cliente.Nome,
        cliente.Cpf,
        cliente.Email,
        cliente.Telefone,
        cliente.DataNascimento,
        cliente.Status);
}
