using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Modelos;

[ExcludeFromCodeCoverage]
public record UsuarioAutenticado
{
    public Guid Id { get; init; }
    public string TipoUsuario { get; init; } = string.Empty;
    public List<string> Escopos { get; init; } = [];

    public bool PossuiEscopo(string escopo)
    {
        return Escopos.Contains(escopo);
    }

    public bool UsuarioDoTipoMedico()
    {
        return string.Equals(TipoUsuario, "medico", StringComparison.OrdinalIgnoreCase);
    }

    public bool UsuarioDoTipoPaciente()
    {
        return string.Equals(TipoUsuario, "paciente", StringComparison.OrdinalIgnoreCase);
    }
}