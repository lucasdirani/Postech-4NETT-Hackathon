using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class EfetuacaoAgendamentoCasoUso(IRepositorioAgendamento repositorio, IServicoAgendamento servicoAgendamento) 
    : IEfetuacaoAgendamentoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;
    private readonly IServicoAgendamento _servicoAgendamento = servicoAgendamento;

    public async Task<EfetuacaoAgendamentoSaida> ExecutarAsync(EfetuacaoAgendamentoEntrada entrada)
    {
        Agendamento? agendamento = await _repositorio.ObterPorIdAsync(entrada.IdAgendamento);
        if (agendamento is null) return new() { SituacaoEfetuacaoAgendamento = SituacaoEfetuacaoAgendamento.AgendamentoNaoEncontrado, Mensagem = "Agendamento inexistente" };
        IReadOnlyList<Agendamento> agendamentos = await _repositorio.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(entrada.IdPaciente, agendamento.Data);
        _ = _servicoAgendamento.ValidarConflitoHorarioAgendamento(agendamentos, agendamento.HorarioInicio, agendamento.HorarioFim);
        agendamento.EfetuarAgendamento(entrada.IdPaciente, entrada.DataEfetuacao);
        _repositorio.Atualizar(agendamento);
        await _repositorio.SalvarAlteracoesAsync();
        return new() { SituacaoEfetuacaoAgendamento = SituacaoEfetuacaoAgendamento.Sucesso };
    }
}