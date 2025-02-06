using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Postech.Hackathon.Agendamentos.Infra.Dados.Contextos;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
public static class DbContextSetup
{
    public static void AdicionarDbContext(this IServiceCollection servicos, IConfiguration configuracao)
    {
        ArgumentNullException.ThrowIfNull(servicos);
        ArgumentNullException.ThrowIfNull(configuracao);
        servicos.AddDbContext<AgendamentoDbContext>(opcoes =>
            opcoes.UseNpgsql(configuracao.GetConnectionString("DefaultConnection")),
            ServiceLifetime.Scoped
        );
    }
}