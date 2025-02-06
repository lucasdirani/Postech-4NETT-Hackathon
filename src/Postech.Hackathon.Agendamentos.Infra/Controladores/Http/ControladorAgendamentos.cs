using Microsoft.Extensions.DependencyInjection;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Http.Interfaces;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http;

public class ControladorAgendamentos
{
    public ControladorAgendamentos(IHttp http)
    {
        http.On<ComandoRequisicaoCadastroAgendamento, ComandoRespostaCadastroAgendamento>(HttpMethod.Post.ToString(), "/agendamentos", async (corpo, valoresRota, serviceProvider) =>
        {
            ICadastroAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<ICadastroAgendamentoCasoUso>();
            CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaCadastroAgendamentoEntrada());
            return new() { IdAgendamento = saida.IdAgendamento };
        });
    }
}