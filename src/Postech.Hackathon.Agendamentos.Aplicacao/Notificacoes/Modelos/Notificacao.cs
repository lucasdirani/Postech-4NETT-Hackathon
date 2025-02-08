using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Enumeradores;

namespace Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;

[ExcludeFromCodeCoverage]
public record Notificacao
{
    [JsonPropertyName("mensagem")]
    public required string Mensagem { get; init; }

    [JsonPropertyName("tipo")]
    public TipoNotificacao Tipo { get; init; }
}