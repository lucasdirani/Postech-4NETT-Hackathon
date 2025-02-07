using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;

public class EdicaoAgendamentoCasoUso(IRepositorioAgendamento repositorio, IServicoAgendamento servicoAgendamento) 
    : IEdicaoAgendamentoCasoUso
{
    private readonly IRepositorioAgendamento _repositorio = repositorio;
    private readonly IServicoAgendamento _servicoAgendamento = servicoAgendamento;

    public async Task<EdicaoAgendamentoSaida> ExecutarAsync(EdicaoAgendamentoEntrada entrada)
    {
        try
        {
            Agendamento? agendamento = await _repositorio.ObterPorIdAsync(entrada.IdAgendamento);
            if (agendamento is null) return new() { SituacaoEdicaoAgendamento = SituacaoEdicaoAgendamento.AgendamentoNaoEncontrado, Mensagem = "Agendamento inexistente" };
            if (!agendamento.PertenceMedico(entrada.IdMedico)) return new() { SituacaoEdicaoAgendamento = SituacaoEdicaoAgendamento.EdicaoNaoPermitida, Mensagem = "Edição não permitida" };
            agendamento.AlterarDataAgendamento(entrada.DataAgendamento, entrada.DataAtualizacao);
            agendamento.AlterarHorarioAgendamento(entrada.HorarioInicioAgendamento, entrada.HorarioFimAgendamento);
            agendamento.AlterarValorAgendamento(entrada.ValorAgendamento);
            IReadOnlyList<Agendamento> agendamentos = await _repositorio.ConsultarAgendamentosMedicoAsync(entrada.IdMedico, entrada.DataAgendamento);
            bool possuiConflito = _servicoAgendamento.ValidarConflitoHorarioAgendamento(agendamentos, entrada.HorarioInicioAgendamento, entrada.HorarioFimAgendamento);
            if (possuiConflito) return new() { SituacaoEdicaoAgendamento = SituacaoEdicaoAgendamento.Conflito };
            _repositorio.Atualizar(agendamento);
            await _repositorio.SalvarAlteracoesAsync();
            return new() { SituacaoEdicaoAgendamento = SituacaoEdicaoAgendamento.Sucesso };
        }
        catch (ExcecaoDominio ex)
        {
            return new() { SituacaoEdicaoAgendamento = SituacaoEdicaoAgendamento.DadosInvalidos, Mensagem = ex.Mensagem };
        }
    }
}