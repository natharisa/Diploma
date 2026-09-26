using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Domain;

namespace DAL
{
    public class JornadaDataMapper_180_njab
    {
        private readonly DatabaseContext _databaseContext_180_njab;

        public JornadaDataMapper_180_njab()
            : this(new DatabaseContext())
        {
        }

        public JornadaDataMapper_180_njab(DatabaseContext databaseContext_180_njab)
        {
            _databaseContext_180_njab = databaseContext_180_njab;
        }

        public List<EspecialidadConsulta_180_njab> ListarEspecialidades_180_njab(int idProfesional_180_njab)
        {
            const string sql_180_njab = @"
                SELECT e.id_especialidad, e.nombre
                FROM dbo.ProfesionalEspecialidad pe
                INNER JOIN dbo.Especialidad e ON e.id_especialidad = pe.id_especialidad
                WHERE pe.id_profesional = @id_profesional
                ORDER BY e.nombre";

            List<EspecialidadConsulta_180_njab> especialidades_180_njab = new List<EspecialidadConsulta_180_njab>();
            try
            {
                _databaseContext_180_njab.Abrir();
                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, new List<SqlParameter>
                {
                    CrearParametro_180_njab("@id_profesional", idProfesional_180_njab)
                });

                foreach (DataRow fila_180_njab in tabla_180_njab.Rows)
                {
                    especialidades_180_njab.Add(new EspecialidadConsulta_180_njab
                    {
                        IdEspecialidad_180_njab = Convert.ToInt32(fila_180_njab["id_especialidad"]),
                        Nombre_180_njab = fila_180_njab["nombre"].ToString()
                    });
                }
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }

            return especialidades_180_njab;
        }

        public List<ConsultorioConsulta_180_njab> ListarConsultoriosDisponibles_180_njab(
            DateTime fecha_180_njab,
            TimeSpan horaInicio_180_njab,
            TimeSpan horaFin_180_njab,
            int idJornadaExcluir_180_njab)
        {
            const string sql_180_njab = @"
                SELECT c.id_consultorio, c.nombre_o_numero
                FROM dbo.Consultorio c
                WHERE c.estado_consultorio = 'Activo'
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.JornadaProfesional j
                      WHERE j.id_consultorio = c.id_consultorio
                        AND j.estado_jornada = 'Activa'
                        AND j.fecha = @fecha
                        AND j.hora_inicio < @hora_fin
                        AND j.hora_fin > @hora_inicio
                        AND j.id_jornada <> @id_jornada_excluir
                  )
                ORDER BY c.nombre_o_numero";

            List<ConsultorioConsulta_180_njab> consultorios_180_njab = new List<ConsultorioConsulta_180_njab>();
            try
            {
                _databaseContext_180_njab.Abrir();
                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, CrearParametrosDisponibilidad_180_njab(
                    fecha_180_njab, horaInicio_180_njab, horaFin_180_njab, idJornadaExcluir_180_njab));
                foreach (DataRow fila_180_njab in tabla_180_njab.Rows)
                {
                    consultorios_180_njab.Add(new ConsultorioConsulta_180_njab
                    {
                        IdConsultorio_180_njab = Convert.ToInt32(fila_180_njab["id_consultorio"]),
                        Nombre_180_njab = fila_180_njab["nombre_o_numero"].ToString()
                    });
                }
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }

            return consultorios_180_njab;
        }

        public List<JornadaProfesionalConsulta_180_njab> ListarJornadas_180_njab(int idProfesional_180_njab)
        {
            const string sql_180_njab = @"
                SELECT j.id_jornada, j.id_profesional, j.id_consultorio,
                       j.fecha, j.hora_inicio, j.hora_fin, j.duracion_turno_min,
                       j.estado_jornada, c.nombre_o_numero AS consultorio,
                       CAST(NULL AS VARCHAR(100)) AS especialidad
                FROM dbo.JornadaProfesional j
                INNER JOIN dbo.Consultorio c ON c.id_consultorio = j.id_consultorio
                WHERE j.id_profesional = @id_profesional
                  AND j.estado_jornada = 'Activa'
                ORDER BY j.fecha, j.hora_inicio, c.nombre_o_numero";

            List<JornadaProfesionalConsulta_180_njab> jornadas_180_njab = new List<JornadaProfesionalConsulta_180_njab>();
            try
            {
                _databaseContext_180_njab.Abrir();
                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, new List<SqlParameter>
                {
                    CrearParametro_180_njab("@id_profesional", idProfesional_180_njab)
                });
                foreach (DataRow fila_180_njab in tabla_180_njab.Rows)
                {
                    jornadas_180_njab.Add(MapearJornada_180_njab(fila_180_njab));
                }
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }

            return jornadas_180_njab;
        }

        public JornadaOperacionResultado_180_njab Crear_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            const string sql_180_njab = @"
                INSERT INTO dbo.JornadaProfesional
                (id_profesional, id_consultorio, fecha, hora_inicio, hora_fin, duracion_turno_min, estado_jornada)
                OUTPUT INSERTED.id_jornada
                VALUES (@id_profesional, @id_consultorio, @fecha, @hora_inicio, @hora_fin, @duracion_turno_min, 'Activa')";

            return EjecutarGuardado_180_njab(sql_180_njab, solicitud_180_njab, 0, "La jornada fue asignada correctamente.");
        }

        public JornadaOperacionResultado_180_njab Modificar_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            const string sql_180_njab = @"
                UPDATE dbo.JornadaProfesional
                SET id_consultorio = @id_consultorio,
                    fecha = @fecha,
                    hora_inicio = @hora_inicio,
                    hora_fin = @hora_fin,
                    duracion_turno_min = @duracion_turno_min
                OUTPUT INSERTED.id_jornada
                WHERE id_jornada = @id_jornada
                  AND id_profesional = @id_profesional
                  AND estado_jornada = 'Activa'";

            try
            {
                _databaseContext_180_njab.Abrir();
                _databaseContext_180_njab.IniciarTx_180_njab(IsolationLevel.Serializable);
                JornadaOperacionResultado_180_njab turnos_180_njab = ValidarTurnosVigentesEnTx_180_njab(solicitud_180_njab.IdJornada_180_njab);
                if (turnos_180_njab.TieneTurnosVigentes_180_njab)
                {
                    _databaseContext_180_njab.Deshacer();
                    return turnos_180_njab;
                }

                JornadaOperacionResultado_180_njab conflicto_180_njab = ValidarConflictoEnTx_180_njab(solicitud_180_njab, solicitud_180_njab.IdJornada_180_njab);
                if (!conflicto_180_njab.Exito_180_njab)
                {
                    _databaseContext_180_njab.Deshacer();
                    return conflicto_180_njab;
                }

                if (!ReferenciasValidasEnTx_180_njab(solicitud_180_njab))
                {
                    _databaseContext_180_njab.Deshacer();
                    return JornadaOperacionResultado_180_njab.CrearError_180_njab("El profesional o el consultorio ya no están disponibles.");
                }

                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, CrearParametrosSolicitud_180_njab(solicitud_180_njab));
                if (tabla_180_njab.Rows.Count == 0)
                {
                    _databaseContext_180_njab.Deshacer();
                    return JornadaOperacionResultado_180_njab.CrearError_180_njab("La jornada seleccionada ya no está activa.");
                }

                int idJornada_180_njab = Convert.ToInt32(tabla_180_njab.Rows[0]["id_jornada"]);
                _databaseContext_180_njab.Confirmar();
                return JornadaOperacionResultado_180_njab.CrearExito_180_njab(idJornada_180_njab, "La jornada fue modificada correctamente.");
            }
            catch
            {
                _databaseContext_180_njab.Deshacer();
                return JornadaOperacionResultado_180_njab.CrearError_180_njab("No se pudo modificar la jornada.");
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }
        }

        public JornadaOperacionResultado_180_njab QuitarAsignacion_180_njab(int idJornada_180_njab, int idProfesional_180_njab)
        {
            const string sql_180_njab = @"
                UPDATE dbo.JornadaProfesional
                SET estado_jornada = 'Cancelada'
                OUTPUT INSERTED.id_jornada
                WHERE id_jornada = @id_jornada
                  AND id_profesional = @id_profesional
                  AND estado_jornada = 'Activa'";

            try
            {
                _databaseContext_180_njab.Abrir();
                _databaseContext_180_njab.IniciarTx_180_njab(IsolationLevel.Serializable);
                JornadaOperacionResultado_180_njab turnos_180_njab = ValidarTurnosVigentesEnTx_180_njab(idJornada_180_njab);
                if (turnos_180_njab.TieneTurnosVigentes_180_njab)
                {
                    _databaseContext_180_njab.Deshacer();
                    return turnos_180_njab;
                }

                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, new List<SqlParameter>
                {
                    CrearParametro_180_njab("@id_jornada", idJornada_180_njab),
                    CrearParametro_180_njab("@id_profesional", idProfesional_180_njab)
                });
                if (tabla_180_njab.Rows.Count == 0)
                {
                    _databaseContext_180_njab.Deshacer();
                    return JornadaOperacionResultado_180_njab.CrearError_180_njab("La jornada seleccionada ya no está activa.");
                }

                _databaseContext_180_njab.Confirmar();
                return JornadaOperacionResultado_180_njab.CrearExito_180_njab(idJornada_180_njab, "La asignación fue quitada y se conservó su historial.");
            }
            catch
            {
                _databaseContext_180_njab.Deshacer();
                return JornadaOperacionResultado_180_njab.CrearError_180_njab("No se pudo quitar la asignación.");
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }
        }

        public JornadaOperacionResultado_180_njab TieneTurnosVigentes_180_njab(int idJornada_180_njab)
        {
            try
            {
                _databaseContext_180_njab.Abrir();
                return ValidarTurnosVigentesEnTx_180_njab(idJornada_180_njab);
            }
            catch
            {
                return JornadaOperacionResultado_180_njab.CrearError_180_njab("No se pudo comprobar el estado de los turnos.");
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }
        }

        private JornadaOperacionResultado_180_njab EjecutarGuardado_180_njab(string sql_180_njab, JornadaSolicitud_180_njab solicitud_180_njab, int idJornadaExcluir_180_njab, string mensajeExito_180_njab)
        {
            try
            {
                _databaseContext_180_njab.Abrir();
                _databaseContext_180_njab.IniciarTx_180_njab(IsolationLevel.Serializable);
                JornadaOperacionResultado_180_njab conflicto_180_njab = ValidarConflictoEnTx_180_njab(solicitud_180_njab, idJornadaExcluir_180_njab);
                if (!conflicto_180_njab.Exito_180_njab)
                {
                    _databaseContext_180_njab.Deshacer();
                    return conflicto_180_njab;
                }

                if (!ReferenciasValidasEnTx_180_njab(solicitud_180_njab))
                {
                    _databaseContext_180_njab.Deshacer();
                    return JornadaOperacionResultado_180_njab.CrearError_180_njab("El profesional o el consultorio ya no están disponibles.");
                }

                DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, CrearParametrosSolicitud_180_njab(solicitud_180_njab));
                if (tabla_180_njab.Rows.Count == 0)
                {
                    _databaseContext_180_njab.Deshacer();
                    return JornadaOperacionResultado_180_njab.CrearError_180_njab("No se pudo guardar la jornada seleccionada.");
                }

                int idJornada_180_njab = Convert.ToInt32(tabla_180_njab.Rows[0]["id_jornada"]);
                _databaseContext_180_njab.Confirmar();
                return JornadaOperacionResultado_180_njab.CrearExito_180_njab(idJornada_180_njab, mensajeExito_180_njab);
            }
            catch
            {
                _databaseContext_180_njab.Deshacer();
                return JornadaOperacionResultado_180_njab.CrearError_180_njab("No se pudo guardar la jornada.");
            }
            finally
            {
                _databaseContext_180_njab.Cerrar();
            }
        }

        private JornadaOperacionResultado_180_njab ValidarConflictoEnTx_180_njab(JornadaSolicitud_180_njab solicitud_180_njab, int idJornadaExcluir_180_njab)
        {
            const string sql_180_njab = @"
                SELECT ISNULL(MAX(CASE WHEN j.id_profesional = @id_profesional THEN 1 ELSE 0 END), 0) AS conflicto_profesional,
                       ISNULL(MAX(CASE WHEN j.id_consultorio = @id_consultorio THEN 1 ELSE 0 END), 0) AS conflicto_consultorio,
                       MIN(j.fecha) AS fecha_conflicto,
                       MIN(j.hora_inicio) AS hora_inicio_conflicto,
                       MIN(j.hora_fin) AS hora_fin_conflicto
                FROM dbo.JornadaProfesional j WITH (UPDLOCK, HOLDLOCK)
                WHERE j.estado_jornada = 'Activa'
                  AND j.fecha = @fecha
                  AND j.hora_inicio < @hora_fin
                  AND j.hora_fin > @hora_inicio
                  AND j.id_jornada <> @id_jornada_excluir
                  AND (j.id_profesional = @id_profesional OR j.id_consultorio = @id_consultorio)";

            DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, CrearParametrosSolicitud_180_njab(solicitud_180_njab, idJornadaExcluir_180_njab));
            DataRow fila_180_njab = tabla_180_njab.Rows[0];
            bool conflictoProfesional_180_njab = Convert.ToInt32(fila_180_njab["conflicto_profesional"]) == 1;
            bool conflictoConsultorio_180_njab = Convert.ToInt32(fila_180_njab["conflicto_consultorio"]) == 1;
            if (!conflictoProfesional_180_njab && !conflictoConsultorio_180_njab)
            {
                return JornadaOperacionResultado_180_njab.CrearExito_180_njab(0, string.Empty);
            }

            JornadaOperacionResultado_180_njab resultado_180_njab = JornadaOperacionResultado_180_njab.CrearError_180_njab(ConstruirMensajeConflicto_180_njab(conflictoProfesional_180_njab, conflictoConsultorio_180_njab, fila_180_njab));
            resultado_180_njab.ConflictoProfesional_180_njab = conflictoProfesional_180_njab;
            resultado_180_njab.ConflictoConsultorio_180_njab = conflictoConsultorio_180_njab;
            if (fila_180_njab["fecha_conflicto"] != DBNull.Value)
            {
                resultado_180_njab.FechaConflicto_180_njab = Convert.ToDateTime(fila_180_njab["fecha_conflicto"]);
                resultado_180_njab.HoraInicioConflicto_180_njab = (TimeSpan)fila_180_njab["hora_inicio_conflicto"];
                resultado_180_njab.HoraFinConflicto_180_njab = (TimeSpan)fila_180_njab["hora_fin_conflicto"];
            }
            return resultado_180_njab;
        }

        private JornadaOperacionResultado_180_njab ValidarTurnosVigentesEnTx_180_njab(int idJornada_180_njab)
        {
            const string sql_180_njab = @"
                SELECT COUNT(1)
                FROM dbo.Turno WITH (UPDLOCK, HOLDLOCK)
                WHERE id_jornada = @id_jornada
                  AND estado_turno IN ('Reservado', 'Confirmado')";
            DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, new List<SqlParameter>
            {
                CrearParametro_180_njab("@id_jornada", idJornada_180_njab)
            });
            bool tieneTurnos_180_njab = Convert.ToInt32(tabla_180_njab.Rows[0][0]) > 0;
            if (tieneTurnos_180_njab)
            {
                JornadaOperacionResultado_180_njab resultado_180_njab = JornadaOperacionResultado_180_njab.CrearError_180_njab("La jornada tiene turnos reservados o confirmados. Deben resolverse mediante reprogramación o cancelación antes de continuar.");
                resultado_180_njab.TieneTurnosVigentes_180_njab = true;
                return resultado_180_njab;
            }
            return JornadaOperacionResultado_180_njab.CrearExito_180_njab(0, string.Empty);
        }

        private bool ReferenciasValidasEnTx_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            const string sql_180_njab = @"
                SELECT CASE WHEN EXISTS
                    (SELECT 1 FROM dbo.Profesional p WHERE p.id_profesional = @id_profesional AND p.estado_profesional = 'Activo') THEN 1 ELSE 0 END AS profesional_valido,
                    CASE WHEN EXISTS
                    (SELECT 1 FROM dbo.Consultorio c WHERE c.id_consultorio = @id_consultorio AND c.estado_consultorio = 'Activo') THEN 1 ELSE 0 END AS consultorio_valido";
            DataTable tabla_180_njab = _databaseContext_180_njab.LeerTexto(sql_180_njab, CrearParametrosSolicitud_180_njab(solicitud_180_njab));
            DataRow fila_180_njab = tabla_180_njab.Rows[0];
            return Convert.ToInt32(fila_180_njab["profesional_valido"]) == 1 &&
                   Convert.ToInt32(fila_180_njab["consultorio_valido"]) == 1;
        }

        private JornadaProfesionalConsulta_180_njab MapearJornada_180_njab(DataRow fila_180_njab)
        {
            return new JornadaProfesionalConsulta_180_njab
            {
                IdJornada_180_njab = Convert.ToInt32(fila_180_njab["id_jornada"]),
                IdProfesional_180_njab = Convert.ToInt32(fila_180_njab["id_profesional"]),
                IdConsultorio_180_njab = Convert.ToInt32(fila_180_njab["id_consultorio"]),
                IdEspecialidad_180_njab = 0,
                Fecha_180_njab = Convert.ToDateTime(fila_180_njab["fecha"]),
                HoraInicio_180_njab = (TimeSpan)fila_180_njab["hora_inicio"],
                HoraFin_180_njab = (TimeSpan)fila_180_njab["hora_fin"],
                DuracionTurnoMinutos_180_njab = Convert.ToInt32(fila_180_njab["duracion_turno_min"]),
                Consultorio_180_njab = fila_180_njab["consultorio"].ToString(),
                Especialidad_180_njab = fila_180_njab["especialidad"] == DBNull.Value ? "Sin especialidad" : fila_180_njab["especialidad"].ToString(),
                Estado_180_njab = fila_180_njab["estado_jornada"].ToString()
            };
        }

        private string ConstruirMensajeConflicto_180_njab(bool conflictoProfesional_180_njab, bool conflictoConsultorio_180_njab, DataRow fila_180_njab)
        {
            string alcance_180_njab = conflictoProfesional_180_njab && conflictoConsultorio_180_njab
                ? "el profesional y el consultorio"
                : conflictoProfesional_180_njab ? "el profesional" : "el consultorio";
            string detalle_180_njab = fila_180_njab["fecha_conflicto"] == DBNull.Value
                ? string.Empty
                : string.Format(" ({0:dd/MM/yyyy} de {1:hh\\:mm} a {2:hh\\:mm})", fila_180_njab["fecha_conflicto"], fila_180_njab["hora_inicio_conflicto"], fila_180_njab["hora_fin_conflicto"]);
            return string.Format("Existe una jornada activa que se solapa para {0}{1}.", alcance_180_njab, detalle_180_njab);
        }

        private List<SqlParameter> CrearParametrosSolicitud_180_njab(JornadaSolicitud_180_njab solicitud_180_njab, int idJornadaExcluir_180_njab = 0)
        {
            return new List<SqlParameter>
            {
                CrearParametro_180_njab("@id_jornada", solicitud_180_njab.IdJornada_180_njab),
                CrearParametro_180_njab("@id_jornada_excluir", idJornadaExcluir_180_njab),
                CrearParametro_180_njab("@id_profesional", solicitud_180_njab.IdProfesional_180_njab),
                CrearParametro_180_njab("@id_consultorio", solicitud_180_njab.IdConsultorio_180_njab),
                CrearParametroFecha_180_njab("@fecha", solicitud_180_njab.Fecha_180_njab),
                CrearParametroHora_180_njab("@hora_inicio", solicitud_180_njab.HoraInicio_180_njab),
                CrearParametroHora_180_njab("@hora_fin", solicitud_180_njab.HoraFin_180_njab),
                CrearParametro_180_njab("@duracion_turno_min", solicitud_180_njab.DuracionTurnoMinutos_180_njab)
            };
        }

        private List<SqlParameter> CrearParametrosDisponibilidad_180_njab(DateTime fecha_180_njab, TimeSpan horaInicio_180_njab, TimeSpan horaFin_180_njab, int idJornadaExcluir_180_njab)
        {
            return new List<SqlParameter>
            {
                CrearParametroFecha_180_njab("@fecha", fecha_180_njab),
                CrearParametroHora_180_njab("@hora_inicio", horaInicio_180_njab),
                CrearParametroHora_180_njab("@hora_fin", horaFin_180_njab),
                CrearParametro_180_njab("@id_jornada_excluir", idJornadaExcluir_180_njab)
            };
        }

        private SqlParameter CrearParametro_180_njab(string nombre_180_njab, int valor_180_njab)
        {
            return new SqlParameter(nombre_180_njab, SqlDbType.Int) { Value = valor_180_njab };
        }

        private SqlParameter CrearParametroFecha_180_njab(string nombre_180_njab, DateTime valor_180_njab)
        {
            return new SqlParameter(nombre_180_njab, SqlDbType.Date) { Value = valor_180_njab.Date };
        }

        private SqlParameter CrearParametroHora_180_njab(string nombre_180_njab, TimeSpan valor_180_njab)
        {
            return new SqlParameter(nombre_180_njab, SqlDbType.Time) { Value = valor_180_njab };
        }
    }
}
