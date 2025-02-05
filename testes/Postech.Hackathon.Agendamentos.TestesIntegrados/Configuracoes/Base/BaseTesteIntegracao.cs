using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Fabricas;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Fixtures;

namespace Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Base;

[ExcludeFromCodeCoverage]
public class BaseTesteIntegracao : IClassFixture<IntegrationTestFixture>
{
    protected readonly HttpClient? ClienteHttp;
    protected readonly AgendamentosWebApplicationFactory FabricaAplicacaoWeb;

    protected BaseTesteIntegracao(IntegrationTestFixture fixture)
    {
        FabricaAplicacaoWeb = fixture.FabricaAplicacaoWeb;
        ClienteHttp = FabricaAplicacaoWeb?.CreateClient();
    }
    
    protected T ObterServico<T>() 
        where T : notnull
    {
        IServiceScope escopo = FabricaAplicacaoWeb.Services.CreateScope();
        return escopo.ServiceProvider.GetRequiredService<T>();
    }
}