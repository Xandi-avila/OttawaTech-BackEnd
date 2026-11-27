using Microsoft.EntityFrameworkCore;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.DTOs.Quadras;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Erros;
using OttawaTech.BackEnd.Models;
using OttawaTech.BackEnd.Regras;

namespace OttawaTech.BackEnd.Services;

public class QuadraService(AppDbContext db)
{
    public async Task<IReadOnlyList<QuadraResponse>> ListarAsync()
    {
        var quadras = await db.Quadras.AsNoTracking().OrderBy(q => q.Id).ToListAsync();
        return quadras.Select(QuadraResponse.De).ToList();
    }

    public async Task<QuadraResponse> ObterAsync(int id) => QuadraResponse.De(await BuscarAsync(id));

    public async Task<QuadraResponse> CriarAsync(QuadraRequest request)
    {
        var quadra = new Quadra();
        PreencherDados(quadra, request);

        db.Quadras.Add(quadra);
        await db.SaveChangesAsync();
        return QuadraResponse.De(quadra);
    }

    public async Task<QuadraResponse> AtualizarAsync(int id, QuadraRequest request)
    {
        var quadra = await BuscarAsync(id);
        PreencherDados(quadra, request);

        await db.SaveChangesAsync();
        return QuadraResponse.De(quadra);
    }

    public async Task InativarAsync(int id)
    {
        var quadra = await BuscarAsync(id);
        quadra.Status = StatusQuadra.Inativa;
        await db.SaveChangesAsync();
    }

    public async Task<DisponibilidadeResponse> ConsultarDisponibilidadeAsync(int id, DateOnly? data)
    {
        if (data is not { } dia)
            throw ApiException.Validacao("Data inválida.", "data");

        if (!await db.Quadras.AnyAsync(q => q.Id == id))
            throw ApiException.NaoEncontrado("Quadra não encontrada.");

        var ocupados = await db.Reservas.AsNoTracking()
            .Where(r => r.QuadraId == id && r.Data == dia && r.Status != StatusReserva.Cancelada)
            .OrderBy(r => r.HoraInicio)
            .Select(r => new IntervaloResponse(r.HoraInicio, r.HoraFim))
            .ToListAsync();

        return new DisponibilidadeResponse(id, dia, RegrasReserva.Abertura, RegrasReserva.Fechamento, ocupados);
    }

    private async Task<Quadra> BuscarAsync(int id) =>
        await db.Quadras.FindAsync(id) ?? throw ApiException.NaoEncontrado("Quadra não encontrada.");

    private static void PreencherDados(Quadra quadra, QuadraRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw ApiException.Validacao("Informe a identificação da quadra.", "nome");

        if (string.IsNullOrWhiteSpace(request.Tipo))
            throw ApiException.Validacao("Informe o tipo da quadra.", "tipo");

        if (request.ValorHora <= 0)
            throw ApiException.Validacao("O valor por hora deve ser maior que zero.", "valorHora");

        if (!Enum.IsDefined(request.Status))
            throw ApiException.Validacao("Status da quadra inválido.", "status");

        quadra.Nome = request.Nome.Trim();
        quadra.Tipo = request.Tipo.Trim();
        quadra.ValorHora = Math.Round(request.ValorHora, 2, MidpointRounding.AwayFromZero);
        quadra.Status = request.Status;
    }
}
