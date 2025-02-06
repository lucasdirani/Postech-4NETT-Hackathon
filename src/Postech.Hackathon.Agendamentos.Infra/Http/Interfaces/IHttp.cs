using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Interfaces;

public interface IHttp
{
    void On<TRequisicao, TResposta>(string metodo, string url, Func<TRequisicao?, IDictionary<string, object?>, IServiceProvider, Task<TResposta>> callback) where TResposta : IComandoResposta;
    void Executar();
}