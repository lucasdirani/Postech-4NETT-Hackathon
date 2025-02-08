using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores.Excecoes;

[ExcludeFromCodeCoverage]
public class ExcecaoDeserializadorRequisicaoHttp(string? mensagem, string? tipoConteudo) : Exception(mensagem)
{
    public string? TipoConteudo { get; } = tipoConteudo;
}