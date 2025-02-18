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

public class EfetuacaoAgendamentoCasoUsoTestes
{
    [Fact(DisplayName = "Efetuar agendamento sem conflitos")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EfetuarAgendamentoSemConflitos_DeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoEfetuado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id)).ReturnsAsync(() => agendamentoSendoEfetuado);
        Agendamento agendamentoEfetuado = new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 7), horarioInicioAgendamento: new(8, 0, 0), horarioFimAgendamento: new(8, 30, 0), dataAtual: new(2025, 2, 1), valorAgendamento: 100);
        agendamentoEfetuado.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: new(2025, 2, 5));
        Agendamento agendamentoAceito = new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 7), horarioInicioAgendamento: new(15, 0, 0), horarioFimAgendamento: new(15, 30, 0), dataAtual: new(2025, 2, 5), valorAgendamento: 100);
        agendamentoAceito.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: new(2025, 2, 5));
        agendamentoAceito.AceitarAgendamento(dataAceitacaoAgendamento: new DateOnly(2025, 2, 5));
        repositorio
            .Setup(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento))
            .ReturnsAsync(() => [ agendamentoEfetuado, agendamentoAceito ]);
        EfetuacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoEfetuado.Id,
            IdPaciente = idPaciente,
            DataEfetuacao = new DateOnly(2025, 2, 6)
        };
        EfetuacaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEfetuacaoAgendamento.Should().Be(SituacaoEfetuacaoAgendamento.Sucesso);
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Efetuar agendamento não existente no sistema")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EfetuarAgendamentoNaoExistente_NaoDeveEfetuarAgendamento()
    {
        // Arrange
        Guid idAgendamentoNaoExistente = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(idAgendamentoNaoExistente)).ReturnsAsync(() => null);
        EfetuacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = idAgendamentoNaoExistente,
            IdPaciente = idPaciente,
            DataEfetuacao = new DateOnly(2025, 2, 6)
        };
        EfetuacaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEfetuacaoAgendamento.Should().Be(SituacaoEfetuacaoAgendamento.AgendamentoNaoEncontrado);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(idAgendamentoNaoExistente), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento), Times.Never());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Paciente com agendamento marcado para mesma data e horário")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_PacienteComAgendamentoMesmaDataHorario_NaoDeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoEfetuado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id)).ReturnsAsync(() => agendamentoSendoEfetuado);
        Agendamento agendamentoAceito = new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 7), horarioInicioAgendamento: new(12, 0, 0), horarioFimAgendamento: new(13, 0, 0), dataAtual: new(2025, 2, 5), valorAgendamento: 100);
        agendamentoAceito.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: new(2025, 2, 5));
        agendamentoAceito.AceitarAgendamento(dataAceitacaoAgendamento: new DateOnly(2025, 2, 5));
        repositorio
            .Setup(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento))
            .ReturnsAsync(() => [ agendamentoAceito ]);
        EfetuacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoEfetuado.Id,
            IdPaciente = idPaciente,
            DataEfetuacao = new DateOnly(2025, 2, 6)
        };
        EfetuacaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEfetuacaoAgendamento.Should().Be(SituacaoEfetuacaoAgendamento.Conflito);
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Paciente não identificado para efetuar o agendamento")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_PacienteNaoIdentificado_NaoDeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoEfetuado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id)).ReturnsAsync(() => agendamentoSendoEfetuado);
        repositorio
            .Setup(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento))
            .ReturnsAsync(() => []);
        EfetuacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoEfetuado.Id,
            IdPaciente = Guid.Empty,
            DataEfetuacao = new DateOnly(2025, 2, 6)
        };
        EfetuacaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEfetuacaoAgendamento.Should().Be(SituacaoEfetuacaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(Guid.Empty, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Efetuar agendamento após a data cadastrada para o agendamento")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EfetuarAgendamentoAposDataCadastradaAgendamento_NaoDeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoEfetuado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id)).ReturnsAsync(() => agendamentoSendoEfetuado);
        Agendamento agendamentoAceito = new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 7), horarioInicioAgendamento: new(16, 0, 0), horarioFimAgendamento: new(17, 0, 0), dataAtual: new(2025, 2, 5), valorAgendamento: 100);
        agendamentoAceito.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: new(2025, 2, 5));
        agendamentoAceito.AceitarAgendamento(dataAceitacaoAgendamento: new DateOnly(2025, 2, 5));
        repositorio
            .Setup(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento))
            .ReturnsAsync(() => [ agendamentoAceito ]);
        EfetuacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoEfetuado.Id,
            IdPaciente = idPaciente,
            DataEfetuacao = new DateOnly(2025, 2, 16)
        };
        EfetuacaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEfetuacaoAgendamento.Should().Be(SituacaoEfetuacaoAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Efetuar agendamento que já foi aceito pelo médico")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_EfetuarAgendamentoAceitoPeloMedico_NaoDeveEfetuarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 7);
        TimeSpan horarioInicioAgendamento = new(12, 0, 0);
        TimeSpan horarioFimAgendamento = new(13, 0, 0);
        DateOnly dataCadastro = new(2025, 2, 5);
        decimal valorAgendamento = 100;
        Agendamento agendamentoSendoEfetuado = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoSendoEfetuado.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: new(2025, 2, 5));
        agendamentoSendoEfetuado.AceitarAgendamento(dataAceitacaoAgendamento: new DateOnly(2025, 2, 5));
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio.Setup(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id)).ReturnsAsync(() => agendamentoSendoEfetuado);
        repositorio
            .Setup(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento))
            .ReturnsAsync(() => []);
        EfetuacaoAgendamentoEntrada entrada = new()
        {
            IdAgendamento = agendamentoSendoEfetuado.Id,
            IdPaciente = idPaciente,
            DataEfetuacao = new DateOnly(2025, 2, 6)
        };
        EfetuacaoAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        EfetuacaoAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.SituacaoEfetuacaoAgendamento.Should().Be(SituacaoEfetuacaoAgendamento.EfetuacaoNaoProcessavel);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.ObterPorIdAsync(agendamentoSendoEfetuado.Id), Times.Once());
        repositorio.Verify(r => r.ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(idPaciente, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }
}