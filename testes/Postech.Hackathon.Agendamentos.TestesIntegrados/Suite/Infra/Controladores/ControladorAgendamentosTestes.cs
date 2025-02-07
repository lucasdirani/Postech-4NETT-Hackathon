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
    [Trait("Action", "/agendamentos")]
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
    [Trait("Action", "/agendamentos")]
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
    [Trait("Action", "/agendamentos")]
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
    [Trait("Action", "/agendamentos")]
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
}