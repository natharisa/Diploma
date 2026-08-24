using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Domain;

namespace DAL
{
    public class PermisoDataMapper
    {
        private readonly DatabaseContext _databaseContext;

        public PermisoDataMapper()
            : this(new DatabaseContext())
        {
        }

        public PermisoDataMapper(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<ComponentePermiso> ListarAsignadosPorUsuario(int idUsuario)
        {
            try
            {
                _databaseContext.Abrir();
                Dictionary<int, ComponentePermiso> componentes = CargarComponentes();
                CargarRelaciones(componentes);

                List<int> idsAsignados = CargarIdsAsignados(idUsuario);
                List<ComponentePermiso> asignados = new List<ComponentePermiso>();
                foreach (int idAsignado in idsAsignados)
                {
                    if (componentes.ContainsKey(idAsignado))
                    {
                        asignados.Add(componentes[idAsignado]);
                    }
                }

                return asignados;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<ComponentePermiso> ListarArbolCompleto()
        {
            try
            {
                _databaseContext.Abrir();
                Dictionary<int, ComponentePermiso> componentes = CargarComponentes();
                HashSet<int> idsHijos = CargarRelaciones(componentes);
                List<ComponentePermiso> raices = new List<ComponentePermiso>();

                foreach (ComponentePermiso componente in componentes.Values)
                {
                    if (!idsHijos.Contains(componente.Id))
                    {
                        raices.Add(componente);
                    }
                }

                OrdenarPorNombre(raices);
                return raices;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<ComponentePermiso> ListarComponentes()
        {
            try
            {
                _databaseContext.Abrir();
                List<ComponentePermiso> componentes = new List<ComponentePermiso>(CargarComponentes().Values);
                componentes.Sort(CompararPorTipoYNombre);
                return componentes;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<ComponentePermiso> ListarFamilias()
        {
            try
            {
                _databaseContext.Abrir();
                List<ComponentePermiso> familias = new List<ComponentePermiso>(CargarComponentesPorTipo("FAMILIA").Values);
                OrdenarPorNombre(familias);
                return familias;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public List<int> ListarIdsComponentesAsignadosPorUsuario(int idUsuario)
        {
            try
            {
                _databaseContext.Abrir();
                return CargarIdsAsignados(idUsuario);
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool CrearFamilia(string codigo, string nombre, string descripcion)
        {
            const string sql = @"
                IF EXISTS (
                    SELECT 1
                    FROM dbo.ComponentePermiso
                    WHERE codigo = @codigo
                      AND tipo <> 'FAMILIA'
                )
                BEGIN
                    SELECT CAST(0 AS INT) AS resultado;
                    RETURN;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM dbo.ComponentePermiso
                    WHERE codigo = @codigo
                      AND tipo = 'FAMILIA'
                )
                BEGIN
                    UPDATE dbo.ComponentePermiso
                    SET nombre = @nombre,
                        descripcion = @descripcion,
                        estado_componente = 'ACTIVO'
                    WHERE codigo = @codigo
                      AND tipo = 'FAMILIA';
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.ComponentePermiso
                    (
                        codigo,
                        nombre,
                        descripcion,
                        tipo,
                        estado_componente
                    )
                    VALUES
                    (
                        @codigo,
                        @nombre,
                        @descripcion,
                        'FAMILIA',
                        'ACTIVO'
                    );
                END;

                SELECT CAST(1 AS INT) AS resultado;";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@codigo", codigo),
                _databaseContext.CrearParametro("@nombre", nombre),
                _databaseContext.CrearParametro("@descripcion", descripcion)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
                return tabla.Rows.Count > 0 && Convert.ToInt32(tabla.Rows[0]["resultado"]) == 1;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public string AgregarRelacion(int idPadre, int idHijo)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_padre", idPadre),
                _databaseContext.CrearParametro("@id_hijo", idHijo)
            };

            try
            {
                _databaseContext.Abrir();
                DataTable tabla = _databaseContext.Leer("sp_ComponentePermiso_AgregarRelacion", parametros);
                return tabla.Rows.Count == 0 ? "ERROR" : tabla.Rows[0]["codigo_resultado"].ToString();
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool QuitarRelacion(int idPadre, int idHijo)
        {
            const string sql = @"
                DELETE FROM dbo.ComponentePermisoRelacion
                WHERE id_padre = @id_padre
                  AND id_hijo = @id_hijo";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@id_padre", idPadre),
                _databaseContext.CrearParametro("@id_hijo", idHijo)
            };

            try
            {
                _databaseContext.Abrir();
                return _databaseContext.EscribirTexto(sql, parametros) >= 0;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool GuardarComponentesUsuario(int idUsuario, List<int> idsComponentes)
        {
            const string desactivarSql = @"
                UPDATE dbo.UsuarioComponentePermiso
                SET estado_usuario_componente = 'INACTIVO'
                WHERE id_usuario = @id_usuario";

            const string guardarSql = @"
                IF EXISTS (
                    SELECT 1
                    FROM dbo.UsuarioComponentePermiso
                    WHERE id_usuario = @id_usuario
                      AND id_componente = @id_componente
                )
                BEGIN
                    UPDATE dbo.UsuarioComponentePermiso
                    SET estado_usuario_componente = 'ACTIVO'
                    WHERE id_usuario = @id_usuario
                      AND id_componente = @id_componente;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.UsuarioComponentePermiso
                    (
                        id_usuario,
                        id_componente,
                        estado_usuario_componente
                    )
                    VALUES
                    (
                        @id_usuario,
                        @id_componente,
                        'ACTIVO'
                    );
                END";

            try
            {
                _databaseContext.Abrir();
                _databaseContext.IniciarTx();

                List<SqlParameter> parametrosUsuario = new List<SqlParameter>
                {
                    _databaseContext.CrearParametro("@id_usuario", idUsuario)
                };

                if (_databaseContext.EscribirTexto(desactivarSql, parametrosUsuario) < 0)
                {
                    _databaseContext.Deshacer();
                    return false;
                }

                foreach (int idComponente in idsComponentes ?? new List<int>())
                {
                    List<SqlParameter> parametros = new List<SqlParameter>
                    {
                        _databaseContext.CrearParametro("@id_usuario", idUsuario),
                        _databaseContext.CrearParametro("@id_componente", idComponente)
                    };

                    if (_databaseContext.EscribirTexto(guardarSql, parametros) < 0)
                    {
                        _databaseContext.Deshacer();
                        return false;
                    }
                }

                _databaseContext.Confirmar();
                return true;
            }
            catch
            {
                _databaseContext.Deshacer();
                return false;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        public bool PuedeAgregarRelacion(int idPadre, int idHijo)
        {
            if (idPadre == idHijo)
            {
                return false;
            }

            const string sql = @"
                ;WITH Descendientes AS
                (
                    SELECT id_hijo
                    FROM dbo.ComponentePermisoRelacion
                    WHERE id_padre = @id_hijo

                    UNION ALL

                    SELECT r.id_hijo
                    FROM dbo.ComponentePermisoRelacion r
                    INNER JOIN Descendientes d
                        ON d.id_hijo = r.id_padre
                )
                SELECT TOP (1) id_hijo
                FROM Descendientes
                WHERE id_hijo = @id_padre";

            try
            {
                _databaseContext.Abrir();
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    _databaseContext.CrearParametro("@id_padre", idPadre),
                    _databaseContext.CrearParametro("@id_hijo", idHijo)
                };

                return _databaseContext.LeerTexto(sql, parametros).Rows.Count == 0;
            }
            finally
            {
                _databaseContext.Cerrar();
            }
        }

        private Dictionary<int, ComponentePermiso> CargarComponentes()
        {
            const string sql = @"
                SELECT
                    id_componente,
                    codigo,
                    nombre,
                    descripcion,
                    tipo,
                    estado_componente
                FROM dbo.ComponentePermiso
                WHERE UPPER(estado_componente) = 'ACTIVO'
                ORDER BY nombre";

            DataTable tabla = _databaseContext.LeerTexto(sql);
            Dictionary<int, ComponentePermiso> componentes = new Dictionary<int, ComponentePermiso>();

            foreach (DataRow fila in tabla.Rows)
            {
                ComponentePermiso componente = CrearComponente(fila);
                componentes[componente.Id] = componente;
            }

            return componentes;
        }

        private Dictionary<int, ComponentePermiso> CargarComponentesPorTipo(string tipo)
        {
            const string sql = @"
                SELECT
                    id_componente,
                    codigo,
                    nombre,
                    descripcion,
                    tipo,
                    estado_componente
                FROM dbo.ComponentePermiso
                WHERE UPPER(estado_componente) = 'ACTIVO'
                  AND tipo = @tipo
                ORDER BY nombre";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                _databaseContext.CrearParametro("@tipo", tipo)
            };

            DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
            Dictionary<int, ComponentePermiso> componentes = new Dictionary<int, ComponentePermiso>();

            foreach (DataRow fila in tabla.Rows)
            {
                ComponentePermiso componente = CrearComponente(fila);
                componentes[componente.Id] = componente;
            }

            return componentes;
        }

        private HashSet<int> CargarRelaciones(Dictionary<int, ComponentePermiso> componentes)
        {
            const string sql = @"
                SELECT r.id_padre, r.id_hijo
                FROM dbo.ComponentePermisoRelacion r
                INNER JOIN dbo.ComponentePermiso padre
                    ON padre.id_componente = r.id_padre
                INNER JOIN dbo.ComponentePermiso hijo
                    ON hijo.id_componente = r.id_hijo
                WHERE UPPER(padre.estado_componente) = 'ACTIVO'
                  AND UPPER(hijo.estado_componente) = 'ACTIVO'";

            DataTable tabla = _databaseContext.LeerTexto(sql);
            HashSet<int> idsHijos = new HashSet<int>();

            foreach (DataRow fila in tabla.Rows)
            {
                int idPadre = Convert.ToInt32(fila["id_padre"]);
                int idHijo = Convert.ToInt32(fila["id_hijo"]);

                if (!componentes.ContainsKey(idPadre) || !componentes.ContainsKey(idHijo))
                {
                    continue;
                }

                FamiliaPermiso padre = componentes[idPadre] as FamiliaPermiso;
                if (padre == null)
                {
                    continue;
                }

                padre.Agregar(componentes[idHijo]);
                idsHijos.Add(idHijo);
            }

            return idsHijos;
        }

        private List<int> CargarIdsAsignados(int idUsuario)
        {
            const string sql = @"
                SELECT uc.id_componente
                FROM dbo.UsuarioComponentePermiso uc
                INNER JOIN dbo.ComponentePermiso c
                    ON c.id_componente = uc.id_componente
                WHERE uc.id_usuario = @id_usuario
                  AND UPPER(uc.estado_usuario_componente) = 'ACTIVO'
                  AND UPPER(c.estado_componente) = 'ACTIVO'
                ORDER BY c.nombre";

            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                _databaseContext.CrearParametro("@id_usuario", idUsuario)
            };

            DataTable tabla = _databaseContext.LeerTexto(sql, parametros);
            List<int> ids = new List<int>();

            foreach (DataRow fila in tabla.Rows)
            {
                ids.Add(Convert.ToInt32(fila["id_componente"]));
            }

            return ids;
        }

        private static ComponentePermiso CrearComponente(DataRow fila)
        {
            string tipo = fila["tipo"].ToString();
            ComponentePermiso componente = string.Equals(tipo, "FAMILIA", StringComparison.OrdinalIgnoreCase)
                ? (ComponentePermiso)new FamiliaPermiso()
                : new Permiso();

            componente.Id = Convert.ToInt32(fila["id_componente"]);
            componente.Codigo = fila["codigo"].ToString();
            componente.Nombre = fila["nombre"].ToString();
            componente.Descripcion = fila["descripcion"] == DBNull.Value ? null : fila["descripcion"].ToString();
            componente.Estado = fila["estado_componente"].ToString();

            return componente;
        }

        private static void OrdenarPorNombre(List<ComponentePermiso> componentes)
        {
            componentes.Sort(delegate (ComponentePermiso primero, ComponentePermiso segundo)
            {
                return string.Compare(primero.Nombre, segundo.Nombre, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static int CompararPorTipoYNombre(ComponentePermiso primero, ComponentePermiso segundo)
        {
            int comparacionTipo = primero.Tipo.CompareTo(segundo.Tipo);
            if (comparacionTipo != 0)
            {
                return comparacionTipo;
            }

            return string.Compare(primero.Nombre, segundo.Nombre, StringComparison.OrdinalIgnoreCase);
        }
    }
}
