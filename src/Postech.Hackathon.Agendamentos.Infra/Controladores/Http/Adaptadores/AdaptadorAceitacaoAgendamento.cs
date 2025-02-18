using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorAceitacaoAgendamento
{
    internal static ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento> Adaptar(AceitacaoAgendamentoSaida saida)
    {
        return saida.SituacaoAceitacaoAgendamento switch
        {
            SituacaoAceitacaoAgendamento.Sucesso => new()
            {
                Dados = new(),
                CodigoResposta = (int)HttpStatusCode.NoContent
            },
            SituacaoAceitacaoAgendamento.DadosInvalidos => new()
            {
                CodigoResposta = (int)HttpStatusCode.BadRequest,
                Mensagens = [new Notificacao() { Mensagem = "Uma ou mais propriedades estão inválidas", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoAceitacaoAgendamento.AceitacaoNaoPermitida => new()
            {
                CodigoResposta = (int)HttpStatusCode.Forbidden,
                Mensagens = [new Notificacao() { Mensagem = "Aceitação não autorizada", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoAceitacaoAgendamento.AgendamentoNaoEncontrado => new()
            {
                CodigoResposta = (int)HttpStatusCode.NotFound,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não foi encontrado", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoAceitacaoAgendamento.AceitacaoNaoProcessavel => new()
            {
                CodigoResposta = (int)HttpStatusCode.UnprocessableEntity,
                Mensagens = [new Notificacao() { Mensagem = "O agendamento não pode ser editado", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException()
        };
    }
}