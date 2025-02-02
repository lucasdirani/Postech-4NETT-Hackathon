using FluentAssertions;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Dominio.Entidades;

public class AgendamentoTestes
{
    [Fact(DisplayName = "Construindo um objeto válido do tipo Agendamento")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_DadosValidos_DeveConstruirAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);

        // Act
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento);

        // Assert
        agendamento.Should().NotBeNull();
        agendamento.Id.Should().NotBeEmpty();
        agendamento.CriadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
        agendamento.IdMedico.Should().Be(idMedico);
        agendamento.Data.Should().Be(dataAgendamento);
        agendamento.HorarioInicio.Should().Be(horarioInicioAgendamento);
        agendamento.HorarioFim.Should().Be(horarioFimAgendamento);
    }
}