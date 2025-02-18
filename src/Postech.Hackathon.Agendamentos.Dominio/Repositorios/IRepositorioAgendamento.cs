using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Projecoes;

namespace Postech.Hackathon.Agendamentos.Dominio.Repositorios;

public interface IRepositorioAgendamento : IRepositorio<Agendamento, Guid>
{
    Task<IReadOnlyList<Agendamento>> ConsultarAgendamentosMedicoAsync(Guid idMedico, DateOnly dataAgendamento);
    Task<IReadOnlyList<Agendamento>> ConsultarAgendamentosEfetuadosOuAceitosDoPacienteAsync(Guid idPaciente, DateOnly dataAgendamento);
    Task<(IReadOnlyList<ProjecaoConsultaAgendamentosPorIdMedico>, int)> ConsultarAgendamentosMedicoAsync(Guid idMedico, int pagina, int tamanhoPagina);
}