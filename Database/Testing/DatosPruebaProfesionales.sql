/*
    DATOS FICTICIOS DE PRUEBA - TECNI SALUD
    ----------------------------------------
    Este archivo NO es un script de instalación ni de producción.

    Antes de ejecutarlo:
    1. Crear en la aplicación cinco usuarios activos con estos nombres:
       pn1.test.lucia, pn1.test.mateo, pn1.test.valentina,
       pn1.test.camila y pn1.test.martina.
    2. Crear esos usuarios desde la aplicación para que la contraseña se
       procese con el servicio de hash vigente. Este script no inserta
       contraseñas ni credenciales.
    3. Revisar y ejecutar primero la sección de vista previa.

    El script:
    - exige DB_NAME() = TecniSalud;
    - muestra los profesionales de prueba que serían eliminados;
    - muestra jornadas, turnos y relaciones dependientes de esos profesionales;
    - se detiene sin borrar si existe una jornada, un turno o una FK no
      contemplada que dependa de un profesional de prueba;
    - elimina únicamente profesionales con matrículas explícitas de prueba y
      sus asociaciones en ProfesionalEspecialidad;
    - no elimina usuarios ni especialidades: una especialidad puede existir
      previamente o estar asociada a profesionales que deben conservarse;
    - agrega las cinco especialidades y cinco profesionales de prueba solo si
      todavía no existen.

    No ejecutar en una base real. El script no debe ser ejecutado por este
    agente: queda preparado para revisión y ejecución manual.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @EjecutarCambios BIT = 0;
/* Cambiar a 1 solamente después de revisar todas las vistas previas. */

IF DB_NAME() <> N'TecniSalud'
BEGIN
    ;THROW 51000, 'Ejecución detenida: la base actual no es TecniSalud.', 1;
END;

IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Profesional', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Especialidad', N'U') IS NULL
   OR OBJECT_ID(N'dbo.ProfesionalEspecialidad', N'U') IS NULL
BEGIN
    ;THROW 51001, 'Ejecución detenida: faltan tablas requeridas del esquema de Tecni Salud.', 1;
END;

IF COL_LENGTH(N'dbo.Profesional', N'dni') IS NULL
BEGIN
    ;THROW 51002, 'Ejecución detenida: falta Profesional.dni. Revisar la migración Profesional_DNI.sql.', 1;
END;

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Profesional')
      AND name = N'dni'
      AND is_nullable = 0
)
BEGIN
    ;THROW 51003, 'Ejecución detenida: Profesional.dni es obligatorio. Revisar valores ficticios antes de continuar.', 1;
END;

DECLARE @ProfesionalesPrueba TABLE
(
    matricula VARCHAR(50) NOT NULL PRIMARY KEY,
    nombre_usuario VARCHAR(100) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    especialidad VARCHAR(100) NOT NULL
);

INSERT INTO @ProfesionalesPrueba
    (matricula, nombre_usuario, nombre, apellido, especialidad)
VALUES
    ('TEST-PN1-CLINICA', 'pn1.test.lucia', 'Lucía', 'Fernández', 'Clínica médica'),
    ('TEST-PN1-PEDIATRIA', 'pn1.test.mateo', 'Mateo', 'Ruiz', 'Pediatría'),
    ('TEST-PN1-CARDIOLOGIA', 'pn1.test.valentina', 'Valentina', 'Morales', 'Cardiología'),
    ('TEST-PN1-DERMATOLOGIA', 'pn1.test.camila', 'Camila', 'Suárez', 'Dermatología'),
    ('TEST-PN1-PSICOLOGIA', 'pn1.test.martina', 'Martina', 'López', 'Psicología');

/*
    VISTA PREVIA 1: estos son exactamente los profesionales que el script
    identifica como datos de prueba mediante matrículas explícitas.
*/
SELECT
    p.id_profesional,
    p.id_usuario,
    u.nombre_usuario,
    p.dni,
    p.nombre,
    p.apellido,
    p.matricula,
    p.estado_profesional
FROM dbo.Profesional p
INNER JOIN dbo.Usuario u ON u.id_usuario = p.id_usuario
INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
ORDER BY p.id_profesional;

/* VISTA PREVIA 2: asociaciones de especialidad que se quitarían junto con esos profesionales. */
SELECT
    p.id_profesional,
    p.matricula,
    e.id_especialidad,
    e.nombre AS especialidad
FROM dbo.Profesional p
INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
INNER JOIN dbo.ProfesionalEspecialidad pe ON pe.id_profesional = p.id_profesional
INNER JOIN dbo.Especialidad e ON e.id_especialidad = pe.id_especialidad
ORDER BY p.matricula, e.nombre;

/* VISTA PREVIA 3: jornadas que bloquean el borrado seguro. */
IF OBJECT_ID(N'dbo.JornadaProfesional', N'U') IS NOT NULL
BEGIN
    SELECT
        p.id_profesional,
        p.matricula,
        j.id_jornada,
        j.fecha,
        j.hora_inicio,
        j.hora_fin,
        j.estado_jornada,
        j.id_consultorio
    FROM dbo.Profesional p
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
    INNER JOIN dbo.JornadaProfesional j ON j.id_profesional = p.id_profesional
    ORDER BY p.matricula, j.fecha, j.hora_inicio;
END;

/* VISTA PREVIA 4: turnos y dependencias de jornadas de prueba. */
IF OBJECT_ID(N'dbo.Turno', N'U') IS NOT NULL
BEGIN
    SELECT
        p.id_profesional,
        p.matricula,
        j.id_jornada,
        t.id_turno,
        t.fecha_hora_inicio,
        t.fecha_hora_fin,
        t.estado_turno,
        t.id_paciente,
        t.id_usuario_creador
    FROM dbo.Profesional p
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
    INNER JOIN dbo.JornadaProfesional j ON j.id_profesional = p.id_profesional
    INNER JOIN dbo.Turno t ON t.id_jornada = j.id_jornada
    ORDER BY p.matricula, j.id_jornada, t.fecha_hora_inicio;
END;

IF OBJECT_ID(N'dbo.HistorialEstadoTurno', N'U') IS NOT NULL
BEGIN
    SELECT
        p.id_profesional,
        p.matricula,
        j.id_jornada,
        t.id_turno,
        h.id_historial,
        h.estado_nuevo,
        h.fecha_cambio
    FROM dbo.Profesional p
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
    INNER JOIN dbo.JornadaProfesional j ON j.id_profesional = p.id_profesional
    INNER JOIN dbo.Turno t ON t.id_jornada = j.id_jornada
    INNER JOIN dbo.HistorialEstadoTurno h ON h.id_turno = t.id_turno
    ORDER BY p.matricula, t.id_turno, h.fecha_cambio;
END;

IF OBJECT_ID(N'dbo.Atencion', N'U') IS NOT NULL
BEGIN
    SELECT
        p.id_profesional,
        p.matricula,
        j.id_jornada,
        t.id_turno,
        a.id_atencion
    FROM dbo.Profesional p
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
    INNER JOIN dbo.JornadaProfesional j ON j.id_profesional = p.id_profesional
    INNER JOIN dbo.Turno t ON t.id_jornada = j.id_jornada
    INNER JOIN dbo.Atencion a ON a.id_turno = t.id_turno
    ORDER BY p.matricula, t.id_turno;
END;

IF OBJECT_ID(N'dbo.Pago', N'U') IS NOT NULL
BEGIN
    SELECT
        p.id_profesional,
        p.matricula,
        j.id_jornada,
        t.id_turno,
        pg.id_pago
    FROM dbo.Profesional p
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
    INNER JOIN dbo.JornadaProfesional j ON j.id_profesional = p.id_profesional
    INNER JOIN dbo.Turno t ON t.id_jornada = j.id_jornada
    INNER JOIN dbo.Pago pg ON pg.id_turno = t.id_turno
    ORDER BY p.matricula, t.id_turno;
END;

/*
    VISTA PREVIA 5: cualquier FK adicional hacia Profesional no contemplada
    por este script. Si aparece alguna, el bloque de borrado se detiene.
*/
SELECT
    fk.name AS foreign_key_name,
    OBJECT_SCHEMA_NAME(fk.parent_object_id) AS child_schema,
    OBJECT_NAME(fk.parent_object_id) AS child_table
FROM sys.foreign_keys fk
WHERE fk.referenced_object_id = OBJECT_ID(N'dbo.Profesional')
  AND OBJECT_NAME(fk.parent_object_id) NOT IN ('ProfesionalEspecialidad', 'JornadaProfesional')
ORDER BY child_schema, child_table, foreign_key_name;

/* VISTA PREVIA 6: usuarios requeridos y especialidades que se usarían. */
SELECT
    pp.nombre_usuario,
    u.id_usuario,
    u.estado_usuario,
    CASE
        WHEN u.id_usuario IS NULL THEN 'FALTA: crear desde la aplicación'
        WHEN u.estado_usuario <> 'ACTIVO' THEN 'REVISAR: el usuario no está activo'
        ELSE 'Disponible'
    END AS estado_prerrequisito
FROM @ProfesionalesPrueba pp
LEFT JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
ORDER BY pp.nombre_usuario;

SELECT
    v.nombre AS especialidad,
    CASE
        WHEN e.id_especialidad IS NULL THEN 'Se agregaría'
        ELSE 'Ya existe; se conservaría'
    END AS accion
FROM
(
    SELECT CAST('Clínica médica' AS VARCHAR(100)) AS nombre
    UNION ALL SELECT 'Pediatría'
    UNION ALL SELECT 'Cardiología'
    UNION ALL SELECT 'Dermatología'
    UNION ALL SELECT 'Psicología'
) v
LEFT JOIN dbo.Especialidad e ON e.nombre = v.nombre
ORDER BY v.nombre;

IF @EjecutarCambios = 0
BEGIN
    PRINT 'Modo vista previa: no se modificó la base. Cambie @EjecutarCambios a 1 después de revisar los resultados.';
    RETURN;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    /* Los usuarios son prerrequisito: se crean desde la aplicación, nunca con contraseñas SQL. */
    IF EXISTS
    (
        SELECT 1
        FROM @ProfesionalesPrueba pp
        LEFT JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
        WHERE u.id_usuario IS NULL
           OR u.estado_usuario <> 'ACTIVO'
    )
    BEGIN
        SELECT
            pp.nombre_usuario,
            u.id_usuario,
            u.estado_usuario,
            'Crear o activar este usuario desde la aplicación antes de ejecutar el script.' AS accion
        FROM @ProfesionalesPrueba pp
        LEFT JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
        WHERE u.id_usuario IS NULL
           OR u.estado_usuario <> 'ACTIVO';

        ;THROW 51004, 'Faltan usuarios activos de prueba o no tienen un estado válido. No se modificó la base.', 1;
    END;

    /* No se reutiliza un usuario que ya representa a otro profesional. */
    IF EXISTS
    (
        SELECT 1
        FROM @ProfesionalesPrueba pp
        INNER JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
        INNER JOIN dbo.Profesional p ON p.id_usuario = u.id_usuario
        WHERE p.matricula <> pp.matricula
    )
    BEGIN
        SELECT
            pp.nombre_usuario,
            pp.matricula AS matricula_esperada,
            p.id_profesional,
            p.matricula AS matricula_actual
        FROM @ProfesionalesPrueba pp
        INNER JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
        INNER JOIN dbo.Profesional p ON p.id_usuario = u.id_usuario
        WHERE p.matricula <> pp.matricula;

        ;THROW 51005, 'Un usuario de prueba ya está asociado a otro profesional. No se modificó la base.', 1;
    END;

    /* Si una matrícula de prueba existe pero apunta a otro usuario, se detiene. */
    IF EXISTS
    (
        SELECT 1
        FROM @ProfesionalesPrueba pp
        INNER JOIN dbo.Profesional p ON p.matricula = pp.matricula
        INNER JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
        WHERE p.id_usuario <> u.id_usuario
    )
    BEGIN
        SELECT
            pp.matricula,
            p.id_profesional,
            p.id_usuario AS id_usuario_actual,
            u.id_usuario AS id_usuario_esperado
        FROM @ProfesionalesPrueba pp
        INNER JOIN dbo.Profesional p ON p.matricula = pp.matricula
        INNER JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
        WHERE p.id_usuario <> u.id_usuario;

        ;THROW 51006, 'Una matrícula de prueba ya está asociada a otro usuario. No se modificó la base.', 1;
    END;

    /* Una jornada es una dependencia funcional: no se borran jornadas ni turnos desde este script. */
    IF OBJECT_ID(N'dbo.JornadaProfesional', N'U') IS NOT NULL
       AND EXISTS
       (
           SELECT 1
           FROM dbo.JornadaProfesional j
           INNER JOIN dbo.Profesional p ON p.id_profesional = j.id_profesional
           INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
       )
    BEGIN
        ;THROW 51007, 'Hay jornadas asociadas a profesionales de prueba. Revise la vista previa y resuélvalas manualmente; no se borró nada.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys fk
        WHERE fk.referenced_object_id = OBJECT_ID(N'dbo.Profesional')
          AND OBJECT_NAME(fk.parent_object_id) NOT IN ('ProfesionalEspecialidad', 'JornadaProfesional')
    )
    BEGIN
        ;THROW 51008, 'Existe una FK adicional hacia Profesional que este script no puede evaluar con seguridad. No se borró nada.', 1;
    END;

    /* Elimina solo las asociaciones de especialidad de los profesionales identificados. */
    DELETE pe
    FROM dbo.ProfesionalEspecialidad pe
    INNER JOIN dbo.Profesional p ON p.id_profesional = pe.id_profesional
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula;

    /* No elimina usuarios: pueden tener auditoría, permisos u otras relaciones. */
    DELETE p
    FROM dbo.Profesional p
    INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula;

    /* Especialidades: se crean solo si no existen y nunca se eliminan automáticamente. */
    INSERT INTO dbo.Especialidad (nombre, descripcion)
    SELECT v.nombre, 'Especialidad ficticia para pruebas de Tecni Salud.'
    FROM
    (
        SELECT CAST('Clínica médica' AS VARCHAR(100)) AS nombre
        UNION ALL SELECT 'Pediatría'
        UNION ALL SELECT 'Cardiología'
        UNION ALL SELECT 'Dermatología'
        UNION ALL SELECT 'Psicología'
    ) v
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Especialidad e
        WHERE e.nombre = v.nombre
    );

    /* Profesionales ficticios: DNI NULL porque el esquema permite NULL y no se inventan DNI. */
    INSERT INTO dbo.Profesional
        (id_usuario, dni, matricula, nombre, apellido)
    SELECT
        u.id_usuario,
        NULL,
        pp.matricula,
        pp.nombre,
        pp.apellido
    FROM @ProfesionalesPrueba pp
    INNER JOIN dbo.Usuario u ON u.nombre_usuario = pp.nombre_usuario
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Profesional p
        WHERE p.matricula = pp.matricula
    );

    /* Relaciones ProfesionalEspecialidad, sin duplicar vínculos existentes. */
    INSERT INTO dbo.ProfesionalEspecialidad (id_profesional, id_especialidad)
    SELECT p.id_profesional, e.id_especialidad
    FROM @ProfesionalesPrueba pp
    INNER JOIN dbo.Profesional p ON p.matricula = pp.matricula
    INNER JOIN dbo.Especialidad e ON e.nombre = pp.especialidad
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.ProfesionalEspecialidad pe
        WHERE pe.id_profesional = p.id_profesional
          AND pe.id_especialidad = e.id_especialidad
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

/* Verificación posterior: debe mostrar los cinco profesionales y sus especialidades. */
SELECT
    p.id_profesional,
    p.dni,
    p.nombre,
    p.apellido,
    p.matricula,
    e.nombre AS especialidad
FROM dbo.Profesional p
INNER JOIN @ProfesionalesPrueba pp ON pp.matricula = p.matricula
LEFT JOIN dbo.ProfesionalEspecialidad pe ON pe.id_profesional = p.id_profesional
LEFT JOIN dbo.Especialidad e ON e.id_especialidad = pe.id_especialidad
ORDER BY p.matricula, e.nombre;
