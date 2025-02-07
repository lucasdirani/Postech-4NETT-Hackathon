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
        agendamento.ModificadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
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
        agendamento.ModificadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
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

    [Fact(DisplayName = "Nova data do agendamento igual a data de atualização")]
    [Trait("Action", "AlterarDataAgendamento")]
    public void AlterarDataAgendamento_NovaDataAgendamentoIgualDataAtualizacao_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        DateOnly novaDataAgendamento = new(2025, 2, 4);
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
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente);
        DateOnly novaDataAgendamento = new(2025, 2, 3);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 2);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarDataAgendamento(novaDataAgendamento, dataAtualizacaoAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarDataAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }

    [Fact(DisplayName = "Alterar o horário de um agendamento")]
    [Trait("Action", "AlterarHorarioAgendamento")]
    public void AlterarHorarioAgendamento_HorarioValido_DeveAlterarHorarioAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        TimeSpan novoHorarioInicioAgendamento = new(14, 0, 0);
        TimeSpan novoHorarioFimAgendamento = new(14, 30, 0);

        // Act
        agendamento.AlterarHorarioAgendamento(novoHorarioInicioAgendamento, novoHorarioFimAgendamento);

        // Assert
        agendamento.HorarioInicio.Should().Be(novoHorarioInicioAgendamento);
        agendamento.HorarioFim.Should().Be(novoHorarioFimAgendamento);
        agendamento.ModificadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact(DisplayName = "Alterar o horário do agendamento para o mesmo horário cadastrado")]
    [Trait("Action", "AlterarHorarioAgendamento")]
    public void AlterarHorarioAgendamento_HorarioOriginal_DevePermanecerHorarioOriginalAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        TimeSpan novoHorarioInicioAgendamento = new(12, 0, 0);
        TimeSpan novoHorarioFimAgendamento = new(12, 30, 0);

        // Act
        agendamento.AlterarHorarioAgendamento(novoHorarioInicioAgendamento, novoHorarioFimAgendamento);

        // Assert
        agendamento.HorarioInicio.Should().Be(horarioInicioAgendamento);
        agendamento.HorarioFim.Should().Be(horarioFimAgendamento);
    }

    [Fact(DisplayName = "Alterar o horário de início para depois do horário de fim do agendamento")]
    [Trait("Action", "AlterarHorarioAgendamento")]
    public void AlterarHorarioAgendamento_HorarioInicioPosteriorHorarioFim_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        TimeSpan novoHorarioInicioAgendamento = new(12, 0, 0);
        TimeSpan novoHorarioFimAgendamento = new(11, 30, 0);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarHorarioAgendamento(novoHorarioInicioAgendamento, novoHorarioFimAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarHorarioAgendamento));
        excecao.Propriedade.Should().Be(nameof(novoHorarioInicioAgendamento));
    }

    [Fact(DisplayName = "Alterar o horário de início para o mesmo horário de fim do agendamento")]
    [Trait("Action", "AlterarHorarioAgendamento")]
    public void AlterarHorarioAgendamento_HorarioInicioMesmoHorarioFim_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        TimeSpan novoHorarioInicioAgendamento = new(12, 0, 0);
        TimeSpan novoHorarioFimAgendamento = new(12, 0, 0);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarHorarioAgendamento(novoHorarioInicioAgendamento, novoHorarioFimAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarHorarioAgendamento));
        excecao.Propriedade.Should().Be(nameof(novoHorarioInicioAgendamento));
    }

    [Fact(DisplayName = "Alterar o horário de um agendamento que está aceito")]
    [Trait("Action", "AlterarHorarioAgendamento")]
    public void AlterarHorarioAgendamento_AlterarHorarioAgendamentoAceito_DeveLancarExcecaoDominio()
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
        TimeSpan novoHorarioInicioAgendamento = new(14, 0, 0);
        TimeSpan novoHorarioFimAgendamento = new(14, 30, 0);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarHorarioAgendamento(novoHorarioInicioAgendamento, novoHorarioFimAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarHorarioAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }

    [Fact(DisplayName = "Alterar o horário de um agendamento que está efetuado")]
    [Trait("Action", "AlterarHorarioAgendamento")]
    public void AlterarHorarioAgendamento_AlterarHorarioAgendamentoEfetuado_DeveLancarExcecaoDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente);
        TimeSpan novoHorarioInicioAgendamento = new(14, 0, 0);
        TimeSpan novoHorarioFimAgendamento = new(14, 30, 0);

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarHorarioAgendamento(novoHorarioInicioAgendamento, novoHorarioFimAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarHorarioAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }

    [Fact(DisplayName = "Alterar o valor de um agendamento")]
    [Trait("Action", "AlterarValorAgendamento")]
    public void AlterarValorAgendamento_ValorValido_DeveAlterarValorAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        decimal novoValorAgendamento = 200;

        // Act
        agendamento.AlterarValorAgendamento(novoValorAgendamento);

        // Assert
        agendamento.Valor.Should().Be(novoValorAgendamento);
        agendamento.ModificadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact(DisplayName = "Alterar o valor do agendamento para o mesmo valor cadastrado")]
    [Trait("Action", "AlterarValorAgendamento")]
    public void AlterarValorAgendamento_ValorOriginal_DevePermanecerValorOriginalAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        decimal novoValorAgendamento = 100;

        // Act
        agendamento.AlterarValorAgendamento(novoValorAgendamento);

        // Assert
        agendamento.Valor.Should().Be(valorAgendamento);
    }

    [Fact(DisplayName = "Novo valor inválido para o agendamento")]
    [Trait("Action", "AlterarValorAgendamento")]
    public void AlterarValorAgendamento_ValorInvalido_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        decimal novoValorAgendamento = 0;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarValorAgendamento(novoValorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarValorAgendamento));
        excecao.Propriedade.Should().Be(nameof(novoValorAgendamento));
    }

    [Fact(DisplayName = "Alterar o valor de um agendamento que está aceito")]
    [Trait("Action", "AlterarValorAgendamento")]
    public void AlterarValorAgendamento_AlterarValorAgendamentoAceito_DeveLancarExcecaoDeDominio()
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
        decimal novoValorAgendamento = 150;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarValorAgendamento(novoValorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarValorAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }

    [Fact(DisplayName = "Alterar o valor de um agendamento que está efetuado")]
    [Trait("Action", "AlterarValorAgendamento")]
    public void AlterarValorAgendamento_AlterarValorAgendamentoEfetuado_DeveLancarExcecaoDeDominio()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente);
        decimal novoValorAgendamento = 150;

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.AlterarValorAgendamento(novoValorAgendamento));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.AlterarValorAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }

    [Fact(DisplayName = "Agendamento que pertence ao médico que realizou o seu cadastro")]
    [Trait("Action", "PertenceMedico")]
    public void PertenceMedico_AgendamentoPertenceMedicoRealizouCadastro_DeveRetornarVerdadeiro()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);

        // Act
        bool pertenceMedico = agendamento.PertenceMedico(idMedico);

        // Assert
        pertenceMedico.Should().BeTrue();
    }

    [Fact(DisplayName = "Agendamento que não pertence ao médico que realizou o seu cadastro")]
    [Trait("Action", "PertenceMedico")]
    public void PertenceMedico_AgendamentoNaoPertenceMedicoRealizouCadastro_DeveRetornarFalso()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);

        // Act
        bool pertenceMedico = agendamento.PertenceMedico(idMedico: Guid.NewGuid());

        // Assert
        pertenceMedico.Should().BeFalse();
    }

    [Fact(DisplayName = "Efetuar agendamento com situação válida")]
    [Trait("Action", "EfetuarAgendamento")]
    public void EfetuarAgendamento_AgendamentoComSituacaoValida_DeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);

        // Act
        agendamento.EfetuarAgendamento(idPaciente);

        // Assert
        agendamento.IdPaciente.Should().Be(idPaciente);
        agendamento.ModificadoEm.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact(DisplayName = "Situação inválida para efetuar agendamento")]
    [Trait("Action", "EfetuarAgendamento")]
    public void EfetuarAgendamento_AgendamentoComSituacaoInvalida_NaoDeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        DateOnly dataAgendamento = new(2025, 2, 2);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(12, 30, 0);
        decimal valorAgendamento = 100;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.AceitarAgendamento();

        // Act
        ExcecaoDominio excecao = Assert.Throws<ExcecaoDominio>(() => agendamento.EfetuarAgendamento(idPaciente));

        // Assert
        excecao.Mensagem.Should().NotBeNullOrEmpty();
        excecao.Acao.Should().Be(nameof(Agendamento.EfetuarAgendamento));
        excecao.Propriedade.Should().Be(nameof(Agendamento.Situacao));
    }
}