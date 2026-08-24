using System;
using System.Collections.Generic;

namespace Domain
{
    public class AuditoriaMemento
    {
        private readonly IReadOnlyDictionary<string, object> _estado;

        public AuditoriaMemento(string entidad, int idEntidad, IDictionary<string, object> estado)
        {
            Entidad = entidad;
            IdEntidad = idEntidad;
            FechaCaptura = DateTime.Now;
            _estado = new Dictionary<string, object>(estado ?? new Dictionary<string, object>());
        }

        public string Entidad { get; private set; }
        public int IdEntidad { get; private set; }
        public DateTime FechaCaptura { get; private set; }
        public IReadOnlyDictionary<string, object> Estado
        {
            get { return _estado; }
        }

        public IReadOnlyDictionary<string, object> GetSavedMemento()
        {
            return _estado;
        }
    }
}
