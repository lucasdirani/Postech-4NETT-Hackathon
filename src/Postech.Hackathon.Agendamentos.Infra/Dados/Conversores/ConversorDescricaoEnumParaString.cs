using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Postech.Hackathon.Agendamentos.Dominio.Auxiliares.Comum;
using Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

namespace Postech.Hackathon.Agendamentos.Infra.Dados.Conversores;

[ExcludeFromCodeCoverage]
internal class ConversorDescricaoEnumParaString<TEnum>(ConverterMappingHints? mappingHints = null) : ValueConverter<TEnum, string>(
          valorEnum => valorEnum.ObterDescricao(),
          valorString => EnumDescricaoAuxiliar<TEnum>.ObterValor(valorString),
          mappingHints)
        where TEnum : Enum
{
}