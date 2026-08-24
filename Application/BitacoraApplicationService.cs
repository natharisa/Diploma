using System.Collections.Generic;
using Domain;
using Repository;

namespace Application
{
    public class BitacoraApplicationService
    {
        private readonly BitacoraRepository _bitacoraRepository;

        public BitacoraApplicationService()
            : this(new BitacoraRepository())
        {
        }

        public BitacoraApplicationService(BitacoraRepository bitacoraRepository)
        {
            _bitacoraRepository = bitacoraRepository;
        }

        public List<BitacoraRegistro> Listar()
        {
            return _bitacoraRepository.Listar();
        }
    }
}
