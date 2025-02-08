using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Constantes;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Modelos;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Usuarios;

[ExcludeFromCodeCoverage]
public class ServicoAutenticacaoUsuario : IServicoAutenticacaoUsuario
{
    public UsuarioAutenticado ObterUsuario(HttpContext contexto)
    {
        _ = contexto.Request.Headers.TryGetValue("Authorization", out StringValues authorization); 
        string? token = authorization.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(token)) return new();
        JwtSecurityTokenHandler handler = new();
        if (!handler.CanReadToken(token)) return new();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        List<Claim> claims = jwtToken.Claims.ToList();
        string? idUsuario = claims.FirstOrDefault(c => c.Type == ClaimUsuarioAutenticado.Id)?.Value;
        string? tipoUsuario = claims.FirstOrDefault(c => c.Type == ClaimUsuarioAutenticado.TipoUsuario)?.Value;
        List<string>? escopos = [.. claims.Where(c => c.Type == ClaimUsuarioAutenticado.Escopos).Select(c => c.Value)];
        return new UsuarioAutenticado
        {
            Id = Guid.TryParse(idUsuario, out var id) ? id : Guid.Empty,
            TipoUsuario = tipoUsuario ?? string.Empty,
            Escopos = escopos
        };
    }
}