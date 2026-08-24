using System.Collections.Generic;
using Domain;
using Repository;

namespace Application
{
    public class IdiomaApplicationService
    {
        private readonly LanguageRepository _languageRepository;
        private readonly TranslationRepository _translationRepository;
        private readonly AuditoriaApplicationService _auditoriaService;

        public IdiomaApplicationService()
            : this(new LanguageRepository(), new TranslationRepository(), new AuditoriaApplicationService())
        {
        }

        public IdiomaApplicationService(
            LanguageRepository languageRepository,
            TranslationRepository translationRepository)
            : this(languageRepository, translationRepository, new AuditoriaApplicationService())
        {
        }

        public IdiomaApplicationService(
            LanguageRepository languageRepository,
            TranslationRepository translationRepository,
            AuditoriaApplicationService auditoriaService)
        {
            _languageRepository = languageRepository;
            _translationRepository = translationRepository;
            _auditoriaService = auditoriaService;
        }

        public List<Idioma> ListarIdiomas(bool soloActivos)
        {
            return _languageRepository.Listar(soloActivos);
        }

        public bool GuardarIdioma(Idioma idioma, int? idUsuarioResponsable, string motivo)
        {
            if (idioma == null ||
                string.IsNullOrWhiteSpace(idioma.Codigo) ||
                string.IsNullOrWhiteSpace(idioma.Nombre))
            {
                return false;
            }

            idioma.Codigo = idioma.Codigo.Trim();
            idioma.Nombre = idioma.Nombre.Trim();

            if (idioma.Id == 0)
            {
                bool creado = _languageRepository.Crear(idioma, idUsuarioResponsable) > 0;
                if (creado)
                {
                    _auditoriaService.RegistrarAlta(idioma.SaveToMemento());
                }

                return creado;
            }

            Idioma idiomaAnterior = _languageRepository.ObtenerPorId(idioma.Id);
            bool actualizado = _languageRepository.Actualizar(idioma, idUsuarioResponsable, motivo);

            if (actualizado)
            {
                Idioma idiomaNuevo = _languageRepository.ObtenerPorId(idioma.Id);
                if (idiomaAnterior != null && idiomaNuevo != null)
                {
                    _auditoriaService.RegistrarModificacion(idiomaAnterior.SaveToMemento(), idiomaNuevo.SaveToMemento());
                }
            }

            return actualizado;
        }

        public List<Etiqueta> ListarEtiquetas()
        {
            return _translationRepository.ListarEtiquetas();
        }

        public bool CrearEtiqueta(Etiqueta etiqueta)
        {
            if (etiqueta == null || string.IsNullOrWhiteSpace(etiqueta.Key))
            {
                return false;
            }

            etiqueta.Key = etiqueta.Key.Trim();
            etiqueta.Descripcion = string.IsNullOrWhiteSpace(etiqueta.Descripcion)
                ? null
                : etiqueta.Descripcion.Trim();

            bool creada = _translationRepository.CrearEtiqueta(etiqueta) > 0;
            if (creada)
            {
                _auditoriaService.RegistrarAlta(etiqueta.SaveToMemento());
            }

            return creada;
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            if (traduccion == null ||
                traduccion.EtiquetaId == 0 ||
                traduccion.IdiomaId == 0 ||
                string.IsNullOrWhiteSpace(traduccion.Texto))
            {
                return false;
            }

            traduccion.Texto = traduccion.Texto.Trim();
            Traduccion traduccionAnterior = _translationRepository.ObtenerTraduccion(traduccion.EtiquetaId, traduccion.IdiomaId);
            bool guardada = _translationRepository.GuardarTraduccion(traduccion);

            if (guardada)
            {
                Traduccion traduccionNueva = _translationRepository.ObtenerTraduccion(traduccion.EtiquetaId, traduccion.IdiomaId);
                if (traduccionNueva != null)
                {
                    if (traduccionAnterior == null)
                    {
                        _auditoriaService.RegistrarAlta(traduccionNueva.SaveToMemento());
                    }
                    else
                    {
                        _auditoriaService.RegistrarModificacion(traduccionAnterior.SaveToMemento(), traduccionNueva.SaveToMemento());
                    }
                }
            }

            return guardada;
        }

        public bool GuardarTraduccionDetectada(string key, string descripcion, int idiomaId, string texto)
        {
            if (string.IsNullOrWhiteSpace(key) ||
                idiomaId == 0 ||
                string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            key = key.Trim();
            texto = texto.Trim();
            descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();

            Etiqueta etiqueta = null;
            foreach (Etiqueta item in _translationRepository.ListarEtiquetas())
            {
                if (string.Equals(item.Key, key, System.StringComparison.OrdinalIgnoreCase))
                {
                    etiqueta = item;
                    break;
                }
            }

            if (etiqueta == null)
            {
                etiqueta = new Etiqueta
                {
                    Key = key,
                    Descripcion = descripcion
                };

                if (_translationRepository.CrearEtiqueta(etiqueta) <= 0)
                {
                    return false;
                }

                _auditoriaService.RegistrarAlta(etiqueta.SaveToMemento());
            }

            Traduccion traduccion = new Traduccion
            {
                EtiquetaId = etiqueta.Id,
                IdiomaId = idiomaId,
                Texto = texto
            };

            return GuardarTraduccion(traduccion);
        }

        public List<Traduccion> ListarTraducciones()
        {
            return _translationRepository.ListarTraducciones();
        }

        public Traduccion ObtenerTraduccion(int etiquetaId, int idiomaId)
        {
            if (etiquetaId == 0 || idiomaId == 0)
            {
                return null;
            }

            return _translationRepository.ObtenerTraduccion(etiquetaId, idiomaId);
        }
    }
}
