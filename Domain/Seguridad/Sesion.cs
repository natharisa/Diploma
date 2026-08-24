using System;

namespace Domain
{
    public sealed class Sesion
    {
        private static readonly Sesion _instance = new Sesion();
        private static readonly object _lock = new object();

        private Usuario _usuario;
        private DateTime _fechaInicio;

        private Sesion()
        {
        }

        public static Sesion ObtenerInstancia()
        {
            return _instance;
        }

        public void IniciarSesion(Usuario usuario)
        {
            lock (_lock)
            {
                _usuario = usuario;
                _fechaInicio = DateTime.Now;
            }
        }

        public Usuario ObtenerUsuario()
        {
            lock (_lock)
            {
                return _usuario;
            }
        }

        public DateTime ObtenerFechaInicio()
        {
            lock (_lock)
            {
                return _fechaInicio;
            }
        }

        public bool HaySesionActiva()
        {
            lock (_lock)
            {
                return _usuario != null;
            }
        }

        public void Logout()
        {
            lock (_lock)
            {
                _usuario = null;
                _fechaInicio = DateTime.MinValue;
            }
        }
    }
}
