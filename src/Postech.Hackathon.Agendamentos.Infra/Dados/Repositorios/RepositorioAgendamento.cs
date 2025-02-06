using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;
using Postech.Hackathon.Agendamentos.Dominio.Repositorios;
using Postech.Hackathon.Agendamentos.Infra.Dados.Contextos;

namespace Postech.Hackathon.Agendamentos.Infra.Dados.Repositorios;

[ExcludeFromCodeCoverage]
public class RepositorioAgendamento(AgendamentoDbContext dbContext) : IRepositorioAgendamento
{
    private readonly AgendamentoDbContext _dbContext = dbContext;

    public void Atualizar(Agendamento entidade)
    {
        _dbContext.Agendamentos.Update(entidade);
    }

    public async Task<IReadOnlyList<Agendamento>> ConsultarAgendamentosMedicoAsync(Guid idMedico, DateOnly dataAgendamento)
    {
        return await _dbContext.Agendamentos.Where(a => a.IdMedico == idMedico && a.Data == dataAgendamento).ToListAsync();
    }

    public async Task<List<Agendamento>> EncontrarAsync(Expression<Func<Agendamento, bool>> expressao)
    {
        return await _dbContext.Agendamentos.Where(expressao).ToListAsync();
    }

    public async Task<bool> ExisteAsync(Expression<Func<Agendamento, bool>> expressao)
    {
        return await _dbContext.Agendamentos.AnyAsync(expressao);
    }

    public async Task InserirAsync(Agendamento entidade)
    {
        await _dbContext.Agendamentos.AddAsync(entidade);
    }

    public async Task<Agendamento?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Agendamentos.FindAsync(id);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}