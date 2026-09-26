/*
    DATOS DE PRUEBA OPCIONALES PARA PN1 - NO EJECUTAR AUTOMATICAMENTE

    Crea cinco profesionales, sus usuarios técnicos, especialidades,
    consultorios y una jornada con un turno reservado para probar el bloqueo
    de ajustes. Es idempotente: identifica todo por valores de prueba y no
    modifica ni elimina registros existentes.

    Ejecutar manualmente sobre TecniSalud únicamente si se necesitan datos
    de prueba. Los usuarios pn1_profesional_* son técnicos y no se les
    asignan permisos; no son necesarios para iniciar sesión como admin.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @idUsuarioAdmin INT;
DECLARE @idProfesional INT;
DECLARE @idEspecialidad INT;
DECLARE @idConsultorio INT;
DECLARE @idJornada INT;
DECLARE @idPaciente INT;
DECLARE @orden INT;
DECLARE @maxOrden INT;
DECLARE @nombreUsuario VARCHAR(100);
DECLARE @email VARCHAR(150);
DECLARE @nombre VARCHAR(100);
DECLARE @apellido VARCHAR(100);
DECLARE @dni VARCHAR(20);
DECLARE @matricula VARCHAR(50);
DECLARE @especialidad1 VARCHAR(100);
DECLARE @especialidad2 VARCHAR(100);
DECLARE @idUsuarioProfesional INT;

SELECT @idUsuarioAdmin = id_usuario
FROM dbo.Usuario
WHERE nombre_usuario = 'admin';

IF @idUsuarioAdmin IS NULL
    THROW 51000, 'No existe el usuario admin. El script no creó datos.', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidad WHERE nombre = 'Clinica PN1 - Prueba')
BEGIN
    INSERT INTO dbo.Especialidad (nombre, descripcion)
    VALUES ('Clinica PN1 - Prueba', 'Especialidad de prueba para recorrer el PN1.');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidad WHERE nombre = 'Pediatria PN1 - Prueba')
BEGIN
    INSERT INTO dbo.Especialidad (nombre, descripcion)
    VALUES ('Pediatria PN1 - Prueba', 'Especialidad de prueba para recorrer el PN1.');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidad WHERE nombre = 'Cardiologia PN1 - Prueba')
BEGIN
    INSERT INTO dbo.Especialidad (nombre, descripcion)
    VALUES ('Cardiologia PN1 - Prueba', 'Especialidad de prueba para recorrer el PN1.');
END;

DECLARE @profesionales TABLE
(
    orden INT PRIMARY KEY,
    nombre_usuario VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    dni VARCHAR(20) NOT NULL,
    matricula VARCHAR(50) NOT NULL,
    especialidad1 VARCHAR(100) NOT NULL,
    especialidad2 VARCHAR(100) NULL
);

INSERT INTO @profesionales
    (orden, nombre_usuario, email, nombre, apellido, dni, matricula, especialidad1, especialidad2)
VALUES
    (1, 'pn1_profesional_01', 'pn1.profesional01@local.invalid', 'Ana', 'Gomez', 'PN1-18000001', 'PN1-PRUEBA-180-01', 'Clinica PN1 - Prueba', 'Pediatria PN1 - Prueba'),
    (2, 'pn1_profesional_02', 'pn1.profesional02@local.invalid', 'Bruno', 'Perez', 'PN1-18000002', 'PN1-PRUEBA-180-02', 'Pediatria PN1 - Prueba', NULL),
    (3, 'pn1_profesional_03', 'pn1.profesional03@local.invalid', 'Carla', 'Sosa', 'PN1-18000003', 'PN1-PRUEBA-180-03', 'Cardiologia PN1 - Prueba', 'Clinica PN1 - Prueba'),
    (4, 'pn1_profesional_04', 'pn1.profesional04@local.invalid', 'Diego', 'Ramirez', 'PN1-18000004', 'PN1-PRUEBA-180-04', 'Clinica PN1 - Prueba', NULL),
    (5, 'pn1_profesional_05', 'pn1.profesional05@local.invalid', 'Elena', 'Torres', 'PN1-18000005', 'PN1-PRUEBA-180-05', 'Cardiologia PN1 - Prueba', 'Pediatria PN1 - Prueba');

SET @orden = 1;
SELECT @maxOrden = MAX(orden) FROM @profesionales;

WHILE @orden <= @maxOrden
BEGIN
    SELECT @nombreUsuario = nombre_usuario,
           @email = email,
           @nombre = nombre,
           @apellido = apellido,
           @dni = dni,
           @matricula = matricula,
           @especialidad1 = especialidad1,
           @especialidad2 = especialidad2
    FROM @profesionales
    WHERE orden = @orden;

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE nombre_usuario = @nombreUsuario)
    BEGIN
        INSERT INTO dbo.Usuario
            (id_idioma, nombre_usuario, email, password_hash, nombre, apellido, estado_usuario)
        VALUES
            (NULL, @nombreUsuario, @email, 'PN1_TEST_ONLY_NOT_A_LOGIN_HASH', @nombre, @apellido, 'ACTIVO');
    END;

    SELECT @idUsuarioProfesional = id_usuario
    FROM dbo.Usuario
    WHERE nombre_usuario = @nombreUsuario;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Profesional
        WHERE id_usuario = @idUsuarioProfesional
          AND matricula <> @matricula
    )
        THROW 51001, 'Un usuario de prueba ya está asociado a otro profesional.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Profesional WHERE matricula = @matricula)
    BEGIN
        INSERT INTO dbo.Profesional
            (id_usuario, matricula, nombre, apellido, telefono, email, estado_profesional, dni)
        VALUES
            (@idUsuarioProfesional, @matricula, @nombre, @apellido, NULL, @email, 'Activo', @dni);
    END;

    SELECT @idProfesional = id_profesional
    FROM dbo.Profesional
    WHERE matricula = @matricula;

    SELECT @idEspecialidad = id_especialidad
    FROM dbo.Especialidad
    WHERE nombre = @especialidad1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.ProfesionalEspecialidad
        WHERE id_profesional = @idProfesional
          AND id_especialidad = @idEspecialidad
    )
    BEGIN
        INSERT INTO dbo.ProfesionalEspecialidad (id_profesional, id_especialidad)
        VALUES (@idProfesional, @idEspecialidad);
    END;

    IF @especialidad2 IS NOT NULL
    BEGIN
        SELECT @idEspecialidad = id_especialidad
        FROM dbo.Especialidad
        WHERE nombre = @especialidad2;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.ProfesionalEspecialidad
            WHERE id_profesional = @idProfesional
              AND id_especialidad = @idEspecialidad
        )
        BEGIN
            INSERT INTO dbo.ProfesionalEspecialidad (id_profesional, id_especialidad)
            VALUES (@idProfesional, @idEspecialidad);
        END;
    END;

    SET @orden = @orden + 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorio WHERE nombre_o_numero = 'Consultorio PN1 - Prueba 1')
BEGIN
    INSERT INTO dbo.Consultorio (nombre_o_numero, ubicacion, estado_consultorio)
    VALUES ('Consultorio PN1 - Prueba 1', 'Sector de pruebas', 'Activo');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorio WHERE nombre_o_numero = 'Consultorio PN1 - Prueba 2')
BEGIN
    INSERT INTO dbo.Consultorio (nombre_o_numero, ubicacion, estado_consultorio)
    VALUES ('Consultorio PN1 - Prueba 2', 'Sector de pruebas', 'Activo');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorio WHERE nombre_o_numero = 'Consultorio PN1 - Prueba 3')
BEGIN
    INSERT INTO dbo.Consultorio (nombre_o_numero, ubicacion, estado_consultorio)
    VALUES ('Consultorio PN1 - Prueba 3', 'Sector de pruebas', 'Activo');
END;

SELECT @idProfesional = id_profesional
FROM dbo.Profesional
WHERE matricula = 'PN1-PRUEBA-180-01';

SELECT @idConsultorio = id_consultorio
FROM dbo.Consultorio
WHERE nombre_o_numero = 'Consultorio PN1 - Prueba 1';

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.JornadaProfesional
    WHERE id_profesional = @idProfesional
      AND id_consultorio = @idConsultorio
      AND fecha = '2099-06-15'
      AND hora_inicio = '08:00'
      AND hora_fin = '12:00'
      AND estado_jornada = 'Activa'
)
BEGIN
    INSERT INTO dbo.JornadaProfesional
        (id_profesional, id_consultorio, fecha, hora_inicio, hora_fin, duracion_turno_min, estado_jornada)
    VALUES
        (@idProfesional, @idConsultorio, '2099-06-15', '08:00', '12:00', 30, 'Activa');
END;

SELECT @idPaciente = id_paciente
FROM dbo.Paciente
WHERE dni = 'PN1-18000006';

IF @idPaciente IS NULL
BEGIN
    INSERT INTO dbo.Paciente
        (id_usuario, dni, nombre, apellido, fecha_nacimiento, telefono, email, direccion, estado_paciente, fecha_alta)
    VALUES
        (NULL, 'PN1-18000006', 'Paciente', 'Prueba PN1', '1990-01-01', NULL,
         'pn1.paciente@local.invalid', NULL, 'Activo', GETDATE());
    SET @idPaciente = CONVERT(INT, SCOPE_IDENTITY());
END;

SELECT @idJornada = id_jornada
FROM dbo.JornadaProfesional
WHERE id_profesional = @idProfesional
  AND id_consultorio = @idConsultorio
  AND fecha = '2099-06-15'
  AND hora_inicio = '08:00'
  AND hora_fin = '12:00'
  AND estado_jornada = 'Activa';

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Turno
    WHERE id_jornada = @idJornada
      AND fecha_hora_inicio = '2099-06-15T08:00:00'
)
BEGIN
    INSERT INTO dbo.Turno
        (id_jornada, id_paciente, id_paciente_obra_social, fecha_hora_inicio,
         fecha_hora_fin, estado_turno, origen_turno, motivo_consulta,
         fecha_reserva, id_usuario_creador)
    VALUES
        (@idJornada, @idPaciente, NULL, '2099-06-15T08:00:00',
         '2099-06-15T08:30:00', 'Reservado', 'Administracion',
         'Turno de prueba para validar el bloqueo de ajustes PN1.', GETDATE(), @idUsuarioAdmin);
END;

COMMIT TRANSACTION;

SELECT COUNT(*) AS profesionales_de_prueba
FROM dbo.Profesional
WHERE matricula LIKE 'PN1-PRUEBA-180-%';
