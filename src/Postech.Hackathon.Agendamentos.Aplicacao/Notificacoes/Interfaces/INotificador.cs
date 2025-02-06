using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;

namespace Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Interfaces;

public interface INotificador
{
    bool PossuiNotificacao();
    bool PossuiNotificacao(TipoNotificacao tipoNotificacao);
    IEnumerable<Notificacao> ObterNotificacoes();
    void Processar(Notificacao notificacao);
    void Processar(IEnumerable<Notificacao> notificacoes);
}