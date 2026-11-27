using OttawaTech.BackEnd.Enums;

namespace OttawaTech.BackEnd.Models;

public class Pagamento
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    public decimal Valor { get; set; }
    public MetodoPagamento Metodo { get; set; }
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;
    public DateTime? DataPagamento { get; set; }
    public Reserva Reserva { get; set; } = null!;
}
