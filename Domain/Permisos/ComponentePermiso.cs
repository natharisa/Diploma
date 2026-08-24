using System.Collections.Generic;
using Abstractions;

namespace Domain
{
    public abstract class ComponentePermiso : IEntity
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }

        public abstract TipoComponentePermiso Tipo { get; }

        public abstract void Agregar(ComponentePermiso componente);
        public abstract void Quitar(ComponentePermiso componente);
        public abstract IReadOnlyList<ComponentePermiso> ObtenerHijos();
        public abstract bool TienePermiso(string codigoPermiso);

        public AuditoriaMemento SaveToMemento()
        {
            return new AuditoriaMemento("ComponentePermiso", Id, new Dictionary<string, object>
            {
                { "Id", Id },
                { "Codigo", Codigo },
                { "Nombre", Nombre },
                { "Descripcion", Descripcion },
                { "Estado", Estado },
                { "Tipo", Tipo.ToString() }
            });
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Nombre) ? Codigo : Nombre;
        }
    }
}
