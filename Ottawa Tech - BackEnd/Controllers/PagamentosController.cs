using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OttawaTech.BackEnd.DTOs.Pagamentos;
using OttawaTech.BackEnd.Enums;
using OttawaTech.BackEnd.Seguranca;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Controllers;

[ApiController]
[Authorize]
[Route("api/pagamentos")]
public class PagamentosController(PagamentoService pagamentos) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PagamentoResponse>>> Listar([FromQuery] PagamentoFiltro filtro) =>
        Ok(await pagamentos.ListarAsync(filtro, Solicitante.De(User)));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PagamentoResponse>> Obter(int id) =>
        Ok(await pagamentos.ObterAsync(id, Solicitante.De(User)));

    [HttpPost]
    public async Task<ActionResult<PagamentoResponse>> Registrar(RegistrarPagamentoRequest request)
    {
        var pagamento = await pagamentos.RegistrarAsync(request, Solicitante.De(User));
        return CreatedAtAction(nameof(Obter), new { id = pagamento.Id }, pagamento);
    }

    [HttpPost("{id:int}/confirmar")]
    [Authorize(Roles = Perfis.Administrador)]
    public async Task<ActionResult<PagamentoResponse>> Confirmar(int id) =>
        Ok(await pagamentos.ConfirmarAsync(id));
}
