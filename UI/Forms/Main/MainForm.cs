using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Application;
using Domain;
using Services;

namespace UI
{
    public partial class MainForm : Form, IObserverLanguage
    {
        private readonly AutorizacionApplicationService _autorizacionService;
        private readonly UsuarioApplicationService _usuarioService;
        private bool _cargandoIdiomas;

        public MainForm()
            : this(new AutorizacionApplicationService(), new UsuarioApplicationService())
        {
        }

        public MainForm(AutorizacionApplicationService autorizacionService, UsuarioApplicationService usuarioService)
        {
            _autorizacionService = autorizacionService;
            _usuarioService = usuarioService;
            InitializeComponent();
            ConfigurarTraducciones();
            Load += MainForm_Load;
            FormClosed += MainForm_FormClosed;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Usuario usuario = Sesion.ObtenerInstancia().ObtenerUsuario();
            LanguageManager.Instance.Attach(this);
            LanguageManager.Instance.Initialize(usuario);
            CargarSelectorIdiomas();
            ActualizarUsuario();

            ConfigurarMenuPorPermisos();
            MostrarPantallaInicial();
        }

        public void OnLanguageChanged(Idioma idioma)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnLanguageChanged(idioma)));
                return;
            }

            TranslationApplier.ApplyMenu(menuStrip1);
            TranslationApplier.Apply(topPanel);
            Text = LanguageManager.Instance.Translate("MAIN_TITLE");
            ActualizarUsuario();
            SeleccionarIdiomaActual();
        }

        private void ConfigurarTraducciones()
        {
            bitacoraToolStripMenuItem.Tag = "MENU_AUDIT";
            auditoriaCambiosToolStripMenuItem.Tag = "MENU_CHANGE_AUDIT";
            usuariosToolStripMenuItem.Tag = "MENU_USERS";
            rolesToolStripMenuItem.Tag = "MENU_ROLES";
            idiomasToolStripMenuItem.Tag = "MENU_LANGUAGES";
            salirToolStripMenuItem.Tag = "MENU_LOGOUT";
            lblIdioma.Tag = "LANGUAGE_SELECTOR";
        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.BitacoraVer))
            {
                return;
            }

            ShowScreen(new BitacoraView());
        }

        private void auditoriaCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.AuditoriaCambiosVer))
            {
                return;
            }

            ShowScreen(new AuditoriaCambiosView());
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.RolVer))
            {
                return;
            }

            ShowScreen(new RolesView());
        }

        private void idiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.IdiomaVer) &&
                !_autorizacionService.TienePermiso(PermisosSistema.TraduccionVer))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_ACCESS_DENIED"));
                return;
            }

            ShowScreen(new IdiomasView());
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.UsuarioVer))
            {
                return;
            }

            ShowScreen(new UsuariosView());
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _usuarioService.RecalcularDigitosVerificadoresUsuarios();
            Sesion.ObtenerInstancia().Logout();
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void ShowScreen(UserControl screen)
        {
            contentPanel.Controls.Clear();
            screen.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(screen);
        }

        public void RefrescarIdiomasDisponibles()
        {
            CargarSelectorIdiomas();
        }

        private void CargarSelectorIdiomas()
        {
            _cargandoIdiomas = true;
            List<Idioma> idiomas = LanguageManager.Instance.ListarIdiomasActivos();
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = idiomas;
            _cargandoIdiomas = false;
            SeleccionarIdiomaActual();
        }

        private void SeleccionarIdiomaActual()
        {
            if (_cargandoIdiomas || LanguageManager.Instance.CurrentLanguage == null)
            {
                return;
            }

            cmbIdiomas.SelectedValue = LanguageManager.Instance.CurrentLanguage.Id;
        }

        private void cmbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoIdiomas)
            {
                return;
            }

            Idioma idioma = cmbIdiomas.SelectedItem as Idioma;
            Usuario usuario = Sesion.ObtenerInstancia().ObtenerUsuario();
            LanguageManager.Instance.ChangeLanguage(idioma, usuario);
        }

        private void ConfigurarMenuPorPermisos()
        {
            bitacoraToolStripMenuItem.Visible = _autorizacionService.TienePermiso(PermisosSistema.BitacoraVer);
            auditoriaCambiosToolStripMenuItem.Visible = _autorizacionService.TienePermiso(PermisosSistema.AuditoriaCambiosVer);
            usuariosToolStripMenuItem.Visible = _autorizacionService.TienePermiso(PermisosSistema.UsuarioVer);
            rolesToolStripMenuItem.Visible = _autorizacionService.TienePermiso(PermisosSistema.RolVer);
            idiomasToolStripMenuItem.Visible = _autorizacionService.TienePermiso(PermisosSistema.IdiomaVer) ||
                                               _autorizacionService.TienePermiso(PermisosSistema.TraduccionVer);
        }

        private void MostrarPantallaInicial()
        {
            if (bitacoraToolStripMenuItem.Visible)
            {
                ShowScreen(new BitacoraView());
                return;
            }

            if (auditoriaCambiosToolStripMenuItem.Visible)
            {
                ShowScreen(new AuditoriaCambiosView());
                return;
            }

            if (usuariosToolStripMenuItem.Visible)
            {
                ShowScreen(new UsuariosView());
                return;
            }

            if (rolesToolStripMenuItem.Visible)
            {
                ShowScreen(new RolesView());
                return;
            }

            if (idiomasToolStripMenuItem.Visible)
            {
                ShowScreen(new IdiomasView());
                return;
            }

            Label mensaje = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = LanguageManager.Instance.Translate("NO_PERMISSIONS_ASSIGNED")
            };

            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(mensaje);
        }

        private bool ValidarPermiso(string codigoPermiso)
        {
            if (_autorizacionService.TienePermiso(codigoPermiso))
            {
                return true;
            }

            MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_ACCESS_DENIED"));
            return false;
        }

        private void ActualizarUsuario()
        {
            Usuario usuario = Sesion.ObtenerInstancia().ObtenerUsuario();
            lblUsuario.Text = usuario != null
                ? string.Format(LanguageManager.Instance.Translate("MAIN_USER"), usuario)
                : LanguageManager.Instance.Translate("MAIN_NO_SESSION");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LanguageManager.Instance.Detach(this);
        }
    }
}
