using Repository;

namespace Application
{
    public class DigitoVerificadorApplicationService
    {
        private readonly DigitoVerificadorRepository _digitoVerificadorRepository;

        public DigitoVerificadorApplicationService()
            : this(new DigitoVerificadorRepository())
        {
        }

        public DigitoVerificadorApplicationService(DigitoVerificadorRepository digitoVerificadorRepository)
        {
            _digitoVerificadorRepository = digitoVerificadorRepository;
        }

        public bool VerificarUsuarios()
        {
            return _digitoVerificadorRepository.VerificarUsuarios();
        }

        public bool RecalcularUsuarios()
        {
            return _digitoVerificadorRepository.RecalcularUsuarios();
        }

        public bool RecalcularUsuarioYDvv(int idUsuario)
        {
            return _digitoVerificadorRepository.RecalcularUsuarioYDvv(idUsuario);
        }

        public bool HayBloqueoUsuarios()
        {
            return _digitoVerificadorRepository.HayBloqueoUsuarios();
        }
    }
}
