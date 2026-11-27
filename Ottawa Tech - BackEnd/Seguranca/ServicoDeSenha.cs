using Microsoft.AspNetCore.Identity;

namespace OttawaTech.BackEnd.Seguranca;

public class ServicoDeSenha
{
    private static readonly object Usuario = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string GerarHash(string senha) => _hasher.HashPassword(Usuario, senha);

    public bool Conferir(string hash, string senha) =>
        _hasher.VerifyHashedPassword(Usuario, hash, senha) != PasswordVerificationResult.Failed;
}
