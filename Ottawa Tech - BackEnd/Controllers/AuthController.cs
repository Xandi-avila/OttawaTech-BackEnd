using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OttawaTech.BackEnd.DTOs.Auth;
using OttawaTech.BackEnd.Services;

namespace OttawaTech.BackEnd.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AutenticacaoResponse>> Login(LoginRequest request) =>
        Ok(await auth.LoginAsync(request));

    [HttpPost("register")]
    public async Task<ActionResult<AutenticacaoResponse>> Registrar(RegistroRequest request) =>
        StatusCode(StatusCodes.Status201Created, await auth.RegistrarAsync(request));
}
