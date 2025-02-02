namespace Postech.Hackathon.Agendamentos.Dominio.Entidades;

public class Agendamento : EntidadeBase
{
    public Agendamento(
        Guid idMedico, 
        DateOnly dataAgendamento, 
        TimeSpan horarioInicioAgendamento, 
        TimeSpan horarioFimAgendamento)
    {
        IdMedico = idMedico;
        Data = dataAgendamento;
        HorarioInicio = horarioInicioAgendamento;
        HorarioFim = horarioFimAgendamento;
    }

    public Guid IdMedico { get; private set; }
    public DateOnly Data { get; private set; }
    public TimeSpan HorarioInicio { get; private set; }
    public TimeSpan HorarioFim { get; private set; }
}