using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Clientes.Modelos;

[ExcludeFromCodeCoverage]
public record RequisicaoValidacaoToken
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;
}