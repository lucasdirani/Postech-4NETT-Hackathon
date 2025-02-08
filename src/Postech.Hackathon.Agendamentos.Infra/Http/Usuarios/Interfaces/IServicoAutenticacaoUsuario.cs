using Microsoft.AspNetCore.Http;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Modelos;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Interfaces;

public interface IServicoAutenticacaoUsuario
{
    UsuarioAutenticado ObterUsuario(HttpContext contexto);
}