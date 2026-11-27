using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OttawaTech.BackEnd.Configuracao;
using OttawaTech.BackEnd.DTOs.Auth;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Seguranca;

public class GeradorDeToken(IOptions<JwtOptions> opcoes, RelogioArena relogio)
{
    private readonly JsonWebTokenHandler _handler = new();

    public AutenticacaoResponse Gerar(UsuarioResponse usuario)
    {
        var jwt = opcoes.Value;
        var agora = relogio.AgoraUtc;
        var expiraEm = agora.AddHours(jwt.ValidadeHoras);

        var claims = new Dictionary<string, object>
        {
            [Claims.Id] = usuario.Id.ToString(),
            [Claims.Nome] = usuario.Nome,
            [Claims.Email] = usuario.Email,
            [Claims.Perfil] = usuario.Perfil
        };
        if (usuario.ClienteId is int clienteId)
            claims[Claims.ClienteId] = clienteId;

        var token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = jwt.Emissor,
            Audience = jwt.Audiencia,
            Claims = claims,
            IssuedAt = agora,
            NotBefore = agora,
            Expires = expiraEm,
            SigningCredentials = new SigningCredentials(jwt.ChaveDeAssinatura, SecurityAlgorithms.HmacSha256)
        });

        return new AutenticacaoResponse(token, expiraEm, usuario);
    }
}
