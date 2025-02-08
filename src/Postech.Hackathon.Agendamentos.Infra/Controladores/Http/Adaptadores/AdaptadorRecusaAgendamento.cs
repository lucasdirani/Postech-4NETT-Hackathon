using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorRecusaAgendamento
{
    internal static ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento> Adaptar(RecusaAgendamentoSaida saida)
    {
        return saida.SituacaoRecusaAgendamento switch
        {
            SituacaoRecusaAgendamento.Sucesso => new()
            {
                Dados = new(),
                CodigoResposta = (int)HttpStatusCode.NoContent
            },
            SituacaoRecusaAgendamento.DadosInvalidos => new()
            {
                CodigoResposta = (int)HttpStatusCode.BadRequest,
                Mensagens = [new Notificacao() { Mensagem = "Uma ou mais propriedades estão inválidas", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoRecusaAgendamento.RecusaNaoPermitida => new()
            {
                CodigoResposta = (int)HttpStatusCode.Forbidden,
                Mensagens = [new Notificacao() { Mensagem = "Recusa não autorizada", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoRecusaAgendamento.AgendamentoNaoEncontrado => new()
            {
                CodigoResposta = (int)HttpStatusCode.NotFound,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não foi encontrado", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoRecusaAgendamento.RecusaNaoProcessavel => new()
            {
                CodigoResposta = (int)HttpStatusCode.UnprocessableEntity,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não pode ser recusado", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException()
        };
    }
}