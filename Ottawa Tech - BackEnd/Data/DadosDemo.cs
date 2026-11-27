using Microsoft.EntityFrameworkCore;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Models;
using OttawaTech.BackEnd.Regras;
using OttawaTech.BackEnd.Seguranca;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Data;

public class DadosDemo(AppDbContext db, ServicoDeSenha senhas, RelogioArena relogio, ILogger<DadosDemo> logger)
{
    private const string SenhaAdministrador = "admin123";
    private const string SenhaClientes = "123456";

    private sealed record ReservaDemo(
        int Cliente,
        int Quadra,
        int Dia,
        TimeOnly Inicio,
        int Minutos,
        StatusReserva Status,
        MetodoPagamento Metodo,
        StatusPagamento StatusPagamento,
        int DiaCriacao,
        TimeOnly HoraCriacao);

    public async Task SemearAsync()
    {
        if (await db.Administradores.AnyAsync())
            return;

        var hashClientes = senhas.GerarHash(SenhaClientes);
        var quadras = CriarQuadras();
        var clientes = CriarClientes(hashClientes);

        db.Administradores.Add(new Administrador
        {
            Nome = "Gabriel Lessa",
            Email = "admin@arena.com",
            SenhaHash = senhas.GerarHash(SenhaAdministrador)
        });
        db.Quadras.AddRange(quadras);
        db.Clientes.AddRange(clientes);
        db.Reservas.AddRange(Reservas().Select(r => CriarReserva(r, clientes, quadras)));

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException erro)
        {
            logger.LogWarning(erro, "Os dados de demonstração já foram criados por outra instância.");
        }
    }

    private Reserva CriarReserva(ReservaDemo demo, IReadOnlyList<Cliente> clientes, IReadOnlyList<Quadra> quadras)
    {
        var quadra = quadras[demo.Quadra - 1];
        var hoje = relogio.Hoje;
        var criadaEm = relogio.ParaUtc(hoje.AddDays(demo.DiaCriacao), demo.HoraCriacao);
        var valor = RegrasReserva.CalcularValor(quadra.ValorHora, demo.Minutos);
        var quitado = demo.StatusPagamento is StatusPagamento.Pago or StatusPagamento.Estornado;

        return new Reserva
        {
            Cliente = clientes[demo.Cliente - 1],
            Quadra = quadra,
            Data = hoje.AddDays(demo.Dia),
            HoraInicio = demo.Inicio,
            HoraFim = demo.Inicio.AddMinutes(demo.Minutos),
            Valor = valor,
            Status = demo.Status,
            DataCriacao = criadaEm,
            Pagamentos =
            [
                new Pagamento
                {
                    Valor = valor,
                    Metodo = demo.Metodo,
                    Status = demo.StatusPagamento,
                    DataPagamento = quitado ? criadaEm.AddMinutes(2) : null
                }
            ]
        };
    }

    private static List<Quadra> CriarQuadras() =>
    [
        new() { Nome = "Quadra 01", Tipo = "Beach Tennis", ValorHora = 80, Status = StatusQuadra.Ativa },
        new() { Nome = "Quadra 02", Tipo = "Beach Tennis", ValorHora = 80, Status = StatusQuadra.Manutencao },
        new() { Nome = "Quadra 03", Tipo = "Beach Tennis", ValorHora = 90, Status = StatusQuadra.Ativa },
        new() { Nome = "Quadra 04", Tipo = "Futevôlei", ValorHora = 70, Status = StatusQuadra.Ativa },
        new() { Nome = "Quadra 05", Tipo = "Beach Tennis", ValorHora = 95, Status = StatusQuadra.Ativa },
        new() { Nome = "Quadra 06", Tipo = "Vôlei de praia", ValorHora = 70, Status = StatusQuadra.Inativa }
    ];

    private static List<Cliente> CriarClientes(string senhaHash)
    {
        Cliente Novo(string nome, string cpf, string telefone, string email, string nascimento,
            StatusCliente status = StatusCliente.Ativo) => new()
        {
            Nome = nome,
            Cpf = cpf,
            Telefone = telefone,
            Email = email,
            DataNascimento = DateOnly.Parse(nascimento),
            SenhaHash = senhaHash,
            Status = status
        };

        return
        [
            Novo("Isadora Oliveira", "012.345.678-90", "(51) 99812-4477", "isadora@email.com", "1998-03-14"),
            Novo("Gabriel Lessa", "987.654.321-00", "(51) 99640-1180", "gabriel@email.com", "1996-07-22"),
            Novo("Alexandre De Ávila", "456.789.123-64", "(51) 99327-6654", "alexandre@email.com", "1990-11-05"),
            Novo("Jeferson Rodrigues", "321.654.987-91", "(51) 99105-3390", "jeferson@email.com", "1993-01-30"),
            Novo("Marina Duarte", "159.753.486-25", "(51) 98874-2015", "marina@email.com", "1997-06-18", StatusCliente.Inativo),
            Novo("Bruna Carvalho", "274.185.963-91", "(51) 99456-7812", "bruna@email.com", "1995-04-12"),
            Novo("Rafael Menezes", "365.298.147-28", "(51) 99231-5566", "rafael@email.com", "1988-09-03"),
            Novo("Camila Rocha", "418.529.637-19", "(51) 99784-3021", "camila@email.com", "2000-02-27"),
            Novo("Lucas Fernandes", "583.716.294-19", "(51) 99518-9047", "lucas@email.com", "1992-12-08"),
            Novo("Patrícia Nunes", "649.283.751-73", "(51) 99367-2284", "patricia@email.com", "1985-05-19"),
            Novo("Thiago Martins", "731.864.295-28", "(51) 99652-7730", "thiago@email.com", "1999-08-25"),
            Novo("Juliana Castro", "852.497.316-19", "(51) 99873-4159", "juliana@email.com", "1994-10-31")
        ];
    }

    private static IEnumerable<ReservaDemo> Reservas()
    {
        const StatusReserva confirmada = StatusReserva.Confirmada;
        const StatusPagamento pago = StatusPagamento.Pago;
        const StatusPagamento pendente = StatusPagamento.Pendente;

        ReservaDemo Nova(int cliente, int quadra, int dia, int hora, int minutos, StatusReserva status,
            MetodoPagamento metodo, StatusPagamento pagamento, int diaCriacao, string horaCriacao) =>
            new(cliente, quadra, dia, new TimeOnly(hora, 0), minutos, status, metodo, pagamento,
                diaCriacao, TimeOnly.Parse(horaCriacao));

        yield return Nova(1, 1, 0, 19, 60, confirmada, MetodoPagamento.Pix, pago, -10, "09:12");
        yield return Nova(2, 3, 0, 8, 120, confirmada, MetodoPagamento.Cartao, pago, -9, "14:03");
        yield return Nova(1, 4, 1, 20, 60, StatusReserva.Pendente, MetodoPagamento.Pix, pendente, -8, "10:47");
        yield return Nova(3, 1, 0, 10, 60, confirmada, MetodoPagamento.Dinheiro, pendente, -8, "19:25");
        yield return Nova(4, 5, 2, 18, 90, confirmada, MetodoPagamento.Pix, pago, -7, "12:30");
        yield return Nova(1, 3, -6, 7, 60, StatusReserva.Concluida, MetodoPagamento.Pix, pago, -7, "20:41");
        yield return Nova(2, 1, 1, 16, 60, confirmada, MetodoPagamento.Cartao, pendente, -5, "08:55");
        yield return Nova(6, 3, 0, 18, 90, confirmada, MetodoPagamento.Pix, pago, -4, "13:18");
        yield return Nova(7, 5, 0, 20, 120, confirmada, MetodoPagamento.Cartao, pago, -4, "21:02");
        yield return Nova(8, 4, 0, 17, 60, confirmada, MetodoPagamento.Dinheiro, pendente, -3, "09:40");
        yield return Nova(9, 1, 1, 18, 90, confirmada, MetodoPagamento.Pix, pago, -3, "17:15");
        yield return Nova(10, 3, 2, 19, 60, confirmada, MetodoPagamento.Cartao, pago, -2, "11:08");
        yield return Nova(11, 4, 3, 8, 120, confirmada, MetodoPagamento.Pix, pago, -2, "19:33");
        yield return Nova(12, 5, 3, 17, 60, confirmada, MetodoPagamento.Dinheiro, pendente, -1, "10:21");
        yield return Nova(3, 1, 2, 7, 60, StatusReserva.Cancelada, MetodoPagamento.Pix, StatusPagamento.Estornado, -1, "15:47");
        yield return Nova(1, 5, 3, 19, 90, confirmada, MetodoPagamento.Pix, pago, -1, "20:10");
    }
}
