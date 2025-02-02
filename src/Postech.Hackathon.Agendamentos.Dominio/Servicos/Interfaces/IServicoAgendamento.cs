using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

public interface IServicoAgendamento
{
    bool ValidarConflitoHorarioAgendamento(IReadOnlyList<Agendamento> agendamentos, TimeSpan horarioInicioNovoAgendamento, TimeSpan horarioFimNovoAgendamento);
}