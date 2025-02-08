using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record CancelaAgendamentoEntrada
{
    public Guid IdAgendamento { get; init; }
    public Guid IdPaciente { get; init; }
    public DateOnly DataCancelamento { get; init; }
    public required string JustificativaCancelamento { get; init; }
}