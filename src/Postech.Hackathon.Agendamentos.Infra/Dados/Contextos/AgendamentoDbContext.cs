using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Infra.Dados.Contextos;

[ExcludeFromCodeCoverage]
public class AgendamentoDbContext(DbContextOptions<AgendamentoDbContext> options) : DbContext(options)
{
    public required DbSet<Agendamento> Agendamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendamentoDbContext).Assembly);
    }
}