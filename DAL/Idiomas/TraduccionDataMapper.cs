using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Domain;

namespace DAL
{
    public class TraduccionDataMapper
    {
        private readonly DatabaseContext _databaseContext;

        public TraduccionDataMapper()
            : this(new DatabaseContext())
        {
        }

        public TraduccionDataMapper(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public int CrearEtiqueta(Etiqueta etiqueta)
        {
            if (etiqueta == null)
            {
                return -1;
            }

            const string sql = @"
                INSERT INTO dbo.Etiqueta (clave, descripcion)
                OUTPUT INSERTED.id_etiqueta
                VALUES (@clave, @descripcion)";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@clave", etiqueta.Key),
                _databaseContext.CrearParametro("@descripcion", etiqueta.Descripcion)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                if (tabla.Rows.Count == 0)
                {
                    return -1;
                }

                etiqueta.Id = Convert.ToInt32(tabla.Rows[0]["id_etiqueta"]);
                return etiqueta.Id;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            if (traduccion == null || traduccion.EtiquetaId == 0 || traduccion.IdiomaId == 0)
            {
                return false;
            }

            const string sql = @"
                MERGE dbo.Traduccion AS target
                USING (SELECT @id_etiqueta AS id_etiqueta, @id_idioma AS id_idioma) AS source
                    ON target.id_etiqueta = source.id_etiqueta
                   AND target.id_idioma = source.id_idioma
                WHEN MATCHED THEN
                    UPDATE SET texto = @texto
                WHEN NOT MATCHED THEN
                    INSERT (id_etiqueta, id_idioma, texto)
                    VALUES (@id_etiqueta, @id_idioma, @texto);";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_etiqueta", traduccion.EtiquetaId),
                _databaseContext.CrearParametro("@id_idioma", traduccion.IdiomaId),
                _databaseContext.CrearParametro("@texto", traduccion.Texto)
            };

            try
            {
                _databaseContext.Abrir();
                return _databaseContext.EscribirTexto(sql, parametros) > 0;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public string ObtenerTexto(string key, int idiomaId)
        {
            const string sql = @"
                SELECT TOP (1) t.texto
                FROM dbo.Traduccion t
                INNER JOIN dbo.Etiqueta e ON e.id_etiqueta = t.id_etiqueta
                WHERE e.clave = @clave
                  AND t.id_idioma = @id_idioma";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@clave", key),
                _databaseContext.CrearParametro("@id_idioma", idiomaId)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                return tabla.Rows.Count == 0 ? null : tabla.Rows[0]["texto"].ToString();
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public Traduccion ObtenerTraduccion(int etiquetaId, int idiomaId)
        {
            const string sql = @"
                SELECT TOP (1)
                    t.id_traduccion,
                    t.id_etiqueta,
                    t.id_idioma,
                    t.texto,
                    e.clave,
                    i.codigo
                FROM dbo.Traduccion t
                INNER JOIN dbo.Etiqueta e ON e.id_etiqueta = t.id_etiqueta
                INNER JOIN dbo.Idioma i ON i.id_idioma = t.id_idioma
                WHERE t.id_etiqueta = @id_etiqueta
                  AND t.id_idioma = @id_idioma";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_etiqueta", etiquetaId),
                _databaseContext.CrearParametro("@id_idioma", idiomaId)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                return tabla.Rows.Count == 0 ? null : MapearTraduccion(tabla.Rows[0]);
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public Dictionary<string, string> ListarPorIdioma(int idiomaId)
        {
            const string sql = @"
                SELECT e.clave, t.texto
                FROM dbo.Traduccion t
                INNER JOIN dbo.Etiqueta e ON e.id_etiqueta = t.id_etiqueta
                WHERE t.id_idioma = @id_idioma";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_idioma", idiomaId)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                Dictionary<string, string> traducciones = new Dictionary<string, string>();

                foreach (DataRow registro in tabla.Rows)
                {
                    traducciones[registro["clave"].ToString()] = registro["texto"].ToString();
                }

                return traducciones;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<Etiqueta> ListarEtiquetas()
        {
            const string sql = @"
                SELECT id_etiqueta, clave, descripcion
                FROM dbo.Etiqueta
                ORDER BY clave";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql);
                List<Etiqueta> etiquetas = new List<Etiqueta>();

                foreach (DataRow registro in tabla.Rows)
                {
                    etiquetas.Add(new Etiqueta
                    {
                        Id = Convert.ToInt32(registro["id_etiqueta"]),
                        Key = registro["clave"].ToString(),
                        Descripcion = registro["descripcion"] == DBNull.Value ? null : registro["descripcion"].ToString()
                    });
                }

                return etiquetas;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<Traduccion> ListarTraducciones()
        {
            const string sql = @"
                SELECT
                    t.id_traduccion,
                    t.id_etiqueta,
                    t.id_idioma,
                    t.texto,
                    e.clave,
                    i.codigo
                FROM dbo.Traduccion t
                INNER JOIN dbo.Etiqueta e ON e.id_etiqueta = t.id_etiqueta
                INNER JOIN dbo.Idioma i ON i.id_idioma = t.id_idioma
                ORDER BY e.clave, i.codigo";

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql);
                List<Traduccion> traducciones = new List<Traduccion>();

                foreach (DataRow registro in tabla.Rows)
                {
                    traducciones.Add(MapearTraduccion(registro));
                }

                return traducciones;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        private static Traduccion MapearTraduccion(DataRow registro)
        {
            return new Traduccion
            {
                Id = Convert.ToInt32(registro["id_traduccion"]),
                EtiquetaId = Convert.ToInt32(registro["id_etiqueta"]),
                IdiomaId = Convert.ToInt32(registro["id_idioma"]),
                Texto = registro["texto"].ToString(),
                EtiquetaKey = registro["clave"].ToString(),
                IdiomaCodigo = registro["codigo"].ToString()
            };
        }
    }
}
