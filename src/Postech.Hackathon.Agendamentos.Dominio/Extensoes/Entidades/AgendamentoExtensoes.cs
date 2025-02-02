using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Dominio.Extensoes.Entidades;

[ExcludeFromCodeCoverage]
public static class AgendamentoExtensoes
{
    public static bool TodosPossuemMesmaData(this IEnumerable<Agendamento> agendamentos)
    {
        if (agendamentos.Any())
        {
            DateOnly dataReferencia = agendamentos.ElementAt(0).Data;
            return agendamentos.All(a => a.Data == dataReferencia);
        }
        return true;
    }
}