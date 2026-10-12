using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ottawa_Tech___BackEnd.Data;
using Ottawa_Tech___BackEnd.Enums;
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
        public async Task<IActionResult> ObeterQuadras()
        {
            var quadras = await _context.Quadras.ToListAsync();
            return Ok(quadras);
        }

        [HttpGet("Filtrar")]
        public async Task<IActionResult> ObeterQuadrasSituacao([FromQuery] BuscarQuadrasQuery filtro)
        {
            IQueryable<Quadra> query = _context.Quadras;

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
                query = query.Where(q => q.Nome.Contains(filtro.Nome));

            if (!string.IsNullOrWhiteSpace(filtro.Tipo))
                query = query.Where(q => q.Tipo.Contains(filtro.Tipo));

            if (filtro.Status.HasValue)
                query = query.Where(q => q.Status == filtro.Status.Value);

            if (filtro.ValorMaximo.HasValue)
                query = query.Where(q => q.ValorHora <= filtro.ValorMaximo.Value);

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