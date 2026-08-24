using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Domain;
using Repository;

namespace Application
{
    public class AuditoriaApplicationService
    {
        private readonly AuditoriaRepository _auditoriaRepository;
        private readonly JavaScriptSerializer _serializer;

        public AuditoriaApplicationService()
            : this(new AuditoriaRepository())
        {
        }

        public AuditoriaApplicationService(AuditoriaRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
            _serializer = new JavaScriptSerializer();
        }

        public bool RegistrarModificacion(AuditoriaMemento estadoAnterior, AuditoriaMemento estadoNuevo)
        {
            return RegistrarCambio(estadoAnterior, estadoNuevo, "UPDATE");
        }

        public bool RegistrarAlta(AuditoriaMemento estadoNuevo)
        {
            return RegistrarCambio(null, estadoNuevo, "CREATE");
        }

        public bool RegistrarCambio(AuditoriaMemento estadoAnterior, AuditoriaMemento estadoNuevo, string accion)
        {
            if (estadoNuevo == null || estadoNuevo.Entidad != "Usuario")
            {
                return true;
            }

            if (estadoAnterior == null || estadoNuevo == null)
            {
                estadoAnterior = new AuditoriaMemento(estadoNuevo.Entidad, estadoNuevo.IdEntidad, new Dictionary<string, object>());
            }

            if (estadoAnterior.Entidad != estadoNuevo.Entidad || estadoAnterior.IdEntidad != estadoNuevo.IdEntidad)
            {
                return false;
            }

            List<AuditoriaCambio> cambios = CalcularCambios(estadoAnterior, estadoNuevo);

            if (cambios.Count == 0)
            {
                return true;
            }

            Usuario usuarioActor = Sesion.ObtenerInstancia().ObtenerUsuario();

            AuditoriaRegistro auditoria = new AuditoriaRegistro
            {
                Entidad = estadoNuevo.Entidad,
                IdEntidad = estadoNuevo.IdEntidad,
                Accion = string.IsNullOrWhiteSpace(accion) ? "UPDATE" : accion.Trim(),
                IdUsuarioActor = usuarioActor?.Id,
                IdentificadorUsuarioActor = usuarioActor?.Username,
                FechaEvento = DateTime.Now,
                EstadoAnteriorJson = _serializer.Serialize(estadoAnterior.Estado),
                EstadoNuevoJson = _serializer.Serialize(estadoNuevo.Estado),
                CambiosJson = _serializer.Serialize(cambios)
            };

            return _auditoriaRepository.Registrar(auditoria);
        }

        public bool RegistrarSnapshot(string entidad, int idEntidad, string accion, IDictionary<string, object> estado)
        {
            return RegistrarCambio(
                new AuditoriaMemento(entidad, idEntidad, new Dictionary<string, object>()),
                new AuditoriaMemento(entidad, idEntidad, estado),
                accion);
        }

        public List<AuditoriaRegistro> ListarTodos()
        {
            List<AuditoriaRegistro> registrosUsuario = new List<AuditoriaRegistro>();

            foreach (AuditoriaRegistro registro in _auditoriaRepository.ListarTodos())
            {
                if (registro.Entidad == "Usuario")
                {
                    registrosUsuario.Add(registro);
                }
            }

            return registrosUsuario;
        }

        public List<AuditoriaRegistro> ListarHistorial(string entidad, int idEntidad)
        {
            return _auditoriaRepository.ListarPorEntidad(entidad, idEntidad);
        }

        private static List<AuditoriaCambio> CalcularCambios(AuditoriaMemento estadoAnterior, AuditoriaMemento estadoNuevo)
        {
            List<AuditoriaCambio> cambios = new List<AuditoriaCambio>();
            SortedSet<string> campos = new SortedSet<string>(StringComparer.Ordinal);

            foreach (string campo in estadoAnterior.Estado.Keys)
            {
                campos.Add(campo);
            }

            foreach (string campo in estadoNuevo.Estado.Keys)
            {
                campos.Add(campo);
            }

            foreach (string campo in campos)
            {
                object valorAnterior = estadoAnterior.Estado.ContainsKey(campo) ? estadoAnterior.Estado[campo] : null;
                object valorNuevo = estadoNuevo.Estado.ContainsKey(campo) ? estadoNuevo.Estado[campo] : null;

                if (object.Equals(valorAnterior, valorNuevo))
                {
                    continue;
                }

                cambios.Add(new AuditoriaCambio
                {
                    Campo = campo,
                    ValorAnterior = valorAnterior,
                    ValorNuevo = valorNuevo
                });
            }

            return cambios;
        }
    }
}
