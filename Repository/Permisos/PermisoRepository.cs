using System.Collections.Generic;
using DAL;
using Domain;

namespace Repository
{
    public class PermisoRepository
    {
        private readonly PermisoDataMapper _permisoDataMapper;

        public PermisoRepository()
            : this(new PermisoDataMapper())
        {
        }

        public PermisoRepository(PermisoDataMapper permisoDataMapper)
        {
            _permisoDataMapper = permisoDataMapper;
        }

        public List<ComponentePermiso> ListarAsignadosPorUsuario(int idUsuario)
        {
            return _permisoDataMapper.ListarAsignadosPorUsuario(idUsuario);
        }

        public List<ComponentePermiso> ListarArbolCompleto()
        {
            return _permisoDataMapper.ListarArbolCompleto();
        }

        public List<ComponentePermiso> ListarComponentes()
        {
            return _permisoDataMapper.ListarComponentes();
        }

        public List<ComponentePermiso> ListarFamilias()
        {
            return _permisoDataMapper.ListarFamilias();
        }

        public List<int> ListarIdsComponentesAsignadosPorUsuario(int idUsuario)
        {
            return _permisoDataMapper.ListarIdsComponentesAsignadosPorUsuario(idUsuario);
        }

        public bool CrearFamilia(string codigo, string nombre, string descripcion)
        {
            return _permisoDataMapper.CrearFamilia(codigo, nombre, descripcion);
        }

        public string AgregarRelacion(int idPadre, int idHijo)
        {
            return _permisoDataMapper.AgregarRelacion(idPadre, idHijo);
        }

        public bool QuitarRelacion(int idPadre, int idHijo)
        {
            return _permisoDataMapper.QuitarRelacion(idPadre, idHijo);
        }

        public bool GuardarComponentesUsuario(int idUsuario, List<int> idsComponentes)
        {
            return _permisoDataMapper.GuardarComponentesUsuario(idUsuario, idsComponentes);
        }

        public bool PuedeAgregarRelacion(int idPadre, int idHijo)
        {
            return _permisoDataMapper.PuedeAgregarRelacion(idPadre, idHijo);
        }
    }
}
