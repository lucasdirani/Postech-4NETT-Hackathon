using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores.Extensoes;

[ExcludeFromCodeCoverage]
public static class HttpContentExtensoes
{
    public static async Task<T?> AsAsync<T>(this HttpContent httpContent)
    {
        return await JsonSerializer.DeserializeAsync<T>(await httpContent.ReadAsStreamAsync());
    }
}