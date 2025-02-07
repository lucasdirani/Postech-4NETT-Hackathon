using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Dominio.Projecoes;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;

[ExcludeFromCodeCoverage]
public record ConsultaAgendamentosPorIdMedicoSaida
{
    public IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico> Agendamentos { get; init; } = [];
    public SituacaoConsultaAgendamentosPorIdMedico SituacaoConsultaAgendamentosPorIdMedico { get; init; }
    public int QuantidadeItens { get; init; }
}