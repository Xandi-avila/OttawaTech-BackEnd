using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ottawa_Tech___BackEnd.Data;
using Ottawa_Tech___BackEnd.Models;
using Ottawa_Tech___BackEnd.Queries;

namespace Ottawa_Tech___BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuadrasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuadrasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObeterQuadras([FromQuery] int pagina = 1, [FromQuery] int porPagina = 10)
        {
            IQueryable<Quadra> query = _context.Quadras;

            int totalRegistros = await query.CountAsync();

            var quadrasPaginadas = await query
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();
            var resultado = new
            {
                TotalItens = totalRegistros,
                PaginaAtual = pagina,
                TamanhoPagina = porPagina,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / porPagina),
                Dados = quadrasPaginadas
            };

            return Ok(resultado);
        }

        [HttpGet("Filtrar")]
        public async Task<IActionResult> ObeterQuadrasSituacao([FromQuery] BuscarQuadrasQuery filtro)
        {
            IQueryable<Quadra> query = _context.Quadras;

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
                query = query.Where(quadra => quadra.Nome.Contains(filtro.Nome));

            if (!string.IsNullOrWhiteSpace(filtro.Tipo))
                query = query.Where(quadra => quadra.Tipo.Contains(filtro.Tipo));

            if (filtro.Status.HasValue)
                query = query.Where(quadra => quadra.Status == filtro.Status.Value);

            if (filtro.ValorMaximo.HasValue)
                query = query.Where(quadra => quadra.ValorHora <= filtro.ValorMaximo.Value);

            if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue)
            {
                // Antes da query rodar extraio o times pures para não ter erro na compilação.
                var dataFiltro = DateOnly.FromDateTime(filtro.DataInicio.Value);
                var horaInicioFiltro = TimeOnly.FromDateTime(filtro.DataInicio.Value);
                var horaFimFiltro = TimeOnly.FromDateTime(filtro.DataFim.Value);

                // Quadras sem nenhuma reserva que conflite no mesmo dia / horario.
                query = query.Where(quadra => !quadra.Reservas.Any(reserva =>
                    reserva.Data == dataFiltro &&
                    horaInicioFiltro < reserva.HoraFim &&
                    horaFimFiltro > reserva.HoraInicio
                ));
            }
            else if (filtro.DataInicio.HasValue)
            {
                var dataFiltro = DateOnly.FromDateTime(filtro.DataInicio.Value);
                var horaInicioFiltro = TimeOnly.FromDateTime(filtro.DataInicio.Value);

                // Se for enviado somente a Data inicio chega se ta livre naquele dia / horario.
                query = query.Where(quadra => !quadra.Reservas.Any(reserva =>
                    reserva.Data == dataFiltro &&
                    horaInicioFiltro >= reserva.HoraInicio &&
                    horaInicioFiltro < reserva.HoraFim
                ));
            }

            int totalRegistros = await query.CountAsync();
            if (totalRegistros == 0)
                return NotFound("Nenhuma quadra encontrada para os filtros aplicados!");

            var quadrasPaginadas = await query
                .Skip((filtro.Pagina - 1) * filtro.PorPagina)
                .Take(filtro.PorPagina)
                .ToListAsync();

            return Ok(new
            {
                TotalItens = totalRegistros,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.PorPagina,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistros / filtro.PorPagina),
                Dados = quadrasPaginadas
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var quadras = await _context.Quadras.FindAsync(id);
            if (quadras == null)
            {
                return NotFound("O id informado não existe.");
            }
            return Ok(quadras);
        }

        [HttpPost]
        public async Task<IActionResult> CriarQuadra([FromBody] Quadra quadra)
        {
            await _context.Quadras.AddAsync(quadra);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CriarQuadra), new { id = quadra.Id }, quadra);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AlterarQuadra(int id, [FromBody] Quadra quadra)
        {
            if (quadra.Id != 0 && quadra.Id != id)
            {
                return BadRequest("O ID da URL não corresponde ao ID enviado no corpo.");
            }

            var alterarQuadra = await _context.Quadras.FindAsync(id);

            if (alterarQuadra == null)
            {
                return NotFound("O id informado não existe.");
            }
            alterarQuadra.Nome = quadra.Nome;
            alterarQuadra.Tipo = quadra.Tipo;
            alterarQuadra.ValorHora = quadra.ValorHora;
            await _context.SaveChangesAsync();
            return Ok(alterarQuadra);
        }

        [HttpPut("Situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id, [FromBody] Quadra quadra)
        {
            if (quadra.Id != 0 && quadra.Id != id)
            {
                return BadRequest("O ID da URL não corresponde ao ID enviado no corpo.");
            }

            var alterarQuadra = await _context.Quadras.FindAsync(id);

            if (alterarQuadra == null)
            {
                return NotFound("O id informado não existe.");
            }
            alterarQuadra.Status = quadra.Status;
            await _context.SaveChangesAsync();
            return Ok(alterarQuadra);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarCliente(int id)
        {
            var alterarQuadra = await _context.Quadras.FindAsync(id);

            if (alterarQuadra == null)
            {
                return NotFound("O id informado não existe.");
            }
            _context.Quadras.Remove(alterarQuadra);
            await _context.SaveChangesAsync();
            return Ok(alterarQuadra);
        }
    }
}