using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Serializadores.Resultados;

[ExcludeFromCodeCoverage]
public record ResultadoSerializacaoRespostaHttp
{
    public required string Conteudo { get; init; }
    public required string TipoConteudo { get; init; }
}