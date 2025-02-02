using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.Hackathon.Agendamentos.Dominio.Entidades;

namespace Postech.Hackathon.Agendamentos.Infra.Dados.Config;

[ExcludeFromCodeCoverage]
internal class ConfigAgendamento : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("tb_agendamentos", "agendamentos");
        builder
            .Property(a => a.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .IsRequired();
        builder.HasKey(a => a.Id);
        builder
            .Property(a => a.CriadoEm)
            .HasColumnName("Criado_Em")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        builder
            .Property(a => a.IdMedico)
            .HasColumnName("Id_Medico")
            .HasColumnType("uuid")
            .IsRequired();
        builder
            .Property(a => a.Data)
            .HasColumnName("Data")
            .HasColumnType("date")
            .IsRequired();
        builder
            .Property(a => a.HorarioInicio)
            .HasColumnName("Hora_Inicio")
            .HasColumnType("time")
            .IsRequired();
        builder
            .Property(a => a.HorarioFim)
            .HasColumnName("Hora_Fim")
            .HasColumnType("time")
            .IsRequired();
        builder
            .HasIndex(a => new { a.IdMedico, a.Data })
            .HasDatabaseName("ix_tb_agendamentos_id_medico_data");
    }
}