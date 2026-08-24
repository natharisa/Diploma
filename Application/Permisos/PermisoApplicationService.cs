using System.Collections.Generic;
using System.Text;
using Domain;
using Repository;

namespace Application
{
    public class PermisoApplicationService
    {
        private readonly PermisoRepository _permisoRepository;
        private readonly AuditoriaApplicationService _auditoriaService;

        public PermisoApplicationService()
            : this(new PermisoRepository(), new AuditoriaApplicationService())
        {
        }

        public PermisoApplicationService(PermisoRepository permisoRepository)
            : this(permisoRepository, new AuditoriaApplicationService())
        {
        }

        public PermisoApplicationService(PermisoRepository permisoRepository, AuditoriaApplicationService auditoriaService)
        {
            _permisoRepository = permisoRepository;
            _auditoriaService = auditoriaService;
        }

        public List<ComponentePermiso> ListarAsignadosPorUsuario(int idUsuario)
        {
            return _permisoRepository.ListarAsignadosPorUsuario(idUsuario);
        }

        public List<ComponentePermiso> ListarArbolCompleto()
        {
            return _permisoRepository.ListarArbolCompleto();
        }

        public List<ComponentePermiso> ListarComponentes()
        {
            return _permisoRepository.ListarComponentes();
        }

        public List<ComponentePermiso> ListarFamilias()
        {
            return _permisoRepository.ListarFamilias();
        }

        public List<int> ListarIdsComponentesAsignadosPorUsuario(int idUsuario)
        {
            return _permisoRepository.ListarIdsComponentesAsignadosPorUsuario(idUsuario);
        }

        public bool CrearFamilia(string codigo, string nombre, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return false;
            }

            string codigoNormalizado = NormalizarCodigo(string.IsNullOrWhiteSpace(codigo) ? nombre : codigo);
            if (string.IsNullOrWhiteSpace(codigoNormalizado))
            {
                return false;
            }

            ComponentePermiso componenteAnterior = BuscarComponentePorCodigo(codigoNormalizado);
            bool guardado = _permisoRepository.CrearFamilia(codigoNormalizado, nombre.Trim(), descripcion);

            if (guardado)
            {
                ComponentePermiso componenteNuevo = BuscarComponentePorCodigo(codigoNormalizado);
                if (componenteNuevo != null)
                {
                    if (componenteAnterior == null)
                    {
                        _auditoriaService.RegistrarAlta(componenteNuevo.SaveToMemento());
                    }
                    else
                    {
                        _auditoriaService.RegistrarModificacion(componenteAnterior.SaveToMemento(), componenteNuevo.SaveToMemento());
                    }
                }
            }

            return guardado;
        }

        public string AgregarRelacion(int idPadre, int idHijo)
        {
            if (idPadre == 0 || idHijo == 0)
            {
                return "DATOS_INVALIDOS";
            }

            string resultado = _permisoRepository.AgregarRelacion(idPadre, idHijo);
            if (resultado == "OK")
            {
                _auditoriaService.RegistrarSnapshot(
                    "ComponentePermisoRelacion",
                    CrearIdRelacion(idPadre, idHijo),
                    "CREATE",
                    CrearEstadoRelacion(idPadre, idHijo));
            }

            return resultado;
        }

        public bool QuitarRelacion(int idPadre, int idHijo)
        {
            if (idPadre == 0 || idHijo == 0)
            {
                return false;
            }

            bool quitada = _permisoRepository.QuitarRelacion(idPadre, idHijo);
            if (quitada)
            {
                _auditoriaService.RegistrarSnapshot(
                    "ComponentePermisoRelacion",
                    CrearIdRelacion(idPadre, idHijo),
                    "DELETE",
                    CrearEstadoRelacion(idPadre, idHijo));
            }

            return quitada;
        }

        public bool GuardarComponentesUsuario(int idUsuario, List<int> idsComponentes)
        {
            if (idUsuario == 0)
            {
                return false;
            }

            List<int> idsAnteriores = _permisoRepository.ListarIdsComponentesAsignadosPorUsuario(idUsuario);
            bool guardado = _permisoRepository.GuardarComponentesUsuario(idUsuario, idsComponentes);

            if (guardado)
            {
                List<int> idsNuevos = _permisoRepository.ListarIdsComponentesAsignadosPorUsuario(idUsuario);
                _auditoriaService.RegistrarCambio(
                    CrearMementoComponentesUsuario(idUsuario, idsAnteriores),
                    CrearMementoComponentesUsuario(idUsuario, idsNuevos),
                    "UPDATE");
            }

            return guardado;
        }

        public bool PuedeAgregarRelacion(int idPadre, int idHijo)
        {
            return _permisoRepository.PuedeAgregarRelacion(idPadre, idHijo);
        }

        private static string NormalizarCodigo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            foreach (char caracter in texto.Trim().ToUpperInvariant())
            {
                if (char.IsLetterOrDigit(caracter))
                {
                    builder.Append(caracter);
                }
                else if (builder.Length > 0 && builder[builder.Length - 1] != '_')
                {
                    builder.Append('_');
                }
            }

            return builder.ToString().Trim('_');
        }

        private ComponentePermiso BuscarComponentePorCodigo(string codigo)
        {
            foreach (ComponentePermiso componente in _permisoRepository.ListarComponentes())
            {
                if (componente.Codigo == codigo)
                {
                    return componente;
                }
            }

            return null;
        }

        private static int CrearIdRelacion(int idPadre, int idHijo)
        {
            return (idPadre * 100000) + idHijo;
        }

        private static Dictionary<string, object> CrearEstadoRelacion(int idPadre, int idHijo)
        {
            return new Dictionary<string, object>
            {
                { "IdPadre", idPadre },
                { "IdHijo", idHijo }
            };
        }

        private static AuditoriaMemento CrearMementoComponentesUsuario(int idUsuario, List<int> idsComponentes)
        {
            List<int> idsOrdenados = idsComponentes == null
                ? new List<int>()
                : new List<int>(idsComponentes);
            idsOrdenados.Sort();

            return new AuditoriaMemento("UsuarioComponentePermiso", idUsuario, new Dictionary<string, object>
            {
                { "IdUsuario", idUsuario },
                { "IdsComponentes", string.Join(",", idsOrdenados) }
            });
        }
    }
}
