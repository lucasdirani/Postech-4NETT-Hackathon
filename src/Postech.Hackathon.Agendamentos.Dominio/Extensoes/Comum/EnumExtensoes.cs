using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Postech.Hackathon.Agendamentos.Dominio.Extensoes.Comum;

[ExcludeFromCodeCoverage]
public static class EnumeratorExtensions
{
    public static string ObterDescricao(this Enum valor)
    {
        FieldInfo? propriedade = valor.GetType().GetField(valor.ToString());
        if (propriedade is null) return string.Empty;
        DescriptionAttribute[] atributos = (DescriptionAttribute[])propriedade.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return atributos.Length > 0 ? atributos[0].Description : valor.ToString();
    }
}