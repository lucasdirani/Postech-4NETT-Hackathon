using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Enumeradores;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Excecoes.Comum;

namespace Postech.Hackathon.Agendamentos.Aplicacao.CasosUso.Saidas.Extensoes;

[ExcludeFromCodeCoverage]
internal static class ExcecaoDominioExtensoes
{
    public static SituacaoEdicaoAgendamento ObterSituacaoEdicaoAgendamento(this ExcecaoDominio excecao)
    {
        return excecao.Propriedade switch
        {
            nameof(Agendamento.Situacao) => SituacaoEdicaoAgendamento.EdicaoNaoProcessavel,
            _ => SituacaoEdicaoAgendamento.DadosInvalidos
        };
    }

    public static SituacaoEfetuacaoAgendamento ObterSituacaoEfetuacaoAgendamento(this ExcecaoDominio excecao)
    {
        return excecao.Propriedade switch
        {
            nameof(Agendamento.Situacao) => SituacaoEfetuacaoAgendamento.EfetuacaoNaoProcessavel,
            _ => SituacaoEfetuacaoAgendamento.DadosInvalidos
        };
    }
}