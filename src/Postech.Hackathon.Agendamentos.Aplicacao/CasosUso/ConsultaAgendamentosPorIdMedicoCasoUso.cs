using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Projecoes;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class ConsultaAgendamentosPorIdMedicoCasoUso(IRepositorioAgendamento repositorio) 
    : IConsultaAgendamentosPorIdMedicoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;

    public async Task<ConsultaAgendamentosPorIdMedicoSaida> ExecutarAsync(ConsultaAgendamentosPorIdMedicoEntrada entrada)
    {
        (IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico>, int) agendamentos = await _repositorio.ConsultarAgendamentosMedicoAsync(entrada.IdMedico, entrada.Pagina, entrada.TamanhoPagina);
        return new() {
            Agendamentos = agendamentos.Item1,
            QuantidadeItens = agendamentos.Item2,
            SituacaoConsultaAgendamentosPorIdMedico = SituacaoConsultaAgendamentosPorIdMedico.Sucesso
        };
    }
}