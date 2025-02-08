using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorCancelaAgendamento
{
    internal static ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento> Adaptar(CancelaAgendamentoSaida saida)
    {
        return saida.SituacaoCancelaAgendamento switch
        {
            SituacaoCancelaAgendamento.Sucesso => new()
            {
                Dados = new(),
                CodigoResposta = (int)HttpStatusCode.NoContent
            },
            SituacaoCancelaAgendamento.DadosInvalidos => new()
            {
                CodigoResposta = (int)HttpStatusCode.BadRequest,
                Mensagens = [new Notificacao() { Mensagem = "Uma ou mais propriedades estão inválidas", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoCancelaAgendamento.CancelamentoNaoPermitido => new()
            {
                CodigoResposta = (int)HttpStatusCode.Forbidden,
                Mensagens = [new Notificacao() { Mensagem = "Cancelamento não autorizado", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoCancelaAgendamento.AgendamentoNaoEncontrado => new()
            {
                CodigoResposta = (int)HttpStatusCode.NotFound,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não foi encontrado", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoCancelaAgendamento.CancelamentoNaoProcessavel => new()
            {
                CodigoResposta = (int)HttpStatusCode.UnprocessableEntity,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não pode ser cancelado", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException()
        };
    }
}