using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DAL
{
    public class DigitoVerificadorDataMapper
    {
        private const string EntidadUsuario = "Usuario";
        private readonly DatabaseContext _databaseContext;

        public DigitoVerificadorDataMapper()
            : this(new DatabaseContext())
        {
        }

        public DigitoVerificadorDataMapper(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool VerificarUsuarios()
        {
            try
            {
                _databaseContext.Abrir();
                List<UsuarioDigitoRegistro> usuarios = LeerUsuariosEnConexion();

                if (usuarios.Count == 0)
                {
                    GuardarDvvEnConexion(CalcularHash(string.Empty));
                    return true;
                }

                string dvvRegistrado = ObtenerDvvEnConexion();
                if (string.IsNullOrWhiteSpace(dvvRegistrado) || TodosSinDvh(usuarios))
                {
                    RecalcularUsuariosEnConexion(usuarios);
                    return true;
                }

                bool integridadValida = true;
                List<string> dvhCalculados = new List<string>();

                foreach (UsuarioDigitoRegistro usuario in usuarios)
                {
                    string dvhCalculado = CalcularDvh(usuario);
                    dvhCalculados.Add(dvhCalculado);

                    if (!string.Equals(usuario.Dvh, dvhCalculado, StringComparison.OrdinalIgnoreCase))
                    {
                        MarcarBloqueoEnConexion(usuario.IdUsuario, true);
                        integridadValida = false;
                    }
                }

                string dvvCalculado = CalcularDvv(dvhCalculados);
                if (!string.Equals(dvvRegistrado, dvvCalculado, StringComparison.OrdinalIgnoreCase))
                {
                    integridadValida = false;
                }

                return integridadValida;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool RecalcularUsuarios()
        {
            try
            {
                _databaseContext.Abrir();
                RecalcularUsuariosEnConexion(LeerUsuariosEnConexion());
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool RecalcularUsuarioYDvv(int idUsuario)
        {
            try
            {
                _databaseContext.Abrir();
                UsuarioDigitoRegistro usuario = LeerUsuarioEnConexion(idUsuario);
                if (usuario == null)
                {
                    return false;
                }

                ActualizarDvhEnConexion(usuario.IdUsuario, CalcularDvh(usuario), usuario.BloqueoDigitoVerificador);
                List<UsuarioDigitoRegistro> usuarios = LeerUsuariosEnConexion();
                GuardarDvvEnConexion(CalcularDvvDesdeUsuarios(usuarios));
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool HayBloqueoUsuarios()
        {
            const string sql = @"
                SELECT TOP (1) id_usuario
                FROM dbo.Usuario
                WHERE bloqueo_digitoverificador = 1";

            try
            {
                _databaseContext.Abrir();
                return _databaseContext.LeerTexto(sql).Rows.Count > 0;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        private void RecalcularUsuariosEnConexion(List<UsuarioDigitoRegistro> usuarios)
        {
            List<string> dvhs = new List<string>();

            foreach (UsuarioDigitoRegistro usuario in usuarios)
            {
                string dvh = CalcularDvh(usuario);
                dvhs.Add(dvh);
                ActualizarDvhEnConexion(usuario.IdUsuario, dvh, false);
            }

            GuardarDvvEnConexion(CalcularDvv(dvhs));
        }

        private List<UsuarioDigitoRegistro> LeerUsuariosEnConexion()
        {
            const string sql = @"
                SELECT
                    id_usuario,
                    id_idioma,
                    nombre_usuario,
                    email,
                    password_hash,
                    nombre,
                    apellido,
                    estado_usuario,
                    intentos_login_fallidos,
                    bloqueo_digitoverificador,
                    dvh
                FROM dbo.Usuario
                ORDER BY id_usuario";

            DataTable tabla = _databaseContext.LeerTexto(sql);
            List<UsuarioDigitoRegistro> usuarios = new List<UsuarioDigitoRegistro>();

            foreach (DataRow fila in tabla.Rows)
            {
                usuarios.Add(Mapear(fila));
            }

            return usuarios;
        }

        private UsuarioDigitoRegistro LeerUsuarioEnConexion(int idUsuario)
        {
            const string sql = @"
                SELECT TOP (1)
                    id_usuario,
                    id_idioma,
                    nombre_usuario,
                    email,
                    password_hash,
                    nombre,
                    apellido,
                    estado_usuario,
                    intentos_login_fallidos,
                    bloqueo_digitoverificador,
                    dvh
                FROM dbo.Usuario
                WHERE id_usuario = @id_usuario";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_usuario", idUsuario)
            };

            DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
            return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
        }

        private void ActualizarDvhEnConexion(int idUsuario, string dvh, bool bloqueoDigitoVerificador)
        {
            const string sql = @"
                UPDATE dbo.Usuario
                SET dvh = @dvh,
                    bloqueo_digitoverificador = @bloqueo_digitoverificador
                WHERE id_usuario = @id_usuario";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_usuario", idUsuario),
                _databaseContext.CrearParametro("@dvh", dvh),
                new SqlParameter("@bloqueo_digitoverificador", SqlDbType.Bit)
                {
                    Value = bloqueoDigitoVerificador
                }
            };

            _databaseContext.EscribirTexto(sql, parametros);
        }

        private void MarcarBloqueoEnConexion(int idUsuario, bool bloqueo)
        {
            const string sql = @"
                UPDATE dbo.Usuario
                SET bloqueo_digitoverificador = @bloqueo_digitoverificador
                WHERE id_usuario = @id_usuario";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_usuario", idUsuario),
                new SqlParameter("@bloqueo_digitoverificador", SqlDbType.Bit)
                {
                    Value = bloqueo
                }
            };

            _databaseContext.EscribirTexto(sql, parametros);
        }

        private string ObtenerDvvEnConexion()
        {
            const string sql = @"
                SELECT TOP (1) dvv
                FROM dbo.DigitoVerificadorVertical
                WHERE entidad = @entidad";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@entidad", EntidadUsuario)
            };

            DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
            return tabla.Rows.Count == 0 ? null : tabla.Rows[0]["dvv"].ToString();
        }

        private void GuardarDvvEnConexion(string dvv)
        {
            const string sql = @"
                MERGE dbo.DigitoVerificadorVertical AS destino
                USING (SELECT @entidad AS entidad) AS origen
                    ON destino.entidad = origen.entidad
                WHEN MATCHED THEN
                    UPDATE SET dvv = @dvv,
                               fecha_calculo = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (entidad, dvv, fecha_calculo)
                    VALUES (@entidad, @dvv, GETDATE());";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@entidad", EntidadUsuario),
                _databaseContext.CrearParametro("@dvv", dvv)
            };

            _databaseContext.EscribirTexto(sql, parametros);
        }

        private static bool TodosSinDvh(List<UsuarioDigitoRegistro> usuarios)
        {
            foreach (UsuarioDigitoRegistro usuario in usuarios)
            {
                if (!string.IsNullOrWhiteSpace(usuario.Dvh))
                {
                    return false;
                }
            }

            return true;
        }

        private static string CalcularDvvDesdeUsuarios(List<UsuarioDigitoRegistro> usuarios)
        {
            List<string> dvhs = new List<string>();

            foreach (UsuarioDigitoRegistro usuario in usuarios)
            {
                dvhs.Add(string.IsNullOrWhiteSpace(usuario.Dvh) ? CalcularDvh(usuario) : usuario.Dvh);
            }

            return CalcularDvv(dvhs);
        }

        private static string CalcularDvv(List<string> dvhs)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string dvh in dvhs)
            {
                builder.Append(dvh ?? string.Empty);
                builder.Append("|");
            }

            return CalcularHash(builder.ToString());
        }

        private static string CalcularDvh(UsuarioDigitoRegistro usuario)
        {
            string datos = string.Join("|", new[]
            {
                usuario.IdUsuario.ToString(CultureInfo.InvariantCulture),
                usuario.IdIdioma.HasValue ? usuario.IdIdioma.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                usuario.NombreUsuario ?? string.Empty,
                usuario.Email ?? string.Empty,
                usuario.PasswordHash ?? string.Empty,
                usuario.Nombre ?? string.Empty,
                usuario.Apellido ?? string.Empty,
                usuario.EstadoUsuario ?? string.Empty,
                usuario.IntentosLoginFallidos.ToString(CultureInfo.InvariantCulture)
            });

            return CalcularHash(datos);
        }

        private static string CalcularHash(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto ?? string.Empty));
                StringBuilder builder = new StringBuilder(bytes.Length * 2);

                foreach (byte valor in bytes)
                {
                    builder.Append(valor.ToString("x2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        private static UsuarioDigitoRegistro Mapear(DataRow fila)
        {
            return new UsuarioDigitoRegistro
            {
                IdUsuario = Convert.ToInt32(fila["id_usuario"]),
                IdIdioma = fila["id_idioma"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["id_idioma"]),
                NombreUsuario = fila["nombre_usuario"].ToString(),
                Email = fila["email"].ToString(),
                PasswordHash = fila["password_hash"].ToString(),
                Nombre = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
                Apellido = fila["apellido"] == DBNull.Value ? null : fila["apellido"].ToString(),
                EstadoUsuario = fila["estado_usuario"].ToString(),
                IntentosLoginFallidos = Convert.ToInt32(fila["intentos_login_fallidos"]),
                BloqueoDigitoVerificador = fila["bloqueo_digitoverificador"] != DBNull.Value &&
                                           Convert.ToBoolean(fila["bloqueo_digitoverificador"]),
                Dvh = fila["dvh"] == DBNull.Value ? null : fila["dvh"].ToString()
            };
        }

        private class UsuarioDigitoRegistro
        {
            public int IdUsuario { get; set; }
            public int? IdIdioma { get; set; }
            public string NombreUsuario { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string EstadoUsuario { get; set; }
            public int IntentosLoginFallidos { get; set; }
            public bool BloqueoDigitoVerificador { get; set; }
            public string Dvh { get; set; }
        }
    }
}
