using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.DTOs.Auth;
using OttawaTech.BackEnd.DTOs.Clientes;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Erros;
using OttawaTech.BackEnd.Models;
using OttawaTech.BackEnd.Regras;
using OttawaTech.BackEnd.Seguranca;

namespace OttawaTech.BackEnd.Services;

public class ClienteService(AppDbContext db, ServicoDeSenha senhas, RelogioArena relogio)
{
    private const int TamanhoMinimoSenha = 6;
    private const string SemAcesso = "Você só pode acessar o seu próprio cadastro.";
    private const string CpfEmUso = "RN01 — já existe um cliente ativo com este CPF.";
    private const string EmailEmUso = "RN02 — este e-mail já está cadastrado.";
    private static readonly DateOnly NascimentoMinimo = new(1900, 1, 1);

    public async Task<IReadOnlyList<ClienteResponse>> ListarAsync(ClienteFiltro filtro)
    {
        var consulta = db.Clientes.AsNoTracking();
        if (filtro.Status is { } status)
            consulta = consulta.Where(c => c.Status == status);

        var clientes = await consulta.OrderBy(c => c.Id).ToListAsync();
        return clientes
            .Where(c => CorrespondeABusca(c, filtro.Busca))
            .Select(ClienteResponse.De)
            .ToList();
    }

    public async Task<ClienteResponse> ObterAsync(int id, Solicitante solicitante)
    {
        solicitante.ExigirAcessoAoCliente(id, SemAcesso);
        return ClienteResponse.De(await BuscarAsync(id));
    }

    public async Task<ClienteResponse> CriarAsync(ClienteRequest request)
    {
        ValidarStatus(request.Status);
        return ClienteResponse.De(await CadastrarAsync(request, request.Status));
    }

    public async Task<Cliente> CadastrarAsync(RegistroRequest request, StatusCliente status)
    {
        ValidarDados(request, exigirSenha: true);

        var cliente = new Cliente { Status = status, SenhaHash = senhas.GerarHash(request.Senha!) };
        PreencherDados(cliente, request);
        await GarantirCadastroUnicoAsync(cliente);

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        return cliente;
    }

    public async Task<ClienteResponse> AtualizarAsync(int id, ClienteRequest request, Solicitante solicitante)
    {
        solicitante.ExigirAcessoAoCliente(id, SemAcesso);
        var cliente = await BuscarAsync(id);

        if (!solicitante.EhAdministrador && request.Status != cliente.Status)
            throw ApiException.AcessoNegado("Somente a administração pode alterar o status do cadastro.");

        ValidarDados(request, exigirSenha: false);
        ValidarStatus(request.Status);

        PreencherDados(cliente, request);
        cliente.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Senha))
            cliente.SenhaHash = senhas.GerarHash(request.Senha);

        await GarantirCadastroUnicoAsync(cliente);
        await db.SaveChangesAsync();
        return ClienteResponse.De(cliente);
    }

    public async Task InativarAsync(int id)
    {
        var cliente = await BuscarAsync(id);
        cliente.Status = StatusCliente.Inativo;
        await db.SaveChangesAsync();
    }

    private async Task<Cliente> BuscarAsync(int id) =>
        await db.Clientes.FindAsync(id) ?? throw ApiException.NaoEncontrado("Cliente não encontrado.");

    private async Task GarantirCadastroUnicoAsync(Cliente cliente)
    {
        var cpfEmUso = cliente.Status == StatusCliente.Ativo && await db.Clientes.AnyAsync(c =>
            c.Id != cliente.Id && c.Status == StatusCliente.Ativo && c.Cpf == cliente.Cpf);
        if (cpfEmUso)
            throw ApiException.Conflito(CpfEmUso, "cpf");

        var emailEmUso = await db.Administradores.AnyAsync(a => a.Email == cliente.Email)
            || await db.Clientes.AnyAsync(c => c.Id != cliente.Id && c.Email == cliente.Email);
        if (emailEmUso)
            throw ApiException.Conflito(EmailEmUso, "email");
    }

    private void ValidarDados(RegistroRequest dados, bool exigirSenha)
    {
        if ((dados.Nome?.Trim().Length ?? 0) < 3)
            throw ApiException.Validacao("Informe o nome completo.", "nome");

        if (!DadosCadastrais.CpfValido(dados.Cpf))
            throw ApiException.Validacao("CPF inválido.", "cpf");

        if (!DadosCadastrais.EmailValido(dados.Email))
            throw ApiException.Validacao("Formato de e-mail inválido.", "email");

        if (!DadosCadastrais.TelefoneValido(dados.Telefone))
            throw ApiException.Validacao("Telefone inválido — informe DDD + número.", "telefone");

        if (dados.DataNascimento is { } nascimento && (nascimento > relogio.Hoje || nascimento < NascimentoMinimo))
            throw ApiException.Validacao("Data de nascimento inválida.", "dataNascimento");

        var senha = dados.Senha ?? string.Empty;
        if ((exigirSenha || senha.Length > 0) && senha.Length < TamanhoMinimoSenha)
            throw ApiException.Validacao($"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres.", "senha");
    }

    private static void ValidarStatus(StatusCliente status)
    {
        if (!Enum.IsDefined(status))
            throw ApiException.Validacao("Status do cliente inválido.", "status");
    }

    private static void PreencherDados(Cliente cliente, RegistroRequest dados)
    {
        cliente.Nome = dados.Nome!.Trim();
        cliente.Cpf = DadosCadastrais.FormatarCpf(dados.Cpf!);
        cliente.Email = DadosCadastrais.NormalizarEmail(dados.Email);
        cliente.Telefone = DadosCadastrais.FormatarTelefone(dados.Telefone!);
        cliente.DataNascimento = dados.DataNascimento;
    }

    private static bool CorrespondeABusca(Cliente cliente, string? busca)
    {
        var termo = SemAcentos(busca);
        if (termo.Length == 0)
            return true;

        if (SemAcentos(cliente.Nome).Contains(termo) || cliente.Email.Contains(termo))
            return true;

        var digitos = DadosCadastrais.SomenteDigitos(busca);
        return digitos.Length > 0 && DadosCadastrais.SomenteDigitos(cliente.Cpf).Contains(digitos);
    }

    private static string SemAcentos(string? texto)
    {
        var semMarcas = (texto ?? string.Empty)
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        return new string(semMarcas.ToArray()).Trim().ToLowerInvariant();
    }
}
