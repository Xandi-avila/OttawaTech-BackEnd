using Microsoft.AspNetCore.Mvc;
using Ottawa_Tech___BackEnd.Data;
using Ottawa_Tech___BackEnd.Models;

namespace Ottawa_Tech___BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarCliente([FromBody] Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CriarCliente), new { id = cliente.Id }, cliente);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound("O cliente informado não existe.");
        }
        return Ok(cliente);
    }
}