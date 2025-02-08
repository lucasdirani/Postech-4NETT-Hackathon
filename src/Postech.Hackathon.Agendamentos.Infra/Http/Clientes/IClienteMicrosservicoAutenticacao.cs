using Postech.Hackathon.Agendamentos.Infra.Http.Clientes.Modelos;
using Refit;

namespace Postech.Hackathon.Agendamentos.Infra.Http.Clientes;

public interface IClienteMicrosservicoAutenticacao
{
    [Post("/v1/autenticacao/validacoes-tokens")]
    Task ValidarTokenAsync([Body] RequisicaoValidacaoToken requisicao);
}