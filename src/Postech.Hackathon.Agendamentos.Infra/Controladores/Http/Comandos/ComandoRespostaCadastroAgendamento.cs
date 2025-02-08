using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRespostaCadastroAgendamento : IComandoResposta
{
    [JsonPropertyName("idAgendamento")]
    public Guid IdAgendamento { get; init; }
}