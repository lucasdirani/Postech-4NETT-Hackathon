using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Constantes;

[ExcludeFromCodeCoverage]
public static class ClaimUsuarioAutenticado
{
    public static readonly string Id = "Id";
    public static readonly string TipoUsuario = "TipoUsuario";
    public static readonly string Escopos = "Escopos";
}