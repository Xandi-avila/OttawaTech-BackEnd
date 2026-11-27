namespace OttawaTech.BackEnd.Erros;

public sealed class ApiException(int status, string titulo, string mensagem, string? campo = null)
    : Exception(mensagem)
{
    public const string MensagemAcessoNegado = "Você não tem permissão para realizar esta operação.";

    public int Status { get; } = status;
    public string Titulo { get; } = titulo;
    public string? Campo { get; } = campo;

    public static ApiException Validacao(string mensagem, string? campo = null) =>
        new(StatusCodes.Status400BadRequest, "Dados inválidos", mensagem, campo);

    public static ApiException NaoAutenticado(string mensagem) =>
        new(StatusCodes.Status401Unauthorized, "Não autenticado", mensagem);

    public static ApiException AcessoNegado(string mensagem = MensagemAcessoNegado) =>
        new(StatusCodes.Status403Forbidden, "Acesso negado", mensagem);

    public static ApiException NaoEncontrado(string mensagem) =>
        new(StatusCodes.Status404NotFound, "Não encontrado", mensagem);

    public static ApiException Conflito(string mensagem, string? campo = null) =>
        new(StatusCodes.Status409Conflict, "Conflito", mensagem, campo);
}
