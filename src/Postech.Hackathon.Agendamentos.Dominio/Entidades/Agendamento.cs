using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Dominio.Entidades;

public class Agendamento : EntidadeBase
{
    public Agendamento(
        Guid idMedico,
        DateOnly dataAgendamento,
        TimeSpan horarioInicioAgendamento,
        TimeSpan horarioFimAgendamento,
        DateOnly dataAtual)
    {
        if (idMedico.EstaVazio())
        {
            throw new ExcecaoDominio("O identificador do médico é obrigatório", nameof(Agendamento), nameof(IdMedico));
        }
        if (dataAgendamento <= dataAtual)
        {
            throw new ExcecaoDominio("A data de agendamento deve ser maior do que a data atual", nameof(Agendamento), nameof(Data));
        }
        if (horarioInicioAgendamento >= horarioFimAgendamento)
        {
            throw new ExcecaoDominio("O horário de início deve ser menor do que o horário de fim", nameof(Agendamento), nameof(HorarioInicio));
        }
        IdMedico = idMedico;
        Data = dataAgendamento;
        HorarioInicio = horarioInicioAgendamento;
        HorarioFim = horarioFimAgendamento;
    }

    private Agendamento() {}

    public Guid IdMedico { get; private set; }       
    public DateOnly Data { get; private set; }
    public TimeSpan HorarioInicio { get; private set; }
    public TimeSpan HorarioFim { get; private set; }
}