using Microsoft.AspNetCore.Mvc;
using Ottawa_Tech___BackEnd.Classes;

namespace Ottawa_Tech___BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControleUsuario : ControllerBase
{
    [HttpGet]
    public Usuario RetornoUsuario()
    {
        Usuario usuario1 = new Usuario();

        usuario1.Nome = "Alexandre";
        usuario1.Email = "alexandre@email.com";
        usuario1.Idade = 30;

        return usuario1;
    }

    [HttpPost]
    public Usuario CriarUsuario(Usuario usuario1)
    {
        usuario1.UsuarioAtivo = true;
        return usuario1;
    }
}