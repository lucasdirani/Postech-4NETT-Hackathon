using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas.Extensoes;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class RecusaAgendamentoCasoUso(IRepositorioAgendamento repositorio) : IRecusaAgendamentoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;

    public async Task<RecusaAgendamentoSaida> ExecutarAsync(RecusaAgendamentoEntrada entrada)
    {
        try
        {
            Agendamento? agendamento = await _repositorio.ObterPorIdAsync(entrada.IdAgendamento);
            if (agendamento is null) return new() { SituacaoRecusaAgendamento = SituacaoRecusaAgendamento.AgendamentoNaoEncontrado, Mensagem = "Agendamento inexistente" };
            if (!agendamento.PertenceMedico(entrada.IdMedico)) return new() { SituacaoRecusaAgendamento = SituacaoRecusaAgendamento.RecusaNaoPermitida, Mensagem = "Recusa não permitida" };
            agendamento.RecusarAgendamento(entrada.DataRecusa, entrada.JustificativaRecusa);
            _repositorio.Atualizar(agendamento);
            await _repositorio.SalvarAlteracoesAsync();
            return new() { SituacaoRecusaAgendamento = SituacaoRecusaAgendamento.Sucesso };
        }
        catch (ExcecaoDominio ex)
        {
            return new() { SituacaoRecusaAgendamento = ex.ObterSituacaoRecusaAgendamento(), Mensagem = ex.Mensagem };
        }
    }
}