using Microsoft.AspNetCore.Mvc;
using Ottawa_Tech___BackEnd.Models;

namespace Ottawa_Tech___BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControleUsuario : ControllerBase
{
    [HttpGet]
    public Cliente RetornoUsuario()
    {
        Cliente Cliente = new Cliente();

        Cliente.Id = 0;
        Cliente.Nome = "Alexandre";
        Cliente.Email = "alexandre@email.com";

        return Cliente;
    }

    [HttpPost]
    public Cliente CriarUsuario(Cliente cliente)
    {
        cliente.ClienteAtivo = true;
        return cliente;
    }
}