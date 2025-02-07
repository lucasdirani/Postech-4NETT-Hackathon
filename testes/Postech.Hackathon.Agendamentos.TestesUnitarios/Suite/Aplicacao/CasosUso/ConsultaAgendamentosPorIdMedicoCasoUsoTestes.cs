using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Aplicacao.CasosUso;

public class ConsultaAgendamentosPorIdMedicoCasoUsoTestes
{
    [Fact(DisplayName = "Consultar agendamentos de um médico")]
    [Trait("Action", "ExecutarAsync")]
    public async Task ExecutarAsync_ConsultarAgendamentosMedico_DeveConsultarAgendamentosMedico()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = new(2025, 2, 2);
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;     
        int pagina = 1;
        int tamanhoPagina = 5;
        (IReadOnlyList<Agendamento>, int) agendamentos = new([
            new Agendamento(idMedico, dataAgendamento, new(11, 0, 0), new(11, 30, 0), dataAtual, valorAgendamento),
            new Agendamento(idMedico, dataAgendamento, new(11, 30, 0), new(12, 0, 0), dataAtual, valorAgendamento),
            new Agendamento(idMedico, dataAgendamento, new(14, 0, 0), new(14, 30, 0), dataAtual, valorAgendamento),
            new Agendamento(idMedico, dataAgendamento, new(14, 30, 0), new(15, 0, 0), dataAtual, valorAgendamento)
        ], 4); 
        Mock<IRepositorioAgendamento> repositorio = new();    
        repositorio
            .Setup(r => r.ConsultarAgendamentosMedicoAsync(idMedico, pagina, tamanhoPagina))
            .ReturnsAsync(() => agendamentos);
        ConsultaAgendamentosPorIdMedicoEntrada entrada = new()
        {
            IdMedico = idMedico,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
        ConsultaAgendamentosPorIdMedicoCasoUso casoUso = new(repositorio.Object);

        // Act
        ConsultaAgendamentosPorIdMedicoSaida saida = await casoUso.ExecutarAsync(entrada);

        // Assert
        saida.Agendamentos.Should().NotBeNullOrEmpty();
        saida.QuantidadeItens.Should().Be(agendamentos.Item1.Count);
        saida.SituacaoConsultaAgendamentosPorIdMedico.Should().Be(SituacaoConsultaAgendamentosPorIdMedico.Sucesso);
        repositorio.Verify(r => r.ConsultarAgendamentosMedicoAsync(idMedico, pagina, tamanhoPagina), Times.Once());
    }
}