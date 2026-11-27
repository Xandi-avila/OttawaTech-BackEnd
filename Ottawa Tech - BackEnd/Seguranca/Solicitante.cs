using System.Security.Claims;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Erros;

namespace OttawaTech.BackEnd.Seguranca;

public sealed record Solicitante(int UsuarioId, string Perfil, int? ClienteId)
{
    public bool EhAdministrador => Perfil == Perfis.Administrador;

    public void ExigirAcessoAoCliente(int clienteId, string mensagem)
    {
        if (!EhAdministrador && ClienteId != clienteId)
            throw ApiException.AcessoNegado(mensagem);
    }

    public static Solicitante De(ClaimsPrincipal usuario) => new(
        int.Parse(usuario.FindFirstValue(Claims.Id)!),
        usuario.FindFirstValue(Claims.Perfil)!,
        int.TryParse(usuario.FindFirstValue(Claims.ClienteId), out var clienteId) ? clienteId : null);
}
