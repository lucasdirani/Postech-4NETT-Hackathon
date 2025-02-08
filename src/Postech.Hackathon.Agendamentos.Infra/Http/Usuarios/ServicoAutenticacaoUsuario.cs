using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Constantes;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Modelos;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Usuarios;

[ExcludeFromCodeCoverage]
public class ServicoAutenticacaoUsuario : IServicoAutenticacaoUsuario
{
    public UsuarioAutenticado ObterUsuario(HttpContext contexto)
    {
        ClaimsPrincipal usuario = contexto.User;
        string? idUsuario = usuario.FindFirst(ClaimUsuarioAutenticado.Id)?.Value;
        string? tipoUsuario = usuario.FindFirst(ClaimUsuarioAutenticado.TipoUsuario)?.Value;
        List<string>? escopos = [.. usuario.FindAll(ClaimUsuarioAutenticado.Escopos).Select(c => c.Value)];
        return new UsuarioAutenticado
        {
            Id = Guid.TryParse(idUsuario, out var id) ? id : Guid.Empty,
            TipoUsuario = tipoUsuario ?? string.Empty,
            Escopos = escopos
        };
    }
}