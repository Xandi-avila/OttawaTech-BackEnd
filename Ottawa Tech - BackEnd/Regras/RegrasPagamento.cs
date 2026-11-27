using OttawaTech.BackEnd.Enums;

namespace OttawaTech.BackEnd.Regras;

public static class RegrasPagamento
{
    public static StatusPagamento StatusInicial(MetodoPagamento metodo) =>
        metodo == MetodoPagamento.Dinheiro ? StatusPagamento.Pendente : StatusPagamento.Pago;

    public static StatusPagamento StatusAoCancelarReserva(StatusPagamento status) => status switch
    {
        StatusPagamento.Pago => StatusPagamento.Estornado,
        StatusPagamento.Pendente => StatusPagamento.Cancelado,
        _ => status
    };

    public static StatusReserva StatusDaReservaAposPagamento(StatusReserva status) =>
        status == StatusReserva.Pendente ? StatusReserva.Confirmada : status;
}
