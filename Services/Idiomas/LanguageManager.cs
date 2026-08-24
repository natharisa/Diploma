using System;
using System.Collections.Generic;
using Domain;
using Repository;

namespace Services
{
    public class LanguageManager : IObservableLanguage
    {
        private static readonly Lazy<LanguageManager> LazyInstance =
            new Lazy<LanguageManager>(() => new LanguageManager());

        private readonly List<IObserverLanguage> _observers = new List<IObserverLanguage>();
        private readonly LanguageRepository _languageRepository;
        private readonly TranslationService _translationService;
        private readonly UsuarioRepository _usuarioRepository;

        public static LanguageManager Instance
        {
            get { return LazyInstance.Value; }
        }

        public Idioma CurrentLanguage { get; private set; }

        public LanguageManager()
            : this(new LanguageRepository(), new TranslationService(), new UsuarioRepository())
        {
        }

        public LanguageManager(
            LanguageRepository languageRepository,
            TranslationService translationService,
            UsuarioRepository usuarioRepository)
        {
            _languageRepository = languageRepository;
            _translationService = translationService;
            _usuarioRepository = usuarioRepository;
        }

        public void Initialize(Usuario usuario)
        {
            Idioma idioma = null;

            if (usuario != null && usuario.IdiomaPreferidoId.HasValue)
            {
                idioma = _languageRepository.ObtenerPorId(usuario.IdiomaPreferidoId.Value);
            }

            if (idioma == null || !idioma.Activo)
            {
                idioma = _languageRepository.ObtenerDefault();
            }

            CurrentLanguage = idioma;
            Notify();
        }

        public void ChangeLanguage(Idioma idioma, Usuario usuario)
        {
            if (idioma == null || idioma.Id == 0 || !idioma.Activo)
            {
                return;
            }

            CurrentLanguage = idioma;

            if (usuario != null && usuario.Id > 0)
            {
                usuario.IdiomaPreferidoId = idioma.Id;
                usuario.Idioma = idioma.Id.ToString();
                _usuarioRepository.ActualizarIdiomaPreferido(usuario.Id, idioma.Id);
            }

            Notify();
        }

        public string Translate(string key)
        {
            return _translationService.Translate(key, CurrentLanguage);
        }

        public List<Idioma> ListarIdiomasActivos()
        {
            return _languageRepository.Listar(true);
        }

        public void Attach(IObserverLanguage observer)
        {
            if (observer == null || _observers.Contains(observer))
            {
                return;
            }

            _observers.Add(observer);
        }

        public void Detach(IObserverLanguage observer)
        {
            if (observer == null)
            {
                return;
            }

            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserverLanguage observer in _observers.ToArray())
            {
                observer.OnLanguageChanged(CurrentLanguage);
            }
        }
    }
}
