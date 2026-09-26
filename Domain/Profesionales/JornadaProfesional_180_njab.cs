using System;

namespace Domain
{
    public class JornadaSolicitud_180_njab
    {
        public int IdJornada_180_njab { get; set; }
        public int IdProfesional_180_njab { get; set; }
        public int IdConsultorio_180_njab { get; set; }
        public DateTime Fecha_180_njab { get; set; }
        public DayOfWeek Dia_180_njab { get; set; }
        public JornadaFranja_180_njab Franja_180_njab { get; set; }
        public TimeSpan HoraInicio_180_njab { get; set; }
        public TimeSpan HoraFin_180_njab { get; set; }
        public int DuracionTurnoMinutos_180_njab { get; set; }
    }

    public class JornadaProfesionalConsulta_180_njab
    {
        public int IdJornada_180_njab { get; set; }
        public int IdProfesional_180_njab { get; set; }
        public int IdConsultorio_180_njab { get; set; }
        public int IdEspecialidad_180_njab { get; set; }
        public DateTime Fecha_180_njab { get; set; }
        public TimeSpan HoraInicio_180_njab { get; set; }
        public TimeSpan HoraFin_180_njab { get; set; }
        public int DuracionTurnoMinutos_180_njab { get; set; }
        public string Consultorio_180_njab { get; set; }
        public string Especialidad_180_njab { get; set; }
        public string Estado_180_njab { get; set; }

        public string Dia_180_njab { get { return Fecha_180_njab.ToString("dddd"); } }

        public string Franja_180_njab
        {
            get
            {
                return HoraInicio_180_njab == new TimeSpan(8, 0, 0)
                    ? "Mañana (08:00 a 12:00)"
                    : "Tarde (14:00 a 18:00)";
            }
        }
    }

    public class EspecialidadConsulta_180_njab
    {
        public int IdEspecialidad_180_njab { get; set; }
        public string Nombre_180_njab { get; set; }
        public override string ToString() { return Nombre_180_njab; }
    }

    public class ConsultorioConsulta_180_njab
    {
        public int IdConsultorio_180_njab { get; set; }
        public string Nombre_180_njab { get; set; }
        public override string ToString() { return Nombre_180_njab; }
    }

    public class JornadaOperacionResultado_180_njab
    {
        public bool Exito_180_njab { get; set; }
        public string Mensaje_180_njab { get; set; }
        public int IdJornada_180_njab { get; set; }
        public bool ConflictoProfesional_180_njab { get; set; }
        public bool ConflictoConsultorio_180_njab { get; set; }
        public bool TieneTurnosVigentes_180_njab { get; set; }
        public DateTime? FechaConflicto_180_njab { get; set; }
        public TimeSpan? HoraInicioConflicto_180_njab { get; set; }
        public TimeSpan? HoraFinConflicto_180_njab { get; set; }

        public static JornadaOperacionResultado_180_njab CrearExito_180_njab(int idJornada_180_njab, string mensaje_180_njab)
        {
            return new JornadaOperacionResultado_180_njab { Exito_180_njab = true, IdJornada_180_njab = idJornada_180_njab, Mensaje_180_njab = mensaje_180_njab };
        }

        public static JornadaOperacionResultado_180_njab CrearError_180_njab(string mensaje_180_njab)
        {
            return new JornadaOperacionResultado_180_njab { Exito_180_njab = false, Mensaje_180_njab = mensaje_180_njab };
        }
    }
}
