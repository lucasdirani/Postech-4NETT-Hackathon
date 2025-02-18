using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using FluentAssertions;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Constantes;
using Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores.Extensoes;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Auxiliares;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Configuracoes.Base;
using Postech.Hackathon.Agendamentos.TestesIntegrados.Fixtures;

namespace Postech.Hackathon.Agendamentos.TestesIntegrados.Suite.Infra.Controladores;

[Collection("Testes Integrados")]
public class ControladorAgendamentosTestes(IntegrationTestFixture fixture) : BaseTesteIntegracao(fixture)
{        
    [Fact(DisplayName = "Requisição para cadastrar um novo agendamento no endpoint /agendamentos")]
    [Trait("Action", "POST /agendamentos")]
    public async Task PostAgendamentos_RequisicaoValidaCadastroAgendamento_DeveRetornar201Created()
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
        HttpRequestMessage requisicao = new(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

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
    public async Task PostAgendamentos_RequisicaoSemCorpoCadastroAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        HttpRequestMessage requisicao = new(HttpMethod.Post, $"/agendamentos");
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);

        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

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
    public async Task PostAgendamentos_TipoDeConteudoNaoSuportado_DeveRetornar415UnsupportedMediaType()
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
        HttpRequestMessage requisicao = new(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(stringWriter.ToString(), Encoding.UTF8, "application/xml"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }
 
    [Fact(DisplayName = "Agendamento enviado para cadastro no endpoint /agendamentos está em conflito")]
    [Trait("Action", "POST /agendamentos")]
    public async Task PostAgendamentos_CadastroAgendamentoEmConflito_DeveRetornar409Conflict()
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
        HttpRequestMessage requisicao = new(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

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
    public async Task PostAgendamentos_RequisicaoDadosInvalidos_DeveRetornar400BadRequest()
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
        HttpRequestMessage requisicao = new(HttpMethod.Post, $"/agendamentos")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

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
    public async Task PutAgendamentos_RequisicaoValidaEdicaoAgendamento_DeveRetornar204NoContent()
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
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

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
    public async Task PutAgendamentos_RequisicaoSemCorpoEdicaoAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{idAgendamento}");
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task PutAgendamentos_RequisicaoDadosInvalidos_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 300;
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
            HoraFim = new TimeSpan(8, 30, 0),
            Valor = 0
        };
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Médico que enviou agendamento para edição no endpoint /agendamentos/{idAgendamento} não realizou o seu cadastro")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task PutAgendamentos_MedicoQueEditouAgendamentoNaoRealizouCadastro_DeveRetornar403Forbidden()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 300;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoEdicaoAgendamento comandoRequisicao = new()
        {
            IdMedico = Guid.NewGuid(),
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today),
            HoraInicio = new TimeSpan(15, 0, 0),
            HoraFim = new TimeSpan(15, 30, 0),
            Valor = 50
        };
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para edição no endpoint /agendamentos/{idAgendamento} não foi encontrado")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task PutAgendamentos_AgendamentoNaoEncontradoParaEdicao_DeveRetornar404NotFound()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        ComandoRequisicaoEdicaoAgendamento comandoRequisicao = new()
        {
            IdMedico = Guid.NewGuid(),
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today),
            HoraInicio = new TimeSpan(15, 0, 0),
            HoraFim = new TimeSpan(15, 30, 0),
            Valor = 50
        };
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{idAgendamento}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Médico que enviou agendamento para edição no endpoint /agendamentos/{idAgendamento} não realizou o seu cadastro")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task PutAgendamentos_EdicaoAgendamentoEmConflito_DeveRetornar409Conflict()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly novaDataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(4));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 300;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamentoEmEdicao = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        List<Agendamento> agendamentosNovaData =
        [
            new(idMedico, dataAgendamento: novaDataAgendamento, horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataCadastro, valorAgendamento),
            new(idMedico, dataAgendamento: novaDataAgendamento, horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataCadastro, valorAgendamento),
            new(idMedico, dataAgendamento: novaDataAgendamento, horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataCadastro, valorAgendamento),
            new(idMedico, dataAgendamento: novaDataAgendamento, horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataCadastro, valorAgendamento),
            new(idMedico, dataAgendamento: novaDataAgendamento, horarioInicioAgendamento: new(12, 0, 0), horarioFimAgendamento: new(12, 30, 0), dataCadastro, valorAgendamento),
        ];
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamentoEmEdicao);
        await repositorio.InserirAsync(agendamentosNovaData);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoEdicaoAgendamento comandoRequisicao = new()
        {
            IdMedico = idMedico,
            Data = novaDataAgendamento,
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today),
            HoraInicio = new TimeSpan(12, 0, 0),
            HoraFim = new TimeSpan(12, 30, 0),
            Valor = 150
        };
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{agendamentoEmEdicao.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para edição no endpoint /agendamentos/{idAgendamento} já foi aceito")]
    [Trait("Action", "PUT /agendamentos/{idAgendamento}")]
    public async Task PutAgendamentos_AgendamentoJaFoiAceito_DeveRetornar422UnprocessableEntity()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 300;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        agendamento.AceitarAgendamento(dataAceitacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoEdicaoAgendamento comandoRequisicao = new()
        {
            IdMedico = idMedico,
            Data = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today),
            HoraInicio = new TimeSpan(15, 0, 0),
            HoraFim = new TimeSpan(15, 30, 0),
            Valor = 50
        };
        HttpRequestMessage requisicao = new(HttpMethod.Put, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaEdicaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição para aceitar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_RequisicaoValidaAceitarAgendamento_DeveRetornar204NoContent()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Aceitar,
            IdUsuario = idMedico,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NoContent);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição para efetuar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_RequisicaoValidaEfetuarAgendamento_DeveRetornar204NoContent()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Efetuar,
            IdUsuario = idPaciente,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NoContent);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Corpo da requisição não fornecido no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_RequisicaoSemCorpoConfirmacaoAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{idAgendamento}");
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos para aceitar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_RequisicaoDadosInvalidosAceitarAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(-3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(-4));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(-4)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Aceitar,
            IdUsuario = idMedico,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today)
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos para efetuar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_RequisicaoDadosInvalidosEfetuarAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(-3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(-4));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Efetuar,
            IdUsuario = idPaciente,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today)
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Médico que enviou agendamento para aceitação no endpoint /agendamentos/{idAgendamento} não realizou o seu cadastro")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_MedicoQueAceitouAgendamentoNaoRealizouCadastro_DeveRetornar403Forbidden()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Aceitar,
            IdUsuario = Guid.NewGuid(),
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para aceitação no endpoint /agendamentos/{idAgendamento} não foi encontrado")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_AgendamentoNaoEncontradoParaAceitacao_DeveRetornar404NotFound()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        Guid idMedico = Guid.NewGuid();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Aceitar,
            IdUsuario = idMedico,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{idAgendamento}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para efetuação no endpoint /agendamentos/{idAgendamento} não foi encontrado")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_AgendamentoNaoEncontradoParaEfetuacao_DeveRetornar404NotFound()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Efetuar,
            IdUsuario = idPaciente,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{idAgendamento}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para efetuação no endpoint /agendamentos/{idAgendamento} está em conflito")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_EfetuacaoAgendamentoEmConflito_DeveRetornar409Conflict()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        Agendamento agendamentoEmConflito = new(idMedico: Guid.NewGuid(), dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamentoEmConflito.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        agendamentoEmConflito.AceitarAgendamento(dataAceitacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.InserirAsync(agendamentoEmConflito);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Efetuar,
            IdUsuario = idPaciente,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento para aceitação no endpoint /agendamentos/{idAgendamento} já foi aceito")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_AgendamentoEnviadoParaAceitacaoJaFoiAceito_DeveRetornar422UnprocessableEntity()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        agendamento.AceitarAgendamento(dataAceitacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Aceitar,
            IdUsuario = idMedico,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para efetuação no endpoint /agendamentos/{idAgendamento} já foi efetuado")]
    [Trait("Action", "PATCH /agendamentos/{idAgendamento}")]
    public async Task PatchAgendamentos_AgendamentoEnviadoParaEfetuacaoJaFoiEfetuado_DeveRetornar422UnprocessableEntity()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoConfirmacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoConfirmacaoAgendamento.Efetuar,
            IdUsuario = idPaciente,
            DataConfirmacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };
        HttpRequestMessage requisicao = new(HttpMethod.Patch, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConfirmacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição para consultar os agendamentos do médico no endpoint /agendamentos")]
    [Trait("Action", "GET /agendamentos")]
    public async Task GetAgendamentos_RequisicaoValidaConsultaAgendamentosMedico_DeveRetornar200Ok()
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
        ComandoRequisicaoConsultaAgendamentosPorIdMedico comandoRequisicao = new();
        int pagina = 1;
        int tamanhoPagina = 5; 
        HttpRequestMessage requisicao = new(HttpMethod.Get, $"/agendamentos?idMedico={idMedico}&pagina={pagina}&tamanhoPagina={tamanhoPagina}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.OK);
        ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Agendamentos.Should().HaveSameCount(agendamentos);
        conteudoMensagemResposta.Dados.PaginaAtual.Should().Be(pagina);
        conteudoMensagemResposta.Dados.TamanhoPagina.Should().Be(tamanhoPagina);
        conteudoMensagemResposta.Dados.QuantidadeItens.Should().Be(agendamentos.Count);
        conteudoMensagemResposta.Dados.TotalPaginas.Should().BeGreaterThan(0);
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos no endpoint /agendamentos")]
    [Trait("Action", "GET /agendamentos")]
    public async Task GetAgendamentos_RequisicaoDadosInvalidos_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idMedico = Guid.Empty;
        int pagina = 0;
        int tamanhoPagina = 0; 
        HttpRequestMessage requisicao = new(HttpMethod.Get, $"/agendamentos?idMedico={idMedico}&pagina={pagina}&tamanhoPagina={tamanhoPagina}");
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamentos não encontrados no endpoint /agendamentos")]
    [Trait("Action", "GET /agendamentos")]
    public async Task GetAgendamentos_AgendamentosNaoEncontrados_DeveRetornar404NotFound()
    {
        // Arrange
        DateOnly dataAtual = new(2025, 2, 1);
        decimal valorAgendamento = 100;
        List<Agendamento> agendamentos =
        [
            new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 0, 0), horarioFimAgendamento: new(9, 30, 0), dataAtual, valorAgendamento),
            new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(9, 30, 0), horarioFimAgendamento: new(10, 0, 0), dataAtual, valorAgendamento),
            new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 0, 0), horarioFimAgendamento: new(10, 30, 0), dataAtual, valorAgendamento),
            new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(10, 30, 0), horarioFimAgendamento: new(11, 0, 0), dataAtual, valorAgendamento),
            new(idMedico: Guid.NewGuid(), dataAgendamento: new(2025, 2, 2), horarioInicioAgendamento: new(12, 0, 0), horarioFimAgendamento: new(12, 30, 0), dataAtual, valorAgendamento),
        ];
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamentos);
        await repositorio.SalvarAlteracoesAsync();
        Guid idMedico = Guid.NewGuid();
        int pagina = 1;
        int tamanhoPagina = 5; 
        HttpRequestMessage requisicao = new(HttpMethod.Get, $"/agendamentos?idMedico={idMedico}&pagina={pagina}&tamanhoPagina={tamanhoPagina}");
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaConsultaAgendamentosPorIdMedico>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição para recusar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_RequisicaoValidaRecusarAgendamento_DeveRetornar204NoContent()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Recusar,
            IdUsuario = idMedico,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Estarei em plantão no hospital"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NoContent);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição para cancelar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_RequisicaoValidaCancelarAgendamento_DeveRetornar204NoContent()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Cancelar,
            IdUsuario = idPaciente,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Reunião importante no trabalho"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NoContent);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().NotBeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeTrue();
        conteudoMensagemResposta.Mensagens.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName = "Corpo da requisição não fornecido no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_RequisicaoSemCorpoAnulacaoAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{idAgendamento}");
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos para recusar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_RequisicaoDadosInvalidosRecusarAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(-3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(-4));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(-4)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Recusar,
            IdUsuario = idMedico,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today),
            Justificativa = string.Empty
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Requisição enviada com dados inválidos para cancelar um agendamento no endpoint /agendamentos/{idAgendamento}")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_RequisicaoDadosInvalidosCancelarAgendamento_DeveRetornar400BadRequest()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(-3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(-4));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(-4)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Cancelar,
            IdUsuario = idPaciente,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today),
            Justificativa = string.Empty
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Médico que enviou agendamento para recusa no endpoint /agendamentos/{idAgendamento} não realizou o seu cadastro")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_MedicoQueRecusouAgendamentoNaoRealizouCadastro_DeveRetornar403Forbidden()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Recusar,
            IdUsuario = Guid.NewGuid(),
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Estarei em plantão no hospital"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Paciente que enviou agendamento para cancelamento no endpoint /agendamentos/{idAgendamento} não efetuou")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_PacienteQueCancelouAgendamentoNaoEfetuou_DeveRetornar403Forbidden()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Cancelar,
            IdUsuario = idPaciente,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Reunião importante no trabalho"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para recusa no endpoint /agendamentos/{idAgendamento} não foi encontrado")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_AgendamentoNaoEncontradoParaRecusa_DeveRetornar404NotFound()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        Guid idMedico = Guid.NewGuid();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Recusar,
            IdUsuario = idMedico,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Estarei em plantão no hospital"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{idAgendamento}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para cancelamento no endpoint /agendamentos/{idAgendamento} não foi encontrado")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_AgendamentoNaoEncontradoParaCancelamento_DeveRetornar404NotFound()
    {
        // Arrange
        Guid idAgendamento = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Cancelar,
            IdUsuario = idPaciente,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Reunião importante no trabalho"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{idAgendamento}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento para recusa no endpoint /agendamentos/{idAgendamento} já foi recusado")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_AgendamentoEnviadoParaRecusaJaFoiRecusado_DeveRetornar422UnprocessableEntity()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente: Guid.NewGuid(), dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        agendamento.RecusarAgendamento(dataRecusaAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)), justificativaRecusa: "Problemas familiares");
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Recusar,
            IdUsuario = idMedico,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Problemas familiares"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoMedico);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Agendamento enviado para cancelamento no endpoint /agendamentos/{idAgendamento} já foi cancelado")]
    [Trait("Action", "DELETE /agendamentos/{idAgendamento}")]
    public async Task DeleteAgendamentos_AgendamentoEnviadoParaCancelamentoJaFoiCancelado_DeveRetornar422UnprocessableEntity()
    {
        // Arrange
        Guid idMedico = Guid.NewGuid();
        Guid idPaciente = Guid.NewGuid();
        DateOnly dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        decimal valorAgendamento = 100;
        TimeSpan horarioInicioAgendamento = new(9, 0, 0);
        TimeSpan horarioFimAgendamento = new(10, 0, 0);
        Agendamento agendamento = new(idMedico, dataAgendamento, horarioInicioAgendamento, horarioFimAgendamento, dataCadastro, valorAgendamento);
        agendamento.EfetuarAgendamento(idPaciente, dataEfetuacaoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        agendamento.CancelarAgendamento(dataCancelamentoAgendamento: DateOnly.FromDateTime(DateTime.Today.AddDays(2)), justificativaCancelamento: "Reunião importante no trabalho");
        IRepositorioAgendamento repositorio = ObterServico<IRepositorioAgendamento>();
        await repositorio.InserirAsync(agendamento);
        await repositorio.SalvarAlteracoesAsync();
        ComandoRequisicaoAnulacaoAgendamento comandoRequisicao = new()
        {
            Acao = AcaoAnulacaoAgendamento.Cancelar,
            IdUsuario = idPaciente,
            DataAnulacao = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Justificativa = "Reunião importante no trabalho"
        };
        HttpRequestMessage requisicao = new(HttpMethod.Delete, $"/agendamentos/{agendamento.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(comandoRequisicao), Encoding.UTF8, "application/json"),
        };
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenAuxiliar.TokenValidoPaciente);
        
        // Act
        using HttpResponseMessage mensagemResposta = await ClienteHttp.SendAsync(requisicao);

        // Assert
        mensagemResposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>? conteudoMensagemResposta = await mensagemResposta.Content.AsAsync<ComandoRespostaGenerico<ComandoRespostaAnulacaoAgendamento>>();
        conteudoMensagemResposta.Should().NotBeNull();
        conteudoMensagemResposta.Dados.Should().BeNull();
        conteudoMensagemResposta.FoiProcessadoComSucesso.Should().BeFalse();
        conteudoMensagemResposta.Mensagens.Should().NotBeNullOrEmpty();
    }
}