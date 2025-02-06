using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Dominio.Servicos;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class ServicoDominioSetup
{
    internal static void AdicionarDependenciaServicoDominio(this IServiceCollection servicos)
    {
        servicos.AddScoped<IServicoAgendamento, ServicoAgendamento>();
    }
}