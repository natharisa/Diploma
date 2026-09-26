using System;
using System.Collections.Generic;
using Domain;
using Repository;

namespace Application
{
    public class JornadaApplicationService_180_njab
    {
        private readonly JornadaRepository_180_njab _jornadaRepository_180_njab;

        public JornadaApplicationService_180_njab()
            : this(new JornadaRepository_180_njab())
        {
        }

        public JornadaApplicationService_180_njab(JornadaRepository_180_njab jornadaRepository_180_njab)
        {
            _jornadaRepository_180_njab = jornadaRepository_180_njab;
        }

        public List<EspecialidadConsulta_180_njab> ListarEspecialidades_180_njab(int idProfesional_180_njab)
        {
            return _jornadaRepository_180_njab.ListarEspecialidades_180_njab(idProfesional_180_njab);
        }

        public List<ConsultorioConsulta_180_njab> ListarConsultoriosDisponibles_180_njab(DateTime fecha_180_njab, JornadaFranja_180_njab franja_180_njab, int idJornadaExcluir_180_njab)
        {
            TimeSpan horaInicio_180_njab;
            TimeSpan horaFin_180_njab;
            if (!JornadaReglas_180_njab.ObtenerHorario_180_njab(franja_180_njab, out horaInicio_180_njab, out horaFin_180_njab))
            {
                return new List<ConsultorioConsulta_180_njab>();
            }

            return _jornadaRepository_180_njab.ListarConsultoriosDisponibles_180_njab(fecha_180_njab, horaInicio_180_njab, horaFin_180_njab, idJornadaExcluir_180_njab);
        }

        public List<JornadaProfesionalConsulta_180_njab> ListarJornadas_180_njab(int idProfesional_180_njab)
        {
            return _jornadaRepository_180_njab.ListarJornadas_180_njab(idProfesional_180_njab);
        }

        public JornadaOperacionResultado_180_njab Asignar_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            JornadaOperacionResultado_180_njab permiso_180_njab = ValidarAdministrador_180_njab();
            if (!permiso_180_njab.Exito_180_njab)
            {
                return permiso_180_njab;
            }

            string mensaje_180_njab;
            if (!JornadaReglas_180_njab.ValidarSolicitud_180_njab(solicitud_180_njab, out mensaje_180_njab))
            {
                return JornadaOperacionResultado_180_njab.CrearError_180_njab(mensaje_180_njab);
            }

            return _jornadaRepository_180_njab.Crear_180_njab(solicitud_180_njab);
        }

        public JornadaOperacionResultado_180_njab Modificar_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            JornadaOperacionResultado_180_njab permiso_180_njab = ValidarAdministrador_180_njab();
            if (!permiso_180_njab.Exito_180_njab)
            {
                return permiso_180_njab;
            }

            string mensaje_180_njab;
            if (!JornadaReglas_180_njab.ValidarSolicitud_180_njab(solicitud_180_njab, out mensaje_180_njab))
            {
                return JornadaOperacionResultado_180_njab.CrearError_180_njab(mensaje_180_njab);
            }

            return _jornadaRepository_180_njab.Modificar_180_njab(solicitud_180_njab);
        }

        public JornadaOperacionResultado_180_njab QuitarAsignacion_180_njab(int idJornada_180_njab, int idProfesional_180_njab)
        {
            JornadaOperacionResultado_180_njab permiso_180_njab = ValidarAdministrador_180_njab();
            if (!permiso_180_njab.Exito_180_njab)
            {
                return permiso_180_njab;
            }

            return _jornadaRepository_180_njab.QuitarAsignacion_180_njab(idJornada_180_njab, idProfesional_180_njab);
        }

        public JornadaOperacionResultado_180_njab TieneTurnosVigentes_180_njab(int idJornada_180_njab)
        {
            JornadaOperacionResultado_180_njab permiso_180_njab = ValidarAdministrador_180_njab();
            if (!permiso_180_njab.Exito_180_njab)
            {
                return permiso_180_njab;
            }

            return _jornadaRepository_180_njab.TieneTurnosVigentes_180_njab(idJornada_180_njab);
        }

        private JornadaOperacionResultado_180_njab ValidarAdministrador_180_njab()
        {
            Usuario usuario_180_njab = Sesion.ObtenerInstancia().ObtenerUsuario();
            if (usuario_180_njab == null || !usuario_180_njab.TienePermiso(PermisosSistema.Administrador))
            {
                return JornadaOperacionResultado_180_njab.CrearError_180_njab("Permiso denegado. Esta operación requiere un administrador.");
            }

            return JornadaOperacionResultado_180_njab.CrearExito_180_njab(0, string.Empty);
        }
    }
}
