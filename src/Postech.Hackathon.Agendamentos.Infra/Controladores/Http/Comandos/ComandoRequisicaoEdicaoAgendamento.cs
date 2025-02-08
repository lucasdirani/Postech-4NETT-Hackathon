using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRequisicaoEdicaoAgendamento
{
    [JsonPropertyName("idMedico")]
    public Guid IdMedico { get; init; }

    [JsonPropertyName("data")]
    public DateOnly Data { get; init; }

    [JsonIgnore]
    public DateOnly DataAtualizacao { get; init; } = DateOnly.FromDateTime(DateTime.Today);

    [JsonPropertyName("horaInicio")]
    public TimeSpan HoraInicio { get; init; }

    [JsonPropertyName("horaFim")]
    public TimeSpan HoraFim { get; init; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; init; }

    internal EdicaoAgendamentoEntrada ConverterParaEdicaoAgendamentoEntrada(Guid idAgendamento)
    {
        return new()
        {
            DataAgendamento = Data,
            DataAtualizacao = DataAtualizacao,
            HorarioFimAgendamento = HoraFim,
            HorarioInicioAgendamento = HoraInicio,
            IdAgendamento = idAgendamento,
            IdMedico = IdMedico,
            ValorAgendamento = Valor
        };
    }
}