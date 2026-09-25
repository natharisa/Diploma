using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DatabaseContext
    {
        private const string CadenaConexion_180_njab = "Data Source=.;Initial Catalog=TecniSalud;Integrated Security=SSPI";

        public SqlConnection Conexion { get; private set; }
        public SqlTransaction Transaccion { get; private set; }

        public void Abrir()
        {
            Conexion = new SqlConnection(CadenaConexion_180_njab);
            Conexion.Open();
        }

        public void Cerrar()
        {
            if (Conexion != null)
            {
                Conexion.Close();
                Conexion = null;
            }
        }

        #region Transaccion
        public void IniciarTx()
        {
            if (Conexion != null)
            {
                Transaccion = Conexion.BeginTransaction();
            }
        }

        public void Confirmar()
        {
            if (Transaccion != null)
            {
                Transaccion.Commit();
                Transaccion = null;
            }
        }

        public void Deshacer()
        {
            if (Transaccion != null)
            {
                Transaccion.Rollback();
                Transaccion = null;
            }
        }
        #endregion
        public SqlParameter CrearParametro(string nombre, string valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor,
                DbType = DbType.String
            };
        }

        public SqlParameter CrearParametro(string nombre, int valor)
        {
            return new SqlParameter
            {
                ParameterName = nombre,
                Value = valor,
                DbType = DbType.Int32
            };
        }

        public int Escribir(string sql, List<SqlParameter> parametros = null)
        {
            using (SqlCommand comando = CrearComando(sql, parametros))
            {
                try
                {
                    return comando.ExecuteNonQuery();
                }
                catch
                {
                    return -1;
                }
            }
        }

        public int EscribirTexto(string sql, List<SqlParameter> parametros = null)
        {
            using (SqlCommand comando = CrearComando(sql, parametros, CommandType.Text))
            {
                try
                {
                    return comando.ExecuteNonQuery();
                }
                catch
                {
                    return -1;
                }
            }
        }

        public DataTable Leer(string sql, List<SqlParameter> parametros = null)
        {
            using (SqlDataAdapter adaptador = new SqlDataAdapter())
            {
                DataTable tabla = new DataTable();
                adaptador.SelectCommand = CrearComando(sql, parametros);
                adaptador.Fill(tabla);
                return tabla;
            }
        }

        public DataTable LeerTexto(string sql, List<SqlParameter> parametros = null)
        {
            using (SqlDataAdapter adaptador = new SqlDataAdapter())
            {
                DataTable tabla = new DataTable();
                adaptador.SelectCommand = CrearComando(sql, parametros, CommandType.Text);
                adaptador.Fill(tabla);
                return tabla;
            }
        }

        private SqlCommand CrearComando(string sql, List<SqlParameter> parametros = null, CommandType commandType = CommandType.StoredProcedure)
        {
            SqlCommand comando = new SqlCommand(sql, Conexion)
            {
                CommandType = commandType
            };

            if (Transaccion != null)
            {
                comando.Transaction = Transaccion;
            }

            if (parametros != null && parametros.Count > 0)
            {
                comando.Parameters.AddRange(parametros.ToArray());
            }

            return comando;
        }

    }
}
