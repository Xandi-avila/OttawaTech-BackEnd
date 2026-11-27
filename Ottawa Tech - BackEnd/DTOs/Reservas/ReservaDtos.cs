using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Models;

namespace OttawaTech.BackEnd.DTOs.Reservas;

public sealed record CriarReservaRequest(int ClienteId, int QuadraId, DateOnly Data, TimeOnly HoraInicio, TimeOnly HoraFim);

public sealed record AlterarReservaRequest(int QuadraId, DateOnly Data, TimeOnly HoraInicio, TimeOnly HoraFim);

public sealed record ReservaFiltro(int? ClienteId, int? QuadraId, DateOnly? Data, StatusReserva? Status);

public sealed record ReservaResponse(
    int Id,
    int ClienteId,
    int QuadraId,
    DateOnly Data,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    decimal Valor,
    StatusReserva Status,
    DateTime DataCriacao)
{
    public static ReservaResponse De(Reserva reserva) => new(
        reserva.Id,
        reserva.ClienteId,
        reserva.QuadraId,
        reserva.Data,
        reserva.HoraInicio,
        reserva.HoraFim,
        reserva.Valor,
        reserva.Status,
        reserva.DataCriacao);
}
