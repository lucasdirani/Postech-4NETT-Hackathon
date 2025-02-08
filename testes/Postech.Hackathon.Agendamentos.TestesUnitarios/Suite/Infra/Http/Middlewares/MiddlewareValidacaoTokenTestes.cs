using System.Net;
using Microsoft.AspNetCore.Http;
using Moq;
using Postech.Hackathon.Agendamentos.Infra.Http.Clientes;
using Postech.Hackathon.Agendamentos.Infra.Http.Clientes.Modelos;
using Postech.Hackathon.Agendamentos.Infra.Http.Middlewares;
using Refit;

namespace Postech.Hackathon.Agendamentos.TestesUnitarios.Suite.Infra.Http.Middlewares;

public class MiddlewareValidacaoTokenTestes
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<IClienteMicrosservicoAutenticacao> _clienteMicrosservicoMock;
    private readonly MiddlewareValidacaoToken _middleware;

    public MiddlewareValidacaoTokenTestes()
    {
        _nextMock = new Mock<RequestDelegate>();
        _clienteMicrosservicoMock = new Mock<IClienteMicrosservicoAutenticacao>();
        _middleware = new MiddlewareValidacaoToken(_nextMock.Object, _clienteMicrosservicoMock.Object);
    }

    private static DefaultHttpContext CriarContextoHttp(string? token)
    {
        DefaultHttpContext contexto = new();
        contexto.Response.Body = new MemoryStream();
        contexto.Request.Headers["Authorization"] = token;
        return contexto;
    }

    [Fact(DisplayName = "Token ausente")]
    [Trait("Action", "InvokeAsync")]
    public async Task InvokeAsync_TokenAusente_DeveRetornar401Unauthorized()
    {
        // Arrange
        DefaultHttpContext contexto = CriarContextoHttp(null);

        // Act
        await _middleware.InvokeAsync(contexto);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, contexto.Response.StatusCode);
    }

    [Fact(DisplayName = "Token vazio")]
    [Trait("Action", "InvokeAsync")]
    public async Task InvokeAsync_TokenVazio_DeveRetornar401Unauthorized()
    {
        // Arrange
        DefaultHttpContext contexto = CriarContextoHttp("");

        // Act
        await _middleware.InvokeAsync(contexto);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, contexto.Response.StatusCode);
    }

    [Fact(DisplayName = "Token inválido")]
    [Trait("Action", "InvokeAsync")]
    public async Task InvokeAsync_TokenInvalido_DeveRetornar401Unauthorized()
    {
        // Arrange
        DefaultHttpContext contexto = CriarContextoHttp("Bearer invalido-token");
        ApiException excecao = await ApiException.Create(new HttpRequestMessage(), HttpMethod.Post, new HttpResponseMessage(HttpStatusCode.Unauthorized), new());
        _clienteMicrosservicoMock.Setup(s => s.ValidarTokenAsync(It.IsAny<RequisicaoValidacaoToken>())).ThrowsAsync(excecao);
            
        // Act
        await _middleware.InvokeAsync(contexto);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, contexto.Response.StatusCode);
    }

    [Fact(DisplayName = "Erro inesperado")]
    [Trait("Action", "InvokeAsync")]
    public async Task InvokeAsync_ErroInesperado_DeveRetornar500InternalServerError()
    {
        // Arrange
        DefaultHttpContext contexto = CriarContextoHttp("Bearer valid-token");
        _clienteMicrosservicoMock
            .Setup(s => s.ValidarTokenAsync(It.IsAny<RequisicaoValidacaoToken>()))
            .ThrowsAsync(new Exception("Erro inesperado"));

        // Act
        await _middleware.InvokeAsync(contexto);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, contexto.Response.StatusCode);
    }

    [Fact(DisplayName = "Token válido")]
    [Trait("Action", "InvokeAsync")]
    public async Task InvokeAsync_TokenValido_DeveChamarProximoMiddleware()
    {
        // Arrange
        DefaultHttpContext contexto = CriarContextoHttp("Bearer valid-token");
        _clienteMicrosservicoMock
            .Setup(s => s.ValidarTokenAsync(It.IsAny<RequisicaoValidacaoToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(contexto);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, contexto.Response.StatusCode);
        _nextMock.Verify(n => n(contexto), Times.Once);
    }
}