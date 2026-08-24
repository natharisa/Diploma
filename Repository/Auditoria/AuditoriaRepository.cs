using DAL;
using Domain;
using System.Collections.Generic;

namespace Repository
{
    public class AuditoriaRepository
    {
        private readonly AuditoriaDataMapper _auditoriaDataMapper;

        public AuditoriaRepository()
            : this(new AuditoriaDataMapper())
        {
        }

        public AuditoriaRepository(AuditoriaDataMapper auditoriaDataMapper)
        {
            _auditoriaDataMapper = auditoriaDataMapper;
        }

        public bool Registrar(AuditoriaRegistro auditoria)
        {
            if (auditoria == null)
            {
                return false;
            }

            return _auditoriaDataMapper.Insertar(auditoria) > 0;
        }

        public List<AuditoriaRegistro> ListarPorEntidad(string entidad, int idEntidad)
        {
            if (string.IsNullOrWhiteSpace(entidad) || idEntidad == 0)
            {
                return new List<AuditoriaRegistro>();
            }

            return _auditoriaDataMapper.ListarPorEntidad(entidad.Trim(), idEntidad);
        }

        public List<AuditoriaRegistro> ListarTodos()
        {
            return _auditoriaDataMapper.ListarTodos();
        }
    }
}
