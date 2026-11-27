using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OttawaTech.BackEnd.DTOs.Reservas;
using OttawaTech.BackEnd.Seguranca;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Controllers;

[ApiController]
[Authorize]
[Route("api/reservas")]
public class ReservasController(ReservaService reservas) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReservaResponse>>> Listar([FromQuery] ReservaFiltro filtro) =>
        Ok(await reservas.ListarAsync(filtro, Solicitante.De(User)));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservaResponse>> Obter(int id) =>
        Ok(await reservas.ObterAsync(id, Solicitante.De(User)));

    [HttpPost]
    public async Task<ActionResult<ReservaResponse>> Criar(CriarReservaRequest request)
    {
        var reserva = await reservas.CriarAsync(request, Solicitante.De(User));
        return CreatedAtAction(nameof(Obter), new { id = reserva.Id }, reserva);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReservaResponse>> Alterar(int id, AlterarReservaRequest request) =>
        Ok(await reservas.AlterarAsync(id, request, Solicitante.De(User)));

    [HttpPost("{id:int}/cancelar")]
    public async Task<ActionResult<ReservaResponse>> Cancelar(int id) =>
        Ok(await reservas.CancelarAsync(id, Solicitante.De(User)));
}
