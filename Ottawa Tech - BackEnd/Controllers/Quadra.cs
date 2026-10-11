using Microsoft.AspNetCore.Mvc;
using Ottawa_Tech___BackEnd.Data;
using Ottawa_Tech___BackEnd.Models;
using Microsoft.EntityFrameworkCore;

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