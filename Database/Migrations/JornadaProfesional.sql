SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.JornadaProfesional', 'U') IS NULL
        THROW 50001, 'La tabla dbo.JornadaProfesional debe existir antes de aplicar esta migración.', 1;

    IF COL_LENGTH('dbo.JornadaProfesional', 'id_especialidad') IS NULL
    BEGIN
        ALTER TABLE dbo.JornadaProfesional
            ADD id_especialidad INT NULL;
    END;
    ELSE IF EXISTS
    (
        SELECT 1
        FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.JornadaProfesional')
          AND name = 'id_especialidad'
          AND is_nullable = 0
    )
    BEGIN
        ALTER TABLE dbo.JornadaProfesional
            ALTER COLUMN id_especialidad INT NULL;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = 'FK_Jornada_Especialidad'
          AND parent_object_id = OBJECT_ID('dbo.JornadaProfesional')
    )
    BEGIN
        ALTER TABLE dbo.JornadaProfesional
            ADD CONSTRAINT FK_Jornada_Especialidad
            FOREIGN KEY (id_especialidad) REFERENCES dbo.Especialidad(id_especialidad);
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_JornadaProfesional_Activas_Fecha_Horario'
          AND object_id = OBJECT_ID('dbo.JornadaProfesional')
    )
    BEGIN
        CREATE INDEX IX_JornadaProfesional_Activas_Fecha_Horario
            ON dbo.JornadaProfesional(fecha, hora_inicio, hora_fin, id_profesional, id_consultorio)
            WHERE estado_jornada = 'Activa';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = 'CK_Jornada_DuracionRango'
          AND parent_object_id = OBJECT_ID('dbo.JornadaProfesional')
    )
    BEGIN
        ALTER TABLE dbo.JornadaProfesional
            ADD CONSTRAINT CK_Jornada_DuracionRango
            CHECK (duracion_turno_min <= DATEDIFF(MINUTE, CAST(hora_inicio AS DATETIME), CAST(hora_fin AS DATETIME)));
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
