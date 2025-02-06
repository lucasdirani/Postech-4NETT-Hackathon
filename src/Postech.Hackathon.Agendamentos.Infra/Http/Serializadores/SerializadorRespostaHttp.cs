using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Postech.Hackathon.Agendamentos.Infra.Http.Serializadores.Excecoes;
using Postech.Hackathon.Agendamentos.Infra.Http.Serializadores.Resultados;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Serializadores;

[ExcludeFromCodeCoverage]
public static class SerializadorRespostaHttp
{
    public static ResultadoSerializacaoRespostaHttp Serializar<T>(T objetoResposta, StringValues cabecalhoAceite)
    {
        if (StringValues.IsNullOrEmpty(cabecalhoAceite) || cabecalhoAceite.ContainsAny("application/*", "*/*", "application/json"))
        {
            return new() { TipoConteudo = "application/json", Conteudo = JsonSerializer.Serialize(objetoResposta) };
        }
        throw new ExcecaoSerializadorRespostaHttp("Os tipos de conteúdo fornecidos não são suportados", cabecalhoAceite);
    }
}