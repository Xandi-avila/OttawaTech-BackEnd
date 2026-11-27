namespace OttawaTech.BackEnd.DTOs.Auth;

public sealed record LoginRequest(string? Email, string? Senha);

public record RegistroRequest
{
    public string? Nome { get; init; }
    public string? Cpf { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
    public DateOnly? DataNascimento { get; init; }
    public string? Senha { get; init; }
}

public sealed record UsuarioResponse(int Id, string Nome, string Email, string Perfil, int? ClienteId);

public sealed record AutenticacaoResponse(string Token, DateTime ExpiraEm, UsuarioResponse Usuario);
