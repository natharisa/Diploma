using System;
using System.Collections.Generic;

namespace Domain
{
    public class Permiso : ComponentePermiso
    {
        public override TipoComponentePermiso Tipo
        {
            get { return TipoComponentePermiso.Permiso; }
        }

        public override void Agregar(ComponentePermiso componente)
        {
            throw new InvalidOperationException("Un permiso no puede contener otros componentes.");
        }

        public override void Quitar(ComponentePermiso componente)
        {
            throw new InvalidOperationException("Un permiso no puede contener otros componentes.");
        }

        public override IReadOnlyList<ComponentePermiso> ObtenerHijos()
        {
            return new List<ComponentePermiso>().AsReadOnly();
        }

        public override bool TienePermiso(string codigoPermiso)
        {
            return !string.IsNullOrWhiteSpace(codigoPermiso) &&
                   string.Equals(Codigo, codigoPermiso, StringComparison.OrdinalIgnoreCase);
        }
    }
}
