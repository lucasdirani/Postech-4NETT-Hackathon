using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record EfetuacaoAgendamentoEntrada
{
    public Guid IdAgendamento { get; init; }
    public Guid IdPaciente { get; init; }
    public DateOnly DataEfetuacao { get; init; }
}