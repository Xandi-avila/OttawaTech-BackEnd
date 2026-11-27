using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OttawaTech.BackEnd.DTOs.Quadras;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Controllers;

[ApiController]
[Authorize]
[Route("api/quadras")]
public class QuadrasController(QuadraService quadras) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<QuadraResponse>>> Listar() =>
        Ok(await quadras.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuadraResponse>> Obter(int id) =>
        Ok(await quadras.ObterAsync(id));

    [HttpGet("{id:int}/disponibilidade")]
    public async Task<ActionResult<DisponibilidadeResponse>> Disponibilidade(int id, [FromQuery] DateOnly? data) =>
        Ok(await quadras.ConsultarDisponibilidadeAsync(id, data));

    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<ActionResult<QuadraResponse>> Criar(QuadraRequest request)
    {
        var quadra = await quadras.CriarAsync(request);
        return CreatedAtAction(nameof(Obter), new { id = quadra.Id }, quadra);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<ActionResult<QuadraResponse>> Atualizar(int id, QuadraRequest request) =>
        Ok(await quadras.AtualizarAsync(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<IActionResult> Inativar(int id)
    {
        await quadras.InativarAsync(id);
        return NoContent();
    }
}
