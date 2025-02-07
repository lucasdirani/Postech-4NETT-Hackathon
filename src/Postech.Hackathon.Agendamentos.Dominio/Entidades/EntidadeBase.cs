using System.Diagnostics.CodeAnalysis;

namespace Postech.Hackathon.Agendamentos.Dominio.Entidades;

[ExcludeFromCodeCoverage]
public abstract class EntidadeBase
{
    public Guid Id { get; }
    public DateTime CriadoEm { get; protected set; }
    public DateTime ModificadoEm { get; protected set; }
    
    protected EntidadeBase()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
        ModificadoEm = DateTime.UtcNow;
    }
}