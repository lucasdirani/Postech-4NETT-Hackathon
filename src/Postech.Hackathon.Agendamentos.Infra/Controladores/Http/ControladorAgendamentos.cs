using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Adaptadores;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Constantes;
using Postech.Hackathon.Agendamentos.Infra.Http.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Requisicoes.Extensoes;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Constantes;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http;

public class ControladorAgendamentos
{
    public ControladorAgendamentos(IHttp http)
    {
        http.On<ComandoRequisicaoCadastroAgendamento, ComandoRespostaCadastroAgendamento>(HttpMethod.Post.ToString(), "/agendamentos", async (corpo, valoresRota, valoresConsulta, usuarioAutenticado, serviceProvider) =>
        {
            INotificador notificador = serviceProvider.GetRequiredService<INotificador>();
            if (corpo is null)
            {
                notificador.Processar(new Notificacao() { Mensagem = "Não foi possível ler o corpo da requisição", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.BadRequest };
            }
            if (!usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.CadastroAgendamento))
            {
                notificador.Processar(new Notificacao() { Mensagem = "Usuário não autorizado", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.Forbidden };
            }
            ICadastroAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<ICadastroAgendamentoCasoUso>();
            CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaCadastroAgendamentoEntrada());
            return AdaptadorCadastroAgendamento.Adaptar(saida);
        });
        http.On<ComandoRequisicaoEdicaoAgendamento, ComandoRespostaEdicaoAgendamento>(HttpMethod.Put.ToString(), "/agendamentos/{idAgendamento}", async (corpo, valoresRota, valoresConsulta, usuarioAutenticado, serviceProvider) =>
        {
            INotificador notificador = serviceProvider.GetRequiredService<INotificador>();
            if (corpo is null)
            {
                notificador.Processar(new Notificacao() { Mensagem = "Não foi possível ler o corpo da requisição", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.BadRequest };
            }
            if (!usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.EditaAgendamento))
            {
                notificador.Processar(new Notificacao() { Mensagem = "Usuário não autorizado", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.Forbidden };
            }
            IEdicaoAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<IEdicaoAgendamentoCasoUso>();
            _ = Guid.TryParse(valoresRota["idAgendamento"]?.ToString(), out Guid idAgendamento);
            EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaEdicaoAgendamentoEntrada(idAgendamento));
            return AdaptadorEdicaoAgendamento.Adaptar(saida);
        });
        http.On<ComandoRequisicaoConfirmacaoAgendamento, ComandoRespostaConfirmacaoAgendamento>(HttpMethod.Patch.ToString(), "/agendamentos/{idAgendamento}", async (corpo, valoresRota, valoresConsulta, usuarioAutenticado, serviceProvider) =>
        {
            INotificador notificador = serviceProvider.GetRequiredService<INotificador>();
            if (corpo is null)
            {
                notificador.Processar(new Notificacao() { Mensagem = "Não foi possível ler o corpo da requisição", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.BadRequest };
            }
            _ = Guid.TryParse(valoresRota["idAgendamento"]?.ToString(), out Guid idAgendamento);
            if (corpo.Acao.Equals(AcaoConfirmacaoAgendamento.Aceitar) && usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.AceitaAgendamento))
            {
                IAceitacaoAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<IAceitacaoAgendamentoCasoUso>();
                AceitacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaAceitacaoAgendamentoEntrada(idAgendamento));
                return AdaptadorAceitacaoAgendamento.Adaptar(saida);
            }
            if (corpo.Acao.Equals(AcaoConfirmacaoAgendamento.Efetuar) && usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.EfetuaAgendamento))
            {
                IEfetuacaoAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<IEfetuacaoAgendamentoCasoUso>();
                EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaEfetuacaoAgendamentoEntrada(idAgendamento));
                return AdaptadorEfetuacaoAgendamento.Adaptar(saida);
            }
            return new() { CodigoResposta = (int) HttpStatusCode.Forbidden };
        });
        http.On<ComandoRequisicaoConsultaAgendamentosPorIdMedico, ComandoRespostaConsultaAgendamentosPorIdMedico>(HttpMethod.Get.ToString(), "/agendamentos", async (corpo, valoresRota, valoresConsulta, usuarioAutenticado, serviceProvider) =>
        {
            if (!usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.ConsultaAgendamento))
            {
                INotificador notificador = serviceProvider.GetRequiredService<INotificador>();
                notificador.Processar(new Notificacao() { Mensagem = "Usuário não autorizado", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.Forbidden };
            }
            IConsultaAgendamentosPorIdMedicoCasoUso casoUso = serviceProvider.GetRequiredService<IConsultaAgendamentosPorIdMedicoCasoUso>();
            ConsultaAgendamentosPorIdMedicoSaida saida = await casoUso.ExecutarAsync(valoresConsulta.ConverterParaCadastroAgendamentoEntrada());
            return AdaptadorConsultaAgendamentosPorIdMedico.Adaptar(saida);
        });
        http.On<ComandoRequisicaoAnulacaoAgendamento, ComandoRespostaAnulacaoAgendamento>(HttpMethod.Delete.ToString(), "/agendamentos/{idAgendamento}", async (corpo, valoresRota, valoresConsulta, usuarioAutenticado, serviceProvider) =>
        {
            INotificador notificador = serviceProvider.GetRequiredService<INotificador>();
            if (corpo is null)
            {
                notificador.Processar(new Notificacao() { Mensagem = "Não foi possível ler o corpo da requisição", Tipo = TipoNotificacao.Erro });
                return new() { Mensagens = notificador.ObterNotificacoes(), CodigoResposta = (int) HttpStatusCode.BadRequest };
            }
            _ = Guid.TryParse(valoresRota["idAgendamento"]?.ToString(), out Guid idAgendamento);
            if (corpo.Acao.Equals(AcaoAnulacaoAgendamento.Recusar) && usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.RecusaAgendamento))
            {
                IRecusaAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<IRecusaAgendamentoCasoUso>();
                RecusaAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaRecusaAgendamentoEntrada(idAgendamento));
                return AdaptadorRecusaAgendamento.Adaptar(saida);
            }
            if (corpo.Acao.Equals(AcaoAnulacaoAgendamento.Cancelar) && usuarioAutenticado.PossuiEscopo(EscopoUsuarioAutenticado.CancelaAgendamento))
            {
                ICancelaAgendamentoCasoUso casoUso = serviceProvider.GetRequiredService<ICancelaAgendamentoCasoUso>();
                CancelaAgendamentoSaida saida = await casoUso.ExecutarAsync(corpo.ConverterParaCancelaAgendamentoEntrada(idAgendamento));
                return AdaptadorCancelaAgendamento.Adaptar(saida);
            }
            return new() { CodigoResposta = (int) HttpStatusCode.Forbidden };
        });
    }
}