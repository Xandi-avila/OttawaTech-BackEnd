using Ottawa_Tech___BackEnd.Enums;

namespace Ottawa_Tech___BackEnd.DTOs;

public class ReservaRespostaDto
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Quadra { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public decimal Valor { get; set; }
    public StatusReservaEnum Status { get; set; }
    public DateTime DataCriacao { get; set; }
}