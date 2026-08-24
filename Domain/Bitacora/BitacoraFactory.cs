using System;

namespace Domain
{
    public abstract class BitacoraFactory
    {
        public IBitacoraEvento Crear(string identificadorUsuario, string descripcion)
        {
            IBitacoraEvento evento = CrearEvento();
            evento.IdentificadorUsuario = identificadorUsuario;
            evento.Descripcion = descripcion;
            evento.Equipo = Environment.MachineName;
            evento.Fecha = DateTime.Now;
            return evento;
        }

        protected abstract IBitacoraEvento CrearEvento();
    }
}
