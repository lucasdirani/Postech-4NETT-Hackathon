using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Dominio.Projecoes;

[ExcludeFromCodeCoverage]
public record ProjecaoConsultaAgendamentosPorIdMedico
{
    public Guid IdAgendamento { get; init; }
    public required string Situacao { get; init; }
    public DateOnly Data { get; init; }
    public TimeSpan HoraInicio { get; init; }
    public TimeSpan HoraFim { get; init; }
}