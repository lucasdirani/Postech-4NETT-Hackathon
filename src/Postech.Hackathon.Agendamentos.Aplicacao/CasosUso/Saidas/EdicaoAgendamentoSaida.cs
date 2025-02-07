using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;

[ExcludeFromCodeCoverage]
public record EdicaoAgendamentoSaida
{
    public SituacaoEdicaoAgendamento SituacaoEdicaoAgendamento { get; init; }
}