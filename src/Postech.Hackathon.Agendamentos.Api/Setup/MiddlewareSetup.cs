using System.Diagnostics.CodeAnalysis;
using Postech.Hackathon.Agendamentos.Infra.Http.Middlewares;

namespace Postech.Hackathon.Agendamentos.Api.Setup;

[ExcludeFromCodeCoverage]
internal static class MiddlewareSetup
{
    internal static IApplicationBuilder  UsarMiddlewareValidacaoToken(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MiddlewareValidacaoToken>();
    }
}