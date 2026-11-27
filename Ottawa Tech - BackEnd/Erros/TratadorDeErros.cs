using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OttawaTech.BackEnd.Erros;

public sealed class TratadorDeErros(IProblemDetailsService problemDetails, ILogger<TratadorDeErros> logger)
    : IExceptionHandler
{
    private const string MensagemErroInesperado = "Erro inesperado no servidor. Tente novamente mais tarde.";

    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception erro, CancellationToken cancellationToken)
    {
        ProblemDetails problema;
        if (erro is ApiException api)
        {
            problema = Criar(api);
        }
        else
        {
            logger.LogError(erro, "Erro não tratado em {Metodo} {Caminho}", http.Request.Method, http.Request.Path);
            problema = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro interno",
                Detail = MensagemErroInesperado
            };
        }

        http.Response.StatusCode = problema.Status!.Value;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = problema,
            Exception = erro
        });
    }

    private static ProblemDetails Criar(ApiException erro)
    {
        var problema = erro.Campo is null
            ? new ProblemDetails()
            : new ValidationProblemDetails(new Dictionary<string, string[]> { [erro.Campo] = [erro.Message] });

        problema.Status = erro.Status;
        problema.Title = erro.Titulo;
        problema.Detail = erro.Message;
        return problema;
    }
}
