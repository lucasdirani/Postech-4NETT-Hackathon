using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;

[ExcludeFromCodeCoverage]
public record CadastroAgendamentoSaida
{
    public Guid IdAgendamento { get; init; }
}