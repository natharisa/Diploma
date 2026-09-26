using System.Collections.Generic;
using DAL;
using Domain;

namespace Repository
{
    public class ProfesionalRepository_180_njab
    {
        private readonly ProfesionalDataMapper_180_njab _profesionalDataMapper_180_njab;

        public ProfesionalRepository_180_njab()
            : this(new ProfesionalDataMapper_180_njab())
        {
        }

        public ProfesionalRepository_180_njab(ProfesionalDataMapper_180_njab profesionalDataMapper_180_njab)
        {
            _profesionalDataMapper_180_njab = profesionalDataMapper_180_njab;
        }

        public List<ProfesionalConsulta_180_njab> Listar_180_njab()
        {
            //Pide al mapper los profesionales para devolverlos a la aplicacion
            return _profesionalDataMapper_180_njab.Listar_180_njab();
        }
    }
}
