using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Domain;

namespace DAL
{
    public class IdiomaDataMapper
    {
        private readonly DatabaseContext _databaseContext;

        public IdiomaDataMapper()
            : this(new DatabaseContext())
        {
        }

        public IdiomaDataMapper(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public int Crear(Idioma idioma, int? idUsuarioResponsable)
        {
            if (idioma == null)
            {
                return -1;
            }

            const string sql = @"
                INSERT INTO dbo.Idioma (codigo, nombre, estado_idioma)
                OUTPUT INSERTED.id_idioma
                VALUES (@codigo, @nombre, @estado_idioma)";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@codigo", idioma.Codigo),
                _databaseContext.CrearParametro("@nombre", idioma.Nombre),
                _databaseContext.CrearParametro("@estado_idioma", EstadoDesdeActivo(idioma.Activo))
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                if (tabla.Rows.Count == 0)
                {
                    return -1;
                }

                idioma.Id = Convert.ToInt32(tabla.Rows[0]["id_idioma"]);
                RegistrarHistorialEnConexion(idioma.Id, null, EstadoDesdeActivo(idioma.Activo), "Alta de idioma", idUsuarioResponsable);
                return idioma.Id;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool Actualizar(Idioma idioma, int? idUsuarioResponsable, string motivo)
        {
            if (idioma == null || idioma.Id == 0)
            {
                return false;
            }

            Idioma idiomaActual = ObtenerPorId(idioma.Id);
            string estadoAnterior = idiomaActual == null ? null : EstadoDesdeActivo(idiomaActual.Activo);
            string estadoNuevo = EstadoDesdeActivo(idioma.Activo);

            const string sql = @"
                UPDATE dbo.Idioma
                SET codigo = @codigo,
                    nombre = @nombre,
                    estado_idioma = @estado_idioma
                WHERE id_idioma = @id_idioma";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_idioma", idioma.Id),
                _databaseContext.CrearParametro("@codigo", idioma.Codigo),
                _databaseContext.CrearParametro("@nombre", idioma.Nombre),
                _databaseContext.CrearParametro("@estado_idioma", estadoNuevo)
            };

            try
            {
                _databaseContext.Abrir();
                int afectados = _databaseContext.EscribirTexto(sql, parametros);
                if (afectados > 0 && estadoAnterior != estadoNuevo)
                {
                    RegistrarHistorialEnConexion(idioma.Id, estadoAnterior, estadoNuevo, motivo, idUsuarioResponsable);
                }

                return afectados > 0;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public Idioma ObtenerPorId(int id)
        {
            const string sql = @"
                SELECT id_idioma, codigo, nombre, estado_idioma
                FROM dbo.Idioma
                WHERE id_idioma = @id_idioma";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_idioma", id)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public Idioma ObtenerDefault()
        {
            const string sql = @"
                SELECT TOP (1) id_idioma, codigo, nombre, estado_idioma
                FROM dbo.Idioma
                WHERE estado_idioma IN ('Activo', 'ACTIVO')
                ORDER BY CASE WHEN codigo IN ('es-AR', 'es') THEN 0 ELSE 1 END, id_idioma";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql);
                return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<Idioma> Listar(bool soloActivos)
        {
            string filtro = soloActivos ? "WHERE estado_idioma IN ('Activo', 'ACTIVO')" : string.Empty;
            string sql = @"
                SELECT id_idioma, codigo, nombre, estado_idioma
                FROM dbo.Idioma
                " + filtro + @"
                ORDER BY nombre";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql);
                List<Idioma> idiomas = new List<Idioma>();

                foreach (DataRow registro in tabla.Rows)
                {
                    idiomas.Add(Mapear(registro));
                }

                return idiomas;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        private void RegistrarHistorialEnConexion(int idiomaId, string estadoAnterior, string estadoNuevo, string motivo, int? idUsuarioResponsable)
        {
            const string sql = @"
                IF OBJECT_ID('dbo.IdiomaEstadoHistorial', 'U') IS NOT NULL
                BEGIN
                    INSERT INTO dbo.IdiomaEstadoHistorial
                        (id_idioma, estado_anterior, estado_nuevo, motivo, id_usuario_responsable)
                    VALUES
                        (@id_idioma, @estado_anterior, @estado_nuevo, @motivo, @id_usuario_responsable)
                END";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_idioma", idiomaId),
                _databaseContext.CrearParametro("@estado_anterior", estadoAnterior),
                _databaseContext.CrearParametro("@estado_nuevo", estadoNuevo),
                _databaseContext.CrearParametro("@motivo", motivo),
                new SqlParameter("@id_usuario_responsable", SqlDbType.Int)
                {
                    Value = idUsuarioResponsable.HasValue ? (object)idUsuarioResponsable.Value : DBNull.Value
                }
            };

            _databaseContext.EscribirTexto(sql, parametros);
        }

        private static Idioma Mapear(DataRow registro)
        {
            return new Idioma
            {
                Id = Convert.ToInt32(registro["id_idioma"]),
                Codigo = registro["codigo"].ToString(),
                Nombre = registro["nombre"].ToString(),
                Activo = EsActivo(registro["estado_idioma"].ToString())
            };
        }

        private static string EstadoDesdeActivo(bool activo)
        {
            return activo ? "Activo" : "Inactivo";
        }

        private static bool EsActivo(string estado)
        {
            return string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(estado, "ACTIVO", StringComparison.OrdinalIgnoreCase);
        }
    }
}
