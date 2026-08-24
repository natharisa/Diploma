using System.Collections.Generic;
using DAL;
using Domain;

namespace Repository
{
    public class UsuarioRepository
    {
        private readonly UsuarioDataMapper _usuarioDataMapper;

        public UsuarioRepository()
            : this(new UsuarioDataMapper())
        {
        }

        public UsuarioRepository(UsuarioDataMapper usuarioDataMapper)
        {
            _usuarioDataMapper = usuarioDataMapper;
        }

        public CodigoRegistroUsuario Crear(Usuario usuario)
        {
            if (usuario == null)
            {
                return CodigoRegistroUsuario.DatosInvalidos;
            }

            return _usuarioDataMapper.Insertar(usuario);
        }

        public Usuario ObtenerPorCredenciales(string username, string password)
        {
            return _usuarioDataMapper.ObtenerPorCredenciales(username, password);
        }

        public bool Existe(string username)
        {
            return _usuarioDataMapper.ExistePorNombreUsuario(username);
        }

        public Usuario ObtenerActivoPorIdentificador(string identificador)
        {
            return _usuarioDataMapper.ObtenerActivoPorIdentificador(identificador);
        }

        public Usuario ObtenerPorId(int id)
        {
            if (id == 0)
            {
                return null;
            }

            return _usuarioDataMapper.ObtenerPorId(id);
        }

        public bool EstaBloqueadoPorIdentificador(string identificador)
        {
            return _usuarioDataMapper.EstaBloqueadoPorIdentificador(identificador);
        }

        public int RegistrarLoginFallidoPorIdentificador(string identificador)
        {
            return _usuarioDataMapper.RegistrarLoginFallidoPorIdentificador(identificador);
        }

        public bool ReiniciarIntentosLoginFallidos(int idUsuario)
        {
            if (idUsuario == 0)
            {
                return false;
            }

            return _usuarioDataMapper.ReiniciarIntentosLoginFallidos(idUsuario) >= 0;
        }

        public void Guardar(Usuario usuario)
        {
            if (usuario == null)
            {
                return;
            }

            if (usuario.Id == 0)
            {
                _usuarioDataMapper.Insertar(usuario);
            }
            else
            {
                _usuarioDataMapper.Editar(usuario);
            }
        }

        public bool Modificar(Usuario usuario)
        {
            if (usuario == null || usuario.Id == 0)
            {
                return false;
            }

            return _usuarioDataMapper.Editar(usuario) > 0;
        }

        public bool ActualizarIdiomaPreferido(int usuarioId, int idiomaId)
        {
            if (usuarioId == 0 || idiomaId == 0)
            {
                return false;
            }

            bool actualizado = _usuarioDataMapper.ActualizarIdiomaPreferido(usuarioId, idiomaId) > 0;

            if (actualizado)
            {
                new DigitoVerificadorDataMapper().RecalcularUsuarios();
            }

            return actualizado;
        }

        public bool RestaurarCampo(int idUsuario, string campo, object valor)
        {
            if (idUsuario == 0)
            {
                return false;
            }

            return _usuarioDataMapper.RestaurarCampo(idUsuario, campo, valor) > 0;
        }

        public void Borrar(Usuario usuario)
        {
            if (usuario == null)
            {
                return;
            }

            _usuarioDataMapper.Borrar(usuario);
        }

        public bool Inhabilitar(Usuario usuario)
        {
            if (usuario == null || usuario.Id == 0)
            {
                return false;
            }

            return _usuarioDataMapper.Borrar(usuario) > 0;
        }

        public List<Usuario> Listar()
        {
            return _usuarioDataMapper.Listar();
        }

    }
}
