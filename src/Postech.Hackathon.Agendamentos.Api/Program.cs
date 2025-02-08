using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Postech.Hackathon.Agendamentos.Api.HealthChecks;
using Postech.Hackathon.Agendamentos.Api.Setup;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http;
using Postech.Hackathon.Agendamentos.Infra.Http.Adaptadores;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5060");
builder.Configuration.AdicionarArquivoJsonPeloAmbiente(builder.Environment.EnvironmentName);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AdicionarDependenciaRefit(builder.Configuration);
builder.Services.AdicionarDependenciaNotificador();
builder.Services.AdicionarDependenciaServicoInfra();
builder.Services.AdicionarDependenciaServicoDominio();
builder.Services.AdicionarDbContext(builder.Configuration);
builder.Services.AdicionarDependenciaRepositorio();
builder.Services.AdicionarDependenciaCasoUso();
builder.Services.AddHealthChecks().AdicionarHealthCheckPostgres(builder.Configuration);

WebApplication app = builder.Build();

app.MapHealthChecks("/health"); 
app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.UseHttpsRedirection();
app.UsarMiddlewareValidacaoToken();

AdaptadorAspNetCore http = new(app);
_ = new ControladorAgendamentos(http);
http.Executar();

[ExcludeFromCodeCoverage]
public partial class Program
{
    protected Program() { }
}