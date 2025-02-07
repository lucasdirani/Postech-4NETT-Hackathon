using FluentAssertions;
using Moq;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Enumeradores;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Projecoes;
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
        int pagina = 1;
        int tamanhoPagina = 5;
        IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico> agendamentosProjecao = [
            new() { 
                IdAgendamento = Guid.NewGuid(),
                Situacao = SituacaoAgendamento.Aceito.ObterDescricao(), 
                Data = DateOnly.FromDateTime(DateTime.Today), 
                HoraFim = new(12, 0, 0), 
                HoraInicio = new(11, 0, 0) 
            },
            new() { 
                IdAgendamento = Guid.NewGuid(),
                Situacao = SituacaoAgendamento.Criado.ObterDescricao(), 
                Data = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 
                HoraFim = new(15, 0, 0), 
                HoraInicio = new(14, 0, 0) 
            }
        ];
        (IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico>, int) agendamentos = new(agendamentosProjecao, agendamentosProjecao.Count); 
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