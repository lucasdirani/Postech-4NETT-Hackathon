using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Requisicoes.Extensoes;

[ExcludeFromCodeCoverage]
public static class HttpRequestExtensoes
{
    public static IDictionary<string, object?> ObterValoresRota(this HttpRequest requisicaoHttp)
    {
        return requisicaoHttp.RouteValues.ToDictionary(route => route.Key, route => route.Value);
    }
}