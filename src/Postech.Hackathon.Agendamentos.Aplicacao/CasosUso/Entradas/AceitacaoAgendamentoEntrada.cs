using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record AceitacaoAgendamentoEntrada
{
    public Guid IdAgendamento { get; init; }
    public Guid IdMedico { get; init; }
    public DateOnly DataAceitacao { get; init; }
}