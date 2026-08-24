using System;
using System.Windows.Forms;
using Domain;
using Services;

namespace UI
{
    public class LocalizedUserControl : UserControl, IObserverLanguage
    {
        protected LocalizedUserControl()
        {
            Load += LocalizedUserControl_Load;
            Disposed += LocalizedUserControl_Disposed;
        }

        public virtual void OnLanguageChanged(Idioma idioma)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnLanguageChanged(idioma)));
                return;
            }

            ApplyTranslations();
        }

        protected virtual void ApplyTranslations()
        {
            TranslationApplier.Apply(this);
        }

        private void LocalizedUserControl_Load(object sender, EventArgs e)
        {
            LanguageManager.Instance.Attach(this);
            OnLanguageChanged(LanguageManager.Instance.CurrentLanguage);
        }

        private void LocalizedUserControl_Disposed(object sender, EventArgs e)
        {
            LanguageManager.Instance.Detach(this);
        }
    }
}
