using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postech.Hackathon.Agendamentos.Infra.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "agendamentos");

            migrationBuilder.CreateTable(
                name: "tb_agendamentos",
                schema: "agendamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Medico = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Paciente = table.Column<Guid>(type: "uuid", nullable: true),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora_Inicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    Hora_Fim = table.Column<TimeSpan>(type: "time", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Situacao = table.Column<string>(type: "varchar", maxLength: 9, nullable: false),
                    Justificativa_Recusa = table.Column<string>(type: "varchar", maxLength: 60, nullable: true),
                    Justificativa_Cancelamento = table.Column<string>(type: "varchar", maxLength: 60, nullable: true),
                    Criado_Em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Modificado_Em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_agendamentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_agendamentos_id_medico_data_ativo",
                schema: "agendamentos",
                table: "tb_agendamentos",
                columns: new[] { "Id_Medico", "Data", "Ativo" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_agendamentos_id_paciente_data_situacao",
                schema: "agendamentos",
                table: "tb_agendamentos",
                columns: new[] { "Id_Paciente", "Data", "Situacao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_agendamentos",
                schema: "agendamentos");
        }
    }
}
