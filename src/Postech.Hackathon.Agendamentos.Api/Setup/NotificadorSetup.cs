using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Interfaces;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class NotificadorSetup
{
    internal static void AdicionarDependenciaNotificador(this IServiceCollection servicos)
    {
        servicos.AddScoped<INotificador, Notificador>();
    }
}