using System;
using System.Collections.Generic;

namespace Domain
{
    public enum JornadaFranja_180_njab
    {
        Manana_180_njab,
        Tarde_180_njab
    }

    public class JornadaFranjaOpcion_180_njab
    {
        public JornadaFranja_180_njab Codigo_180_njab { get; set; }
        public string Descripcion_180_njab { get; set; }
        public TimeSpan HoraInicio_180_njab { get; set; }
        public TimeSpan HoraFin_180_njab { get; set; }

        public override string ToString()
        {
            return Descripcion_180_njab;
        }
    }

    public static class JornadaReglas_180_njab
    {
        public static List<JornadaFranjaOpcion_180_njab> ListarFranjas_180_njab()
        {
            return new List<JornadaFranjaOpcion_180_njab>
            {
                new JornadaFranjaOpcion_180_njab
                {
                    Codigo_180_njab = JornadaFranja_180_njab.Manana_180_njab,
                    Descripcion_180_njab = "Mañana (08:00 a 12:00)",
                    HoraInicio_180_njab = new TimeSpan(8, 0, 0),
                    HoraFin_180_njab = new TimeSpan(12, 0, 0)
                },
                new JornadaFranjaOpcion_180_njab
                {
                    Codigo_180_njab = JornadaFranja_180_njab.Tarde_180_njab,
                    Descripcion_180_njab = "Tarde (14:00 a 18:00)",
                    HoraInicio_180_njab = new TimeSpan(14, 0, 0),
                    HoraFin_180_njab = new TimeSpan(18, 0, 0)
                }
            };
        }

        public static bool ObtenerHorario_180_njab(JornadaFranja_180_njab franja_180_njab, out TimeSpan horaInicio_180_njab, out TimeSpan horaFin_180_njab)
        {
            if (franja_180_njab == JornadaFranja_180_njab.Manana_180_njab)
            {
                horaInicio_180_njab = new TimeSpan(8, 0, 0);
                horaFin_180_njab = new TimeSpan(12, 0, 0);
                return true;
            }

            if (franja_180_njab == JornadaFranja_180_njab.Tarde_180_njab)
            {
                horaInicio_180_njab = new TimeSpan(14, 0, 0);
                horaFin_180_njab = new TimeSpan(18, 0, 0);
                return true;
            }

            horaInicio_180_njab = TimeSpan.Zero;
            horaFin_180_njab = TimeSpan.Zero;
            return false;
        }

        public static bool ValidarSolicitud_180_njab(JornadaSolicitud_180_njab solicitud_180_njab, out string mensaje_180_njab)
        {
            mensaje_180_njab = string.Empty;
            if (solicitud_180_njab == null || solicitud_180_njab.IdProfesional_180_njab <= 0)
            {
                mensaje_180_njab = "Debe seleccionar un profesional.";
                return false;
            }

            if (solicitud_180_njab.IdConsultorio_180_njab <= 0)
            {
                mensaje_180_njab = "Debe seleccionar un consultorio disponible.";
                return false;
            }

            if (solicitud_180_njab.Fecha_180_njab == DateTime.MinValue || solicitud_180_njab.Fecha_180_njab.DayOfWeek == DayOfWeek.Sunday)
            {
                mensaje_180_njab = "La fecha debe corresponder a un día de lunes a sábado.";
                return false;
            }

            if (solicitud_180_njab.Dia_180_njab < DayOfWeek.Monday || solicitud_180_njab.Dia_180_njab > DayOfWeek.Saturday)
            {
                mensaje_180_njab = "Debe seleccionar un día de atención entre lunes y sábado.";
                return false;
            }

            if (solicitud_180_njab.Fecha_180_njab.DayOfWeek != solicitud_180_njab.Dia_180_njab)
            {
                mensaje_180_njab = "La fecha no coincide con el día de atención seleccionado.";
                return false;
            }

            TimeSpan horaInicio_180_njab;
            TimeSpan horaFin_180_njab;
            if (!ObtenerHorario_180_njab(solicitud_180_njab.Franja_180_njab, out horaInicio_180_njab, out horaFin_180_njab))
            {
                mensaje_180_njab = "Debe seleccionar una franja válida.";
                return false;
            }

            if (solicitud_180_njab.DuracionTurnoMinutos_180_njab <= 0)
            {
                mensaje_180_njab = "La duración del turno debe ser positiva.";
                return false;
            }

            if (solicitud_180_njab.DuracionTurnoMinutos_180_njab > (int)(horaFin_180_njab - horaInicio_180_njab).TotalMinutes)
            {
                mensaje_180_njab = "La duración no puede superar la franja seleccionada.";
                return false;
            }

            solicitud_180_njab.HoraInicio_180_njab = horaInicio_180_njab;
            solicitud_180_njab.HoraFin_180_njab = horaFin_180_njab;
            return true;
        }
    }
}
