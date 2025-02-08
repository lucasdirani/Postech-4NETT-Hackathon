using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Requisicoes.Extensoes;

[ExcludeFromCodeCoverage]
public static class HttpRequestExtensoes
{
    public static IDictionary<string, object?> ObterValoresRota(this HttpRequest requisicaoHttp)
    {
        return requisicaoHttp.RouteValues.ToDictionary(route => route.Key, route => route.Value);
    }

    public static IDictionary<string, StringValues> ObterValoresConsulta(this HttpRequest requisicaoHttp)
    {
        return requisicaoHttp.Query.ToDictionary(query => query.Key, query => query.Value);
    }
}