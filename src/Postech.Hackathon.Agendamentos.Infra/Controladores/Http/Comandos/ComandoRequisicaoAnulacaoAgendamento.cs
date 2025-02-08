using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRequisicaoAnulacaoAgendamento
{
    [JsonPropertyName("acao")]
    public required string Acao { get; init; }

    [JsonPropertyName("idUsuario")]
    public Guid IdUsuario { get; init; }

    [JsonPropertyName("justificativa")]
    public string Justificativa { get; init; } = string.Empty;

    [JsonIgnore]
    public DateOnly DataAnulacao { get; init; } = DateOnly.FromDateTime(DateTime.Today);

    internal RecusaAgendamentoEntrada ConverterParaRecusaAgendamentoEntrada(Guid idAgendamento)
    {
        return new()
        {
            DataRecusa = DataAnulacao,
            IdAgendamento = idAgendamento,
            IdMedico = IdUsuario,
            JustificativaRecusa = Justificativa
        };
    }

    internal CancelaAgendamentoEntrada ConverterParaCancelaAgendamentoEntrada(Guid idAgendamento)
    {
        return new()
        {
            DataCancelamento = DataAnulacao,
            IdAgendamento = idAgendamento,
            IdPaciente = IdUsuario,
            JustificativaCancelamento = Justificativa
        };
    }
}