using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Application;
using Domain;
using Services;

namespace UI
{
    public partial class RolesView : LocalizedUserControl
    {
        private readonly PermisoApplicationService _permisoService;
        private readonly AutorizacionApplicationService _autorizacionService;
        private TextBox txtCodigoRol;
        private TextBox txtNombreRol;
        private TextBox txtDescripcionRol;
        private Label lblFamiliaSeleccionada;
        private ComboBox cmbComponenteHijo;
        private Button btnCrearRol;
        private Button btnAgregarHijo;
        private Button btnQuitarHijo;

        public RolesView()
            : this(new PermisoApplicationService())
        {
        }

        public RolesView(PermisoApplicationService permisoService)
        {
            _permisoService = permisoService;
            _autorizacionService = new AutorizacionApplicationService();
            InitializeComponent();
            ConfigurarTraducciones();
            ConfigurarAdministracion();
            CargarArbol();
            CargarCombos();
            ActualizarAccionesSeleccionadas();
        }

        private void ConfigurarTraducciones()
        {
            lblTitulo.Tag = "ROLES_TITLE";
            lblDescripcion.Tag = "ROLES_DESCRIPTION";
            groupBoxRoles.Tag = "ROLES_STRUCTURE";
        }

        protected override void ApplyTranslations()
        {
            base.ApplyTranslations();
            if (lblFamiliaSeleccionada != null)
            {
                ActualizarAccionesSeleccionadas();
            }
        }

        private void CargarArbol()
        {
            treeViewRoles.Nodes.Clear();
            List<ComponentePermiso> componentes = _permisoService.ListarArbolCompleto();

            foreach (ComponentePermiso componente in componentes)
            {
                AgregarNodo(componente, treeViewRoles.Nodes);
            }

            treeViewRoles.AfterSelect -= treeViewRoles_AfterSelect;
            treeViewRoles.AfterSelect += treeViewRoles_AfterSelect;
            treeViewRoles.ExpandAll();
        }

        private void ConfigurarAdministracion()
        {
            groupBoxRoles.Location = new Point(24, 98);
            groupBoxRoles.Size = new Size(500, 390);

            GroupBox groupBoxEdicion = new GroupBox
            {
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Location = new Point(548, 98),
                Name = "groupBoxEdicionRoles",
                Size = new Size(328, 390),
                Tag = "ROLES_ADMIN",
                Text = "Administracion de roles"
            };

            Label lblCodigo = CrearLabel("Codigo", 18, 30, "ROLE_CODE");
            txtCodigoRol = CrearTextBox(18, 48);

            Label lblNombre = CrearLabel("Nombre", 18, 80, "FIELD_NAME");
            txtNombreRol = CrearTextBox(18, 98);

            Label lblDescripcionRol = CrearLabel("Descripcion", 18, 130, "GRID_DESCRIPTION");
            txtDescripcionRol = CrearTextBox(18, 148);

            btnCrearRol = CrearBoton("Crear rol", 18, 181, "BTN_CREATE_ROLE");
            btnCrearRol.Click += btnCrearRol_Click;

            Label lblRolPadre = CrearLabel("Familia seleccionada en el arbol", 18, 222, "ROLE_SELECTED_FAMILY");
            lblFamiliaSeleccionada = new Label
            {
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(18, 240),
                Size = new Size(280, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblHijo = CrearLabel("Permiso o familia a agregar", 18, 272, "ROLE_CHILD_COMPONENT");
            cmbComponenteHijo = CrearCombo(18, 290);

            btnAgregarHijo = CrearBoton("Agregar", 18, 326, "BTN_ADD");
            btnAgregarHijo.Click += btnAgregarHijo_Click;

            btnQuitarHijo = CrearBoton("Quitar seleccionado", 128, 326, "BTN_REMOVE_SELECTED");
            btnQuitarHijo.Size = new Size(170, 31);
            btnQuitarHijo.Click += btnQuitarHijo_Click;

            groupBoxEdicion.Controls.Add(lblCodigo);
            groupBoxEdicion.Controls.Add(txtCodigoRol);
            groupBoxEdicion.Controls.Add(lblNombre);
            groupBoxEdicion.Controls.Add(txtNombreRol);
            groupBoxEdicion.Controls.Add(lblDescripcionRol);
            groupBoxEdicion.Controls.Add(txtDescripcionRol);
            groupBoxEdicion.Controls.Add(btnCrearRol);
            groupBoxEdicion.Controls.Add(lblRolPadre);
            groupBoxEdicion.Controls.Add(lblFamiliaSeleccionada);
            groupBoxEdicion.Controls.Add(lblHijo);
            groupBoxEdicion.Controls.Add(cmbComponenteHijo);
            groupBoxEdicion.Controls.Add(btnAgregarHijo);
            groupBoxEdicion.Controls.Add(btnQuitarHijo);
            Controls.Add(groupBoxEdicion);

            btnCrearRol.Visible = _autorizacionService.TienePermiso(PermisosSistema.RolCrear);
            btnAgregarHijo.Visible = _autorizacionService.TienePermiso(PermisosSistema.RolEditar);
            btnQuitarHijo.Visible = _autorizacionService.TienePermiso(PermisosSistema.RolEditar);
        }

        private void CargarCombos()
        {
            List<ComponentePermiso> componentes = _permisoService.ListarComponentes();

            cmbComponenteHijo.DataSource = null;
            cmbComponenteHijo.DataSource = componentes;
        }

        private void btnCrearRol_Click(object sender, System.EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.RolCrear))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_ROLE_CREATE_DENIED"));
                return;
            }

            bool creado = _permisoService.CrearFamilia(
                txtCodigoRol.Text,
                txtNombreRol.Text,
                txtDescripcionRol.Text);

            MessageBox.Show(creado
                ? LanguageManager.Instance.Translate("ROLE_CREATED")
                : LanguageManager.Instance.Translate("ROLE_CREATE_ERROR"));

            if (creado)
            {
                txtCodigoRol.Clear();
                txtNombreRol.Clear();
                txtDescripcionRol.Clear();
                CargarArbol();
                CargarCombos();
                ActualizarAccionesSeleccionadas();
            }
        }

        private void btnAgregarHijo_Click(object sender, System.EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.RolEditar))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_ROLE_EDIT_DENIED"));
                return;
            }

            ComponentePermiso padre = ObtenerFamiliaSeleccionada();
            ComponentePermiso hijo = cmbComponenteHijo.SelectedItem as ComponentePermiso;

            if (padre == null || hijo == null)
            {
                MessageBox.Show(LanguageManager.Instance.Translate("ROLE_SELECT_FAMILY_AND_COMPONENT"));
                return;
            }

            string resultado = _permisoService.AgregarRelacion(padre.Id, hijo.Id);
            MessageBox.Show(ObtenerMensajeRelacion(resultado));
            CargarArbol();
            CargarCombos();
            ActualizarAccionesSeleccionadas();
        }

        private void btnQuitarHijo_Click(object sender, System.EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.RolEditar))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_ROLE_EDIT_DENIED"));
                return;
            }

            if (treeViewRoles.SelectedNode == null || treeViewRoles.SelectedNode.Parent == null)
            {
                MessageBox.Show(LanguageManager.Instance.Translate("ROLE_SELECT_CHILD"));
                return;
            }

            ComponentePermiso padre = treeViewRoles.SelectedNode.Parent.Tag as ComponentePermiso;
            ComponentePermiso hijo = treeViewRoles.SelectedNode.Tag as ComponentePermiso;

            if (padre == null || hijo == null)
            {
                MessageBox.Show(LanguageManager.Instance.Translate("ROLE_RELATION_IDENTIFY_ERROR"));
                return;
            }

            bool quitado = _permisoService.QuitarRelacion(padre.Id, hijo.Id);
            MessageBox.Show(quitado
                ? LanguageManager.Instance.Translate("ROLE_RELATION_REMOVED")
                : LanguageManager.Instance.Translate("ROLE_RELATION_REMOVE_ERROR"));
            CargarArbol();
            CargarCombos();
            ActualizarAccionesSeleccionadas();
        }

        private void treeViewRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ActualizarAccionesSeleccionadas();
        }

        private static void AgregarNodo(ComponentePermiso componente, TreeNodeCollection destino)
        {
            TreeNode nodo = new TreeNode(FormatearTexto(componente))
            {
                Tag = componente
            };

            destino.Add(nodo);

            foreach (ComponentePermiso hijo in componente.ObtenerHijos())
            {
                AgregarNodo(hijo, nodo.Nodes);
            }
        }

        private static string FormatearTexto(ComponentePermiso componente)
        {
            return componente.Tipo == TipoComponentePermiso.Familia
                ? "Familia: " + componente.Nombre
                : componente.Codigo + " - " + componente.Nombre;
        }

        private ComponentePermiso ObtenerFamiliaSeleccionada()
        {
            if (treeViewRoles.SelectedNode == null)
            {
                return null;
            }

            ComponentePermiso componente = treeViewRoles.SelectedNode.Tag as ComponentePermiso;

            if (componente != null && componente.Tipo == TipoComponentePermiso.Familia)
            {
                return componente;
            }

            return null;
        }

        private void ActualizarAccionesSeleccionadas()
        {
            ComponentePermiso familiaSeleccionada = ObtenerFamiliaSeleccionada();
            lblFamiliaSeleccionada.Text = familiaSeleccionada == null
                ? LanguageManager.Instance.Translate("ROLE_SELECT_FAMILY")
                : familiaSeleccionada.Nombre;

            btnAgregarHijo.Enabled = familiaSeleccionada != null &&
                                     _autorizacionService.TienePermiso(PermisosSistema.RolEditar);

            if (treeViewRoles.SelectedNode != null && treeViewRoles.SelectedNode.Parent != null)
            {
                ComponentePermiso hijo = treeViewRoles.SelectedNode.Tag as ComponentePermiso;
                ComponentePermiso padre = treeViewRoles.SelectedNode.Parent.Tag as ComponentePermiso;
                btnQuitarHijo.Text = hijo == null || padre == null
                    ? LanguageManager.Instance.Translate("BTN_REMOVE_SELECTED")
                    : string.Format(LanguageManager.Instance.Translate("BTN_REMOVE_FROM"), padre.Nombre);
                btnQuitarHijo.Enabled = _autorizacionService.TienePermiso(PermisosSistema.RolEditar);
                return;
            }

            btnQuitarHijo.Text = LanguageManager.Instance.Translate("BTN_REMOVE_SELECTED");
            btnQuitarHijo.Enabled = false;
        }

        private static Label CrearLabel(string texto, int x, int y, string tag)
        {
            return new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(x, y),
                Tag = tag,
                Text = texto
            };
        }

        private static TextBox CrearTextBox(int x, int y)
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(x, y),
                Size = new Size(280, 25)
            };
        }

        private static ComboBox CrearCombo(int x, int y)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(x, y),
                Size = new Size(280, 25)
            };
        }

        private static Button CrearBoton(string texto, int x, int y, string tag)
        {
            return new Button
            {
                BackColor = Color.FromArgb(13, 110, 253),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(x, y),
                Size = new Size(100, 31),
                Tag = tag,
                Text = texto,
                UseVisualStyleBackColor = false
            };
        }

        private static string ObtenerMensajeRelacion(string resultado)
        {
            switch (resultado)
            {
                case "OK":
                    return LanguageManager.Instance.Translate("ROLE_RELATION_ADDED");

                case "AUTO_REFERENCIA":
                    return LanguageManager.Instance.Translate("ROLE_SELF_REFERENCE_ERROR");

                case "PADRE_INVALIDO":
                    return LanguageManager.Instance.Translate("ROLE_INVALID_PARENT");

                case "HIJO_INVALIDO":
                    return LanguageManager.Instance.Translate("ROLE_INVALID_CHILD");

                case "CICLO_DETECTADO":
                    return LanguageManager.Instance.Translate("ROLE_CYCLE_ERROR");

                default:
                    return LanguageManager.Instance.Translate("ROLE_RELATION_ADD_ERROR");
            }
        }
    }
}
