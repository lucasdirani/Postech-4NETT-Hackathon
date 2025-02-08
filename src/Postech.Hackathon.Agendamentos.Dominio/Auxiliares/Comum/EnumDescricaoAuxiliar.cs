using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Postech.Hackathon.Agendamentos.Dominio.Auxiliares.Comum;

[ExcludeFromCodeCoverage]
public static class EnumDescricaoAuxiliar<T> 
    where T : Enum
{
    public static T ObterValor(string descricao)
    {
        var tipo = typeof(T);
        if (!tipo.IsEnum)
        {
            throw new InvalidOperationException($"{tipo.Name} não é um Enum");
        }
        foreach (FieldInfo propriedade in tipo.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (propriedade.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute attribute 
                && attribute.Description == descricao)
            {
                object? valor = propriedade.GetValue(null);
                return valor is null ? throw new InvalidOperationException($"Descrição não pode ser convertida '{descricao}'.") : (T) valor;
            }
        }
        throw new ArgumentException($"Nenhum valor possui a descrição '{descricao}'.");
    }
}