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

public class CadastroAgendamentoCasoUsoTestes
{
    [Fact(DisplayName = "Cadastrar agendamento sem conflitos")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarAgendamentoSemConflitos_DeveCadastrarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(12, 0, 0),
            HorarioFimAgendamento = new TimeSpan(12, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(14, 0, 0), new(14, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(14, 30, 0), new(15, 0, 0), dataAtual, valorAgendamento)
            ]);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().NotBeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.Sucesso);
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Cadastrar primeiro agendamento do dia")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarPrimeiroAgendamentoDia_DeveCadastrarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(12, 0, 0),
            HorarioFimAgendamento = new TimeSpan(12, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() => []);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().NotBeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.Sucesso);
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }

    [Fact(DisplayName = "Cadastrar agendamento com conflito")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarAgendamentoComConflito_NaoDeveCadastrarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(12, 0, 0),
            HorarioFimAgendamento = new TimeSpan(12, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(12, 0, 0), new(12, 30, 0), dataAtual, valorAgendamento),
            ]);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().BeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.Conflito);
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Cadastrar agendamento sem o médico responsável")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarAgendamentoSemMedicoResponsavel_NaoDeveCadastrarAgendamento()
    {
        // Arrange
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = Guid.Empty,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(12, 0, 0),
            HorarioFimAgendamento = new TimeSpan(12, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(Guid.Empty, dataAgendamento))
            .ReturnsAsync(() => []);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().BeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.DadosInvalidos);
        saida.Mensagem.Should().NotBeNullOrEmpty();
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Cadastrar agendamento antes da data atual")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarAgendamentoAntesDataAtual_NaoDeveCadastrarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 3);
        decimal valorAgendamento = 150;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(14, 0, 0),
            HorarioFimAgendamento = new TimeSpan(14, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(12, 0, 0), new(12, 30, 0), dataAtual, valorAgendamento),
            ]);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().BeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.DadosInvalidos);
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Cadastrar agendamento com horário de início após o horário de fim")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarAgendamentoHorarioInicioAposHorarioFim_NaoDeveCadastrarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(15, 0, 0),
            HorarioFimAgendamento = new TimeSpan(14, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(12, 0, 0), new(12, 30, 0), dataAtual, valorAgendamento),
            ]);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().BeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.DadosInvalidos);
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }

    [Fact(DisplayName = "Cadastrar agendamento sem valor")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_CadastrarAgendamentoSemValor_NaoDeveCadastrarAgendamento()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 0;
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(15, 0, 0),
            HorarioFimAgendamento = new TimeSpan(15, 30, 0),
            ValorAgendamento = valorAgendamento
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
                new Agendamento(idMedico, dataAgendamento, new(12, 0, 0), new(12, 30, 0), dataAtual, valorAgendamento),
            ]);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().BeEmpty();
        saida.SituacaoCadastroAgendamento.Should().Be(SituacaoCadastroAgendamento.DadosInvalidos);
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Never());
    }
}