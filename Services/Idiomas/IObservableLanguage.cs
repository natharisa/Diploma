namespace Services
{
    public interface IObservableLanguage
    {
        void Attach(IObserverLanguage observer);
        void Detach(IObserverLanguage observer);
        void Notify();
    }
}
