using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Domain;

namespace DAL
{
    public class BitacoraDataMapper
    {
        private readonly DatabaseContext _databaseContext;

        public BitacoraDataMapper()
            : this(new DatabaseContext())
        {
        }

        public BitacoraDataMapper(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public int Insertar(BitacoraRegistro bitacora)
        {
            if (bitacora == null)
            {
                return -1;
            }

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                CrearParametro("@id_usuario", bitacora.IdUsuario),
                CrearParametro("@identificador_usuario", bitacora.IdentificadorUsuario),
                CrearParametro("@modulo", bitacora.Modulo),
                CrearParametro("@accion", bitacora.Accion),
                CrearParametro("@nivel", bitacora.Nivel),
                CrearParametro("@descripcion", bitacora.Descripcion),
                CrearParametro("@equipo", bitacora.Equipo),
                CrearParametro("@fecha_evento", bitacora.Fecha)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.Leer("sp_Bitacora_Registrar", parametros);

                if (tabla.Rows.Count == 0)
                {
                    return -1;
                }

                bitacora.Id = Convert.ToInt32(tabla.Rows[0]["id_bitacora"]);
                return bitacora.Id;
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

        public List<BitacoraRegistro> Listar()
        {
            const string sql = @"
                SELECT
                    id_bitacora,
                    id_usuario,
                    identificador_usuario,
                    modulo,
                    accion,
                    nivel,
                    descripcion,
                    equipo,
                    fecha_evento
                FROM dbo.Bitacora
                ORDER BY fecha_evento DESC, id_bitacora DESC";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql);
                List<BitacoraRegistro> registros = new List<BitacoraRegistro>();

                foreach (DataRow fila in tabla.Rows)
                {
                    registros.Add(MapearBitacora(fila));
                }

                return registros;
            }
            catch
            {
                return new List<BitacoraRegistro>();
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        #region CreacParametros
        private static SqlParameter CrearParametro(string nombre, string valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor,
                DbType = DbType.String
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
        #endregion
        private static BitacoraRegistro MapearBitacora(DataRow fila)
        {
            return new BitacoraRegistro
            {
                Id = Convert.ToInt32(fila["id_bitacora"]),
                IdUsuario = fila["id_usuario"] == DBNull.Value ? (int?)null : Convert.ToInt32(fila["id_usuario"]),
                IdentificadorUsuario = fila["identificador_usuario"] == DBNull.Value ? null : fila["identificador_usuario"].ToString(),
                Modulo = fila["modulo"].ToString(),
                Accion = fila["accion"].ToString(),
                Nivel = fila["nivel"].ToString(),
                Descripcion = fila["descripcion"] == DBNull.Value ? null : fila["descripcion"].ToString(),
                Equipo = fila["equipo"] == DBNull.Value ? null : fila["equipo"].ToString(),
                Fecha = Convert.ToDateTime(fila["fecha_evento"])
            };
        }
    }
}
