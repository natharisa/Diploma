SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.ProfesionalEspecialidad', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.ProfesionalEspecialidad (
            id_profesional INT NOT NULL,
            id_especialidad INT NOT NULL,
            CONSTRAINT PK_ProfesionalEspecialidad PRIMARY KEY (id_profesional, id_especialidad),
            CONSTRAINT FK_ProfesionalEspecialidad_Profesional FOREIGN KEY (id_profesional) REFERENCES dbo.Profesional(id_profesional),
            CONSTRAINT FK_ProfesionalEspecialidad_Especialidad FOREIGN KEY (id_especialidad) REFERENCES dbo.Especialidad(id_especialidad)
        );
    END;

    IF COL_LENGTH('dbo.Profesional', 'id_especialidad') IS NOT NULL
    BEGIN
        EXEC sys.sp_executesql N'
            INSERT INTO dbo.ProfesionalEspecialidad (id_profesional, id_especialidad)
            SELECT p.id_profesional, p.id_especialidad
            FROM dbo.Profesional p
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM dbo.ProfesionalEspecialidad pe
                WHERE pe.id_profesional = p.id_profesional
                  AND pe.id_especialidad = p.id_especialidad
            );';
    END;

    IF EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = 'FK_Profesional_Especialidad'
          AND parent_object_id = OBJECT_ID('dbo.Profesional')
    )
    BEGIN
        ALTER TABLE dbo.Profesional
            DROP CONSTRAINT FK_Profesional_Especialidad;
    END;

    IF COL_LENGTH('dbo.Profesional', 'id_especialidad') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.Profesional
            DROP COLUMN id_especialidad;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
