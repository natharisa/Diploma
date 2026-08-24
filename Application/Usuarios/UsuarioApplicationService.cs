using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Script.Serialization;
using Domain;
using Repository;
using Services;

namespace Application
{
    public class UsuarioApplicationService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly PermisoRepository _permisoRepository;
        private readonly BitacoraRepository _bitacoraRepository;
        private readonly AuditoriaApplicationService _auditoriaService;
        private readonly DigitoVerificadorApplicationService _digitoVerificadorService;
        private readonly PlainTextPasswordService _passwordService;
        private readonly JavaScriptSerializer _serializer;
        private BitacoraFactory _bitacoraFactory;

        public UsuarioApplicationService()
            : this(new UsuarioRepository(), new PermisoRepository(), new BitacoraRepository(), new AuditoriaApplicationService(), new DigitoVerificadorApplicationService(), new PlainTextPasswordService())
        {
        }

        public UsuarioApplicationService(
            UsuarioRepository usuarioRepository,
            PermisoRepository permisoRepository,
            BitacoraRepository bitacoraRepository,
            PlainTextPasswordService passwordService)
            : this(usuarioRepository, permisoRepository, bitacoraRepository, new AuditoriaApplicationService(), new DigitoVerificadorApplicationService(), passwordService)
        {
        }

        public UsuarioApplicationService(
            UsuarioRepository usuarioRepository,
            PermisoRepository permisoRepository,
            BitacoraRepository bitacoraRepository,
            AuditoriaApplicationService auditoriaService,
            PlainTextPasswordService passwordService)
            : this(usuarioRepository, permisoRepository, bitacoraRepository, auditoriaService, new DigitoVerificadorApplicationService(), passwordService)
        {
        }

        public UsuarioApplicationService(
            UsuarioRepository usuarioRepository,
            PermisoRepository permisoRepository,
            BitacoraRepository bitacoraRepository,
            AuditoriaApplicationService auditoriaService,
            DigitoVerificadorApplicationService digitoVerificadorService,
            PlainTextPasswordService passwordService)
        {
            _usuarioRepository = usuarioRepository;
            _permisoRepository = permisoRepository;
            _bitacoraRepository = bitacoraRepository;
            _auditoriaService = auditoriaService;
            _digitoVerificadorService = digitoVerificadorService;
            _passwordService = passwordService;
            _serializer = new JavaScriptSerializer();
        }

        //Registrar el usuario
        public CodigoRegistroUsuario CrearUsuario(Usuario nuevoUsuario)
        {
            if (nuevoUsuario == null ||
                string.IsNullOrWhiteSpace(nuevoUsuario.Username) ||
                string.IsNullOrWhiteSpace(nuevoUsuario.Email) ||
                string.IsNullOrWhiteSpace(nuevoUsuario.Password))
            {
                return CodigoRegistroUsuario.DatosInvalidos;
            }

            if (!EsFormatoEmailValido(nuevoUsuario.Email))
            {
                return CodigoRegistroUsuario.EmailInvalido;
            }

            //Aplicacion del hash
            Usuario usuarioProtegido = new Usuario
            {
                Id = nuevoUsuario.Id,
                Username = nuevoUsuario.Username.Trim(),
                Email = nuevoUsuario.Email.Trim(),
                Password = _passwordService.Hash(nuevoUsuario.Password),
                Nombre = string.IsNullOrWhiteSpace(nuevoUsuario.Nombre) ? null : nuevoUsuario.Nombre.Trim(),
                Apellido = string.IsNullOrWhiteSpace(nuevoUsuario.Apellido) ? null : nuevoUsuario.Apellido.Trim(),
                Idioma = nuevoUsuario.Idioma,
                IdiomaPreferidoId = nuevoUsuario.IdiomaPreferidoId,
                Estado = "ACTIVO"
            };

            //Registro de la falla al ingresar REGISTRO
            CodigoRegistroUsuario resultado = _usuarioRepository.Crear(usuarioProtegido);
            this._bitacoraFactory = new RegistroFallidoBitacoraFactory();

            if (resultado == CodigoRegistroUsuario.Creado)
            {
                _digitoVerificadorService.RecalcularUsuarios();
                _auditoriaService.RegistrarAlta(usuarioProtegido.CrearMemento());
            }

            if (resultado == CodigoRegistroUsuario.UsuarioExistente)
            {
                RegistrarRegistroFallido(usuarioProtegido.Username, "Intento de registro con usuario existente.");
            }
            else if (resultado == CodigoRegistroUsuario.EmailExistente)
            {
                RegistrarRegistroFallido(usuarioProtegido.Email, "Intento de registro con email existente.");
            }

            return resultado;
        }

        //LOGIN registro de falla
        public Usuario Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            string identificador = username.Trim();
            string passwordProtegida = _passwordService.Hash(password);
            bool integridadUsuariosValida = _digitoVerificadorService.VerificarUsuarios();
            Usuario usuario = _usuarioRepository.ObtenerPorCredenciales(identificador, passwordProtegida);

            if (usuario == null)
            {
                if (integridadUsuariosValida)
                {
                    Usuario usuarioExistente = _usuarioRepository.ObtenerActivoPorIdentificador(identificador);

                    if (usuarioExistente != null)
                    {
                        int intentosFallidos = _usuarioRepository.RegistrarLoginFallidoPorIdentificador(identificador);
                        _digitoVerificadorService.RecalcularUsuarioYDvv(usuarioExistente.Id);
                        this._bitacoraFactory = new LoginFallidoBitacoraFactory();
                        string descripcion = intentosFallidos >= 3
                            ? "Intento de login con contrasena incorrecta. Usuario deshabilitado por alcanzar 3 intentos fallidos."
                            : "Intento de login con contrasena incorrecta.";

                        RegistrarLoginFallido(usuarioExistente, descripcion);
                    }
                }

                return null;
            }

            usuario.ComponentesPermiso = _permisoRepository.ListarAsignadosPorUsuario(usuario.Id);

            if (!integridadUsuariosValida && !EsAdministrador(usuario))
            {
                return null;
            }

            _usuarioRepository.ReiniciarIntentosLoginFallidos(usuario.Id);
            _digitoVerificadorService.RecalcularUsuarioYDvv(usuario.Id);
            usuario.IntentosLoginFallidos = 0;

            this._bitacoraFactory = new LoginExitosoBitacoraFactory();
            RegistrarLoginExitoso(usuario, "Login exitoso.");
            return usuario;
        }

        public bool ExisteUsuario(string username)
        {
            return _usuarioRepository.Existe(username);
        }

        public bool EstaBloqueado(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            return _usuarioRepository.EstaBloqueadoPorIdentificador(username.Trim());
        }

        public void Grabar(Usuario usuario)
        {
            _usuarioRepository.Guardar(usuario);
            _digitoVerificadorService.RecalcularUsuarios();
        }

        public bool ModificarUsuario(Usuario usuario)
        {
            if (usuario == null ||
                usuario.Id == 0 ||
                string.IsNullOrWhiteSpace(usuario.Username) ||
                string.IsNullOrWhiteSpace(usuario.Email))
            {
                return false;
            }

            if (!EsFormatoEmailValido(usuario.Email))
            {
                return false;
            }

            Usuario usuarioNormalizado = new Usuario
            {
                Id = usuario.Id,
                Username = usuario.Username.Trim(),
                Email = usuario.Email.Trim(),
                Password = string.IsNullOrWhiteSpace(usuario.Password) ? null : _passwordService.Hash(usuario.Password),
                Nombre = string.IsNullOrWhiteSpace(usuario.Nombre) ? null : usuario.Nombre.Trim(),
                Apellido = string.IsNullOrWhiteSpace(usuario.Apellido) ? null : usuario.Apellido.Trim(),
                Idioma = usuario.Idioma,
                IdiomaPreferidoId = usuario.IdiomaPreferidoId,
                Estado = string.IsNullOrWhiteSpace(usuario.Estado) ? "ACTIVO" : usuario.Estado.Trim()
            };

            Usuario usuarioAnterior = _usuarioRepository.ObtenerPorId(usuarioNormalizado.Id);

            if (usuarioAnterior == null)
            {
                return false;
            }

            AuditoriaMemento estadoAnterior = usuarioAnterior.CrearMemento();
            bool modificado = _usuarioRepository.Modificar(usuarioNormalizado);

            if (!modificado)
            {
                return false;
            }

            _digitoVerificadorService.RecalcularUsuarios();
            Usuario usuarioPosterior = _usuarioRepository.ObtenerPorId(usuarioNormalizado.Id);

            if (usuarioPosterior != null)
            {
                _auditoriaService.RegistrarModificacion(estadoAnterior, usuarioPosterior.CrearMemento());
            }

            return true;
        }

        public void Borrar(Usuario usuario)
        {
            _usuarioRepository.Borrar(usuario);
            _digitoVerificadorService.RecalcularUsuarios();
        }

        public bool InhabilitarUsuario(Usuario usuario)
        {
            if (usuario == null || usuario.Id == 0)
            {
                return false;
            }

            Usuario usuarioAnterior = _usuarioRepository.ObtenerPorId(usuario.Id);
            bool inhabilitado = _usuarioRepository.Inhabilitar(usuario);

            if (inhabilitado)
            {
                _digitoVerificadorService.RecalcularUsuarios();
                Usuario usuarioPosterior = _usuarioRepository.ObtenerPorId(usuario.Id);
                if (usuarioAnterior != null && usuarioPosterior != null)
                {
                    _auditoriaService.RegistrarCambio(usuarioAnterior.CrearMemento(), usuarioPosterior.CrearMemento(), "DISABLE");
                }
            }

            return inhabilitado;
        }

        public List<Usuario> Listar()
        {
            return _usuarioRepository.Listar();
        }

        public bool RecalcularDigitosVerificadoresUsuarios()
        {
            return _digitoVerificadorService.RecalcularUsuarios();
        }

        public bool HayBloqueoDigitoVerificador()
        {
            return !_digitoVerificadorService.VerificarUsuarios() ||
                   _digitoVerificadorService.HayBloqueoUsuarios();
        }

        public bool RestaurarCampoDesdeAuditoria(AuditoriaRegistro auditoria, string campo)
        {
            if (auditoria == null ||
                auditoria.Entidad != "Usuario" ||
                auditoria.IdEntidad == 0 ||
                string.IsNullOrWhiteSpace(campo) ||
                string.IsNullOrWhiteSpace(auditoria.EstadoAnteriorJson))
            {
                return false;
            }

            Dictionary<string, object> estadoAnterior;
            try
            {
                estadoAnterior = _serializer.Deserialize<Dictionary<string, object>>(auditoria.EstadoAnteriorJson);
            }
            catch
            {
                return false;
            }

            if (estadoAnterior == null || !estadoAnterior.ContainsKey(campo))
            {
                return false;
            }

            object valorAnterior = estadoAnterior[campo];
            if (!EsCampoRestaurable(campo) || !EsValorRestaurableValido(campo, valorAnterior))
            {
                return false;
            }

            Usuario usuarioAnterior = _usuarioRepository.ObtenerPorId(auditoria.IdEntidad);
            if (usuarioAnterior == null)
            {
                return false;
            }

            AuditoriaMemento estadoActual = usuarioAnterior.CrearMemento();
            bool restaurado = _usuarioRepository.RestaurarCampo(auditoria.IdEntidad, campo, valorAnterior);
            if (!restaurado)
            {
                return false;
            }

            _digitoVerificadorService.RecalcularUsuarios();

            Usuario usuarioRestaurado = _usuarioRepository.ObtenerPorId(auditoria.IdEntidad);
            if (usuarioRestaurado != null)
            {
                _auditoriaService.RegistrarCambio(estadoActual, usuarioRestaurado.CrearMemento(), "RESTORE_FIELD");
            }

            return true;
        }

        private void RegistrarLoginFallido(Usuario usuario, string descripcion)
        {
            if (usuario == null)
            {
                return;
            }

            IBitacoraEvento evento = _bitacoraFactory.Crear(NormalizarIdentificador(usuario.Username), descripcion);
            evento.IdUsuario = usuario.Id;
            _bitacoraRepository.Registrar(evento);
        }

        private void RegistrarLoginExitoso(Usuario usuario, string descripcion)
        {
            if (usuario == null)
            {
                return;
            }

            IBitacoraEvento evento = _bitacoraFactory.Crear(NormalizarIdentificador(usuario.Username), descripcion);
            evento.IdUsuario = usuario.Id;
            _bitacoraRepository.Registrar(evento);
        }

        private void RegistrarRegistroFallido(string identificador, string descripcion)
        {
            IBitacoraEvento evento = _bitacoraFactory.Crear(NormalizarIdentificador(identificador), descripcion);
            _bitacoraRepository.Registrar(evento);
        }

        private static string NormalizarIdentificador(string username)
        {
            return string.IsNullOrWhiteSpace(username) ? null : username.Trim();
        }

        private static bool EsAdministrador(Usuario usuario)
        {
            if (usuario == null || usuario.ComponentesPermiso == null)
            {
                return false;
            }

            foreach (ComponentePermiso componente in usuario.ComponentesPermiso)
            {
                if (EsComponenteAdministrador(componente))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool EsComponenteAdministrador(ComponentePermiso componente)
        {
            if (componente == null)
            {
                return false;
            }

            if (componente.Codigo == PermisosSistema.Administrador)
            {
                return true;
            }

            foreach (ComponentePermiso hijo in componente.ObtenerHijos())
            {
                if (EsComponenteAdministrador(hijo))
                {
                    return true;
                }
            }

            return false;
        }

        public bool EsEmailValido(string email)
        {
            return EsFormatoEmailValido(email);
        }

        private static bool EsFormatoEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailNormalizado = email.Trim();

            try
            {
                MailAddress direccion = new MailAddress(emailNormalizado);
                return direccion.Address == emailNormalizado;
            }
            catch
            {
                return false;
            }
        }

        private static bool EsCampoRestaurable(string campo)
        {
            switch (campo)
            {
                case "Username":
                case "Email":
                case "Nombre":
                case "Apellido":
                case "IdiomaPreferidoId":
                case "Estado":
                case "IntentosLoginFallidos":
                case "BloqueoDigitoVerificador":
                    return true;

                default:
                    return false;
            }
        }

        private static bool EsValorRestaurableValido(string campo, object valor)
        {
            switch (campo)
            {
                case "Username":
                    return valor != null && !string.IsNullOrWhiteSpace(valor.ToString());

                case "Email":
                    return valor != null && EsFormatoEmailValido(valor.ToString());

                case "Estado":
                    string estado = valor == null ? null : valor.ToString();
                    return estado == "ACTIVO" || estado == "INACTIVO" || estado == "BLOQUEADO";

                default:
                    return true;
            }
        }
    }
}
