using Microsoft.EntityFrameworkCore;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.DTOs.Auth;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Erros;
using OttawaTech.BackEnd.Models;
using OttawaTech.BackEnd.Regras;
using OttawaTech.BackEnd.Seguranca;

namespace OttawaTech.BackEnd.Services;

public class AuthService(AppDbContext db, ClienteService clientes, ServicoDeSenha senhas, GeradorDeToken tokens)
{
    private const string CredenciaisInvalidas = "E-mail ou senha inválidos.";
    private const string CadastroInativo = "Cadastro inativo. Procure a administração da arena.";

    public async Task<AutenticacaoResponse> LoginAsync(LoginRequest request)
    {
        var email = DadosCadastrais.NormalizarEmail(request.Email);
        var senha = request.Senha ?? string.Empty;

        var administrador = await db.Administradores.AsNoTracking().FirstOrDefaultAsync(a => a.Email == email);
        if (administrador is not null)
        {
            if (!senhas.Conferir(administrador.SenhaHash, senha))
                throw ApiException.NaoAutenticado(CredenciaisInvalidas);

            return tokens.Gerar(new UsuarioResponse(
                administrador.Id, administrador.Nome, administrador.Email, Perfis.Administrador, null));
        }

        var cliente = await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
        if (cliente is null || !senhas.Conferir(cliente.SenhaHash, senha))
            throw ApiException.NaoAutenticado(CredenciaisInvalidas);

        if (cliente.Status != StatusCliente.Ativo)
            throw ApiException.AcessoNegado(CadastroInativo);

        return tokens.Gerar(UsuarioDoCliente(cliente));
    }

    public async Task<AutenticacaoResponse> RegistrarAsync(RegistroRequest request)
    {
        var cliente = await clientes.CadastrarAsync(request, StatusCliente.Ativo);
        return tokens.Gerar(UsuarioDoCliente(cliente));
    }

    private static UsuarioResponse UsuarioDoCliente(Cliente cliente) =>
        new(cliente.Id, cliente.Nome, cliente.Email, Perfis.Cliente, cliente.Id);
}
