using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Infra.Dados.Repositorios;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
public static class RepositorioSetup
{
    public static void AdicionarDependenciaRepositorio(this IServiceCollection servicos)
    {
        ArgumentNullException.ThrowIfNull(servicos);
        servicos.AddScoped<IRepositorioAgendamento, RepositorioAgendamento>();
    }
}