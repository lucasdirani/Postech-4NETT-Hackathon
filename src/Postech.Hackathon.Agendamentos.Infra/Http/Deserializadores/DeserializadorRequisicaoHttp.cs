using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores.Excecoes;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores;

[ExcludeFromCodeCoverage]
public static class DeserializadorRequisicaoHttp
{
    public static TRequisicao? Deserializar<TRequisicao>(string? corpoRequisicao, string? tipoConteudo)
    {
        return string.IsNullOrEmpty(corpoRequisicao) ? default : tipoConteudo switch
        {
            "application/json" => JsonSerializer.Deserialize<TRequisicao>(corpoRequisicao),
            "application/json; charset=utf-8" => JsonSerializer.Deserialize<TRequisicao>(corpoRequisicao),
            _ => throw new ExcecaoDeserializadorRequisicaoHttp("O tipo do conteúdo fornecido não é suportado", tipoConteudo)
        };
    }
}