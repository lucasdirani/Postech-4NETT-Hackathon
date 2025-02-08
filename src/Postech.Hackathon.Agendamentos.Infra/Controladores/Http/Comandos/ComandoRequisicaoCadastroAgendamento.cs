using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRequisicaoCadastroAgendamento
{
    [JsonPropertyName("idMedico")]
    public Guid IdMedico { get; init; }

    [JsonPropertyName("data")]
    public DateOnly Data { get; init; }

    [JsonPropertyName("horaInicio")]
    public TimeSpan HoraInicio { get; init; }

    [JsonPropertyName("horaFim")]
    public TimeSpan HoraFim { get; init; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; init; }

    [JsonIgnore]
    public DateOnly DataAtual { get; init; } = DateOnly.FromDateTime(DateTime.Today);

    internal CadastroAgendamentoEntrada ConverterParaCadastroAgendamentoEntrada()
    {
        return new()
        {
            DataAgendamento = Data,
            DataAtual = DataAtual,
            HorarioFimAgendamento = HoraFim,
            HorarioInicioAgendamento = HoraInicio,
            IdMedico = IdMedico,
            ValorAgendamento = Valor
        };
    }
}