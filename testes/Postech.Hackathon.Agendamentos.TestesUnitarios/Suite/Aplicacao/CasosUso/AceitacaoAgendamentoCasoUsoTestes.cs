using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Aplicacao.CasosUso;

public class AceitacaoAgendamentoCasoUsoTestes
{
    [Fact(DisplayName = "Aceitar agendamento em situação válida")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_AceitarAgendamentoSituacaoValida_DeveAceitarAgendamento()
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
        Agendamento agendamentoSendoAceito = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoAceito.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoAceito.Id)).ReturnsAsync(() => agendamentoSendoAceito);
        AceitacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoAceito.Id,
            IdMedico = idMedico,
            DataAceitacao = new DateOnly(2025, 2, 6)
        };
        AceitacaoAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        AceitacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoAceitacaoAgendamento.Should().Be(SituacaoAceitacaoAgendamento.Sucesso);
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoAceito.Id), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Aceitar agendamento não existente no sistema")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_AceitarAgendamentoNaoExistente_NaoDeveAceitarAgendamento()
    {
        // Arrange
        Guid idAgendamentoNaoExistente = Guid.NewGuid();
        Guid idMedico = Guid.NewGuid();
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(idAgendamentoNaoExistente)).ReturnsAsync(() => null);
        AceitacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = idAgendamentoNaoExistente,
            IdMedico = idMedico,
            DataAceitacao = new DateOnly(2025, 2, 6)
        };
        AceitacaoAgendamentoCasoUso casoUso = new(repositorio.Object);

        // Act
        AceitacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoAceitacaoAgendamento.Should().Be(SituacaoAceitacaoAgendamento.AgendamentoNaoEncontrado);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(idAgendamentoNaoExistente), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }
}