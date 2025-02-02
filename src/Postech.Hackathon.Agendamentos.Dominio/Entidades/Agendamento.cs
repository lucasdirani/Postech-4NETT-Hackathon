using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Dominio.Entidades;

public class Agendamento(
    Guid idMedico,
    DateOnly dataAgendamento,
    TimeSpan horarioInicioAgendamento,
    TimeSpan horarioFimAgendamento) : EntidadeBase
{
    public Guid IdMedico { get; private set; } = idMedico.EstaVazio() 
        ? throw new ExcecaoDominio("O identificador do médico é obrigatório", nameof(Agendamento), nameof(IdMedico)) 
        : idMedico;
        
    public DateOnly Data { get; private set; } = dataAgendamento;
    public TimeSpan HorarioInicio { get; private set; } = horarioInicioAgendamento;
    public TimeSpan HorarioFim { get; private set; } = horarioFimAgendamento;
}