using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Dominio.Servicos;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Aplicacao.CasosUso;

public class EdicaoAgendamentoCasoUsoTestes
{
    [Fact(DisplayName = "Editar agendamento com dados válidos e sem conflitos")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoComDadosValidosSemConflitos_DeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 200;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, novaDataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, novaDataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, novaDataAgendamento, new(14, 0, 0), new(14, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, novaDataAgendamento, new(14, 30, 0), new(15, 0, 0), dataAtual, valorAgendamento)
            ]);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = idMedico,
            DataAtualizacao = new DateOnly(2025, 2, 6),
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = horarioInicioAgendamento,
            HorarioFimAgendamento = horarioFimAgendamento,
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.Sucesso);
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Editar agendamento não existente no sistema")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoNaoExistente_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idAgendamentoNaoExistente = Guid.NewGuid();
        Guid idMedico = Guid.NewGuid();
        DateOnly novaDataAgendamento = new(2025, 2, 8);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(idAgendamentoNaoExistente)).ReturnsAsync(() => null);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = idAgendamentoNaoExistente,
            IdMedico = idMedico,
            DataAtualizacao = new DateOnly(2025, 2, 6),
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(11, 0, 0),
            HorarioFimAgendamento = new TimeSpan(12, 0, 0),
            ValorAgendamento = 100
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.AgendamentoNaoEncontrado);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(idAgendamentoNaoExistente), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Médico que não criou o agendamento realizando a sua edição")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_MedicoNaoCriouAgendamentoRealizandoEdicao_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 200;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = Guid.NewGuid(),
            DataAtualizacao = new DateOnly(2025, 2, 6),
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = horarioInicioAgendamento,
            HorarioFimAgendamento = horarioFimAgendamento,
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.EdicaoNaoPermitida);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Editar agendamento com nova data inválida")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoComNovaDataInvalida_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 9);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 200;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = idMedico,
            DataAtualizacao = dataAtualizacaoAgendamento,
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = horarioInicioAgendamento,
            HorarioFimAgendamento = horarioFimAgendamento,
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Editar agendamento com novo horário inválido")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoComNovoHorarioInvalido_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 150;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = idMedico,
            DataAtualizacao = dataAtualizacaoAgendamento,
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = new(14, 0, 0),
            HorarioFimAgendamento = new(13, 30, 0),
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Editar agendamento com novo valor inválido")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoComNovoValorInvalido_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 0;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = idMedico,
            DataAtualizacao = dataAtualizacaoAgendamento,
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = horarioInicioAgendamento,
            HorarioFimAgendamento = horarioFimAgendamento,
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Editar agendamento aceito")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoAceito_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 50;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.AceitarAgendamento();
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = idMedico,
            DataAtualizacao = dataAtualizacaoAgendamento,
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = horarioInicioAgendamento,
            HorarioFimAgendamento = horarioFimAgendamento,
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Editar agendamento efetuado")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EditarAgendamentoEfetuado_NaoDeveEditarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly novaDataAgendamento = new (2025, 2, 8);
        DateOnly dataAtualizacaoAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataAtual = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        decimal novoValorAgendamento = 50;
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataAtual, valorAgendamento);
        agendamento.EfetuarAgendamento();
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamento.Id)).ReturnsAsync(() => agendamento);
        EdicaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamento.Id,
            IdMedico = idMedico,
            DataAtualizacao = dataAtualizacaoAgendamento,
            DataAgendamento = novaDataAgendamento,
            HorarioInicioAgendamento = horarioInicioAgendamento,
            HorarioFimAgendamento = horarioFimAgendamento,
            ValorAgendamento = novoValorAgendamento
        };
        EdicaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EdicaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEdicaoAgendamento.Should().Be(SituacaoEdicaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamento.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, novaDataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }
}