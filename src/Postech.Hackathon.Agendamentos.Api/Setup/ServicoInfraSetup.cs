using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Interfaces;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class ServicoInfraSetup
{
    internal static void AdicionarDependenciaServicoInfra(this IServiceCollection servicos)
    {
        servicos.AddScoped<IServicoAutenticacaoUsuario, ServicoAutenticacaoUsuario>();
    }
}