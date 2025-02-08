using Microsoft.Extensions.Primitives;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Modelos;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Interfaces;

public interface IHttp
{
    void On<TRequisicao, TResposta>(string metodo, string url, Func<TRequisicao?, IDictionary<string, object?>, IDictionary<string, StringValues>, UsuarioAutenticado, IServiceProvider, Task<ComandoRespostaGenerico<TResposta>>> callback) where TResposta : IComandoResposta;
    void Executar();
}