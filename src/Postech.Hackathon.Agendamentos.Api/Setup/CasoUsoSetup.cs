using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Interfaces;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class CasoUsoSetup
{
    internal static void AdicionarDependenciaCasoUso(this IServiceCollection servicos)
    {
        servicos.AddScoped<ICadastroAgendamentoCasoUso, CadastroAgendamentoCasoUso>();
        servicos.AddScoped<IEdicaoAgendamentoCasoUso, EdicaoAgendamentoCasoUso>();
        servicos.AddScoped<IEfetuacaoAgendamentoCasoUso, EfetuacaoAgendamentoCasoUso>();
        servicos.AddScoped<IAceitacaoAgendamentoCasoUso, AceitacaoAgendamentoCasoUso>();
        servicos.AddScoped<IRecusaAgendamentoCasoUso, RecusaAgendamentoCasoUso>();
        servicos.AddScoped<ICancelaAgendamentoCasoUso, CancelaAgendamentoCasoUso>();
        servicos.AddScoped<IConsultaAgendamentosPorIdMedicoCasoUso, ConsultaAgendamentosPorIdMedicoCasoUso>();
    }
}