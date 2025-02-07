using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using FluentAssertions;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores.Extensoes;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Base;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Fixtures;

namespace Postech.Hackathon.Agendamentos.TestesIntegrados.Suite.Infra.Controladores;

[Collection("Testes Integrados")]
public class ControladorAgendamentosTestes(IntegrationTestFixture fixture) : BaseTesteIntegracao(fixture)
{        
    [Fact(DisplayName = "Requisição para cadastrar um novo agendamento no endpoint /agendamentos")]
    [Trait("Action", "POST /agendamentos")]
    public async Task Agendamentos_RequisicaoValidaCadastroAgendamento_DeveRetornar201Created()
    {
        // Arrange
        ComandoRequisicaoCadastroAgendamento comandoRequisicao = new()
        {
            IdMedico = Guid.NewGuid(),
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            HoraInicio = new TimeSpan(12, 0, 0),
            HoraFim = new TimeSpan(12, 30, 0),
            Valor = 100
        };
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        });

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Created);
        ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
        conteudoMensagemResposta.Dados.IdAgendamento.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Corpo da requisição não fornecido no endpoint /agendamentos")]
    [Trait("Action", "POST /agendamentos")]
    public async Task Agendamentos_RequisicaoSemCorpoCadastroAgendamento_DeveRetornar400BadRequest()
    {
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/agendamentos"));

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Tipo de conteúdo enviado para o endpoint /agendamentos não é suportado")]
    [Trait("Action", "POST /agendamentos")]
    public async Task Agendamentos_TipoDeConteudoNaoSuportado_DeveRetornar415UnsupportedMediaType()
    {
        // Arrange
        ComandoRequisicaoCadastroAgendamento comandoRequisicao = new()
        {
            IdMedico = Guid.NewGuid(),
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            HoraInicio = new TimeSpan(12, 0, 0),
            HoraFim = new TimeSpan(12, 30, 0),
            Valor = 100
        };
        XmlSerializer serializador = new(typeof(ComandoRequisicaoCadastroAgendamento));
        using StringWriter stringWriter = new();
        serializador.Serialize(stringWriter, comandoRequisicao);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(stringWriter.ToString(), Encoding.UTF8, "application/xml"),
        });

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }
 
    [Fact(DisplayName = "Agendamento enviado para cadastro no endpoint /agendamentos está em conflito")]
    [Trait("Action", "POST /agendamentos")]
    public async Task Agendamentos_CadastroAgendamentoEmConflito_DeveRetornar409Conflict()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        List<Agendamento> agendamentos =
        [
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataAtual, valorAgendamento),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataAtual, valorAgendamento),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataAtual, valorAgendamento),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataAtual, valorAgendamento),
            new(idMedico, dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(12, 0, 0), horarioFimAgendamento: new(12, 30, 0), dataAtual, valorAgendamento),
        ];
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamentos);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoCadastroAgendamento comandoRequisicao = new()
        {
            IdMedico = idMedico,
            Data = new(2025, 2, 2),
            HoraInicio = new TimeSpan(12, 0, 0),
            HoraFim = new TimeSpan(12, 30, 0),
            Valor = valorAgendamento,
            DataAtual = dataAtual
        };
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        });

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos no endpoint /agendamentos")]
    [Trait("Action", "POST /agendamentos")]
    public async Task Agendamentos_RequisicaoDadosInvalidos_DeveRetornar400BadRequest()
    {
        // Arrange
        ComandoRequisicaoCadastroAgendamento comandoRequisicao = new()
        {
            IdMedico = Guid.Empty,
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            HoraInicio = new TimeSpan(12, 0, 0),
            HoraFim = new TimeSpan(12, 30, 0),
            Valor = 0
        };
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        });

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaCadastroAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição para editar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task Agendamentos_RequisicaoValidaEdicaoAgendamento_DeveRetornar204NoContent()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoEdicaoAgendamento comandoRequisicao = new()
        {
            IdMedico = idMedico,
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today),
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFim = new TimeSpan(9, 30, 0),
            Valor = 200
        };
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Put, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        });

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NoContent);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Corpo da requisição não fornecido no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task Agendamentos_RequisicaoSemCorpoEdicaoAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(new HttpRequestMessage(HttpMethod.Put, $"/agendamentos/{idAgendamento}"));

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }
}