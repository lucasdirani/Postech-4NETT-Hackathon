using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Infra.Http.Clientes;
using Refit;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class RefitSetup
{
    internal static void AdicionarDependenciaRefit(this IServiceCollection servicos, IConfiguration configuracao)
    {
        string? enderecoBase = configuracao["Clientes:Autenticacao:BaseAddress"];
        if (string.IsNullOrEmpty(enderecoBase))
        {
            throw new InvalidOperationException("A configuração 'Clientes:Autenticacao:BaseAddress' não foi encontrada ou está vazia.");
        }
        servicos
            .AddRefitClient<IClienteMicrosservicoAutenticacao>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(enderecoBase));
    }
}