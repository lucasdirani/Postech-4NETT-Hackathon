using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Http.Interfaces;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http;

public class ControladorAgendamentos
{
    public ControladorAgendamentos(IHttp http)
    {
        http.On<ComandoRequisicaoCadastroAgendamento, ComandoRespostaCadastroAgendamento>(HttpMethod.Post.ToString(), "/agendamentos", async (corpo, valoresRota, serviceProvider) =>
        {
            INotificador notificador = serviceProvider.GetRequiredService<INotificador>();
            if (corpo is null)
            {
                notificador.Processar(new Notificacao() { Mensagem = "Não foi possível ler o corpo da requisição", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.BadRequest };
            }
            ICadastroAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<ICadastroAgendamentoCasoUso>();
            CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaCadastroAgendamentoEntrada());
            return AdaptadorCadastroAgendamento.Adaptar(saida);
        });
    }
}