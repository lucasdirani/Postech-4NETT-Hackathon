using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class CadastroAgendamentoCasoUso(IRepositorioAgendamento repositorio, IServicoAgendamento servicoAgendamento) 
    : ICadastroAgendamentoCasoUso
{
    private IRepositorioAgendamento _repositorio = repositorio;
    private IServicoAgendamento _servicoAgendamento = servicoAgendamento;

    public async Task<CadastroAgendamentoSaida> ExecutarAsync(CadastroAgendamentoEntrada entrada)
    {
        IReadOnlyList<Agendamento> agendamentos = await _repositorio.ConsultarAgendamentosMedicoAsync(entrada.IdMedico, entrada.DataAgendamento);
        _ = _servicoAgendamento.ValidarConflitoHorarioAgendamento(agendamentos, entrada.HorarioInicioAgendamento, entrada.HorarioFimAgendamento);
        Agendamento agendamento = entrada.ConverterParaAgendamento();
        await _repositorio.InserirAsync(agendamento);
        await _repositorio.SalvarAlteracoesAsync();
        return new() { IdAgendamento = agendamento.Id };
    }
}