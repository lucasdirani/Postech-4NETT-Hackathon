using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Dominio.Repositorios;

public interface IRepositorioAgendamento : IRepositorio<Agendamento, Guid>
{
    Task<IReadOnlyList<Agendamento>> ConsultarAgendamentosMedicoAsync(Guid idMedico, DateOnly dataAgendamento);
    Task<IReadOnlyList<Agendamento>> ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(Guid idPaciente, DateOnly dataAgendamento);
}