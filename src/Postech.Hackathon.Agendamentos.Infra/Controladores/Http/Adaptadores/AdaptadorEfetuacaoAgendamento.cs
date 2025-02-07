using System.Diagnostics.CodeAnalysis;
using System.Net;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
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
            _ => throw new NotImplementedException()
        };
    }
}