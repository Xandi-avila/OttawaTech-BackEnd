using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Models;

namespace OttawaTech.BackEnd.DTOs.Quadras;

public sealed record QuadraRequest(string? Nome, string? Tipo, decimal ValorHora, StatusQuadra Status);

public sealed record QuadraResponse(int Id, string Nome, string Tipo, decimal ValorHora, StatusQuadra Status)
{
    public static QuadraResponse De(Quadra quadra) =>
        new(quadra.Id, quadra.Nome, quadra.Tipo, quadra.ValorHora, quadra.Status);
}

public sealed record IntervaloResponse(TimeOnly Inicio, TimeOnly Fim);

public sealed record DisponibilidadeResponse(
    int QuadraId,
    DateOnly Data,
    TimeOnly Abertura,
    TimeOnly Fechamento,
    IReadOnlyList<IntervaloResponse> Ocupados);
