using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Projecoes;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class ConsultaAgendamentosPorIdMedicoCasoUso(IRepositorioAgendamento repositorio) 
    : IConsultaAgendamentosPorIdMedicoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;

    public async Task<ConsultaAgendamentosPorIdMedicoSaida> ExecutarAsync(ConsultaAgendamentosPorIdMedicoEntrada entrada)
    {
        if (entrada.NaoEstaValida()) 
        {
            return new() { 
                SituacaoConsultaAgendamentosPorIdMedico = SituacaoConsultaAgendamentosPorIdMedico.DadosInvalidos, 
                Mensagem = "Pelo menos um dado de entrada está inválido",
                PaginaAtual = entrada.Pagina,
                TamanhoPagina = entrada.TamanhoPagina
            };
        }
        (IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico>, int) agendamentos = await _repositorio.ConsultarAgendamentosMedicoAsync(entrada.IdMedico, entrada.Pagina, entrada.TamanhoPagina);
        if (!agendamentos.Item1.Any())
        {
            return new() { 
                SituacaoConsultaAgendamentosPorIdMedico = SituacaoConsultaAgendamentosPorIdMedico.AgendamentoNaoEncontrado, 
                Mensagem = "Nenhum agendamento foi localizado para o médico",
                PaginaAtual = entrada.Pagina,
                TamanhoPagina = entrada.TamanhoPagina
            };
        }
        return new() {
            Agendamentos = agendamentos.Item1,
            QuantidadeItens = agendamentos.Item2,
            TotalPaginas = (int)Math.Ceiling((double)agendamentos.Item2 / entrada.TamanhoPagina),
            PaginaAtual = entrada.Pagina,
            TamanhoPagina = entrada.TamanhoPagina,
            SituacaoConsultaAgendamentosPorIdMedico = SituacaoConsultaAgendamentosPorIdMedico.Sucesso
        };
    }
}