using FluentAssertions;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Servicos;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Dominio.Servicos;

public class ServicoAgendamentoTestes
{
    private readonly ServicoAgendamento _servico = new();

    [Fact(DisplayName = "Novo agendamento acontecendo logo depois (sem conflito)")]
    [Trait("Action", "ValidarConflitoHorarioAgendamento")]
    public void ValidarConflitoHorarioAgendamento_NovoAgendamentoAcontecendoLogoDepois_DeveRetornarFalso()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        IReadOnlyList<Agendamento> agendamentos =
        [
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(11, 0, 0), horarioFimAgendamento: new(11, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 0, 0), horarioFimAgendamento: new(15, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 30, 0), horarioFimAgendamento: new(16, 0, 0), dataAtual)
        ];
        TimeSpan horarioInicioNovoAgendamento = new(16, 0, 0);
        TimeSpan horarioFimNovoAgendamento = new(16, 30, 0);

        // Act
        bool possuiConflito = _servico.ValidarConflitoHorarioAgendamento(agendamentos, horarioInicioNovoAgendamento, horarioFimNovoAgendamento);

        // Assert
        possuiConflito.Should().BeFalse();
    }

    [Fact(DisplayName = "Novo agendamento acontecendo logo antes (sem conflito)")]
    [Trait("Action", "ValidarConflitoHorarioAgendamento")]
    public void ValidarConflitoHorarioAgendamento_NovoAgendamentoAcontecendoLogoAntes_DeveRetornarFalso()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        IReadOnlyList<Agendamento> agendamentos =
        [
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(11, 0, 0), horarioFimAgendamento: new(11, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 0, 0), horarioFimAgendamento: new(15, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 30, 0), horarioFimAgendamento: new(16, 0, 0), dataAtual)
        ];
        TimeSpan horarioInicioNovoAgendamento = new(8, 0, 0);
        TimeSpan horarioFimNovoAgendamento = new(8, 30, 0);

        // Act
        bool possuiConflito = _servico.ValidarConflitoHorarioAgendamento(agendamentos, horarioInicioNovoAgendamento, horarioFimNovoAgendamento);

        // Assert
        possuiConflito.Should().BeFalse();
    }

    [Fact(DisplayName = "Novo agendamento acontecendo em um horário distinto (sem conflito)")]
    [Trait("Action", "ValidarConflitoHorarioAgendamento")]
    public void ValidarConflitoHorarioAgendamento_NovoAgendamentoAcontecendoHorarioDistinto_DeveRetornarFalso()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        IReadOnlyList<Agendamento> agendamentos =
        [
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(11, 0, 0), horarioFimAgendamento: new(11, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 0, 0), horarioFimAgendamento: new(15, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 30, 0), horarioFimAgendamento: new(16, 0, 0), dataAtual)
        ];
        TimeSpan horarioInicioNovoAgendamento = new(17, 0, 0);
        TimeSpan horarioFimNovoAgendamento = new(17, 30, 0);

        // Act
        bool possuiConflito = _servico.ValidarConflitoHorarioAgendamento(agendamentos, horarioInicioNovoAgendamento, horarioFimNovoAgendamento);

        // Assert
        possuiConflito.Should().BeFalse();
    }

    [Fact(DisplayName = "Novo agendamento acontecendo em um mesmo intervalo (conflito)")]
    [Trait("Action", "ValidarConflitoHorarioAgendamento")]
    public void ValidarConflitoHorarioAgendamento_NovoAgendamentoAcontecendoMesmoIntervalo_DeveRetornarVerdadeiro()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        IReadOnlyList<Agendamento> agendamentos =
        [
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(11, 0, 0), horarioFimAgendamento: new(11, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 0, 0), horarioFimAgendamento: new(15, 30, 0), dataAtual),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(15, 30, 0), horarioFimAgendamento: new(16, 0, 0), dataAtual)
        ];
        TimeSpan horarioInicioNovoAgendamento = new(9, 0, 0);
        TimeSpan horarioFimNovoAgendamento = new(9, 30, 0);

        // Act
        bool possuiConflito = _servico.ValidarConflitoHorarioAgendamento(agendamentos, horarioInicioNovoAgendamento, horarioFimNovoAgendamento);

        // Assert
        possuiConflito.Should().BeTrue();
    }
}