using DAL;

namespace Repository
{
    public class DigitoVerificadorRepository
    {
        private readonly DigitoVerificadorDataMapper _digitoVerificadorDataMapper;

        public DigitoVerificadorRepository()
            : this(new DigitoVerificadorDataMapper())
        {
        }

        public DigitoVerificadorRepository(DigitoVerificadorDataMapper digitoVerificadorDataMapper)
        {
            _digitoVerificadorDataMapper = digitoVerificadorDataMapper;
        }

        public bool VerificarUsuarios()
        {
            return _digitoVerificadorDataMapper.VerificarUsuarios();
        }

        public bool RecalcularUsuarios()
        {
            return _digitoVerificadorDataMapper.RecalcularUsuarios();
        }

        public bool RecalcularUsuarioYDvv(int idUsuario)
        {
            return _digitoVerificadorDataMapper.RecalcularUsuarioYDvv(idUsuario);
        }

        public bool HayBloqueoUsuarios()
        {
            return _digitoVerificadorDataMapper.HayBloqueoUsuarios();
        }
    }
}
