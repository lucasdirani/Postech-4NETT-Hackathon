using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Postech.Hackathon.Agendamentos.Dominio.Projecoes;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;

[ExcludeFromCodeCoverage]
public record ComandoRespostaConsultaAgendamentosPorIdMedico : IComandoResposta 
{
    [JsonPropertyName("agendamentos")]
    public IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico> Agendamentos { get; init; } = [];

    [JsonPropertyName("quantidadeItens")]
    public int QuantidadeItens { get; init; }

    [JsonPropertyName("totalPaginas")]
    public int TotalPaginas { get; init; }

    [JsonPropertyName("paginaAtual")]
    public int PaginaAtual { get; init; }

    [JsonPropertyName("tamanhoPagina")]
    public int TamanhoPagina { get; init; }
}