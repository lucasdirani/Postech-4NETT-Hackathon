using Postech.Hackathon.Agendamentos.Dominio.Enumeradores;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Dominio.Entidades;

public class Agendamento : EntidadeBase
{
    public Agendamento(
        Guid idMedico,
        DateOnly dataAgendamento,
        TimeSpan horarioInicioAgendamento,
        TimeSpan horarioFimAgendamento,
        DateOnly dataAtual,
        decimal valorAgendamento)
    {
        if (idMedico.EstaVazio())
        {
            throw new ExcecaoDominio("O identificador do médico é obrigatório", nameof(Agendamento), nameof(IdMedico));
        }
        if (dataAgendamento <= dataAtual)
        {
            throw new ExcecaoDominio("A data de agendamento deve ser maior do que a data atual", nameof(Agendamento), nameof(Data));
        }
        if (horarioInicioAgendamento >= horarioFimAgendamento)
        {
            throw new ExcecaoDominio("O horário de início deve ser menor do que o horário de fim", nameof(Agendamento), nameof(HorarioInicio));
        }
        if (valorAgendamento <= 0)
        {
            throw new ExcecaoDominio("O valor do agendamento deve ser maior do que zero", nameof(Agendamento), nameof(Valor));
        }
        IdMedico = idMedico;
        Data = dataAgendamento;
        HorarioInicio = horarioInicioAgendamento;
        HorarioFim = horarioFimAgendamento;
        Valor = valorAgendamento;
        Situacao = SituacaoAgendamento.Criado;
    }

    private Agendamento() {}

    public Guid IdMedico { get; private set; }       
    public Guid? IdPaciente { get; private set; }
    public DateOnly Data { get; private set; }
    public TimeSpan HorarioInicio { get; private set; }
    public TimeSpan HorarioFim { get; private set; }
    public decimal Valor { get; private set; }
    public SituacaoAgendamento Situacao { get; private set; }
    public string? JustificativaRecusa { get; private set; }

    public void AceitarAgendamento(DateOnly dataAceitacaoAgendamento)
    {
        if (dataAceitacaoAgendamento >= Data)
        {
            throw new ExcecaoDominio("A data de aceitação deve ser anterior a data do agendamento", nameof(AceitarAgendamento), nameof(dataAceitacaoAgendamento));
        }
        if (AgendamentoFoiEfetuado())
        {
            Situacao = SituacaoAgendamento.Aceito;
            ModificadoEm = DateTime.UtcNow;
            return;
        }    
        throw new ExcecaoDominio("O agendamento não pode ser aceito", nameof(AceitarAgendamento), nameof(Situacao));
    }

    public void AlterarDataAgendamento(DateOnly novaDataAgendamento, DateOnly dataAtualizacaoAgendamento)
    {
        if (novaDataAgendamento == Data)
        {
            return;
        }
        if (novaDataAgendamento <= dataAtualizacaoAgendamento)
        {
            throw new ExcecaoDominio("A data de agendamento deve ser maior do que a data de atualização", nameof(AlterarDataAgendamento), nameof(novaDataAgendamento));
        }
        if (AgendamentoFoiAceito() || AgendamentoFoiEfetuado())
        {
            throw new ExcecaoDominio("A data não pode ser alterada com o agendamento aceito ou efetuado", nameof(AlterarDataAgendamento), nameof(Situacao));
        }
        Data = novaDataAgendamento;
        ModificadoEm = DateTime.UtcNow;
    }

    private bool AgendamentoFoiEfetuado()
    {
        return Situacao is SituacaoAgendamento.Efetuado;
    }

    public void EfetuarAgendamento(Guid idPaciente, DateOnly dataEfetuacaoAgendamento)
    {
        if (idPaciente.EstaVazio())
        {
            throw new ExcecaoDominio("O identificador do paciente é obrigatório", nameof(EfetuarAgendamento), nameof(idPaciente));
        }
        if (dataEfetuacaoAgendamento >= Data)
        {
            throw new ExcecaoDominio("A data de efetuação deve ser anterior a data do agendamento", nameof(EfetuarAgendamento), nameof(dataEfetuacaoAgendamento));
        }
        if (AgendamentoEstaCriado())
        {
            Situacao = SituacaoAgendamento.Efetuado;
            IdPaciente = idPaciente;
            ModificadoEm = DateTime.UtcNow;
            return;
        }
        throw new ExcecaoDominio("O agendamento não pode ser efetuado", nameof(EfetuarAgendamento), nameof(Situacao));
    }

    private bool AgendamentoEstaCriado()
    {
        return Situacao is SituacaoAgendamento.Criado;
    }

    private bool AgendamentoFoiAceito()
    {
        return Situacao is SituacaoAgendamento.Aceito;
    }

    public void AlterarHorarioAgendamento(TimeSpan novoHorarioInicioAgendamento, TimeSpan novoHorarioFimAgendamento)
    {
        if (novoHorarioInicioAgendamento == HorarioInicio && novoHorarioFimAgendamento == HorarioFim)
        {
            return;
        }
        if (novoHorarioInicioAgendamento >= novoHorarioFimAgendamento)
        {
            throw new ExcecaoDominio("O horário de início deve ser menor do que o horário de fim", nameof(AlterarHorarioAgendamento), nameof(novoHorarioInicioAgendamento));
        }
        if (AgendamentoFoiAceito() || AgendamentoFoiEfetuado())
        {
            throw new ExcecaoDominio("O horário não pode ser alterado com o agendamento aceito ou efetuado", nameof(AlterarHorarioAgendamento), nameof(Situacao));
        }
        HorarioInicio = novoHorarioInicioAgendamento;
        HorarioFim = novoHorarioFimAgendamento;
        ModificadoEm = DateTime.UtcNow;
    }

    public void AlterarValorAgendamento(decimal novoValorAgendamento)
    {
        if (novoValorAgendamento == Valor)
        {
            return;
        }
        if (novoValorAgendamento <= 0)
        {
            throw new ExcecaoDominio("O valor do agendamento deve ser maior do que zero", nameof(AlterarValorAgendamento), nameof(novoValorAgendamento));
        }
        if (AgendamentoFoiAceito() || AgendamentoFoiEfetuado())
        {
            throw new ExcecaoDominio("O valor não pode ser alterado com o agendamento aceito ou efetuado", nameof(AlterarValorAgendamento), nameof(Situacao));
        }
        Valor = novoValorAgendamento;
        ModificadoEm = DateTime.UtcNow;
    }

    public bool PertenceMedico(Guid idMedico)
    {
        return IdMedico == idMedico;
    }

    public void RecusarAgendamento(DateOnly dataRecusaAgendamento, string justificativaRecusa)
    {
        ModificadoEm = DateTime.UtcNow;
        JustificativaRecusa = justificativaRecusa;
        Situacao = SituacaoAgendamento.Recusado;
    }
}