using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
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
        CadastroAgendamentoEntrada entrada = new()
        {
            IdMedico = idMedico,
            DataAtual = dataAtual,
            DataAgendamento = dataAgendamento,
            HorarioInicioAgendamento = new TimeSpan(12, 0, 0),
            HorarioFimAgendamento = new TimeSpan(12, 30, 0)
        };
        Mock<IRepositorioAgendamento> repositorio = new();  
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento))
            .ReturnsAsync(() =>
            [
                new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual),
                new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual),
                new Agendamento(idMedico, dataAgendamento, new(14, 0, 0), new(14, 30, 0), dataAtual),
                new Agendamento(idMedico, dataAgendamento, new(14, 30, 0), new(15, 0, 0), dataAtual)
            ]);
        CadastroAgendamentoCasoUso casoUso = new(repositorio.Object, new ServicoAgendamento());

        // Act
        CadastroAgendamentoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.IdAgendamento.Should().NotBeEmpty();
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, dataAgendamento), Times.Once());
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once());
    }
}