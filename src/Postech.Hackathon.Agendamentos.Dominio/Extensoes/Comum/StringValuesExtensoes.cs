using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

[ExcludeFromCodeCoverage]
public static class StringValuesExtensoes
{
    public static bool ContainsAny(this StringValues fonte, params string[] valores)
    {
        return valores.Any(fonte.Contains);
    }
}