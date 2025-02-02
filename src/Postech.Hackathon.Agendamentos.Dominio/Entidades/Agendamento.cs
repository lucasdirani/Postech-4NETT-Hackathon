using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Dominio.Entidades;

public class Agendamento(
    Guid idMedico,
    DateOnly dataAgendamento,
    TimeSpan horarioInicioAgendamento,
    TimeSpan horarioFimAgendamento,
    DateOnly dataAtual) : EntidadeBase
{
    public Guid IdMedico { get; private set; } = idMedico.EstaVazio() 
        ? throw new ExcecaoDominio("O identificador do médico é obrigatório", nameof(Agendamento), nameof(IdMedico)) 
        : idMedico;
        
    public DateOnly Data { get; private set; } = dataAgendamento < dataAtual
        ? throw new ExcecaoDominio("A data de agendamento deve ser igual ou maior do que a data atual", nameof(Agendamento), nameof(Data))
        : dataAgendamento;

    public TimeSpan HorarioInicio { get; private set; } = horarioInicioAgendamento;
    public TimeSpan HorarioFim { get; private set; } = horarioFimAgendamento;
}