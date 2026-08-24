using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Domain;

namespace DAL
{
    public class AuditoriaDataMapper
    {
        private readonly DatabaseContext _databaseContext;

        public AuditoriaDataMapper()
            : this(new DatabaseContext())
        {
        }

        public AuditoriaDataMapper(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public int Insertar(AuditoriaRegistro auditoria)
        {
            if (auditoria == null)
            {
                return -1;
            }

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                CrearParametro("@entidad", auditoria.Entidad),
                CrearParametro("@id_entidad", auditoria.IdEntidad),
                CrearParametro("@accion", auditoria.Accion),
                CrearParametro("@id_usuario_actor", auditoria.IdUsuarioActor),
                CrearParametro("@identificador_usuario_actor", auditoria.IdentificadorUsuarioActor),
                CrearParametro("@fecha_evento", auditoria.FechaEvento),
                CrearParametro("@estado_anterior_json", auditoria.EstadoAnteriorJson),
                CrearParametro("@estado_nuevo_json", auditoria.EstadoNuevoJson),
                CrearParametro("@cambios_json", auditoria.CambiosJson)
            };

            const string sql = @"
                INSERT INTO dbo.Auditoria
                (
                    entidad,
                    id_entidad,
                    accion,
                    id_usuario_actor,
                    identificador_usuario_actor,
                    fecha_evento,
                    estado_anterior_json,
                    estado_nuevo_json,
                    cambios_json
                )
                OUTPUT INSERTED.id_auditoria
                VALUES
                (
                    @entidad,
                    @id_entidad,
                    @accion,
                    @id_usuario_actor,
                    @identificador_usuario_actor,
                    @fecha_evento,
                    @estado_anterior_json,
                    @estado_nuevo_json,
                    @cambios_json
                )";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);

                if (tabla.Rows.Count == 0)
                {
                    return -1;
                }

                auditoria.Id = Convert.ToInt32(tabla.Rows[0]["id_auditoria"]);
                return auditoria.Id;
            }
            catch
            {
                return -1;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<AuditoriaRegistro> ListarPorEntidad(string entidad, int idEntidad)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                CrearParametro("@entidad", entidad),
                CrearParametro("@id_entidad", idEntidad)
            };

            const string sql = @"
                SELECT
                    id_auditoria,
                    entidad,
                    id_entidad,
                    accion,
                    id_usuario_actor,
                    identificador_usuario_actor,
                    fecha_evento,
                    estado_anterior_json,
                    estado_nuevo_json,
                    cambios_json
                FROM dbo.Auditoria
                WHERE entidad = @entidad
                  AND id_entidad = @id_entidad
                ORDER BY fecha_evento DESC, id_auditoria DESC";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                List<AuditoriaRegistro> registros = new List<AuditoriaRegistro>();

                foreach (DataRow fila in tabla.Rows)
                {
                    registros.Add(MapearAuditoria(fila));
                }

                return registros;
            }
            catch
            {
                return new List<AuditoriaRegistro>();
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<AuditoriaRegistro> ListarTodos()
        {
            const string sql = @"
                SELECT
                    id_auditoria,
                    entidad,
                    id_entidad,
                    accion,
                    id_usuario_actor,
                    identificador_usuario_actor,
                    fecha_evento,
                    estado_anterior_json,
                    estado_nuevo_json,
                    cambios_json
                FROM dbo.Auditoria
                ORDER BY fecha_evento DESC, id_auditoria DESC";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql);
                List<AuditoriaRegistro> registros = new List<AuditoriaRegistro>();

                foreach (DataRow fila in tabla.Rows)
                {
                    registros.Add(MapearAuditoria(fila));
                }

                return registros;
            }
            catch
            {
                return new List<AuditoriaRegistro>();
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        private static SqlParameter CrearParametro(string nombre, string valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor,
                DbType = DbType.String
            };
        }

        private static SqlParameter CrearParametro(string nombre, int valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = valor,
                DbType = DbType.Int32
            };
        }

        private static SqlParameter CrearParametro(string nombre, int? valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = valor.HasValue ? (object)valor.Value : DBNull.Value,
                DbType = DbType.Int32
            };
        }

        private static SqlParameter CrearParametro(string nombre, DateTime valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = valor,
                DbType = DbType.DateTime
            };
        }

        private static AuditoriaRegistro MapearAuditoria(DataRow fila)
        {
            return new AuditoriaRegistro
            {
                Id = Convert.ToInt32(fila["id_auditoria"]),
                Entidad = fila["entidad"].ToString(),
                IdEntidad = Convert.ToInt32(fila["id_entidad"]),
                Accion = fila["accion"].ToString(),
                IdUsuarioActor = fila["id_usuario_actor"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["id_usuario_actor"]),
                IdentificadorUsuarioActor = fila["identificador_usuario_actor"] == DBNull.Value ? null : fila["identificador_usuario_actor"].ToString(),
                FechaEvento = Convert.ToDateTime(fila["fecha_evento"]),
                EstadoAnteriorJson = fila["estado_anterior_json"] == DBNull.Value ? null : fila["estado_anterior_json"].ToString(),
                EstadoNuevoJson = fila["estado_nuevo_json"] == DBNull.Value ? null : fila["estado_nuevo_json"].ToString(),
                CambiosJson = fila["cambios_json"] == DBNull.Value ? null : fila["cambios_json"].ToString()
            };
        }
    }
}
