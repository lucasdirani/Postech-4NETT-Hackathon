using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record ConsultaAgendamentosPorIdMedicoEntrada
{
    public Guid IdMedico { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }

    internal bool NaoEstaValida()
    {
        return IdMedico.EstaVazio() || TamanhoPagina <= 0;
    }
}