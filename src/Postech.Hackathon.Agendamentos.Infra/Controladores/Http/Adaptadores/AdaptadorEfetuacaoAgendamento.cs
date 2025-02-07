using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorEfetuacaoAgendamento
{
    internal static ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento> Adaptar(EfetuacaoAgendamentoSaida saida)
    {
        return saida.SituacaoEfetuacaoAgendamento switch
        {
            SituacaoEfetuacaoAgendamento.Sucesso => new()
            {
                Dados = new(),
                CodigoResposta = (int)HttpStatusCode.NoContent
            },
            SituacaoEfetuacaoAgendamento.DadosInvalidos => new()
            {
                CodigoResposta = (int)HttpStatusCode.BadRequest,
                Mensagens = [new Notificacao() { Mensagem = "Uma ou mais propriedades estão inválidas", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoEfetuacaoAgendamento.AgendamentoNaoEncontrado => new()
            {
                CodigoResposta = (int)HttpStatusCode.NotFound,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não foi encontrado", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoEfetuacaoAgendamento.Conflito => new()
            {
                CodigoResposta = (int)HttpStatusCode.Conflict,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento está em conflito", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException()
        };
    }
}