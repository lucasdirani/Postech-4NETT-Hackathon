using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;

[ExcludeFromCodeCoverage]
public record CadastroAgendamentoSaida
{
    public Guid IdAgendamento { get; init; }
    public SituacaoCadastroAgendamento SituacaoCadastroAgendamento { get; init; }
    public string? Mensagem { get; init; }
}