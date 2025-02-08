using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Postech.Hackathon.Agendamentos.Infra.Http.Clientes;
using Postech.Hackathon.Agendamentos.Infra.Http.Clientes.Modelos;
using Refit;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Middlewares;

public class MiddlewareValidacaoToken(RequestDelegate next, IClienteMicrosservicoAutenticacao microsservicoAutenticacao)
{
    private readonly RequestDelegate _next = next;
    private readonly IClienteMicrosservicoAutenticacao _microsservicoAutenticacao = microsservicoAutenticacao;

    public async Task InvokeAsync(HttpContext contexto)
    {
        if (!contexto.Request.Headers.TryGetValue("Authorization", out StringValues token) || string.IsNullOrWhiteSpace(token))
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        string? valorToken = token.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        try
        {
            await _microsservicoAutenticacao.ValidarTokenAsync(new RequisicaoValidacaoToken { Token = valorToken });
            await _next(contexto);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (Exception)
        {
            contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}