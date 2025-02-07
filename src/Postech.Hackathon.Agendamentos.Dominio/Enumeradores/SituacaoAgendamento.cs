using System.ComponentModel;

namespace Postech.Hackathon.Agendamentos.Dominio.Enumeradores;

public enum SituacaoAgendamento
{
    [Description("CRIADO")]
    Criado,
    [Description("ACEITO")]
    Aceito,
}