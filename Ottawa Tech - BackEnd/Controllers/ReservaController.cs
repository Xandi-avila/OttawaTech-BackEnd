using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ottawa_Tech___BackEnd.Data;
using Ottawa_Tech___BackEnd.DTOs;
using Ottawa_Tech___BackEnd.Models;
using Ottawa_Tech___BackEnd.Queries;

namespace Ottawa_Tech___BackEnd.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObeterReservas([FromQuery] int pagina = 1, [FromQuery] int porPagina = 10)
        {
            IQueryable<ReservaModel> query = _context.Reservas;

            int totalRegistros = await query.CountAsync();

            var ReservaPaginadas = await query
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .Select(reserva => new ReservaRespostaDto
                {
                    Id = reserva.Id,
                    Cliente = reserva.Cliente.Nome,
                    Quadra = reserva.Quadra.Nome,
                    Data = reserva.Data,
                    HoraInicio = reserva.HoraInicio,
                    HoraFim = reserva.HoraFim,
                    Valor = reserva.Valor,
                    Status = reserva.Status,
                    DataCriacao = reserva.DataCriacao
                })
                .ToListAsync();
            var resultadoPaginado = new
            {
                TotalItens = totalRegistros,
                PaginaAtual = pagina,
                TamanhoPagina = porPagina,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / porPagina),
                Dados = ReservaPaginadas
            };

            return Ok(resultadoPaginado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterReservaPorId(int id)
        {
            var reserva = await _context.Reservas
                .Where(reserva => reserva.Id == id)
                .Select(reserva => new ReservaRespostaDto
                {
                    Id = reserva.Id,
                    Cliente = reserva.Cliente.Nome,
                    Quadra = reserva.Quadra.Nome,
                    Data = reserva.Data,
                    HoraInicio = reserva.HoraInicio,
                    HoraFim = reserva.HoraFim,
                    Valor = reserva.Valor,
                    Status = reserva.Status,
                    DataCriacao = reserva.DataCriacao
                })
                .FirstOrDefaultAsync();

            return reserva is null ? NotFound() : Ok(reserva);
        }

        [HttpPost]
        public async Task<IActionResult> CriarReserva([FromBody] CriarReservaDto reservaDto)
        {
            var cliente = await _context.Clientes.FindAsync(reservaDto.ClienteId);
            if (cliente is null)
                return NotFound($"Cliente {reservaDto.ClienteId} não encontrado.");

            var quadra = await _context.Quadras.FindAsync(reservaDto.QuadraId);
            if (quadra is null)
                return NotFound($"Quadra {reservaDto.QuadraId} não encontrada.");

            var reserva = new ReservaModel
            {
                ClienteId = reservaDto.ClienteId,
                QuadraId = reservaDto.QuadraId,
                Cliente = cliente,
                Quadra = quadra,
                Valor = reservaDto.Valor,
                Status = reservaDto.Status
            };

            await _context.Reservas.AddAsync(reserva);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterReservaPorId), new { id = reserva.Id }, ParaDto(reserva));
        }

        private static ReservaRespostaDto ParaDto(ReservaModel reserva) => new()
        {
            Id = reserva.Id,
            Cliente = reserva.Cliente.Nome,
            Quadra = reserva.Quadra.Nome,
            Data = reserva.Data,
            HoraInicio = reserva.HoraInicio,
            HoraFim = reserva.HoraFim,
            Valor = reserva.Valor,
            Status = reserva.Status,
            DataCriacao = reserva.DataCriacao
        };
    }
}