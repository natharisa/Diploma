using Abstractions;

namespace Domain
{
    public class LoginFallidoBitacoraFactory : BitacoraFactory
    {
        protected override IBitacoraEvento CrearEvento()
        {
            return new Bitacora
            {
                Modulo = BitacoraModulo.Seguridad,
                Accion = BitacoraAccion.LoginFallido,
                Nivel = BitacoraNivel.Advertencia
            };
        }
    }
}
