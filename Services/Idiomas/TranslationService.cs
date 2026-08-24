using System.Collections.Generic;
using Domain;
using Repository;

namespace Services
{
    public class TranslationService
    {
        private readonly TranslationRepository _translationRepository;

        public TranslationService()
            : this(new TranslationRepository())
        {
        }

        public TranslationService(TranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public string Translate(string key, Idioma idioma)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            if (idioma == null || idioma.Id == 0)
            {
                return key;
            }

            string texto = _translationRepository.ObtenerTexto(key, idioma.Id);
            return string.IsNullOrWhiteSpace(texto) ? key : texto;
        }

        public Dictionary<string, string> ListarPorIdioma(Idioma idioma)
        {
            if (idioma == null || idioma.Id == 0)
            {
                return new Dictionary<string, string>();
            }

            return _translationRepository.ListarPorIdioma(idioma.Id);
        }
    }
}
