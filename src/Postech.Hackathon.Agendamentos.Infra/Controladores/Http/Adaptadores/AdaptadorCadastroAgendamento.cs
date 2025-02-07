using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorCadastroAgendamento
{
    public static ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento> Adaptar(CadastroAgendamentoSaida saida)
    {
        return saida.SituacaoCadastroAgendamento switch
        {
            SituacaoCadastroAgendamento.Sucesso => new()
            {
                Dados = new ComandoRespostaCadastroAgendamento() { IdAgendamento = saida.IdAgendamento },
                CodigoResposta = (int)HttpStatusCode.Created
            },
            SituacaoCadastroAgendamento.Conflito => new()
            {
                CodigoResposta = (int)HttpStatusCode.Conflict,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento está em conflito", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException(),
        };
    }
}