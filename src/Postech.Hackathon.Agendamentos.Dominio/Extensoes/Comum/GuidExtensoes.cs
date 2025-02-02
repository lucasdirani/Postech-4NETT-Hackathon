using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

[ExcludeFromCodeCoverage]
public static class GuidExtensoes
{
    public static bool EstaVazio(this Guid guid)
    {
        return guid == Guid.Empty;
    }
}