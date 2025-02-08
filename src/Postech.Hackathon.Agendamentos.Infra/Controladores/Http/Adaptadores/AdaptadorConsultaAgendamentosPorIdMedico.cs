using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
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
            _ => throw new NotImplementedException(),
        };
    }
}