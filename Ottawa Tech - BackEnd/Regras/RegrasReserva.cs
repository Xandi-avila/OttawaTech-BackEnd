using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Models;

namespace OttawaTech.BackEnd.Regras;

public static class RegrasReserva
{
    public static readonly TimeOnly Abertura = new(7, 0);
    public static readonly TimeOnly Fechamento = new(22, 0);
    public static readonly int[] DuracoesMinutos = [60, 90, 120];
    public const int JanelaDias = 30;
    public const int AntecedenciaCancelamentoHoras = 4;

    public static decimal CalcularValor(decimal valorHora, int duracaoMinutos) =>
        Math.Round(valorHora * duracaoMinutos / 60m, 2, MidpointRounding.AwayFromZero);

    public static int DuracaoEmMinutos(TimeOnly inicio, TimeOnly fim) =>
        (int)(fim.ToTimeSpan() - inicio.ToTimeSpan()).TotalMinutes;

    public static bool DentroDaJanela(DateOnly data, DateOnly hoje)
    {
        var dias = data.DayNumber - hoje.DayNumber;
        return dias is >= 0 and < JanelaDias;
    }

    public static bool DentroDoFuncionamento(TimeOnly inicio, TimeOnly fim) =>
        inicio >= Abertura && fim <= Fechamento && fim > inicio;

    public static StatusReserva StatusEfetivo(Reserva reserva, DateTime agora) =>
        reserva.Status == StatusReserva.Confirmada && reserva.Fim <= agora
            ? StatusReserva.Concluida
            : reserva.Status;

    public static string? MotivoParaNaoCancelar(Reserva reserva, DateTime agora, bool administrador)
    {
        if (reserva.Status == StatusReserva.Cancelada)
            return "A reserva já está cancelada.";

        if (reserva.Status == StatusReserva.Concluida || reserva.Fim <= agora)
            return "Reservas concluídas não podem ser canceladas.";

        if (!administrador && agora > reserva.Inicio.AddHours(-AntecedenciaCancelamentoHoras))
            return $"RN08 — o cancelamento é permitido até {AntecedenciaCancelamentoHoras} horas antes do horário da reserva.";

        return null;
    }
}
