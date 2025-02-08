using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record EdicaoAgendamentoEntrada
{
    public Guid IdAgendamento { get; init; }
    public Guid IdMedico { get; init; }
    public DateOnly DataAtualizacao { get; init; }
    public DateOnly DataAgendamento { get; init; }
    public TimeSpan HorarioInicioAgendamento { get; init; }
    public TimeSpan HorarioFimAgendamento { get; init; }
    public decimal ValorAgendamento { get; init; }
}