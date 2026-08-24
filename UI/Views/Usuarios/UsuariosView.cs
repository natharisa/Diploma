using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Application;
using Domain;
using Services;

namespace UI
{
    public partial class UsuariosView : LocalizedUserControl
    {
        private readonly UsuarioApplicationService _usuarioService;
        private readonly PermisoApplicationService _permisoService;
        private readonly AutorizacionApplicationService _autorizacionService;
        private List<Usuario> _usuarios;
        private List<ComponentePermiso> _familiasDisponibles;
        private Usuario _usuarioSeleccionado;
        private bool _preparandoFormulario;
        private GroupBox groupBoxRolesUsuario;
        private Label lblRolesInfo;
        private ComboBox cmbRolUsuario;
        private Button btnGuardarRoles;
        private Button btnRecalcularDigitos;

        public UsuariosView()
            : this(new UsuarioApplicationService())
        {
        }

        public UsuariosView(UsuarioApplicationService usuarioService)
        {
            _usuarioService = usuarioService;
            _permisoService = new PermisoApplicationService();
            _autorizacionService = new AutorizacionApplicationService();
            InitializeComponent();
            ConfigurarTraducciones();
            ConfigurarGrilla();
            ConfigurarAsignacionRoles();
            ConfigurarPermisos();
            CargarUsuarios();
            CargarRolesDisponibles();
            PrepararNuevoUsuario();
        }

        private void ConfigurarTraducciones()
        {
            lblTitulo.Tag = "USERS_TITLE";
            lblDescripcion.Tag = "USERS_DESCRIPTION";
            groupBoxDetalle.Tag = "USERS_DETAIL";
            lblUsuario.Tag = "FIELD_USER";
            lblEmail.Tag = "FIELD_EMAIL";
            lblNombre.Tag = "FIELD_NAME";
            lblApellido.Tag = "FIELD_LASTNAME";
            lblPassword.Tag = "FIELD_NEW_PASSWORD";
            lblEstado.Tag = "FIELD_STATUS";
            btnNuevo.Tag = "BTN_NEW";
            btnInhabilitar.Tag = "BTN_DISABLE";
            columnId.Tag = "GRID_ID";
            columnUsuario.Tag = "GRID_USER";
            columnEmail.Tag = "GRID_EMAIL";
            columnEstado.Tag = "GRID_STATUS";
            columnBloqueoDigitoVerificador.Tag = "GRID_DV_BLOCK";
        }

        private void ConfigurarGrilla()
        {
            dgvUsuarios.AutoGenerateColumns = false;
            columnId.Width = 45;
            columnUsuario.Width = 90;
            columnEmail.MinimumWidth = 110;
            columnEstado.Width = 70;
            columnBloqueoDigitoVerificador.Width = 75;
            cmbEstado.Items.AddRange(new object[] { "ACTIVO", "INACTIVO" });
        }

        protected override void ApplyTranslations()
        {
            base.ApplyTranslations();
            if (btnRecalcularDigitos != null)
            {
                btnRecalcularDigitos.Text = LanguageManager.Instance.Translate("BTN_RECALCULATE_DV");
            }

            ActualizarEstadoRolesUsuario();
        }

        private void CargarUsuarios()
        {
            _usuarios = _usuarioService.Listar();
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = _usuarios;
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (_preparandoFormulario)
            {
                return;
            }

            if (dgvUsuarios.CurrentRow == null)
            {
                return;
            }

            Usuario usuario = dgvUsuarios.CurrentRow.DataBoundItem as Usuario;
            if (usuario == null)
            {
                return;
            }

            _usuarioSeleccionado = usuario;
            txtUsuario.Text = usuario.Username;
            txtEmail.Text = usuario.Email;
            txtNombre.Text = usuario.Nombre;
            txtApellido.Text = usuario.Apellido;
            txtPassword.Text = string.Empty;
            cmbEstado.SelectedItem = string.IsNullOrWhiteSpace(usuario.Estado) ? "ACTIVO" : usuario.Estado;
            lblModo.Tag = "USERS_EDIT_MODE";
            lblModo.Text = LanguageManager.Instance.Translate("USERS_EDIT_MODE");
            btnGuardar.Tag = "BTN_SAVE";
            btnGuardar.Text = LanguageManager.Instance.Translate("BTN_SAVE");
            btnGuardar.Enabled = _autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar);
            btnInhabilitar.Enabled = _autorizacionService.TienePermiso(PermisosSistema.UsuarioInhabilitar) &&
                                     usuario.Id > 0 &&
                                     usuario.Estado != "INACTIVO";
            CargarRolesUsuario(usuario.Id);
            ActualizarEstadoRolesUsuario();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararNuevoUsuario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                CrearUsuario();
                return;
            }

            ModificarUsuario();
        }

        private void btnInhabilitar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona un usuario para inhabilitar.");
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "El usuario no podra iniciar sesion mientras este inactivo. Deseas continuar?",
                "Inhabilitar usuario",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            bool resultado = _usuarioService.InhabilitarUsuario(_usuarioSeleccionado);
            MessageBox.Show(resultado ? "Usuario inhabilitado correctamente." : "No se pudo inhabilitar el usuario.");
            CargarUsuarios();
            PrepararNuevoUsuario();
        }

        private void CrearUsuario()
        {
            if (!ValidarCamposAlta())
            {
                return;
            }

            Usuario usuario = CrearUsuarioDesdeFormulario();
            CodigoRegistroUsuario resultado = _usuarioService.CrearUsuario(usuario);

            if (resultado == CodigoRegistroUsuario.Creado)
            {
                MessageBox.Show("Usuario creado correctamente.");
                CargarUsuarios();
                PrepararNuevoUsuario();
                return;
            }

            MessageBox.Show(ObtenerMensajeRegistro(resultado));
        }

        private void ModificarUsuario()
        {
            if (!ValidarCamposModificacion())
            {
                return;
            }

            Usuario usuario = CrearUsuarioDesdeFormulario();
            usuario.Id = _usuarioSeleccionado.Id;

            bool resultado = _usuarioService.ModificarUsuario(usuario);
            MessageBox.Show(resultado ? "Usuario modificado correctamente." : "No se pudo modificar el usuario.");
            CargarUsuarios();
            PrepararNuevoUsuario();
        }

        private Usuario CrearUsuarioDesdeFormulario()
        {
            return new Usuario
            {
                Username = txtUsuario.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text,
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Estado = cmbEstado.SelectedItem == null ? "ACTIVO" : cmbEstado.SelectedItem.ToString()
            };
        }

        private bool ValidarCamposAlta()
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Completa usuario, email y contrasena para crear.");
                return false;
            }

            return ValidarCamposModificacion();
        }

        private bool ValidarCamposModificacion()
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Completa usuario y email.");
                return false;
            }

            if (!_usuarioService.EsEmailValido(txtEmail.Text))
            {
                MessageBox.Show("Ingresa un email valido.");
                return false;
            }

            return true;
        }

        private void PrepararNuevoUsuario()
        {
            _preparandoFormulario = true;
            _usuarioSeleccionado = null;
            dgvUsuarios.ClearSelection();
            txtUsuario.Clear();
            txtEmail.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtPassword.Clear();
            cmbEstado.SelectedItem = "ACTIVO";
            lblModo.Tag = "USERS_CREATE_MODE";
            lblModo.Text = LanguageManager.Instance.Translate("USERS_CREATE_MODE");
            btnGuardar.Tag = "BTN_CREATE";
            btnGuardar.Text = LanguageManager.Instance.Translate("BTN_CREATE");
            btnGuardar.Enabled = _autorizacionService.TienePermiso(PermisosSistema.UsuarioCrear);
            btnInhabilitar.Enabled = false;
            LimpiarRolesUsuario();
            ActualizarEstadoRolesUsuario();
            txtUsuario.Focus();
            _preparandoFormulario = false;
        }

        private void ConfigurarPermisos()
        {
            btnNuevo.Visible = _autorizacionService.TienePermiso(PermisosSistema.UsuarioCrear);
            btnGuardar.Visible = _autorizacionService.TienePermiso(PermisosSistema.UsuarioCrear) ||
                                 _autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar);
            btnInhabilitar.Visible = _autorizacionService.TienePermiso(PermisosSistema.UsuarioInhabilitar);
            btnGuardarRoles.Visible = _autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar);
            btnRecalcularDigitos.Visible = _autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar);
            ActualizarEstadoRolesUsuario();
        }

        private void ConfigurarAsignacionRoles()
        {
            dgvUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            dgvUsuarios.Location = new Point(24, 98);
            dgvUsuarios.Size = new Size(390, 390);

            btnRecalcularDigitos = new Button
            {
                BackColor = Color.FromArgb(13, 110, 253),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(424, 54),
                Size = new Size(120, 31),
                Tag = "BTN_RECALCULATE_DV",
                Text = LanguageManager.Instance.Translate("BTN_RECALCULATE_DV"),
                UseVisualStyleBackColor = false
            };
            btnRecalcularDigitos.Click += btnRecalcularDigitos_Click;

            groupBoxRolesUsuario = new GroupBox
            {
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Location = new Point(424, 98),
                Size = new Size(120, 390),
                Tag = "USER_ROLES",
                Text = "Roles"
            };

            lblRolesInfo = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(10, 24),
                Size = new Size(100, 82),
                Tag = "USER_ROLE_SELECT_HELP",
                Text = "Selecciona un usuario para asignarle un rol."
            };

            cmbRolUsuario = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, 126),
                Size = new Size(100, 23)
            };

            btnGuardarRoles = new Button
            {
                BackColor = Color.FromArgb(25, 135, 84),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 343),
                Size = new Size(100, 31),
                Tag = "BTN_SAVE",
                Text = "Guardar",
                UseVisualStyleBackColor = false
            };
            btnGuardarRoles.Click += btnGuardarRoles_Click;

            groupBoxRolesUsuario.Controls.Add(lblRolesInfo);
            groupBoxRolesUsuario.Controls.Add(cmbRolUsuario);
            groupBoxRolesUsuario.Controls.Add(btnGuardarRoles);
            Controls.Add(btnRecalcularDigitos);
            Controls.Add(groupBoxRolesUsuario);
        }

        private void CargarRolesDisponibles()
        {
            _familiasDisponibles = _permisoService.ListarFamilias();
            LimpiarRolesUsuario();
            ActualizarEstadoRolesUsuario();
        }

        private void CargarRolesUsuario(int idUsuario)
        {
            if (_familiasDisponibles == null)
            {
                CargarRolesDisponibles();
            }

            List<int> idsAsignados = _permisoService.ListarIdsComponentesAsignadosPorUsuario(idUsuario);

            cmbRolUsuario.DataSource = null;
            cmbRolUsuario.DataSource = _familiasDisponibles;

            ComponentePermiso rolAsignado = null;
            foreach (ComponentePermiso familia in _familiasDisponibles)
            {
                if (idsAsignados.Contains(familia.Id))
                {
                    rolAsignado = familia;
                    break;
                }
            }

            cmbRolUsuario.SelectedItem = rolAsignado;
            ActualizarEstadoRolesUsuario();
        }

        private void LimpiarRolesUsuario()
        {
            cmbRolUsuario.DataSource = null;

            if (_familiasDisponibles == null)
            {
                return;
            }

            cmbRolUsuario.DataSource = _familiasDisponibles;
            cmbRolUsuario.SelectedIndex = -1;
        }

        private void ActualizarEstadoRolesUsuario()
        {
            if (cmbRolUsuario == null || btnGuardarRoles == null || lblRolesInfo == null)
            {
                return;
            }

            bool puedeEditar = _autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar);
            bool hayUsuarioSeleccionado = _usuarioSeleccionado != null && _usuarioSeleccionado.Id > 0;
            bool hayRoles = cmbRolUsuario.Items.Count > 0;

            cmbRolUsuario.Enabled = puedeEditar && hayUsuarioSeleccionado && hayRoles;
            btnGuardarRoles.Enabled = puedeEditar && hayUsuarioSeleccionado && hayRoles;

            if (!puedeEditar)
            {
                lblRolesInfo.Text = LanguageManager.Instance.Translate("USER_ROLE_EDIT_DENIED");
                return;
            }

            if (!hayRoles)
            {
                lblRolesInfo.Text = LanguageManager.Instance.Translate("USER_ROLE_EMPTY");
                return;
            }

            lblRolesInfo.Text = hayUsuarioSeleccionado
                ? LanguageManager.Instance.Translate("USER_ROLE_SELECT_ONE")
                : LanguageManager.Instance.Translate("USER_ROLE_SELECT_HELP");
        }

        private void btnGuardarRoles_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona un usuario para asignarle roles.");
                return;
            }

            if (!_autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar))
            {
                MessageBox.Show("No tenes permisos para modificar roles de usuarios.");
                return;
            }

            ComponentePermiso rolSeleccionado = cmbRolUsuario.SelectedItem as ComponentePermiso;

            if (rolSeleccionado == null)
            {
                MessageBox.Show("Selecciona un rol para el usuario.");
                return;
            }

            List<int> idsSeleccionados = new List<int> { rolSeleccionado.Id };

            bool guardado = _permisoService.GuardarComponentesUsuario(_usuarioSeleccionado.Id, idsSeleccionados);
            MessageBox.Show(guardado ? "Rol asignado correctamente." : "No se pudo asignar el rol.");

            if (guardado)
            {
                CargarRolesUsuario(_usuarioSeleccionado.Id);
            }
        }

        private void btnRecalcularDigitos_Click(object sender, EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.UsuarioEditar))
            {
                MessageBox.Show("No tenes permisos para recalcular digitos verificadores.");
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "Se recalcularan los digitos verificadores de todos los usuarios y se quitaran los bloqueos por DV. Deseas continuar?",
                "Recalcular digitos verificadores",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            bool recalculado = _usuarioService.RecalcularDigitosVerificadoresUsuarios();
            MessageBox.Show(recalculado
                ? "Digitos verificadores recalculados correctamente."
                : "No se pudieron recalcular los digitos verificadores.");

            CargarUsuarios();
            PrepararNuevoUsuario();
        }

        private static string ObtenerMensajeRegistro(CodigoRegistroUsuario resultado)
        {
            switch (resultado)
            {
                case CodigoRegistroUsuario.DatosInvalidos:
                    return "Completa todos los campos obligatorios.";

                case CodigoRegistroUsuario.EmailInvalido:
                    return "Ingresa un email valido.";

                case CodigoRegistroUsuario.UsuarioExistente:
                    return "Ya existe un usuario con ese nombre.";

                case CodigoRegistroUsuario.EmailExistente:
                    return "Ya existe un usuario con ese email.";

                case CodigoRegistroUsuario.IdiomaDefaultInexistente:
                    return "No se pudo crear el usuario porque falta el idioma default.";

                default:
                    return "Ocurrio un error tecnico al crear el usuario.";
            }
        }
    }
}
