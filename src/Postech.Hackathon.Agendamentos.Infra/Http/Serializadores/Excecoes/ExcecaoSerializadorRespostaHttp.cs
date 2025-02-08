using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Serializadores.Excecoes;

[ExcludeFromCodeCoverage]
public class ExcecaoSerializadorRespostaHttp(string? mensagem, StringValues valoresCabecalhoAceite) : Exception(mensagem)
{
    public StringValues ValoresCabecalhoAceite { get; } = valoresCabecalhoAceite;
}