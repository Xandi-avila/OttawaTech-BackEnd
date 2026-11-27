using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.Seguranca;

namespace OttawaTech.BackEnd.Configuracao;

public static class ConfiguracaoDaApi
{
    private const string PoliticaCors = "Front";
    private const string MensagemDadosInvalidos = "Dados inválidos. Revise os campos e tente novamente.";

    public static IServiceCollection AddBancoDeDados(this IServiceCollection services, IConfiguration configuracao)
    {
        var postgres = configuracao.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(postgres) && configuracao["DATABASE_URL"] is { Length: > 0 } url)
            postgres = ConverterUrlPostgres(url);

        return services.AddDbContext<AppDbContext>(opcoes =>
        {
            if (string.IsNullOrWhiteSpace(postgres))
                opcoes.UseSqlite(configuracao.GetConnectionString("Sqlite"));
            else
                opcoes.UseNpgsql(postgres);
        });
    }

    public static IServiceCollection AddAutenticacaoJwt(this IServiceCollection services, IConfiguration configuracao)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuracao.GetSection(JwtOptions.Secao))
            .Validate(jwt => jwt.Chave.Length >= JwtOptions.TamanhoMinimoChave,
                $"Configure Jwt:Chave com pelo menos {JwtOptions.TamanhoMinimoChave} caracteres.")
            .ValidateOnStart();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, opcoes) =>
            {
                var jwt = opcoes.Value;
                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Emissor,
                    ValidAudience = jwt.Audiencia,
                    IssuerSigningKey = jwt.ChaveDeAssinatura,
                    NameClaimType = Claims.Nome,
                    RoleClaimType = Claims.Perfil,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddCorsDoFront(this IServiceCollection services, IConfiguration configuracao)
    {
        var origens = (configuracao["Cors:Origens"] ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(origem => origem.TrimEnd('/'))
            .ToArray();

        return services.AddCors(cors => cors.AddPolicy(PoliticaCors, politica =>
        {
            if (origens.Length == 0)
                politica.AllowAnyOrigin();
            else
                politica.WithOrigins(origens);

            politica.AllowAnyHeader().AllowAnyMethod();
        }));
    }

    public static IApplicationBuilder UseCorsDoFront(this IApplicationBuilder app) => app.UseCors(PoliticaCors);

    public static IMvcBuilder ResponderValidacaoEmPortugues(this IMvcBuilder mvc) =>
        mvc.ConfigureApiBehaviorOptions(opcoes =>
            opcoes.InvalidModelStateResponseFactory = contexto =>
            {
                var erros = contexto.ModelState
                    .Where(item => item.Value?.Errors.Count > 0)
                    .ToDictionary(item => item.Key, _ => new[] { "Valor inválido." });

                return new BadRequestObjectResult(new ValidationProblemDetails(erros)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Dados inválidos",
                    Detail = MensagemDadosInvalidos
                });
            });

    public static IServiceCollection AddDocumentacao(this IServiceCollection services) =>
        services.AddOpenApi(opcoes => opcoes.AddDocumentTransformer((documento, _, _) =>
        {
            documento.Info.Title = "Ottawa Tech — API da Arena";
            documento.Components ??= new OpenApiComponents();
            documento.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            documento.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Token recebido em /api/auth/login."
            };
            documento.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, documento)] = []
                }
            ];
            return Task.CompletedTask;
        }));

    private static string ConverterUrlPostgres(string url)
    {
        if (!url.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !url.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return url;

        var uri = new Uri(url);
        var credenciais = uri.UserInfo.Split(':', 2);
        var semSsl = uri.Query.Contains("sslmode=disable", StringComparison.OrdinalIgnoreCase);

        return new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            Username = Uri.UnescapeDataString(credenciais[0]),
            Password = credenciais.Length > 1 ? Uri.UnescapeDataString(credenciais[1]) : null,
            SslMode = semSsl ? SslMode.Disable : SslMode.Require
        }.ConnectionString;
    }
}
