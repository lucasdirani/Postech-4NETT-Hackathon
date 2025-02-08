using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Postech.Hackathon.Agendamentos.Api.HealthChecks;

[ExcludeFromCodeCoverage]
internal static class PostgreSQLHealthCheck
{
    internal static void AdicionarHealthCheckPostgres(this IHealthChecksBuilder healthChecks, IConfiguration configuracao)
    {
        string? stringConexao = configuracao.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(stringConexao))
        {
            throw new InvalidOperationException("String de conexão ausente.");
        }
        healthChecks.AddNpgSql(connectionString: stringConexao, name: nameof(PostgreSQLHealthCheck), failureStatus: HealthStatus.Unhealthy);
    }
}