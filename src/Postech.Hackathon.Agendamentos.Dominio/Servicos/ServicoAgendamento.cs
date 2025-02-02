using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Dominio.Servicos;

public class ServicoAgendamento : IServicoAgendamento
{
    public bool ValidarConflitoHorarioAgendamento(
        IReadOnlyList<Agendamento> agendamentos, 
        TimeSpan horarioInicioNovoAgendamento, 
        TimeSpan horarioFimNovoAgendamento)
    {
        return agendamentos.Any(a => horarioInicioNovoAgendamento < a.HorarioFim && horarioFimNovoAgendamento > a.HorarioInicio);
    }
}