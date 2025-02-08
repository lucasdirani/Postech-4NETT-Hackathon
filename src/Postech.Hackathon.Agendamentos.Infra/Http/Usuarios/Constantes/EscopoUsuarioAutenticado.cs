using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Constantes;

[ExcludeFromCodeCoverage]
public static class EscopoUsuarioAutenticado
{
    public static readonly string CadastroAgendamento = "medico_agendamento.write";
    public static readonly string EditaAgendamento = "medico_agendamento.write";
    public static readonly string ConsultaAgendamento = "paciente_agendamento.read";
    public static readonly string AceitaAgendamento = "medico_agendamento.write";
    public static readonly string RecusaAgendamento = "medico_agendamento.write";
    public static readonly string EfetuaAgendamento = "paciente_agendamento.write";
    public static readonly string CancelaAgendamento = "paciente_agendamento.write";
}