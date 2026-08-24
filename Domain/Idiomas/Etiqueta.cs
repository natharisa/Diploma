using Abstractions;

namespace Domain
{
    public class Etiqueta : IEntity
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Descripcion { get; set; }

        public AuditoriaMemento SaveToMemento()
        {
            return new AuditoriaMemento("Etiqueta", Id, new System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", Id },
                { "Key", Key },
                { "Descripcion", Descripcion }
            });
        }

        public override string ToString()
        {
            return Key ?? string.Empty;
        }
    }
}
