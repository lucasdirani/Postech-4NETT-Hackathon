CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'agendamentos') THEN
        CREATE SCHEMA agendamentos;
    END IF;
END $EF$;

CREATE TABLE agendamentos.tb_agendamentos (
    "Id" uuid NOT NULL,
    "Id_Medico" uuid NOT NULL,
    "Id_Paciente" uuid,
    "Data" date NOT NULL,
    "Hora_Inicio" time NOT NULL,
    "Hora_Fim" time NOT NULL,
    "Valor" numeric(8,2) NOT NULL,
    "Situacao" varchar NOT NULL,
    "Justificativa_Recusa" varchar,
    "Justificativa_Cancelamento" varchar,
    "Criado_Em" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "Modificado_Em" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "Ativo" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_tb_agendamentos" PRIMARY KEY ("Id")
);

CREATE INDEX ix_tb_agendamentos_id_medico_data_ativo ON agendamentos.tb_agendamentos ("Id_Medico", "Data", "Ativo");

CREATE INDEX ix_tb_agendamentos_id_paciente_data_situacao ON agendamentos.tb_agendamentos ("Id_Paciente", "Data", "Situacao");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250208133422_Inicial', '8.0.12');

COMMIT;