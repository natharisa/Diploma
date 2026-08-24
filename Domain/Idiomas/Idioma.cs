using Abstractions;

namespace Domain
{
    public class Idioma : IEntity
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }

        public AuditoriaMemento SaveToMemento()
        {
            return new AuditoriaMemento("Idioma", Id, new System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", Id },
                { "Codigo", Codigo },
                { "Nombre", Nombre },
                { "Activo", Activo }
            });
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Nombre) ? Codigo : Nombre;
        }
    }
}
