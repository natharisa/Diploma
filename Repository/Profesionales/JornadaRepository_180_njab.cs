using System;
using System.Collections.Generic;
using DAL;
using Domain;

namespace Repository
{
    public class JornadaRepository_180_njab
    {
        private readonly JornadaDataMapper_180_njab _jornadaDataMapper_180_njab;

        public JornadaRepository_180_njab()
            : this(new JornadaDataMapper_180_njab())
        {
        }

        public JornadaRepository_180_njab(JornadaDataMapper_180_njab jornadaDataMapper_180_njab)
        {
            _jornadaDataMapper_180_njab = jornadaDataMapper_180_njab;
        }

        public List<EspecialidadConsulta_180_njab> ListarEspecialidades_180_njab(int idProfesional_180_njab)
        {
            return _jornadaDataMapper_180_njab.ListarEspecialidades_180_njab(idProfesional_180_njab);
        }

        public List<ConsultorioConsulta_180_njab> ListarConsultoriosDisponibles_180_njab(DateTime fecha_180_njab, TimeSpan horaInicio_180_njab, TimeSpan horaFin_180_njab, int idJornadaExcluir_180_njab)
        {
            return _jornadaDataMapper_180_njab.ListarConsultoriosDisponibles_180_njab(fecha_180_njab, horaInicio_180_njab, horaFin_180_njab, idJornadaExcluir_180_njab);
        }

        public List<JornadaProfesionalConsulta_180_njab> ListarJornadas_180_njab(int idProfesional_180_njab)
        {
            return _jornadaDataMapper_180_njab.ListarJornadas_180_njab(idProfesional_180_njab);
        }

        public JornadaOperacionResultado_180_njab Crear_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            return _jornadaDataMapper_180_njab.Crear_180_njab(solicitud_180_njab);
        }

        public JornadaOperacionResultado_180_njab Modificar_180_njab(JornadaSolicitud_180_njab solicitud_180_njab)
        {
            return _jornadaDataMapper_180_njab.Modificar_180_njab(solicitud_180_njab);
        }

        public JornadaOperacionResultado_180_njab QuitarAsignacion_180_njab(int idJornada_180_njab, int idProfesional_180_njab)
        {
            return _jornadaDataMapper_180_njab.QuitarAsignacion_180_njab(idJornada_180_njab, idProfesional_180_njab);
        }

        public JornadaOperacionResultado_180_njab TieneTurnosVigentes_180_njab(int idJornada_180_njab)
        {
            return _jornadaDataMapper_180_njab.TieneTurnosVigentes_180_njab(idJornada_180_njab);
        }
    }
}
