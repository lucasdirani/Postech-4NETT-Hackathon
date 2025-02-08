using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Aplicacao.CasosUso;

public class CancelaAgendamentoCasoUsoTestes
{
    [Fact(DisplayName = "Cancelar agendamento em situação válida")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CancelarAgendamentoSituacaoValida_DeveCancelarAgendamento()
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
        Agendamento agendamentoSendoCancelado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoCancelado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id)).ReturnsAsync(() => agendamentoSendoCancelado);
        CancelaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoCancelado.Id,
            IdPaciente = idPaciente,
            DataCancelamento = new DateOnly(2025, 2, 6),
            JustificativaCancelamento = "Reunião importante no trabalho"
        };
        CancelaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        CancelaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoCancelaAgendamento.Should().Be(SituacaoCancelaAgendamento.Sucesso);
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Cancelar agendamento não existente no sistema")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CancelarAgendamentoNaoExistente_NaoDeveCancelarAgendamento()
    {
        // Arrange
        Guid idAgendamentoNaoExistente = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(idAgendamentoNaoExistente)).ReturnsAsync(() => null);
        CancelaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = idAgendamentoNaoExistente,
            IdPaciente = idPaciente,
            DataCancelamento = new DateOnly(2025, 2, 6),
            JustificativaCancelamento = "Reunião importante no trabalho"
        };
        CancelaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        CancelaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoCancelaAgendamento.Should().Be(SituacaoCancelaAgendamento.AgendamentoNaoEncontrado);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(idAgendamentoNaoExistente), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Paciente que cancelou o agendamento não é o mesmo que efetuou")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_PacienteCancelouAgendamentoDiferenteEfetuou_NaoDeveCancelarAgendamento()
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
        Agendamento agendamentoSendoCancelado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoCancelado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id)).ReturnsAsync(() => agendamentoSendoCancelado);
        CancelaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoCancelado.Id,
            IdPaciente = Guid.NewGuid(),
            DataCancelamento = new DateOnly(2025, 2, 6),
            JustificativaCancelamento = "Reunião importante no trabalho"
        };
        CancelaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        CancelaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoCancelaAgendamento.Should().Be(SituacaoCancelaAgendamento.CancelamentoNaoPermitido);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Cancelar agendamento após a data cadastrada para o agendamento")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CancelarAgendamentoAposDataCadastradaAgendamento_NaoDeveCancelarAgendamento()
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
        Agendamento agendamentoSendoCancelado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoCancelado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id)).ReturnsAsync(() => agendamentoSendoCancelado);
        CancelaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoCancelado.Id,
            IdPaciente = idPaciente,
            DataCancelamento = new DateOnly(2025, 2, 8),
            JustificativaCancelamento = "Reunião importante no trabalho"
        };
        CancelaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        CancelaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoCancelaAgendamento.Should().Be(SituacaoCancelaAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Cancelar agendamento recusado pelo médico")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CancelarAgendamentoRecusadoPeloMedico_NaoDeveCancelarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoCancelado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoCancelado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: new(2025, 2, 5));
        agendamentoSendoCancelado.RecusarAgendamento(dataRecusaAgendamento: new(2025, 2, 5), justificativaRecusa: "Estarei em plantão no hospital");
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id)).ReturnsAsync(() => agendamentoSendoCancelado);
        CancelaAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoCancelado.Id,
            IdPaciente = idPaciente,
            DataCancelamento = new DateOnly(2025, 2, 6),
            JustificativaCancelamento = "Reunião importante no trabalho"
        };
        CancelaAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        CancelaAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoCancelaAgendamento.Should().Be(SituacaoCancelaAgendamento.CancelamentoNaoProcessavel);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoCancelado.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }
}