using FluentAssertions;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Enumeradores;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Dominio.Entidades;

public class AgendamentoTestes
{
    [Fact(DisplayName = "Construindo um objeto válido do tipo Agendamento")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_DadosValidos_DeveConstruirAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;

        // Act
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);

        // Assert
        agendamento.Should().NotBeNull();
        agendamento.Id.Should().NotBeEmpty();
        agendamento.CriadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
        agendamento.IdMedico.Should().Be(idMedico);
        agendamento.Data.Should().Be(dataAgendamento);
        agendamento.HorarioInicio.Should().Be(horarioInicioAgendamento);
        agendamento.HorarioFim.Should().Be(horarioFimAgendamento);
        agendamento.Valor.Should().Be(valorAgendamento);
        agendamento.Situacao.Should().Be(SituacaoAgendamento.Criado);
    }

    [Fact(DisplayName = "Identificador inválido para o médico que cadastrou o agendamento")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_IdMedicoComValorInvalido_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.Empty;
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(15, 0, 0);
        TimeSpan horarioFimAgendamento = new(15, 30, 0);
        decimal valorAgendamento = 150;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => new Agendamento(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.IdMedico));
    }

    [Fact(DisplayName = "Data do agendamento anterior a data atual")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_DataAgendamentoAnteriorDataAtual_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 3);
        DateOnly dataAgendamento = new(2025, 2, 1);
        TimeSpan horarioInicioAgendamento = new(15, 0, 0);
        TimeSpan horarioFimAgendamento = new(15, 30, 0);
        decimal valorAgendamento = 100;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => new Agendamento(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Data));
    }

    [Fact(DisplayName = "Data do agendamento igual a data atual")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_DataAgendamentoIgualDataAtual_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 3);
        DateOnly dataAgendamento = new(2025, 2, 3);
        TimeSpan horarioInicioAgendamento = new(15, 0, 0);
        TimeSpan horarioFimAgendamento = new(15, 30, 0);
        decimal valorAgendamento = 200;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => new Agendamento(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Data));
    }

    [Fact(DisplayName = "Horário de início posterior ao horário de fim do agendamento")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_HorarioInicioPosteriorHorarioFim_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(16, 0, 0);
        TimeSpan horarioFimAgendamento = new(15, 30, 0);
        decimal valorAgendamento = 100;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => new Agendamento(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.HorarioInicio));
    }

    [Fact(DisplayName = "Horário de início no mesmo horário de fim do agendamento")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_HorarioInicioMesmoHorarioFim_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(16, 0, 0);
        TimeSpan horarioFimAgendamento = new(16, 0, 0);
        decimal valorAgendamento = 200;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => new Agendamento(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.HorarioInicio));
    }

    [Fact(DisplayName = "Valor inválido para o agendamento")]
    [Trait("Action", "Agendamento")]
    public void Agendamento_ValorInvalido_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(15, 0, 0);
        TimeSpan horarioFimAgendamento = new(15, 30, 0);
        decimal valorAgendamento = 0;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => new Agendamento(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Valor));
    }

    [Fact(DisplayName = "Alterar a data de um agendamento")]
    [Trait("Action", "AlterarDataAgendamento")]
    public void AlterarDataAgendamento_DataValida_DeveAlterarDataAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        DateOnly novaDataAgendamento = new(2025, 2, 3);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 2);

        // Act
        agendamento.AlterarDataAgendamento(novaDataAgendamento, dataAtualizacaoAgendamento);

        // Assert
        agendamento.Data.Should().Be(novaDataAgendamento);
    }

    [Fact(DisplayName = "Alterar a data do agendamento para a mesma data cadastrada")]
    [Trait("Action", "AlterarDataAgendamento")]
    public void AlterarDataAgendamento_DataOriginal_DevePermanecerDataOriginalAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        DateOnly novaDataAgendamento = new(2025, 2, 2);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 2);

        // Act
        agendamento.AlterarDataAgendamento(novaDataAgendamento, dataAtualizacaoAgendamento);

        // Assert
        agendamento.Data.Should().Be(dataAgendamento);
    }

    [Fact(DisplayName = "Nova data do agendamento anterior a data de atualização")]
    [Trait("Action", "AlterarDataAgendamento")]
    public void AlterarDataAgendamento_NovaDataAgendamentoAnteriorDataAtualizacao_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        DateOnly novaDataAgendamento = new(2025, 2, 3);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 4);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarDataAgendamento(novaDataAgendamento, dataAtualizacaoAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarDataAgendamento));
        excecao.Propriedade.Should().Be(nameof(novaDataAgendamento));
    }

    [Fact(DisplayName = "Alterar a data de um agendamento que está aceito")]
    [Trait("Action", "AlterarDataAgendamento")]
    public void AlterarDataAgendamento_AlterarDataAgendamentoAceito_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.AceitarAgendamento();
        DateOnly novaDataAgendamento = new(2025, 2, 3);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 2);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarDataAgendamento(novaDataAgendamento, dataAtualizacaoAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarDataAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }

    [Fact(DisplayName = "Alterar a data de um agendamento que está efetuado")]
    [Trait("Action", "AlterarDataAgendamento")]
    public void AlterarDataAgendamento_AlterarDataAgendamentoEfetuado_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.EfetuarAgendamento();
        DateOnly novaDataAgendamento = new(2025, 2, 3);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 2);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarDataAgendamento(novaDataAgendamento, dataAtualizacaoAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarDataAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }
}