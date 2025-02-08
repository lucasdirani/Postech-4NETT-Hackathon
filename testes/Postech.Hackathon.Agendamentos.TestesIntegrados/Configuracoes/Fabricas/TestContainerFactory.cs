using Testcontainers.PostgreSql;

namespace Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Fabricas;

public static class TestContainerFactory
{
    private static PostgreSqlContainer? _container;
    private static readonly Lazy<Task> _initializeTask = new(InitializeAsync);
    public static string ConnectionString { get; private set; } = string.Empty;

    public static async Task InitializeAsync()
    {
        if (_container is null)
        {
            _container = new PostgreSqlBuilder()
                .WithDatabase("bd_agendamentos")
                .WithUsername("admin")
                .WithPassword("12345")
                .WithPortBinding("5432", "5432")
                .Build();
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
        }
    }

    public static Task EnsureInitialized() => _initializeTask.Value;
    
    public static async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
            _container = null;
        }
    }
}