using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public static class AdaptadorConsultaAgendamentosPorIdMedico
{
    public static ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico> Adaptar(ConsultaAgendamentosPorIdMedicoSaida saida)
    {
        return saida.SituacaoConsultaAgendamentosPorIdMedico switch
        {
            SituacaoConsultaAgendamentosPorIdMedico.Sucesso => new()
            {
                Dados = new ComandoRespostaConsultaAgendamentosPorIdMedico() { 
                    Agendamentos = saida.Agendamentos,
                    PaginaAtual = saida.PaginaAtual,
                    QuantidadeItens = saida.QuantidadeItens,
                    TamanhoPagina = saida.TamanhoPagina,
                    TotalPaginas = saida.TotalPaginas,
                },
                CodigoResposta = (int)HttpStatusCode.OK
            },
            SituacaoConsultaAgendamentosPorIdMedico.DadosInvalidos => new()
            {
                CodigoResposta = (int)HttpStatusCode.BadRequest,
                Mensagens = [new Notificacao() { Mensagem = "Uma ou mais propriedades estão inválidas", Tipo = TipoNotificacao.Erro }]
            },
            SituacaoConsultaAgendamentosPorIdMedico.AgendamentoNaoEncontrado => new()
            {
                CodigoResposta = (int)HttpStatusCode.NotFound,
                Mensagens = [new Notificacao() { Mensagem = "Os agendamentos não foram encontrados", Tipo = TipoNotificacao.Erro }]
            },
            _ => throw new NotImplementedException(),
        };
    }
}