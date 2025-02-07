using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record ConsultaAgendamentosPorIdMedicoEntrada
{
    public Guid IdMedico { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}