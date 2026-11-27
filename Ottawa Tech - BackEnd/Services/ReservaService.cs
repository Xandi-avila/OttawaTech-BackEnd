using Microsoft.EntityFrameworkCore;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.DTOs.Reservas;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Erros;
using OttawaTech.BackEnd.Models;
using OttawaTech.BackEnd.Regras;
using OttawaTech.BackEnd.Seguranca;

namespace OttawaTech.BackEnd.Services;

public class ReservaService(AppDbContext db, RelogioArena relogio)
{
    private const string SemAcesso = "Você só pode acessar as suas próprias reservas.";

    public async Task<IReadOnlyList<ReservaResponse>> ListarAsync(ReservaFiltro filtro, Solicitante solicitante)
    {
        var clienteId = filtro.ClienteId;
        if (!solicitante.EhAdministrador)
        {
            if (clienteId is not null && clienteId != solicitante.ClienteId)
                throw ApiException.AcessoNegado(SemAcesso);
            clienteId = solicitante.ClienteId;
        }

        await ConcluirReservasEncerradasAsync();

        var consulta = db.Reservas.AsNoTracking();
        if (clienteId is not null)
            consulta = consulta.Where(r => r.ClienteId == clienteId);
        if (filtro.QuadraId is not null)
            consulta = consulta.Where(r => r.QuadraId == filtro.QuadraId);
        if (filtro.Data is not null)
            consulta = consulta.Where(r => r.Data == filtro.Data);
        if (filtro.Status is not null)
            consulta = consulta.Where(r => r.Status == filtro.Status);

        var reservas = await consulta.OrderBy(r => r.Id).ToListAsync();
        return reservas.Select(ReservaResponse.De).ToList();
    }

    public async Task<ReservaResponse> ObterAsync(int id, Solicitante solicitante)
    {
        await ConcluirReservasEncerradasAsync();
        var reserva = await BuscarAsync(id, solicitante);
        return ReservaResponse.De(reserva);
    }

    public async Task<ReservaResponse> CriarAsync(CriarReservaRequest request, Solicitante solicitante)
    {
        solicitante.ExigirAcessoAoCliente(request.ClienteId, "Você só pode fazer reservas para o seu próprio cadastro.");

        var valor = await ValidarAgendamentoAsync(request.ClienteId, request.QuadraId, request.Data,
            request.HoraInicio, request.HoraFim, ignorarReservaId: null);

        var reserva = new Reserva
        {
            ClienteId = request.ClienteId,
            QuadraId = request.QuadraId,
            Data = request.Data,
            HoraInicio = request.HoraInicio,
            HoraFim = request.HoraFim,
            Valor = valor,
            Status = StatusReserva.Pendente,
            DataCriacao = relogio.AgoraUtc
        };

        db.Reservas.Add(reserva);
        await db.SaveChangesAsync();
        return ReservaResponse.De(reserva);
    }

    public async Task<ReservaResponse> AlterarAsync(int id, AlterarReservaRequest request, Solicitante solicitante)
    {
        await ConcluirReservasEncerradasAsync();
        var reserva = await BuscarAsync(id, solicitante);

        var motivo = RegrasReserva.MotivoParaNaoCancelar(reserva, relogio.Agora, solicitante.EhAdministrador);
        if (motivo is not null)
            throw ApiException.Validacao($"Esta reserva não pode ser alterada. {motivo}");

        var valor = await ValidarAgendamentoAsync(reserva.ClienteId, request.QuadraId, request.Data,
            request.HoraInicio, request.HoraFim, ignorarReservaId: reserva.Id);

        if (reserva.PagamentoAtivo is { } pagamento && pagamento.Valor != valor)
        {
            if (pagamento.Status == StatusPagamento.Pago)
                throw ApiException.Validacao(
                    "RN07 — a alteração muda o valor de uma reserva já paga. Cancele-a e faça uma nova reserva.");
            pagamento.Valor = valor;
        }

        reserva.QuadraId = request.QuadraId;
        reserva.Data = request.Data;
        reserva.HoraInicio = request.HoraInicio;
        reserva.HoraFim = request.HoraFim;
        reserva.Valor = valor;

        await db.SaveChangesAsync();
        return ReservaResponse.De(reserva);
    }

    public async Task<ReservaResponse> CancelarAsync(int id, Solicitante solicitante)
    {
        await ConcluirReservasEncerradasAsync();
        var reserva = await BuscarAsync(id, solicitante);

        var motivo = RegrasReserva.MotivoParaNaoCancelar(reserva, relogio.Agora, solicitante.EhAdministrador);
        if (motivo is not null)
            throw ApiException.Validacao(motivo);

        reserva.Status = StatusReserva.Cancelada;
        if (reserva.PagamentoAtivo is { } pagamento)
            pagamento.Status = RegrasPagamento.StatusAoCancelarReserva(pagamento.Status);

        await db.SaveChangesAsync();
        return ReservaResponse.De(reserva);
    }

    private async Task<Reserva> BuscarAsync(int id, Solicitante solicitante)
    {
        var reserva = await db.Reservas.Include(r => r.Pagamentos).FirstOrDefaultAsync(r => r.Id == id)
            ?? throw ApiException.NaoEncontrado("Reserva não encontrada.");
        solicitante.ExigirAcessoAoCliente(reserva.ClienteId, SemAcesso);
        return reserva;
    }

    private async Task ConcluirReservasEncerradasAsync()
    {
        var agora = relogio.Agora;
        var hoje = DateOnly.FromDateTime(agora);
        var horaAtual = new TimeOnly(agora.Hour, agora.Minute, agora.Second);

        await db.Reservas
            .Where(r => r.Status == StatusReserva.Confirmada
                && (r.Data < hoje || (r.Data == hoje && r.HoraFim <= horaAtual)))
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, StatusReserva.Concluida));
    }

    private async Task<decimal> ValidarAgendamentoAsync(
        int clienteId, int quadraId, DateOnly data, TimeOnly inicio, TimeOnly fim, int? ignorarReservaId)
    {
        var cliente = await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clienteId)
            ?? throw ApiException.Validacao("RN05 — toda reserva deve estar associada a um cliente.", "clienteId");
        if (cliente.Status != StatusCliente.Ativo)
            throw ApiException.Validacao("Cliente inativo não pode realizar reservas.", "clienteId");

        var quadra = await db.Quadras.AsNoTracking().FirstOrDefaultAsync(q => q.Id == quadraId)
            ?? throw ApiException.Validacao("RN06 — toda reserva deve estar associada a uma quadra.", "quadraId");
        if (quadra.Status != StatusQuadra.Ativa)
            throw ApiException.Validacao($"RN03 — {quadra.Nome} {DescreverIndisponibilidade(quadra.Status)} não pode ser reservada.", "quadraId");

        var duracao = RegrasReserva.DuracaoEmMinutos(inicio, fim);
        if (!RegrasReserva.DuracoesMinutos.Contains(duracao))
            throw ApiException.Validacao("Duração inválida. Escolha 1 hora, 1h30 ou 2 horas.", "horaFim");

        var agora = relogio.Agora;
        if (!RegrasReserva.DentroDaJanela(data, DateOnly.FromDateTime(agora)))
            throw ApiException.Validacao("As reservas podem ser feitas de hoje até 30 dias à frente.", "data");

        if (!RegrasReserva.DentroDoFuncionamento(inicio, fim))
            throw ApiException.Validacao("O horário da reserva deve estar entre 07:00 e 22:00.", "horaInicio");

        if (data.ToDateTime(inicio) <= agora)
            throw ApiException.Validacao("Não é possível reservar um horário que já passou.", "horaInicio");

        var conflito = await db.Reservas.AnyAsync(r =>
            r.QuadraId == quadraId
            && r.Data == data
            && r.Status != StatusReserva.Cancelada
            && r.Id != ignorarReservaId
            && r.HoraInicio < fim
            && inicio < r.HoraFim);
        if (conflito)
            throw ApiException.Conflito("RN04 — já existe uma reserva para esta quadra neste horário.", "horaInicio");

        return RegrasReserva.CalcularValor(quadra.ValorHora, duracao);
    }

    private static string DescreverIndisponibilidade(StatusQuadra status) => status switch
    {
        StatusQuadra.Manutencao => "em manutenção",
        StatusQuadra.Inativa => "inativa",
        _ => "indisponível"
    };
}
