using Microsoft.EntityFrameworkCore;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.DTOs.Pagamentos;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Erros;
using OttawaTech.BackEnd.Models;
using OttawaTech.BackEnd.Regras;
using OttawaTech.BackEnd.Seguranca;

namespace OttawaTech.BackEnd.Services;

public class PagamentoService(AppDbContext db, RelogioArena relogio)
{
    private const string SemAcesso = "Você só pode acessar os pagamentos das suas reservas.";

    public async Task<IReadOnlyList<PagamentoResponse>> ListarAsync(PagamentoFiltro filtro, Solicitante solicitante)
    {
        var clienteId = filtro.ClienteId;
        if (!solicitante.EhAdministrador)
        {
            if (clienteId is not null && clienteId != solicitante.ClienteId)
                throw ApiException.AcessoNegado(SemAcesso);
            clienteId = solicitante.ClienteId;
        }

        var consulta = db.Pagamentos.AsNoTracking();
        if (clienteId is not null)
            consulta = consulta.Where(p => p.Reserva.ClienteId == clienteId);
        if (filtro.ReservaId is not null)
            consulta = consulta.Where(p => p.ReservaId == filtro.ReservaId);
        if (filtro.Status is not null)
            consulta = consulta.Where(p => p.Status == filtro.Status);

        var pagamentos = await consulta.OrderBy(p => p.Id).ToListAsync();
        return pagamentos.Select(PagamentoResponse.De).ToList();
    }

    public async Task<PagamentoResponse> ObterAsync(int id, Solicitante solicitante)
    {
        var pagamento = await db.Pagamentos.AsNoTracking().Include(p => p.Reserva).FirstOrDefaultAsync(p => p.Id == id)
            ?? throw ApiException.NaoEncontrado("Pagamento não encontrado.");
        solicitante.ExigirAcessoAoCliente(pagamento.Reserva.ClienteId, SemAcesso);
        return PagamentoResponse.De(pagamento);
    }

    public async Task<PagamentoResponse> RegistrarAsync(RegistrarPagamentoRequest request, Solicitante solicitante)
    {
        if (!Enum.IsDefined(request.Metodo))
            throw ApiException.Validacao("Selecione uma forma de pagamento válida.", "metodo");

        var reserva = await db.Reservas.Include(r => r.Pagamentos).FirstOrDefaultAsync(r => r.Id == request.ReservaId)
            ?? throw ApiException.NaoEncontrado("Reserva não encontrada.");
        solicitante.ExigirAcessoAoCliente(reserva.ClienteId, SemAcesso);

        if (reserva.Status == StatusReserva.Cancelada)
            throw ApiException.Validacao("Não é possível registrar pagamento de uma reserva cancelada.");

        if (reserva.PagamentoAtivo is not null)
            throw ApiException.Conflito("RN10 — esta reserva já possui um pagamento registrado.");

        var status = RegrasPagamento.StatusInicial(request.Metodo);
        var pagamento = new Pagamento
        {
            Valor = reserva.Valor,
            Metodo = request.Metodo,
            Status = status,
            DataPagamento = status == StatusPagamento.Pago ? relogio.AgoraUtc : null
        };
        reserva.Pagamentos.Add(pagamento);
        reserva.Status = RegrasPagamento.StatusDaReservaAposPagamento(reserva.Status);

        await db.SaveChangesAsync();
        return PagamentoResponse.De(pagamento);
    }

    public async Task<PagamentoResponse> ConfirmarAsync(int id)
    {
        var pagamento = await db.Pagamentos.Include(p => p.Reserva).FirstOrDefaultAsync(p => p.Id == id)
            ?? throw ApiException.NaoEncontrado("Pagamento não encontrado.");

        if (pagamento.Status != StatusPagamento.Pendente)
            throw ApiException.Conflito("Somente pagamentos pendentes podem ser confirmados.");

        pagamento.Status = StatusPagamento.Pago;
        pagamento.DataPagamento = relogio.AgoraUtc;
        pagamento.Reserva.Status = RegrasPagamento.StatusDaReservaAposPagamento(pagamento.Reserva.Status);

        await db.SaveChangesAsync();
        return PagamentoResponse.De(pagamento);
    }
}
