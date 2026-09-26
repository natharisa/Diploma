IF COL_LENGTH('dbo.Profesional', 'dni') IS NULL
BEGIN
    ALTER TABLE dbo.Profesional
        ADD dni VARCHAR(20) NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_Profesional_DNI'
      AND object_id = OBJECT_ID('dbo.Profesional')
)
BEGIN
    CREATE UNIQUE INDEX UX_Profesional_DNI
        ON dbo.Profesional(dni)
        WHERE dni IS NOT NULL;
END;
GO
