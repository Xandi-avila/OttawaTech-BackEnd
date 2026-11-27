using System.Text.RegularExpressions;

namespace OttawaTech.BackEnd.Regras;

public static partial class DadosCadastrais
{
    public static string SomenteDigitos(string? valor) =>
        new((valor ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

    public static string NormalizarEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public static bool EmailValido(string? email) => EmailRegex().IsMatch((email ?? string.Empty).Trim());

    public static bool TelefoneValido(string? telefone) => SomenteDigitos(telefone).Length is 10 or 11;

    public static bool CpfValido(string? cpf)
    {
        var digitos = SomenteDigitos(cpf);
        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
            return false;

        return DigitoVerificador(digitos[..9]) == digitos[9] - '0'
            && DigitoVerificador(digitos[..10]) == digitos[10] - '0';
    }

    public static string FormatarCpf(string cpf)
    {
        var d = SomenteDigitos(cpf);
        return $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}";
    }

    public static string FormatarTelefone(string telefone)
    {
        var d = SomenteDigitos(telefone);
        var corte = d.Length == 11 ? 7 : 6;
        return $"({d[..2]}) {d[2..corte]}-{d[corte..]}";
    }

    private static int DigitoVerificador(string digitos)
    {
        var pesoInicial = digitos.Length + 1;
        var soma = digitos.Select((digito, indice) => (digito - '0') * (pesoInicial - indice)).Sum();
        var resto = soma * 10 % 11;
        return resto == 10 ? 0 : resto;
    }

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$")]
    private static partial Regex EmailRegex();
}
