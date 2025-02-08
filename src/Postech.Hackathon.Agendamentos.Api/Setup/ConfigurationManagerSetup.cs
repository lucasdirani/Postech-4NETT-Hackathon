using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class ConfigurationManagerSetup
{
    internal static void AdicionarArquivoJsonPeloAmbiente(this ConfigurationManager configuracao, string nomeAmbiente)
    {
        configuracao.AddJsonFile($"appsettings.{nomeAmbiente}.json", optional: false, reloadOnChange: true);
    }
}