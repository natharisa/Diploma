using System;
using System.Windows.Forms;
using Application;
using Domain;
using Services;

namespace UI
{
    public partial class Login : Form
    {
        private readonly UsuarioApplicationService _usuarioService;
        private readonly UiTextService _uiTextService;

        public Login()
            : this(new UsuarioApplicationService(), new UiTextService())
        {
        }

        public Login(UsuarioApplicationService usuarioService, UiTextService uiTextService)
        {
            _usuarioService = usuarioService;
            _uiTextService = uiTextService;
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Usuario usuarioValidado = _usuarioService.Login(txtUser.Text, txtPass.Text);

            if (usuarioValidado != null)
            {
                Sesion sesion = Sesion.ObtenerInstancia();

                sesion.IniciarSesion(usuarioValidado);
                LanguageManager.Instance.Initialize(usuarioValidado);
                MessageBox.Show(_uiTextService.BuildWelcomeMessage(sesion.ObtenerUsuario().ToString()));
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                if (_usuarioService.EstaBloqueado(txtUser.Text))
                {
                    MessageBox.Show("El usuario esta bloqueado. Contacte a un administrador.");
                    return;
                }

                if (_usuarioService.HayBloqueoDigitoVerificador())
                {
                    MessageBox.Show("Se detecto una inconsistencia de integridad. Solo un administrador puede iniciar sesion.");
                    return;
                }

                MessageBox.Show("Usuario, email o contrasena incorrectos.");
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            using (Registro registro = new Registro(_usuarioService))
            {
                if (registro.ShowDialog(this) == DialogResult.OK)
                {
                    txtUser.Text = registro.UsuarioRegistrado;
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
        }
    }
}
