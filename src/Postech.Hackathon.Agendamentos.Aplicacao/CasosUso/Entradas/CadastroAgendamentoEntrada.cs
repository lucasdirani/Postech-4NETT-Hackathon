using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

[ExcludeFromCodeCoverage]
public record CadastroAgendamentoEntrada
{
    public Guid IdMedico { get; init; }
    public DateOnly DataAtual { get; init; }
    public DateOnly DataAgendamento { get; init; }
    public TimeSpan HorarioInicioAgendamento { get; init; }
    public TimeSpan HorarioFimAgendamento { get; init; }
    public decimal ValorAgendamento { get; init; }

    internal Agendamento ConverterParaAgendamento()
    {
        return new Agendamento(IdMedico, DataAgendamento, HorarioInicioAgendamento, HorarioFimAgendamento, DataAtual, ValorAgendamento);
    }
}