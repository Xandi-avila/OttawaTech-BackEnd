using Microsoft.Extensions.Options;
using OttawaTech.BackEnd.Configuracao;

namespace OttawaTech.BackEnd.Services;

public class RelogioArena(TimeProvider tempo, IOptions<ArenaOptions> opcoes)
{
    private readonly TimeZoneInfo _fuso = TimeZoneInfo.FindSystemTimeZoneById(opcoes.Value.FusoHorario);

    public DateTime AgoraUtc => tempo.GetUtcNow().UtcDateTime;

    public DateTime Agora => TimeZoneInfo.ConvertTimeFromUtc(AgoraUtc, _fuso);

    public DateOnly Hoje => DateOnly.FromDateTime(Agora);

    public DateTime ParaUtc(DateOnly data, TimeOnly hora) =>
        TimeZoneInfo.ConvertTimeToUtc(data.ToDateTime(hora), _fuso);
}
