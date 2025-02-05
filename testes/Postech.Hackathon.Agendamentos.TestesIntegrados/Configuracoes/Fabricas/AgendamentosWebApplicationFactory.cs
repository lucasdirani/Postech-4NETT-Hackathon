using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Postech.Hackathon.Agendamentos.Infra.Dados.Contextos;

namespace Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Fabricas;

public class AgendamentosWebApplicationFactory(string stringConexao) : WebApplicationFactory<Program>
{
    public readonly string StringConexao = stringConexao;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(DbContextOptions<AgendamentoDbContext>));
            if (dbContextServiceDescriptor is not null)
            {
                services.Remove(dbContextServiceDescriptor);
            }
            services.AddDbContext<AgendamentoDbContext>((provider, optionsBuilder) =>
            {
                optionsBuilder.UseNpgsql(StringConexao, npgsqlBuilder =>
                {
                    npgsqlBuilder.EnableRetryOnFailure(3);
                });
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope scope = serviceProvider.CreateScope();
            AgendamentoDbContext db = scope.ServiceProvider.GetRequiredService<AgendamentoDbContext>();
            db.Database.EnsureCreated();
        });
    }
}