using Domain;

namespace Services
{
    public interface IObserverLanguage
    {
        void OnLanguageChanged(Idioma idioma);
    }
}
