using System;
using System.Windows.Forms;
using Application;
using Domain;

namespace UI
{
    public partial class Registro : Form
    {
        private readonly UsuarioApplicationService _usuarioService;

        public string UsuarioRegistrado { get; private set; }

        public Registro(UsuarioApplicationService usuarioService)
        {
            _usuarioService = usuarioService;
            InitializeComponent();
        }

        private void btnCrearCuenta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Completa todos los campos para registrarte.");
                return;
            }

            Usuario nuevo = new Usuario
            {
                Username = txtUser.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPass.Text,
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim()
            };

            CodigoRegistroUsuario resultado = _usuarioService.CrearUsuario(nuevo);

            if (resultado == CodigoRegistroUsuario.Creado)
            {
                UsuarioRegistrado = nuevo.Username;
                MessageBox.Show("Usuario registrado con exito.");
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(ObtenerMensajeRegistro(resultado));
            }
        }

        private static string ObtenerMensajeRegistro(CodigoRegistroUsuario resultado)
        {
            switch (resultado)
            {
                case CodigoRegistroUsuario.DatosInvalidos:
                    return "Completa todos los campos para registrarte.";

                case CodigoRegistroUsuario.EmailInvalido:
                    return "Ingresa un email valido.";

                case CodigoRegistroUsuario.UsuarioExistente:
                    return "Ya existe un usuario con ese nombre.";

                case CodigoRegistroUsuario.EmailExistente:
                    return "Ya existe un usuario con ese email.";

                case CodigoRegistroUsuario.IdiomaDefaultInexistente:
                    return "No se pudo registrar el usuario porque falta el idioma default.";

                default:
                    return "Ocurrio un error tecnico al registrar el usuario.";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
