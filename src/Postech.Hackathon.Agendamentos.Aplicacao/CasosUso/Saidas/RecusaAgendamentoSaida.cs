using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;

[ExcludeFromCodeCoverage]
public record RecusaAgendamentoSaida
{
    public SituacaoRecusaAgendamento SituacaoRecusaAgendamento { get; init; }
    public string? Mensagem { get; init; }
}