using System.Collections.Generic;
using Domain;
using Repository;

namespace Application
{
    public class ProfesionalApplicationService_180_njab
    {
        private readonly ProfesionalRepository_180_njab _profesionalRepository_180_njab;

        public ProfesionalApplicationService_180_njab()
            : this(new ProfesionalRepository_180_njab())
        {
        }

        public ProfesionalApplicationService_180_njab(ProfesionalRepository_180_njab profesionalRepository_180_njab)
        {
            _profesionalRepository_180_njab = profesionalRepository_180_njab;
        }

        public List<ProfesionalConsulta_180_njab> Listar_180_njab()
        {
            //Delega la consulta al repositorio
            return _profesionalRepository_180_njab.Listar_180_njab();
        }
    }
}
