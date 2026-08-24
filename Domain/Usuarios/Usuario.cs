using Abstractions;
using System.Collections.Generic;

namespace Domain
{
    public class Usuario : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Idioma { get; set; }
        public int? IdiomaPreferidoId { get; set; }
        public string Estado { get; set; }
        public int IntentosLoginFallidos { get; set; }
        public bool BloqueoDigitoVerificador { get; set; }
        public string Dvh { get; set; }
        public List<ComponentePermiso> ComponentesPermiso { get; set; } = new List<ComponentePermiso>();

        public AuditoriaMemento CrearMemento()
        {
            return SaveToMemento();
        }

        public AuditoriaMemento SaveToMemento()
        {
            return new AuditoriaMemento("Usuario", Id, new Dictionary<string, object>
            {
                { "Id", Id },
                { "Username", Username },
                { "Email", Email },
                { "Nombre", Nombre },
                { "Apellido", Apellido },
                { "Idioma", Idioma },
                { "IdiomaPreferidoId", IdiomaPreferidoId },
                { "Estado", Estado },
                { "IntentosLoginFallidos", IntentosLoginFallidos },
                { "BloqueoDigitoVerificador", BloqueoDigitoVerificador }
            });
        }

        public void RestoreFromMemento(AuditoriaMemento memento)
        {
            if (memento == null || memento.Entidad != "Usuario")
            {
                return;
            }

            IReadOnlyDictionary<string, object> estado = memento.GetSavedMemento();
            Id = ObtenerValor<int>(estado, "Id", Id);
            Username = ObtenerValor<string>(estado, "Username", Username);
            Email = ObtenerValor<string>(estado, "Email", Email);
            Nombre = ObtenerValor<string>(estado, "Nombre", Nombre);
            Apellido = ObtenerValor<string>(estado, "Apellido", Apellido);
            Idioma = ObtenerValor<string>(estado, "Idioma", Idioma);
            IdiomaPreferidoId = ObtenerValor<int?>(estado, "IdiomaPreferidoId", IdiomaPreferidoId);
            Estado = ObtenerValor<string>(estado, "Estado", Estado);
            IntentosLoginFallidos = ObtenerValor<int>(estado, "IntentosLoginFallidos", IntentosLoginFallidos);
            BloqueoDigitoVerificador = ObtenerValor<bool>(estado, "BloqueoDigitoVerificador", BloqueoDigitoVerificador);
        }

        public bool TienePermiso(string codigoPermiso)
        {
            if (ComponentesPermiso == null || string.IsNullOrWhiteSpace(codigoPermiso))
            {
                return false;
            }

            foreach (ComponentePermiso componente in ComponentesPermiso)
            {
                if (componente.TienePermiso(codigoPermiso))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string nombreCompleto = $"{Nombre} {Apellido}".Trim();

            if (!string.IsNullOrWhiteSpace(nombreCompleto))
            {
                return nombreCompleto;
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                return Username;
            }

            return Email ?? string.Empty;
        }

        private static T ObtenerValor<T>(IReadOnlyDictionary<string, object> estado, string campo, T valorActual)
        {
            if (estado == null || !estado.ContainsKey(campo) || estado[campo] == null)
            {
                return valorActual;
            }

            if (estado[campo] is T)
            {
                return (T)estado[campo];
            }

            return valorActual;
        }
    }
}
