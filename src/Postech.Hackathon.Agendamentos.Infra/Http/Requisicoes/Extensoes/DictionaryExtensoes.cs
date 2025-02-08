using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Entradas;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Requisicoes.Extensoes;

[ExcludeFromCodeCoverage]
public static class DictionaryExtensoes
{
    internal static ConsultaAgendamentosPorIdMedicoEntrada ConverterParaCadastroAgendamentoEntrada(this IDictionary<string, StringValues> valoresConsulta)
    {
        if (valoresConsulta is null || !valoresConsulta.Any())
        {
            return new();
        }
        _ = valoresConsulta.TryGetValue("idMedico", out StringValues valorIdMedico);
        _ = Guid.TryParse(valorIdMedico, out Guid idMedico);
        _ = valoresConsulta.TryGetValue("pagina", out StringValues valorPagina);
        _ = int.TryParse(valorPagina, out int pagina);
        _ = valoresConsulta.TryGetValue("tamanhoPagina", out StringValues valorTamanhoPagina);
        _ = int.TryParse(valorTamanhoPagina, out int tamanhoPagina);
        return new()
        {
            IdMedico = idMedico,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }
}