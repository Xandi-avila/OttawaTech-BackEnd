using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OttawaTech.BackEnd.DTOs.Clientes;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Seguranca;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Controllers;

[ApiController]
[Authorize]
[Route("api/clientes")]
public class ClientesController(ClienteService clientes) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> Listar([FromQuery] ClienteFiltro filtro) =>
        Ok(await clientes.ListarAsync(filtro));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> Obter(int id) =>
        Ok(await clientes.ObterAsync(id, Solicitante.De(User)));

    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<ActionResult<ClienteResponse>> Criar(ClienteRequest request)
    {
        var cliente = await clientes.CriarAsync(request);
        return CreatedAtAction(nameof(Obter), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> Atualizar(int id, ClienteRequest request) =>
        Ok(await clientes.AtualizarAsync(id, request, Solicitante.De(User)));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<IActionResult> Inativar(int id)
    {
        await clientes.InativarAsync(id);
        return NoContent();
    }
}
