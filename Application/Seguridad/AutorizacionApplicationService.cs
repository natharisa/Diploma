using Domain;

namespace Application
{
    public class AutorizacionApplicationService
    {
        public bool TienePermiso(string codigoPermiso)
        {
            Usuario usuario = Sesion.ObtenerInstancia().ObtenerUsuario();
            return usuario != null && usuario.TienePermiso(codigoPermiso);
        }

        public bool TienePermiso(Usuario usuario, string codigoPermiso)
        {
            return usuario != null && usuario.TienePermiso(codigoPermiso);
        }
    }
}
