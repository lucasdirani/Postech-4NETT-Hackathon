using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos;
using Postech.Hackathon.Agendamentos.Infra.Controladores.Http.Comandos.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores;
using Postech.Hackathon.Agendamentos.Infra.Http.Deserializadores.Excecoes;
using Postech.Hackathon.Agendamentos.Infra.Http.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Requisicoes.Extensoes;
using Postech.Hackathon.Agendamentos.Infra.Http.Serializadores;
using Postech.Hackathon.Agendamentos.Infra.Http.Serializadores.Resultados;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Interfaces;
using Postech.Hackathon.Agendamentos.Infra.Http.Usuarios.Modelos;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Adaptadores;

[ExcludeFromCodeCoverage]
public class AdaptadorAspNetCore(WebApplication app) : IHttp
{
    private readonly WebApplication _app = app;

    public void On<TRequisicao, TResposta>(
        string metodo, 
        string url, 
        Func<TRequisicao?, IDictionary<string, object?>, IDictionary<string, StringValues>, UsuarioAutenticado, IServiceProvider, Task<ComandoRespostaGenerico<TResposta>>> callback)
        where TResposta : IComandoResposta
    {
        _app.MapMethods(url, [metodo.ToUpper()], async (HttpContext contexto, CancellationToken token) =>
        {
            try
            {
                string? corpoRequisicao = await new StreamReader(contexto.Request.Body).ReadToEndAsync(token);
                TRequisicao? corpo = DeserializadorRequisicaoHttp.Deserializar<TRequisicao>(corpoRequisicao, contexto.Request.Headers.ContentType);
                IDictionary<string, object?> valoresRota = contexto.Request.ObterValoresRota();
                IDictionary<string, StringValues>  valoresConsulta = contexto.Request.ObterValoresConsulta();
                IServicoAutenticacaoUsuario servicoAutenticacaoUsuario = contexto.RequestServices.GetRequiredService<IServicoAutenticacaoUsuario>();
                ComandoRespostaGenerico<TResposta> conteudoResposta = await callback(corpo, valoresRota, valoresConsulta, servicoAutenticacaoUsuario.ObterUsuario(contexto), contexto.RequestServices);
                ResultadoSerializacaoRespostaHttp resultadoSerializacao = SerializadorRespostaHttp.Serializar(conteudoResposta, contexto.Request.Headers.Accept);
                contexto.Response.ContentType = resultadoSerializacao.TipoConteudo;
                contexto.Response.StatusCode = conteudoResposta.CodigoResposta;
                await contexto.Response.WriteAsync(resultadoSerializacao.Conteudo, token);
            }
            catch (ExcecaoDeserializadorRequisicaoHttp ex)
            {
                contexto.Response.StatusCode = (int) HttpStatusCode.UnsupportedMediaType;
                await contexto.Response.WriteAsync(ex.Message, token);
            }
            catch (Exception ex)
            {
                contexto.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await contexto.Response.WriteAsync(ex.Message, token);
            }
        });
    }

    public void Executar()
    {
        _app.Run();
    }
}