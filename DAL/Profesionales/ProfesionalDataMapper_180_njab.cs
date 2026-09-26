using System;
using System.Collections.Generic;
using System.Data;
using Domain;

namespace DAL
{
    public class ProfesionalDataMapper_180_njab
    {
        private readonly DatabaseContext _databaseContext_180_njab;

        public ProfesionalDataMapper_180_njab()
            : this(new DatabaseContext())
        {
        }

        public ProfesionalDataMapper_180_njab(DatabaseContext databaseContext_180_njab)
        {
            _databaseContext_180_njab = databaseContext_180_njab;
        }

        public List<ProfesionalConsulta_180_njab> Listar_180_njab()
        {
            //Conecta con la base para consultar los profesionales
            const string sql_180_njab = @"
                SELECT p.id_profesional,
                       p.dni,
                       p.nombre,
                       p.apellido,
                       p.matricula,
                       ISNULL(STUFF((
                           SELECT ', ' + e.nombre
                           FROM dbo.ProfesionalEspecialidad pe
                           INNER JOIN dbo.Especialidad e ON e.id_especialidad = pe.id_especialidad
                           WHERE pe.id_profesional = p.id_profesional
                           ORDER BY e.nombre
                           FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') AS especialidades
                FROM dbo.Profesional p
                ORDER BY p.apellido, p.nombre, p.matricula";

            try
            {
                _databaseContext_180_njab.Abrir();
                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab);
                List<ProfesionalConsulta_180_njab> profesionales_180_njab = new List<ProfesionalConsulta_180_njab>();

                foreach (DataRow fila_180_njab in tabla_180_njab.Rows)
                {
                    profesionales_180_njab.Add(new ProfesionalConsulta_180_njab
                    {
                        Id_180_njab = Convert.ToInt32(fila_180_njab["id_profesional"]),
                        Dni_180_njab = fila_180_njab["dni"] == DBNull.Value ? null : fila_180_njab["dni"].ToString(),
                        Nombre_180_njab = fila_180_njab["nombre"].ToString(),
                        Apellido_180_njab = fila_180_njab["apellido"].ToString(),
                        Matricula_180_njab = fila_180_njab["matricula"].ToString(),
                        Especialidad_180_njab = fila_180_njab["especialidades"].ToString()
                    });
                }

                return profesionales_180_njab;
            }
            catch
            {
                return new List<ProfesionalConsulta_180_njab>();
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }
        }
    }
}
