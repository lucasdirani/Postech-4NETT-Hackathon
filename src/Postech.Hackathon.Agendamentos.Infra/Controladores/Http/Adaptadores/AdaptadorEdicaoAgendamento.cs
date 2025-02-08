using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorEdicaoAgendamento
{
    internal static ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento> Adaptar(EdicaoAgendamentoSaida saida)
    {
        return saida.SituacaoEdicaoAgendamento switch
        {
            SituacaoEdicaoAgendamento.Sucesso => new()
            {
                Dados = new(),
                CodigoResposta = (int)HttpStatusCode.NoContent
            },
            SituacaoEdicaoAgendamento.DadosInvalidos => new()
            {
                CodigoResposta = (int)HttpStatusCode.BadRequest,
                Mensagens = [new Notificacao() { Mensagem = "Uma ou mais propriedades estão inválidas", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoEdicaoAgendamento.EdicaoNaoPermitida => new()
            {
                CodigoResposta = (int)HttpStatusCode.Forbidden,
                Mensagens = [new Notificacao() { Mensagem = "Edição não autorizada", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoEdicaoAgendamento.AgendamentoNaoEncontrado => new()
            {
                CodigoResposta = (int)HttpStatusCode.NotFound,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não foi encontrado", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoEdicaoAgendamento.Conflito => new()
            {
                CodigoResposta = (int)HttpStatusCode.Conflict,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento está em conflito", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoEdicaoAgendamento.EdicaoNaoProcessavel => new()
            {
                CodigoResposta = (int)HttpStatusCode.UnprocessableEntity,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não pode ser editado", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException()
        };
    }
}