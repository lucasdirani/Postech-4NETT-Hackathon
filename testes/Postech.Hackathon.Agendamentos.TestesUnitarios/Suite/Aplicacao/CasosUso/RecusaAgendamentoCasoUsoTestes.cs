using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Aplicacao.CasosUso;

public class RecusaAgendamentoCasoUsoTestes
{
    [Fact(DisplayName = "Recusar agendamento em situação válida")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_RecusarAgendamentoSituacaoValida_DeveRecusarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly dataEfetuacaoAgendamento = new(2025, 2, 6);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoRecusado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoRecusado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id)).ReturnsAsync(() => agendamentoSendoRecusado);
        RecusaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoRecusado.Id,
            IdMedico = idMedico,
            DataRecusa = new DateOnly(2025, 2, 6),
            JustificativaRecusa = "Estarei em plantão no hospital"
        };
        RecusaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        RecusaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoRecusaAgendamento.Should().Be(SituacaoRecusaAgendamento.Sucesso);
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Recusar agendamento não existente no sistema")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_RecusarAgendamentoNaoExistente_NaoDeveRecusarAgendamento()
    {
        // Arrange
        Guid idAgendamentoNaoExistente = Guid.NewGuid();
        Guid idMedico = Guid.NewGuid();
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(idAgendamentoNaoExistente)).ReturnsAsync(() => null);
        RecusaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = idAgendamentoNaoExistente,
            IdMedico = idMedico,
            DataRecusa = new DateOnly(2025, 2, 6),
            JustificativaRecusa = "Estarei em plantão no hospital"
        };
        RecusaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        RecusaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoRecusaAgendamento.Should().Be(SituacaoRecusaAgendamento.AgendamentoNaoEncontrado);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(idAgendamentoNaoExistente), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Médico que recusou o agendamento não é o mesmo que realizou o cadastro")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_MedicoRecusouAgendamentoDiferenteRealizouCadastro_NaoDeveRecusarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly dataEfetuacaoAgendamento = new(2025, 2, 6);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoRecusado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoRecusado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id)).ReturnsAsync(() => agendamentoSendoRecusado);
        RecusaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoRecusado.Id,
            IdMedico = Guid.NewGuid(),
            DataRecusa = new DateOnly(2025, 2, 6),
            JustificativaRecusa = "Estarei em plantão no hospital"
        };
        RecusaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        RecusaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoRecusaAgendamento.Should().Be(SituacaoRecusaAgendamento.RecusaNaoPermitida);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Recusar agendamento após a data cadastrada para o agendamento")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_RecusarAgendamentoAposDataCadastradaAgendamento_NaoDeveRecusarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        DateOnly dataEfetuacaoAgendamento = new(2025, 2, 6);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoRecusado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoRecusado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id)).ReturnsAsync(() => agendamentoSendoRecusado);
        RecusaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoRecusado.Id,
            IdMedico = idMedico,
            DataRecusa = new DateOnly(2025, 2, 8),
            JustificativaRecusa = "Estarei em plantão no hospital"
        };
        RecusaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        RecusaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoRecusaAgendamento.Should().Be(SituacaoRecusaAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Recusar agendamento que não foi efetuado pelo paciente")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_RecusarAgendamentoNaoEfetuadoPeloPaciente_NaoDeveRecusarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoRecusado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id)).ReturnsAsync(() => agendamentoSendoRecusado);
        RecusaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoRecusado.Id,
            IdMedico = idMedico,
            DataRecusa = new DateOnly(2025, 2, 6),
            JustificativaRecusa = "Estarei em plantão no hospital"
        };
        RecusaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        RecusaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoRecusaAgendamento.Should().Be(SituacaoRecusaAgendamento.RecusaNaoProcessavel);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoRecusado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }
}