using Microsoft.IdentityModel.JsonWebTokens;

namespace OttawaTech.BackEnd.Seguranca;

public static class Claims
{
    public const string Id = JwtRegisteredClaimNames.Sub;
    public const string Nome = JwtRegisteredClaimNames.Name;
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string Perfil = "role";
    public const string ClienteId = "clienteId";
}
