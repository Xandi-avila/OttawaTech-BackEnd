using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Models;

namespace OttawaTech.BackEnd.DTOs.Pagamentos;

public sealed record RegistrarPagamentoRequest(int ReservaId, MetodoPagamento Metodo);

public sealed record PagamentoFiltro(StatusPagamento? Status, int? ClienteId, int? ReservaId);

public sealed record PagamentoResponse(
    int Id,
    int ReservaId,
    decimal Valor,
    MetodoPagamento Metodo,
    StatusPagamento Status,
    DateTime? DataPagamento)
{
    public static PagamentoResponse De(Pagamento pagamento) => new(
        pagamento.Id,
        pagamento.ReservaId,
        pagamento.Valor,
        pagamento.Metodo,
        pagamento.Status,
        pagamento.DataPagamento);
}
