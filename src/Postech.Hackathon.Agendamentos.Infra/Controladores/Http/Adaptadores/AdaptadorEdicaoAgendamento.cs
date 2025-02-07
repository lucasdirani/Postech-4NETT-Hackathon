using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
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
            _ => throw new NotImplementedException()
        };
    }
}