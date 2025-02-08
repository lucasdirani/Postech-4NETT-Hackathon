using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record RecusaAgendamentoEntrada
{
    public Guid IdAgendamento { get; init; }
    public Guid IdMedico { get; init; }
    public DateOnly DataRecusa { get; init; }
    public required string JustificativaRecusa { get; init; }
}