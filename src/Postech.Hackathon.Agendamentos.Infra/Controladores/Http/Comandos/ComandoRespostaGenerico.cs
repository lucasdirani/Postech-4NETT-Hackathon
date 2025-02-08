using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Aplicacao.Notificacoes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRespostaGenerico<T> where T : IComandoResposta
{
    [JsonPropertyName("dados")]
    public T? Dados { get; init; }

    [JsonPropertyName("mensagens")]
    public IEnumerable<Notificacao>? Mensagens { get; init; }

    [JsonPropertyName("foiProcessadoComSucesso")]
    public bool FoiProcessadoComSucesso => Dados is not null;

    [JsonIgnore]
    public int CodigoResposta { get; init; }
}