using Ottawa_Tech___BackEnd.Enums;

namespace Ottawa_Tech___BackEnd.DTOs;

public class CriarReservaDto
{
    public int ClienteId { get; set; }
    public int QuadraId { get; set; }
    public decimal Valor { get; set; }
    public StatusReservaEnum Status { get; set; }
}