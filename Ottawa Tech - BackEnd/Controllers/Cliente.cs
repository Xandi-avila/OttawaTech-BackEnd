using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet]
    public async Task<IActionResult> ObterTodosClientes()
    {
        var clientes = await _context.Clientes.ToListAsync();
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound("O id informado não existe.");
        }
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> CriarCliente([FromBody] Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CriarCliente), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AlterarCliente(int id, [FromBody] Cliente cliente)
    {
        if (cliente.Id != 0 && cliente.Id != id)
        {
            return BadRequest("O ID da URL não corresponde ao ID enviado no corpo.");
        }

        var alterarCliente = await _context.Clientes.FindAsync(id);

        if (alterarCliente == null)
        {
            return NotFound("O id informado não existe.");
        }
        alterarCliente.Nome = cliente.Nome;
        alterarCliente.Cpf = cliente.Cpf;
        alterarCliente.Telefone = cliente.Telefone;
        alterarCliente.Email = cliente.Email;
        alterarCliente.DataNascimento = cliente.DataNascimento;
        alterarCliente.Senha = cliente.Senha;
        await _context.SaveChangesAsync();
        return Ok(alterarCliente);
    }

    [HttpPut("Situacao/{id}")]
    public async Task<IActionResult> AlterarSituacao(int id, [FromBody] Cliente cliente)
    {
        if (cliente.Id != 0 && cliente.Id != id)
        {
            return BadRequest("O ID da URL não corresponde ao ID enviado no corpo.");
        }

        var alterarCliente = await _context.Clientes.FindAsync(id);

        if (alterarCliente == null)
        {
            return NotFound("O id informado não existe.");
        }
        alterarCliente.Situacao = cliente.Situacao;
        await _context.SaveChangesAsync();
        return Ok(alterarCliente);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarCliente(int id)
    {
        var deletarCliente = await _context.Clientes.FindAsync(id);

        if (deletarCliente == null)
        {
            return NotFound("O id informado não existe.");
        }
        _context.Clientes.Remove(deletarCliente);
        await _context.SaveChangesAsync();
        return Ok(deletarCliente);
    }
}