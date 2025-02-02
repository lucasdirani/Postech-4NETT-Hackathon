using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Servicos.Interfaces;

namespace Postech.Hackathon.Agendamentos.Dominio.Servicos;

public class ServicoAgendamento : IServicoAgendamento
{
    public bool ValidarConflitoHorarioAgendamento(
        IReadOnlyList<Agendamento> agendamentos, 
        TimeSpan horarioInicioNovoAgendamento, 
        TimeSpan horarioFimNovoAgendamento)
    {
        if (!agendamentos.TodosPossuemMesmaData())
        {
            throw new ExcecaoDominio("Os agendamentos devem estar na mesma data", nameof(ValidarConflitoHorarioAgendamento), nameof(Agendamento.Data));
        }
        return agendamentos.Any(a => horarioInicioNovoAgendamento < a.HorarioFim && horarioFimNovoAgendamento > a.HorarioInicio);
    }
}