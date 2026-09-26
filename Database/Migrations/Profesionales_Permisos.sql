SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.ComponentePermiso', 'U') IS NULL
       OR OBJECT_ID('dbo.ComponentePermisoRelacion', 'U') IS NULL
       OR OBJECT_ID('dbo.UsuarioComponentePermiso', 'U') IS NULL
       OR OBJECT_ID('dbo.UsuarioRol', 'U') IS NULL
       OR OBJECT_ID('dbo.Rol', 'U') IS NULL
       OR OBJECT_ID('dbo.Usuario', 'U') IS NULL
    BEGIN
        THROW 50002, 'Faltan tablas del mecanismo de permisos para reparar el acceso a profesionales.', 1;
    END;

    DECLARE @usuarioAdministrador VARCHAR(100) = 'admin';

    IF NOT EXISTS (SELECT 1 FROM dbo.ComponentePermiso WHERE codigo = 'ADMINISTRADOR')
    BEGIN
        INSERT INTO dbo.ComponentePermiso (codigo, nombre, descripcion, tipo, estado_componente)
        VALUES ('ADMINISTRADOR', 'Administrador', 'Familia con acceso total al sistema', 'FAMILIA', 'ACTIVO');
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.ComponentePermiso WHERE codigo = 'PROFESIONAL_VER')
    BEGIN
        INSERT INTO dbo.ComponentePermiso (codigo, nombre, descripcion, tipo, estado_componente)
        VALUES ('PROFESIONAL_VER', 'Ver profesionales', 'Permite consultar profesionales y acceder a sus jornadas', 'PERMISO', 'ACTIVO');
    END;

    UPDATE dbo.ComponentePermiso
    SET estado_componente = 'ACTIVO'
    WHERE codigo IN ('ADMINISTRADOR', 'PROFESIONAL_VER');

    DECLARE @idAdministrador INT =
    (
        SELECT TOP (1) id_componente
        FROM dbo.ComponentePermiso
        WHERE codigo = 'ADMINISTRADOR'
          AND UPPER(estado_componente) = 'ACTIVO'
    );

    DECLARE @idProfesionalVer INT =
    (
        SELECT TOP (1) id_componente
        FROM dbo.ComponentePermiso
        WHERE codigo = 'PROFESIONAL_VER'
          AND UPPER(estado_componente) = 'ACTIVO'
    );

    IF @idAdministrador IS NULL OR @idProfesionalVer IS NULL
        THROW 50003, 'Los componentes de permisos para profesionales no están activos.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.ComponentePermisoRelacion
        WHERE id_padre = @idAdministrador
          AND id_hijo = @idProfesionalVer
    )
    BEGIN
        INSERT INTO dbo.ComponentePermisoRelacion (id_padre, id_hijo)
        VALUES (@idAdministrador, @idProfesionalVer);
    END;

    UPDATE uc
    SET estado_usuario_componente = 'INACTIVO'
    FROM dbo.UsuarioComponentePermiso uc
    INNER JOIN dbo.Usuario u ON u.id_usuario = uc.id_usuario
    WHERE u.nombre_usuario = @usuarioAdministrador
      AND uc.estado_usuario_componente = 'ACTIVO'
      AND uc.id_componente <> @idAdministrador;

    UPDATE uc
    SET estado_usuario_componente = 'ACTIVO'
    FROM dbo.UsuarioComponentePermiso uc
    INNER JOIN dbo.Usuario u ON u.id_usuario = uc.id_usuario
    WHERE u.nombre_usuario = @usuarioAdministrador
      AND uc.id_componente = @idAdministrador;

    UPDATE uc
    SET estado_usuario_componente = 'INACTIVO'
    FROM dbo.UsuarioComponentePermiso uc
    INNER JOIN dbo.UsuarioRol ur
        ON ur.id_usuario = uc.id_usuario
       AND UPPER(ur.estado_usuario_rol) = 'ACTIVO'
    INNER JOIN dbo.Rol r
        ON r.id_rol = ur.id_rol
       AND UPPER(r.estado_rol) = 'ACTIVO'
       AND UPPER(r.nombre) = 'ADMINISTRADOR'
    WHERE uc.estado_usuario_componente = 'ACTIVO'
      AND uc.id_componente <> @idAdministrador
      AND
      (
          EXISTS
          (
              SELECT 1
              FROM dbo.Usuario u
              WHERE u.id_usuario = uc.id_usuario
                AND u.nombre_usuario = @usuarioAdministrador
          )
          OR EXISTS
          (
              SELECT 1
              FROM dbo.UsuarioRol ur2
              INNER JOIN dbo.Rol r2 ON r2.id_rol = ur2.id_rol
              WHERE ur2.id_usuario = uc.id_usuario
                AND UPPER(ur2.estado_usuario_rol) = 'ACTIVO'
                AND UPPER(r2.estado_rol) = 'ACTIVO'
                AND UPPER(r2.nombre) = 'ADMINISTRADOR'
          )
      );

    UPDATE uc
    SET estado_usuario_componente = 'ACTIVO'
    FROM dbo.UsuarioComponentePermiso uc
    INNER JOIN dbo.UsuarioRol ur
        ON ur.id_usuario = uc.id_usuario
       AND UPPER(ur.estado_usuario_rol) = 'ACTIVO'
    INNER JOIN dbo.Rol r
        ON r.id_rol = ur.id_rol
       AND UPPER(r.estado_rol) = 'ACTIVO'
       AND UPPER(r.nombre) = 'ADMINISTRADOR'
    WHERE uc.id_componente = @idAdministrador
      AND
      (
          EXISTS
          (
              SELECT 1
              FROM dbo.Usuario u
              WHERE u.id_usuario = uc.id_usuario
                AND u.nombre_usuario = @usuarioAdministrador
          )
          OR EXISTS
          (
              SELECT 1
              FROM dbo.UsuarioRol ur2
              INNER JOIN dbo.Rol r2 ON r2.id_rol = ur2.id_rol
              WHERE ur2.id_usuario = uc.id_usuario
                AND UPPER(ur2.estado_usuario_rol) = 'ACTIVO'
                AND UPPER(r2.estado_rol) = 'ACTIVO'
                AND UPPER(r2.nombre) = 'ADMINISTRADOR'
          )
      );

    INSERT INTO dbo.UsuarioComponentePermiso (id_usuario, id_componente, estado_usuario_componente)
    SELECT DISTINCT ur.id_usuario, @idAdministrador, 'ACTIVO'
    FROM dbo.UsuarioRol ur
    INNER JOIN dbo.Rol r ON r.id_rol = ur.id_rol
    WHERE UPPER(ur.estado_usuario_rol) = 'ACTIVO'
      AND UPPER(r.estado_rol) = 'ACTIVO'
      AND UPPER(r.nombre) = 'ADMINISTRADOR'
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.UsuarioComponentePermiso uc
          WHERE uc.id_usuario = ur.id_usuario
            AND uc.id_componente = @idAdministrador
      );

    INSERT INTO dbo.UsuarioComponentePermiso (id_usuario, id_componente, estado_usuario_componente)
    SELECT u.id_usuario, @idAdministrador, 'ACTIVO'
    FROM dbo.Usuario u
    WHERE u.nombre_usuario = @usuarioAdministrador
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.UsuarioComponentePermiso uc
          WHERE uc.id_usuario = u.id_usuario
            AND uc.id_componente = @idAdministrador
      );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
