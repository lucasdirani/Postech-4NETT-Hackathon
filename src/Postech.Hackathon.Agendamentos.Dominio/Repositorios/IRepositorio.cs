using System.Linq.Expressions;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Dominio.Repositorios;

public interface IRepositorio<T, in TId> 
        where T : EntidadeBase 
        where TId : struct
{
    Task<bool> ExisteAsync(Expression<Func<T, bool>> expressao);
    Task<List<T>> EncontrarAsync(Expression<Func<T, bool>> expressao);
    Task<T?> ObterPorIdAsync(TId id);
    Task InserirAsync(T entidade);
    Task SalvarAlteracoesAsync();
    void Atualizar(T entidade);
}