using Abstractions;

namespace Domain
{
    public class LoginExitosoBitacoraFactory : BitacoraFactory
    {
        protected override IBitacoraEvento CrearEvento()
        {
            return new Bitacora
            {
                Modulo = BitacoraModulo.Seguridad,
                Accion = BitacoraAccion.LoginExitoso,
                Nivel = BitacoraNivel.Informacion
            };
        }
    }
}
