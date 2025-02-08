using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;

[ExcludeFromCodeCoverage]
public class ExcecaoDominio(string mensagem, string acao, string? propriedade) : Exception(mensagem)
{
    public string Mensagem { get; } = mensagem;
    public string Acao { get; } = acao;
    public string? Propriedade { get; } = propriedade;
}