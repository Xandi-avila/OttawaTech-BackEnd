using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OttawaTech.BackEnd.Configuracao;

public class JwtOptions
{
    public const string Secao = "Jwt";
    public const int TamanhoMinimoChave = 32;

    public string Chave { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ValidadeHoras { get; set; } = 8;

    public SymmetricSecurityKey ChaveDeAssinatura => new(Encoding.UTF8.GetBytes(Chave));
}
