using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class AceitacaoAgendamentoCasoUso(IRepositorioAgendamento repositorio) : IAceitacaoAgendamentoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;

    public async Task<AceitacaoAgendamentoSaida> ExecutarAsync(AceitacaoAgendamentoEntrada entrada)
    {
        Agendamento? agendamento = await _repositorio.ObterPorIdAsync(entrada.IdAgendamento);
        agendamento.AceitarAgendamento(entrada.DataAceitacao);
        _repositorio.Atualizar(agendamento);
        await _repositorio.SalvarAlteracoesAsync();
        return new() { SituacaoAceitacaoAgendamento = SituacaoAceitacaoAgendamento.Sucesso };
    }
}