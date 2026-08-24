using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Application;
using Domain;
using Services;

namespace UI
{
    public class IdiomasView : LocalizedUserControl
    {
        private readonly IdiomaApplicationService _idiomaService;
        private readonly AutorizacionApplicationService _autorizacionService;
        private readonly Label lblTitulo = new Label();
        private readonly Label lblDescripcion = new Label();
        private readonly DataGridView dgvIdiomas = new DataGridView();
        private readonly TreeView tvComponentes = new TreeView();
        private readonly GroupBox groupIdioma = new GroupBox();
        private readonly GroupBox groupTraduccion = new GroupBox();
        private readonly Label lblComponentes = new Label();
        private readonly TextBox txtComponenteSeleccionado = new TextBox();
        private readonly TextBox txtCodigo = new TextBox();
        private readonly TextBox txtNombre = new TextBox();
        private readonly CheckBox chkActivo = new CheckBox();
        private readonly Button btnNuevoIdioma = new Button();
        private readonly Button btnGuardarIdioma = new Button();
        private readonly TextBox txtKey = new TextBox();
        private readonly TextBox txtDescripcionEtiqueta = new TextBox();
        private readonly ComboBox cmbIdiomas = new ComboBox();
        private readonly TextBox txtTraduccion = new TextBox();
        private readonly Button btnGuardarTraduccion = new Button();
        private readonly Button btnRefrescarComponentes = new Button();

        private List<Idioma> _idiomas = new List<Idioma>();
        private List<Etiqueta> _etiquetas = new List<Etiqueta>();
        private Idioma _idiomaSeleccionado;
        private bool _cargandoDatos;

        public IdiomasView()
            : this(new IdiomaApplicationService())
        {
        }

        public IdiomasView(IdiomaApplicationService idiomaService)
        {
            _idiomaService = idiomaService;
            _autorizacionService = new AutorizacionApplicationService();
            ConstruirVista();
            ConfigurarTraducciones();
            ConfigurarPermisos();
            CargarDatos();
            PrepararNuevoIdioma();
            CargarArbolComponentes();
            Load += IdiomasView_Load;
        }

        private void ConstruirVista()
        {
            BackColor = Color.White;
            Size = new Size(960, 620);

            lblTitulo.SetBounds(18, 18, 300, 36);
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblDescripcion.SetBounds(20, 58, 650, 24);
            lblDescripcion.Font = new Font("Segoe UI", 10F);
            lblDescripcion.ForeColor = Color.FromArgb(108, 117, 125);

            dgvIdiomas.SetBounds(24, 98, 300, 170);
            dgvIdiomas.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            dgvIdiomas.AutoGenerateColumns = false;
            dgvIdiomas.AllowUserToAddRows = false;
            dgvIdiomas.AllowUserToDeleteRows = false;
            dgvIdiomas.ReadOnly = true;
            dgvIdiomas.MultiSelect = false;
            dgvIdiomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvIdiomas.RowHeadersVisible = false;
            dgvIdiomas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id", Width = 55, Tag = "GRID_ID" });
            dgvIdiomas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Codigo", HeaderText = "Codigo", Width = 90, Tag = "LANGUAGE_CODE" });
            dgvIdiomas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, Tag = "FIELD_NAME" });
            dgvIdiomas.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Activo", HeaderText = "Activo", Width = 70, Tag = "LANGUAGE_ACTIVE" });
            dgvIdiomas.SelectionChanged += dgvIdiomas_SelectionChanged;

            lblComponentes.SetBounds(24, 282, 178, 22);
            lblComponentes.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnRefrescarComponentes.SetBounds(212, 278, 112, 28);
            btnRefrescarComponentes.Click += btnRefrescarComponentes_Click;

            tvComponentes.SetBounds(24, 312, 300, 260);
            tvComponentes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tvComponentes.HideSelection = false;
            tvComponentes.AfterSelect += tvComponentes_AfterSelect;

            groupIdioma.SetBounds(340, 98, 520, 150);
            groupIdioma.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupIdioma.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            Label lblCodigo = CrearLabel("Codigo", 16, 30, "LANGUAGE_CODE");
            Label lblNombre = CrearLabel("Nombre", 150, 30, "FIELD_NAME");
            txtCodigo.SetBounds(16, 50, 120, 25);
            txtNombre.SetBounds(150, 50, 250, 25);
            chkActivo.SetBounds(16, 84, 120, 25);
            btnNuevoIdioma.SetBounds(230, 104, 82, 30);
            btnGuardarIdioma.SetBounds(318, 104, 82, 30);
            btnNuevoIdioma.Click += btnNuevoIdioma_Click;
            btnGuardarIdioma.Click += btnGuardarIdioma_Click;
            groupIdioma.Controls.AddRange(new Control[] { lblCodigo, lblNombre, txtCodigo, txtNombre, chkActivo, btnNuevoIdioma, btnGuardarIdioma });

            groupTraduccion.SetBounds(340, 258, 520, 314);
            groupTraduccion.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupTraduccion.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            txtComponenteSeleccionado.SetBounds(16, 44, 484, 25);
            txtComponenteSeleccionado.ReadOnly = true;
            txtKey.SetBounds(16, 92, 210, 25);
            txtKey.ReadOnly = true;
            txtDescripcionEtiqueta.SetBounds(236, 92, 264, 25);
            txtDescripcionEtiqueta.ReadOnly = true;
            cmbIdiomas.SetBounds(16, 140, 210, 25);
            txtTraduccion.SetBounds(16, 190, 360, 25);
            btnGuardarTraduccion.SetBounds(386, 188, 114, 29);
            cmbIdiomas.SelectedIndexChanged += cmbIdiomas_SelectedIndexChanged;
            btnGuardarTraduccion.Click += btnGuardarTraduccion_Click;
            groupTraduccion.Controls.AddRange(new Control[]
            {
                CrearLabel("Componente", 16, 24, "COMPONENT_SELECTED"),
                txtComponenteSeleccionado,
                CrearLabel("Etiqueta", 16, 72, "LABEL_TAG"),
                CrearLabel("Descripcion", 236, 72, "GRID_DESCRIPTION"),
                txtKey,
                txtDescripcionEtiqueta,
                CrearLabel("Idioma", 16, 120, "LANGUAGE_SELECTOR"),
                CrearLabel("Traduccion", 16, 170, "TRANSLATION_TEXT"),
                cmbIdiomas,
                txtTraduccion,
                btnGuardarTraduccion
            });

            Controls.AddRange(new Control[]
            {
                lblTitulo,
                lblDescripcion,
                dgvIdiomas,
                lblComponentes,
                btnRefrescarComponentes,
                tvComponentes,
                groupIdioma,
                groupTraduccion
            });

            Resize += IdiomasView_Resize;
            AjustarLayout();
        }

        private Label CrearLabel(string texto, int x, int y, string tag)
        {
            return new Label
            {
                Text = texto,
                Tag = tag,
                Location = new Point(x, y),
                Size = new Size(125, 18),
                Font = new Font("Segoe UI", 9F)
            };
        }

        private void ConfigurarTraducciones()
        {
            lblTitulo.Tag = "LANGUAGES_TITLE";
            lblDescripcion.Tag = "LANGUAGES_DESCRIPTION";
            groupIdioma.Tag = "LANGUAGE_DETAIL";
            groupTraduccion.Tag = "TRANSLATION_DETAIL";
            lblComponentes.Tag = "COMPONENT_TREE";
            chkActivo.Tag = "LANGUAGE_ACTIVE";
            btnNuevoIdioma.Tag = "BTN_NEW";
            btnGuardarIdioma.Tag = "BTN_SAVE";
            btnGuardarTraduccion.Tag = "BTN_SAVE";
            btnRefrescarComponentes.Tag = "BTN_REFRESH";
        }

        private void CargarDatos()
        {
            _cargandoDatos = true;
            _idiomas = _idiomaService.ListarIdiomas(false);
            _etiquetas = _idiomaService.ListarEtiquetas();

            dgvIdiomas.DataSource = null;
            dgvIdiomas.DataSource = _idiomas;

            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = new List<Idioma>(_idiomas);
            _cargandoDatos = false;
        }

        private void PrepararNuevoIdioma()
        {
            _idiomaSeleccionado = null;
            txtCodigo.Clear();
            txtNombre.Clear();
            chkActivo.Checked = true;
        }

        private void dgvIdiomas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvIdiomas.CurrentRow == null)
            {
                return;
            }

            _idiomaSeleccionado = dgvIdiomas.CurrentRow.DataBoundItem as Idioma;
            if (_idiomaSeleccionado == null)
            {
                return;
            }

            txtCodigo.Text = _idiomaSeleccionado.Codigo;
            txtNombre.Text = _idiomaSeleccionado.Nombre;
            chkActivo.Checked = _idiomaSeleccionado.Activo;
        }

        private void btnNuevoIdioma_Click(object sender, EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.IdiomaCrear))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_LANGUAGE_CREATE_DENIED"));
                return;
            }

            PrepararNuevoIdioma();
        }

        private void btnGuardarIdioma_Click(object sender, EventArgs e)
        {
            if (_idiomaSeleccionado == null &&
                !_autorizacionService.TienePermiso(PermisosSistema.IdiomaCrear))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_LANGUAGE_CREATE_DENIED"));
                return;
            }

            if (_idiomaSeleccionado != null &&
                !_autorizacionService.TienePermiso(PermisosSistema.IdiomaEditar))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_LANGUAGE_EDIT_DENIED"));
                return;
            }

            Idioma idioma = _idiomaSeleccionado ?? new Idioma();
            idioma.Codigo = txtCodigo.Text;
            idioma.Nombre = txtNombre.Text;
            idioma.Activo = chkActivo.Checked;

            Usuario usuario = Sesion.ObtenerInstancia().ObtenerUsuario();
            bool resultado = _idiomaService.GuardarIdioma(
                idioma,
                usuario == null ? (int?)null : usuario.Id,
                "Cambio de activacion desde administracion de idiomas.");

            MessageBox.Show(resultado
                ? LanguageManager.Instance.Translate("LANGUAGE_SAVED")
                : LanguageManager.Instance.Translate("SAVE_ERROR"));

            CargarDatos();
            CargarSelectorPrincipalSiCorresponde();
            CargarArbolComponentes();
        }

        private void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            if (!_autorizacionService.TienePermiso(PermisosSistema.TraduccionEditar))
            {
                MessageBox.Show(LanguageManager.Instance.Translate("SECURITY_TRANSLATION_EDIT_DENIED"));
                return;
            }

            Idioma idioma = cmbIdiomas.SelectedItem as Idioma;

            bool resultado = _idiomaService.GuardarTraduccionDetectada(
                txtKey.Text,
                txtDescripcionEtiqueta.Text,
                idioma == null ? 0 : idioma.Id,
                txtTraduccion.Text);

            MessageBox.Show(resultado
                ? LanguageManager.Instance.Translate("TRANSLATION_SAVED")
                : LanguageManager.Instance.Translate("SAVE_ERROR"));

            if (resultado && idioma != null && LanguageManager.Instance.CurrentLanguage != null &&
                idioma.Id == LanguageManager.Instance.CurrentLanguage.Id)
            {
                LanguageManager.Instance.Notify();
            }

            string key = txtKey.Text;
            txtTraduccion.Clear();
            CargarDatos();
            SeleccionarEtiquetaPorClave(key);
            CargarTraduccionSeleccionada();
        }

        private void ConfigurarPermisos()
        {
            bool puedeVerIdiomas = _autorizacionService.TienePermiso(PermisosSistema.IdiomaVer);
            bool puedeCrearIdiomas = _autorizacionService.TienePermiso(PermisosSistema.IdiomaCrear);
            bool puedeEditarIdiomas = _autorizacionService.TienePermiso(PermisosSistema.IdiomaEditar);
            bool puedeVerTraducciones = _autorizacionService.TienePermiso(PermisosSistema.TraduccionVer);
            bool puedeEditarTraducciones = _autorizacionService.TienePermiso(PermisosSistema.TraduccionEditar);

            dgvIdiomas.Visible = puedeVerIdiomas;
            groupIdioma.Visible = puedeVerIdiomas;
            lblComponentes.Visible = puedeVerTraducciones;
            btnRefrescarComponentes.Visible = puedeVerTraducciones;
            tvComponentes.Visible = puedeVerTraducciones;
            groupTraduccion.Visible = puedeVerTraducciones;

            btnNuevoIdioma.Visible = puedeCrearIdiomas;
            btnGuardarIdioma.Visible = puedeCrearIdiomas || puedeEditarIdiomas;
            btnGuardarTraduccion.Visible = puedeEditarTraducciones;
            txtTraduccion.ReadOnly = !puedeEditarTraducciones;
        }

        private void btnRefrescarComponentes_Click(object sender, EventArgs e)
        {
            CargarArbolComponentes();
        }

        private void IdiomasView_Resize(object sender, EventArgs e)
        {
            AjustarLayout();
        }

        private void IdiomasView_Load(object sender, EventArgs e)
        {
            CargarArbolComponentes();
        }

        private void cmbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTraduccionSeleccionada();
        }

        private void tvComponentes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UiComponentInfo info = e.Node == null ? null : e.Node.Tag as UiComponentInfo;
            if (info == null)
            {
                return;
            }

            txtComponenteSeleccionado.Text = info.Path;

            if (!string.IsNullOrWhiteSpace(info.Key))
            {
                SeleccionarEtiquetaPorClave(info.Key);
                return;
            }

            txtKey.Clear();
            txtDescripcionEtiqueta.Text = "Componente sin etiqueta de UI. Asignar Tag en la pantalla para traducirlo.";
            txtTraduccion.Clear();
            CargarTraduccionSeleccionada();
        }

        private void CargarTraduccionSeleccionada()
        {
            if (_cargandoDatos)
            {
                return;
            }

            Idioma idioma = cmbIdiomas.SelectedItem as Idioma;
            Etiqueta etiqueta = ObtenerEtiquetaSeleccionada();
            if (etiqueta == null || idioma == null)
            {
                txtTraduccion.Clear();
                return;
            }

            Traduccion traduccion = _idiomaService.ObtenerTraduccion(etiqueta.Id, idioma.Id);
            txtTraduccion.Text = traduccion == null ? string.Empty : traduccion.Texto;
        }

        private void SeleccionarEtiquetaPorClave(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            foreach (Etiqueta etiqueta in _etiquetas)
            {
                if (string.Equals(etiqueta.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    txtKey.Text = etiqueta.Key;
                    txtDescripcionEtiqueta.Text = etiqueta.Descripcion;
                    CargarTraduccionSeleccionada();
                    return;
                }
            }

            txtKey.Text = key;
            txtDescripcionEtiqueta.Text = "Etiqueta detectada desde componente visual.";
            txtTraduccion.Clear();
        }

        private void CargarArbolComponentes()
        {
            tvComponentes.BeginUpdate();
            tvComponentes.Nodes.Clear();

            Form form = FindForm();
            if (form != null)
            {
                TreeNode formNode = CrearNodoRaiz("Formulario: " + NombreVisible(form));
                AgregarControles(form.Controls, formNode, NombreVisible(form));

                if (form.MainMenuStrip != null)
                {
                    TreeNode menuNode = CrearNodoRaiz("Menu principal");
                    foreach (ToolStripItem item in form.MainMenuStrip.Items)
                    {
                        AgregarToolStripItem(item, menuNode, "Menu principal");
                    }

                    formNode.Nodes.Insert(0, menuNode);
                }

                tvComponentes.Nodes.Add(formNode);
            }
            else
            {
                TreeNode viewNode = CrearNodoRaiz("Vista: " + GetType().Name);
                AgregarControles(Controls, viewNode, GetType().Name);
                tvComponentes.Nodes.Add(viewNode);
            }

            tvComponentes.Nodes.Add(CrearNodoCatalogoEtiquetas());
            tvComponentes.ExpandAll();
            tvComponentes.EndUpdate();
        }

        private TreeNode CrearNodoCatalogoEtiquetas()
        {
            TreeNode catalogo = CrearNodoRaiz("Etiquetas disponibles");

            AgregarSeccionCatalogo(catalogo, "Menu", new[]
            {
                new UiCatalogItem("MAIN_TITLE", "Titulo de la ventana principal"),
                new UiCatalogItem("MAIN_USER", "Texto de usuario autenticado"),
                new UiCatalogItem("MAIN_NO_SESSION", "Texto sin sesion"),
                new UiCatalogItem("MENU_AUDIT", "Menu bitacora"),
                new UiCatalogItem("MENU_USERS", "Menu usuarios"),
                new UiCatalogItem("MENU_ROLES", "Menu roles"),
                new UiCatalogItem("MENU_LANGUAGES", "Menu idiomas"),
                new UiCatalogItem("MENU_LOGOUT", "Menu salir"),
                new UiCatalogItem("LANGUAGE_SELECTOR", "Selector de idioma"),
                new UiCatalogItem("SECURITY_ACCESS_DENIED", "Acceso denegado"),
                new UiCatalogItem("NO_PERMISSIONS_ASSIGNED", "Sin permisos asignados")
            });

            AgregarSeccionCatalogo(catalogo, "Bitacora", new[]
            {
                new UiCatalogItem("AUDIT_TITLE", "Titulo de bitacora"),
                new UiCatalogItem("AUDIT_DESCRIPTION", "Descripcion de bitacora"),
                new UiCatalogItem("AUDIT_EMPTY", "Mensaje sin eventos"),
                new UiCatalogItem("AUDIT_COUNT", "Cantidad de eventos registrados"),
                new UiCatalogItem("BTN_REFRESH", "Boton actualizar"),
                new UiCatalogItem("GRID_ID", "Columna id"),
                new UiCatalogItem("GRID_DATE", "Columna fecha"),
                new UiCatalogItem("GRID_USER_ID", "Columna id usuario"),
                new UiCatalogItem("GRID_USER", "Columna usuario"),
                new UiCatalogItem("GRID_MODULE", "Columna modulo"),
                new UiCatalogItem("GRID_ACTION", "Columna accion"),
                new UiCatalogItem("GRID_LEVEL", "Columna nivel"),
                new UiCatalogItem("GRID_DESCRIPTION", "Columna descripcion"),
                new UiCatalogItem("GRID_DEVICE", "Columna equipo")
            });

            AgregarSeccionCatalogo(catalogo, "Usuarios", new[]
            {
                new UiCatalogItem("USERS_TITLE", "Titulo de usuarios"),
                new UiCatalogItem("USERS_DESCRIPTION", "Descripcion de usuarios"),
                new UiCatalogItem("USERS_DETAIL", "Detalle de usuario"),
                new UiCatalogItem("USERS_CREATE_MODE", "Modo crear usuario"),
                new UiCatalogItem("USERS_EDIT_MODE", "Modo modificar usuario"),
                new UiCatalogItem("FIELD_USER", "Campo usuario"),
                new UiCatalogItem("FIELD_EMAIL", "Campo email"),
                new UiCatalogItem("FIELD_NAME", "Campo nombre"),
                new UiCatalogItem("FIELD_LASTNAME", "Campo apellido"),
                new UiCatalogItem("FIELD_NEW_PASSWORD", "Campo contrasena nueva"),
                new UiCatalogItem("FIELD_STATUS", "Campo estado"),
                new UiCatalogItem("GRID_ID", "Columna id"),
                new UiCatalogItem("GRID_USER", "Columna usuario"),
                new UiCatalogItem("GRID_EMAIL", "Columna email"),
                new UiCatalogItem("GRID_STATUS", "Columna estado"),
                new UiCatalogItem("GRID_DV_BLOCK", "Columna bloqueo digito verificador"),
                new UiCatalogItem("BTN_NEW", "Boton nuevo"),
                new UiCatalogItem("BTN_CREATE", "Boton crear"),
                new UiCatalogItem("BTN_SAVE", "Boton guardar"),
                new UiCatalogItem("BTN_DISABLE", "Boton inhabilitar"),
                new UiCatalogItem("BTN_RECALCULATE_DV", "Boton recalcular digitos verificadores"),
                new UiCatalogItem("USER_ROLES", "Grupo roles de usuario"),
                new UiCatalogItem("USER_ROLE_SELECT_HELP", "Ayuda para seleccionar usuario"),
                new UiCatalogItem("USER_ROLE_SELECT_ONE", "Ayuda para seleccionar rol"),
                new UiCatalogItem("USER_ROLE_EMPTY", "Sin roles disponibles"),
                new UiCatalogItem("USER_ROLE_EDIT_DENIED", "Permiso denegado para roles de usuario")
            });

            AgregarSeccionCatalogo(catalogo, "Roles", new[]
            {
                new UiCatalogItem("ROLES_TITLE", "Titulo de roles"),
                new UiCatalogItem("ROLES_DESCRIPTION", "Descripcion de roles"),
                new UiCatalogItem("ROLES_STRUCTURE", "Estructura de roles"),
                new UiCatalogItem("ROLES_ADMIN", "Administracion de roles"),
                new UiCatalogItem("ROLE_CODE", "Campo codigo de rol"),
                new UiCatalogItem("ROLE_SELECTED_FAMILY", "Familia seleccionada"),
                new UiCatalogItem("ROLE_CHILD_COMPONENT", "Permiso o familia a agregar"),
                new UiCatalogItem("ROLE_SELECT_FAMILY", "Seleccionar familia"),
                new UiCatalogItem("ROLE_SELECT_CHILD", "Seleccionar permiso hijo"),
                new UiCatalogItem("ROLE_SELECT_FAMILY_AND_COMPONENT", "Seleccionar familia y componente"),
                new UiCatalogItem("ROLE_CREATED", "Rol creado"),
                new UiCatalogItem("ROLE_CREATE_ERROR", "Error al crear rol"),
                new UiCatalogItem("ROLE_RELATION_ADDED", "Relacion agregada"),
                new UiCatalogItem("ROLE_RELATION_ADD_ERROR", "Error al agregar relacion"),
                new UiCatalogItem("ROLE_RELATION_REMOVED", "Relacion quitada"),
                new UiCatalogItem("ROLE_RELATION_REMOVE_ERROR", "Error al quitar relacion"),
                new UiCatalogItem("ROLE_RELATION_IDENTIFY_ERROR", "Error al identificar relacion"),
                new UiCatalogItem("ROLE_SELF_REFERENCE_ERROR", "Error por autoreferencia"),
                new UiCatalogItem("ROLE_INVALID_PARENT", "Padre invalido"),
                new UiCatalogItem("ROLE_INVALID_CHILD", "Hijo invalido"),
                new UiCatalogItem("ROLE_CYCLE_ERROR", "Ciclo detectado"),
                new UiCatalogItem("BTN_CREATE_ROLE", "Boton crear rol"),
                new UiCatalogItem("BTN_ADD", "Boton agregar"),
                new UiCatalogItem("BTN_REMOVE_SELECTED", "Boton quitar seleccionado"),
                new UiCatalogItem("BTN_REMOVE_FROM", "Boton quitar desde familia"),
                new UiCatalogItem("SECURITY_ROLE_CREATE_DENIED", "Permiso denegado para crear roles"),
                new UiCatalogItem("SECURITY_ROLE_EDIT_DENIED", "Permiso denegado para modificar roles")
            });

            return catalogo;
        }

        private void AgregarSeccionCatalogo(TreeNode parentNode, string nombre, IEnumerable<UiCatalogItem> items)
        {
            TreeNode seccion = CrearNodoRaiz(nombre);

            foreach (UiCatalogItem item in items)
            {
                seccion.Nodes.Add(CrearNodoComponente(
                    "Etiqueta",
                    item.Key,
                    item.Description,
                    item.Key,
                    nombre + "/" + item.Key));
            }

            parentNode.Nodes.Add(seccion);
        }

        private TreeNode CrearNodoRaiz(string texto)
        {
            return new TreeNode(texto)
            {
                Tag = new UiComponentInfo
                {
                    Path = texto,
                    SuggestedKey = NormalizarClave(texto)
                }
            };
        }

        private void AgregarControles(Control.ControlCollection controls, TreeNode parentNode, string parentPath)
        {
            foreach (Control control in controls)
            {
                string path = parentPath + "/" + NombreVisible(control);
                TreeNode node = CrearNodoComponente(control.GetType().Name, NombreVisible(control), control.Text, control.Tag as string, path);
                parentNode.Nodes.Add(node);

                DataGridView dataGridView = control as DataGridView;
                if (dataGridView != null)
                {
                    AgregarColumnas(dataGridView, node, path);
                }

                if (control.Controls.Count > 0)
                {
                    AgregarControles(control.Controls, node, path);
                }
            }
        }

        private void AgregarColumnas(DataGridView dataGridView, TreeNode parentNode, string parentPath)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                string path = parentPath + "/Column:" + NombreVisible(column);
                parentNode.Nodes.Add(CrearNodoComponente(
                    "DataGridViewColumn",
                    NombreVisible(column),
                    column.HeaderText,
                    column.Tag as string,
                    path));
            }
        }

        private void AgregarToolStripItem(ToolStripItem item, TreeNode parentNode, string parentPath)
        {
            string path = parentPath + "/" + NombreVisible(item);
            TreeNode node = CrearNodoComponente(item.GetType().Name, NombreVisible(item), item.Text, item.Tag as string, path);
            parentNode.Nodes.Add(node);

            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem == null)
            {
                return;
            }

            foreach (ToolStripItem child in dropDownItem.DropDownItems)
            {
                AgregarToolStripItem(child, node, path);
            }
        }

        private TreeNode CrearNodoComponente(string tipo, string nombre, string texto, string key, string path)
        {
            string detalleClave = string.IsNullOrWhiteSpace(key) ? "sin etiqueta" : key;
            string detalleTexto = string.IsNullOrWhiteSpace(texto) ? string.Empty : " - " + texto;
            return new TreeNode(tipo + ": " + nombre + " [" + detalleClave + "]" + detalleTexto)
            {
                Tag = new UiComponentInfo
                {
                    Key = key,
                    Path = path,
                    SuggestedKey = NormalizarClave(path),
                    CurrentText = texto
                }
            };
        }

        private static string NombreVisible(Control control)
        {
            if (!string.IsNullOrWhiteSpace(control.Name))
            {
                return control.Name;
            }

            if (!string.IsNullOrWhiteSpace(control.Text))
            {
                return control.Text;
            }

            return control.GetType().Name;
        }

        private static string NombreVisible(DataGridViewColumn column)
        {
            if (!string.IsNullOrWhiteSpace(column.Name))
            {
                return column.Name;
            }

            if (!string.IsNullOrWhiteSpace(column.HeaderText))
            {
                return column.HeaderText;
            }

            return "Column";
        }

        private static string NombreVisible(ToolStripItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                return item.Name;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                return item.Text;
            }

            return item.GetType().Name;
        }

        private static string NormalizarClave(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "UI_COMPONENT";
            }

            StringBuilder builder = new StringBuilder();
            foreach (char c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(char.ToUpperInvariant(c));
                }
                else if (builder.Length > 0 && builder[builder.Length - 1] != '_')
                {
                    builder.Append('_');
                }
            }

            return builder.ToString().Trim('_');
        }

        private void CargarSelectorPrincipalSiCorresponde()
        {
            Form form = FindForm();
            MainForm mainForm = form as MainForm;
            if (mainForm != null)
            {
                mainForm.RefrescarIdiomasDisponibles();
            }
        }

        private void AjustarLayout()
        {
            int margen = 24;
            int anchoDisponible = Math.Max(900, ClientSize.Width);
            int altoDisponible = Math.Max(580, ClientSize.Height);
            int anchoIzquierdo = Math.Min(620, Math.Max(420, (int)(anchoDisponible * 0.40)));
            int xDerecha = margen + anchoIzquierdo + 24;
            int anchoDerecha = Math.Max(430, anchoDisponible - xDerecha - margen);

            lblTitulo.SetBounds(margen, 18, anchoDisponible - (margen * 2), 36);
            lblDescripcion.SetBounds(margen + 2, 58, anchoDisponible - (margen * 2), 24);

            dgvIdiomas.SetBounds(margen, 98, anchoIzquierdo, 170);
            lblComponentes.SetBounds(margen, 282, anchoIzquierdo - 130, 22);
            btnRefrescarComponentes.SetBounds(margen + anchoIzquierdo - 112, 278, 112, 28);
            tvComponentes.SetBounds(margen, 312, anchoIzquierdo, Math.Max(230, altoDisponible - 336));

            groupIdioma.SetBounds(xDerecha, 98, anchoDerecha, 150);
            groupTraduccion.SetBounds(xDerecha, 258, anchoDerecha, Math.Max(314, altoDisponible - 282));

            txtNombre.Width = Math.Max(250, groupIdioma.Width - txtNombre.Left - 24);
            btnGuardarIdioma.Left = groupIdioma.Width - btnGuardarIdioma.Width - 24;
            btnNuevoIdioma.Left = btnGuardarIdioma.Left - btnNuevoIdioma.Width - 8;

            txtComponenteSeleccionado.Width = Math.Max(300, groupTraduccion.Width - 32);
            txtDescripcionEtiqueta.Width = Math.Max(240, groupTraduccion.Width - txtDescripcionEtiqueta.Left - 16);
            cmbIdiomas.Width = Math.Max(210, (groupTraduccion.Width - 32) / 2);
            txtTraduccion.Width = Math.Max(300, groupTraduccion.Width - btnGuardarTraduccion.Width - 48);
            btnGuardarTraduccion.Left = txtTraduccion.Right + 16;
        }

        private Etiqueta ObtenerEtiquetaSeleccionada()
        {
            foreach (Etiqueta etiqueta in _etiquetas)
            {
                if (string.Equals(etiqueta.Key, txtKey.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return etiqueta;
                }
            }

            return null;
        }

        private class UiComponentInfo
        {
            public string Key { get; set; }
            public string SuggestedKey { get; set; }
            public string Path { get; set; }
            public string CurrentText { get; set; }
        }

        private class UiCatalogItem
        {
            public UiCatalogItem(string key, string description)
            {
                Key = key;
                Description = description;
            }

            public string Key { get; private set; }
            public string Description { get; private set; }
        }
    }
}
