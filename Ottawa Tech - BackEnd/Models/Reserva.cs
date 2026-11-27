using OttawaTech.BackEnd.Enums;

namespace OttawaTech.BackEnd.Models;

public class Reserva
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int QuadraId { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public decimal Valor { get; set; }
    public StatusReserva Status { get; set; } = StatusReserva.Pendente;
    public DateTime DataCriacao { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Quadra Quadra { get; set; } = null!;
    public ICollection<Pagamento> Pagamentos { get; set; } = [];

    public DateTime Inicio => Data.ToDateTime(HoraInicio);
    public DateTime Fim => Data.ToDateTime(HoraFim);

    public Pagamento? PagamentoAtivo =>
        Pagamentos.FirstOrDefault(p => p.Status is StatusPagamento.Pendente or StatusPagamento.Pago);
}
