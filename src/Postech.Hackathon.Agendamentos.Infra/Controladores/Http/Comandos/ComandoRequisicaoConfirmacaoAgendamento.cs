using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRequisicaoConfirmacaoAgendamento
{
    [JsonPropertyName("acao")]
    public required string Acao { get; init; }

    [JsonPropertyName("idUsuario")]
    public Guid IdUsuario { get; init; }

    [JsonIgnore]
    public DateOnly DataConfirmacao { get; init; } = DateOnly.FromDateTime(DateTime.Today);

    internal AceitacaoAgendamentoEntrada ConverterParaAceitacaoAgendamentoEntrada(Guid idAgendamento)
    {
        return new()
        {
            DataAceitacao = DataConfirmacao,
            IdAgendamento = idAgendamento,
            IdMedico = IdUsuario
        };
    }

    internal EfetuacaoAgendamentoEntrada ConverterParaEfetuacaoAgendamentoEntrada(Guid idAgendamento)
    {
        return new()
        {
            DataEfetuacao = DataConfirmacao,
            IdAgendamento = idAgendamento,
            IdPaciente = IdUsuario
        };
    }
}