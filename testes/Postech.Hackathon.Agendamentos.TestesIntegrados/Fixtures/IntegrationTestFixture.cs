using Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Fabricas;

namespace Postech.Hackathon.Agendamentos.TestesIntegrados.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    public AgendamentosWebApplicationFactory FabricaAplicacaoWeb { get; private set; }
    public static string StringConexao => TestContainerFactory.ConnectionString;

    public async Task InitializeAsync()
    {
        await TestContainerFactory.EnsureInitialized();
        FabricaAplicacaoWeb = new(StringConexao);
    }

    public async Task DisposeAsync()
    {
        await TestContainerFactory.DisposeAsync();
    }
}