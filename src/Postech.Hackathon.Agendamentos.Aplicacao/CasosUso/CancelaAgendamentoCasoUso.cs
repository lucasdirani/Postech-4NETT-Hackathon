using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas.Extensoes;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class CancelaAgendamentoCasoUso(IRepositorioAgendamento repositorio) : ICancelaAgendamentoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;

    public async Task<CancelaAgendamentoSaida> ExecutarAsync(CancelaAgendamentoEntrada entrada)
    {
        try
        {
            Agendamento? agendamento = await _repositorio.ObterPorIdAsync(entrada.IdAgendamento);
            if (agendamento is null) return new() { SituacaoCancelaAgendamento = SituacaoCancelaAgendamento.AgendamentoNaoEncontrado, Mensagem = "Agendamento inexistente" };
            if (!agendamento.PertencePaciente(entrada.IdPaciente)) return new() { SituacaoCancelaAgendamento = SituacaoCancelaAgendamento.CancelamentoNaoPermitido, Mensagem = "Cancelamento não permitido" };
            agendamento.CancelarAgendamento(entrada.DataCancelamento, entrada.JustificativaCancelamento);
            _repositorio.Atualizar(agendamento);
            await _repositorio.SalvarAlteracoesAsync();
            return new() { SituacaoCancelaAgendamento = SituacaoCancelaAgendamento.Sucesso };
        }
        catch (ExcecaoDominio ex)
        {
            return new() { SituacaoCancelaAgendamento = ex.ObterSituacaoCancelaAgendamento(), Mensagem = ex.Mensagem };
        }
    }
}