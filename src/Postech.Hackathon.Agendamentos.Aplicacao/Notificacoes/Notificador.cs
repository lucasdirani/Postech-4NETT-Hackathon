using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;

namespace Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes;

[ExcludeFromCodeCoverage]
public class Notificador : INotificador
{
    private readonly List<Notificacao> _notificacoes = [];

    public IEnumerable<Notificacao> ObterNotificacoes()
    {
        return _notificacoes.AsEnumerable();
    }

    public void Processar(Notificacao notificacao)
    {
        _notificacoes.Add(notificacao);
    }

    public void Processar(IEnumerable<Notificacao> notificacoes)
    {
        foreach (Notificacao notificacao in notificacoes)
        {
            Processar(notificacao);
        }
    }

    public bool PossuiNotificacao()
    {
        return _notificacoes.Count != 0;
    }

    public bool PossuiNotificacao(TipoNotificacao notificationType)
    {
        return _notificacoes.Exists(notification => notification.Tipo.Equals(notificationType));
    }
}