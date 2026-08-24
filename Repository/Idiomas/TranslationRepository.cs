using System.Collections.Generic;
using DAL;
using Domain;

namespace Repository
{
    public class TranslationRepository
    {
        private readonly TraduccionDataMapper _traduccionDataMapper;

        public TranslationRepository()
            : this(new TraduccionDataMapper())
        {
        }

        public TranslationRepository(TraduccionDataMapper traduccionDataMapper)
        {
            _traduccionDataMapper = traduccionDataMapper;
        }

        public int CrearEtiqueta(Etiqueta etiqueta)
        {
            return _traduccionDataMapper.CrearEtiqueta(etiqueta);
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            return _traduccionDataMapper.GuardarTraduccion(traduccion);
        }

        public string ObtenerTexto(string key, int idiomaId)
        {
            return _traduccionDataMapper.ObtenerTexto(key, idiomaId);
        }

        public Traduccion ObtenerTraduccion(int etiquetaId, int idiomaId)
        {
            return _traduccionDataMapper.ObtenerTraduccion(etiquetaId, idiomaId);
        }

        public Dictionary<string, string> ListarPorIdioma(int idiomaId)
        {
            return _traduccionDataMapper.ListarPorIdioma(idiomaId);
        }

        public List<Etiqueta> ListarEtiquetas()
        {
            return _traduccionDataMapper.ListarEtiquetas();
        }

        public List<Traduccion> ListarTraducciones()
        {
            return _traduccionDataMapper.ListarTraducciones();
        }
    }
}
