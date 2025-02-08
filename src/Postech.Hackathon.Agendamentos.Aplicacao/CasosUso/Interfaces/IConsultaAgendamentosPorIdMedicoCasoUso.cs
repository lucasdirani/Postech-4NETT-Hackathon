using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;

public interface IConsultaAgendamentosPorIdMedicoCasoUso
{
    Task<ConsultaAgendamentosPorIdMedicoSaida> ExecutarAsync(ConsultaAgendamentosPorIdMedicoEntrada entrada);
}