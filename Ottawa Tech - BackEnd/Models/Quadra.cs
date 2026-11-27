using OttawaTech.BackEnd.Enums;

namespace OttawaTech.BackEnd.Models;

public class Quadra
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal ValorHora { get; set; }
    public StatusQuadra Status { get; set; } = StatusQuadra.Ativa;
    public ICollection<Reserva> Reservas { get; set; } = [];
}
