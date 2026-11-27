using Microsoft.AspNetCore.Mvc;
using OttawaTech.BackEnd.Configuracao;
using OttawaTech.BackEnd.Data;
using OttawaTech.BackEnd.Erros;
using OttawaTech.BackEnd.Seguranca;
using OttawaTech.BackEnd.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("PORT") is { Length: > 0 } porta)
    builder.WebHost.UseUrls($"http://0.0.0.0:{porta}");

builder.Services.Configure<ArenaOptions>(builder.Configuration.GetSection(ArenaOptions.Secao));
builder.Services.Configure<MvcOptions>(opcoes => opcoes.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddControllers().ResponderValidacaoEmPortugues();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorDeErros>();
builder.Services.AddDocumentacao();
builder.Services.AddCorsDoFront(builder.Configuration);
builder.Services.AddAutenticacaoJwt(builder.Configuration);
builder.Services.AddBancoDeDados(builder.Configuration);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<RelogioArena>();
builder.Services.AddSingleton<ServicoDeSenha>();
builder.Services.AddSingleton<GeradorDeToken>();
builder.Services.AddScoped<DadosDemo>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<QuadraService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<PagamentoService>();

var app = builder.Build();

await PrepararBancoAsync(app);

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCorsDoFront();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

app.Run();

static async Task PrepararBancoAsync(WebApplication app)
{
    using var escopo = app.Services.CreateScope();
    var servicos = escopo.ServiceProvider;

    await servicos.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();

    if (app.Configuration.GetValue<bool>("Arena:SemearDadosDemo"))
        await servicos.GetRequiredService<DadosDemo>().SemearAsync();
}
