using Abstractions;

namespace Domain
{
    public class RegistroFallidoBitacoraFactory : BitacoraFactory
    {
        protected override IBitacoraEvento CrearEvento()
        {
            return new Bitacora
            {
                Modulo = BitacoraModulo.Seguridad,
                Accion = BitacoraAccion.RegistroFallido,
                Nivel = BitacoraNivel.Advertencia
            };
        }
    }
}
