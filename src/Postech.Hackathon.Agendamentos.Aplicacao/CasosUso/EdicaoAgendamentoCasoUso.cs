using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class EdicaoAgendamentoCasoUso(IRepositorioAgendamento repositorio, IServicoAgendamento servicoAgendamento) 
    : IEdicaoAgendamentoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;
    private readonly IServicoAgendamento _servicoAgendamento = servicoAgendamento;

    public async Task<EdicaoAgendamentoSaida> ExecutarAsync(EdicaoAgendamentoEntrada entrada)
    {
        Agendamento? agendamento = await _repositorio.ObterPorIdAsync(entrada.IdAgendamento);
        agendamento.AlterarDataAgendamento(entrada.DataAgendamento, entrada.DataAtualizacao);
        agendamento.AlterarHorarioAgendamento(entrada.HorarioInicioAgendamento, entrada.HorarioFimAgendamento);
        agendamento.AlterarValorAgendamento(entrada.ValorAgendamento);
        IReadOnlyList<Agendamento> agendamentos = await _repositorio.ConsultarAgendamentosMedicoAsync(entrada.IdMedico, entrada.DataAgendamento);
        _ = _servicoAgendamento.ValidarConflitoHorarioAgendamento(agendamentos, entrada.HorarioInicioAgendamento, entrada.HorarioFimAgendamento);
        _repositorio.Atualizar(agendamento);
        await _repositorio.SalvarAlteracoesAsync();
        return new() { SituacaoEdicaoAgendamento = SituacaoEdicaoAgendamento.Sucesso };
    }
}