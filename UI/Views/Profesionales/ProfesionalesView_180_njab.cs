using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Application;
using Domain;

namespace UI
{
    //Esta pantalla muestra la lista y permite elegir un profesional
    public class ProfesionalesView_180_njab : LocalizedUserControl
    {
        private readonly ProfesionalApplicationService_180_njab _profesionalService_180_njab;
        private readonly AutorizacionApplicationService _autorizacionService_180_njab;
        private readonly Action<UserControl> _navegar_180_njab;
        private readonly DataGridView _gridProfesionales_180_njab;
        private readonly Button _btnAsignarJornada_180_njab;
        private readonly Label _lblEstado_180_njab;
        private readonly TableLayoutPanel _layoutPrincipal_180_njab;
        private readonly Panel _grillaContenedor_180_njab;
        private readonly Label _lblSinProfesionales_180_njab;
        private List<ProfesionalConsulta_180_njab> _profesionales_180_njab;

        public ProfesionalConsulta_180_njab ProfesionalSeleccionado_180_njab { get; private set; }

        public ProfesionalesView_180_njab()
            : this(new ProfesionalApplicationService_180_njab(), null)
        {
        }

        public ProfesionalesView_180_njab(Action<UserControl> navegar_180_njab)
            : this(new ProfesionalApplicationService_180_njab(), navegar_180_njab)
        {
        }

        public ProfesionalesView_180_njab(ProfesionalApplicationService_180_njab profesionalService_180_njab)
            : this(profesionalService_180_njab, null)
        {
        }

        public ProfesionalesView_180_njab(ProfesionalApplicationService_180_njab profesionalService_180_njab, Action<UserControl> navegar_180_njab)
        {
            _profesionalService_180_njab = profesionalService_180_njab;
            _autorizacionService_180_njab = new AutorizacionApplicationService();
            _navegar_180_njab = navegar_180_njab ?? MostrarPantallaEnMismaVista_180_njab;
            BackColor = Color.FromArgb(245, 247, 250);
            Padding = new Padding(24, 20, 24, 18);

            _layoutPrincipal_180_njab = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 3,
                Dock = DockStyle.Fill,
                BackColor = BackColor,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            _layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(_layoutPrincipal_180_njab);

            Panel encabezado_180_njab = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(4, 0, 4, 14)
            };
            Label titulo_180_njab = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 21F, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 45, 56),
                Text = "Profesionales"
            };
            Label descripcion_180_njab = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Padding = new Padding(0, 5, 0, 0),
                Text = "Consultá los profesionales registrados y elegí uno para administrar sus jornadas."
            };
            encabezado_180_njab.Controls.Add(descripcion_180_njab);
            encabezado_180_njab.Controls.Add(titulo_180_njab);
            _layoutPrincipal_180_njab.Controls.Add(encabezado_180_njab, 0, 0);

            _grillaContenedor_180_njab = CrearPanelSeccion_180_njab();
            _grillaContenedor_180_njab.Padding = new Padding(1);
            _gridProfesionales_180_njab = CrearGrillaProfesionales_180_njab();
            _lblSinProfesionales_180_njab = new Label
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Text = "No hay profesionales cargados en la base de datos.",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            _grillaContenedor_180_njab.Controls.Add(_gridProfesionales_180_njab);
            _grillaContenedor_180_njab.Controls.Add(_lblSinProfesionales_180_njab);
            _layoutPrincipal_180_njab.Controls.Add(_grillaContenedor_180_njab, 0, 1);

            TableLayoutPanel pie_180_njab = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Height = 58,
                Margin = new Padding(0, 12, 0, 0),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            pie_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pie_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _lblEstado_180_njab = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Padding = new Padding(12, 0, 12, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pie_180_njab.Controls.Add(_lblEstado_180_njab, 0, 0);

            _btnAsignarJornada_180_njab = CrearBotonPrincipal_180_njab("Asignar jornada");
            _btnAsignarJornada_180_njab.Anchor = AnchorStyles.Right;
            _btnAsignarJornada_180_njab.Margin = new Padding(12, 8, 0, 8);
            _btnAsignarJornada_180_njab.Click += btnAsignarJornada_Click_180_njab;
            pie_180_njab.Controls.Add(_btnAsignarJornada_180_njab, 1, 0);
            _layoutPrincipal_180_njab.Controls.Add(pie_180_njab, 0, 2);

            ActualizarBotonAsignar_180_njab(_autorizacionService_180_njab.TienePermiso(PermisosSistema.Administrador));
            CargarProfesionales_180_njab();
        }

        private DataGridView CrearGrillaProfesionales_180_njab()
        {
            DataGridView grilla_180_njab = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeight = 38,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                Dock = DockStyle.Fill,
                EnableHeadersVisualStyles = false,
                MultiSelect = false,
                Name = "gridProfesionales_180_njab",
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 34 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            grilla_180_njab.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                SelectionBackColor = Color.FromArgb(210, 232, 239),
                SelectionForeColor = Color.FromArgb(28, 45, 56),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 10, 0)
            };
            grilla_180_njab.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 251),
                SelectionBackColor = Color.FromArgb(210, 232, 239),
                SelectionForeColor = Color.FromArgb(28, 45, 56)
            };
            grilla_180_njab.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(35, 76, 89),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 10, 0)
            };
            grilla_180_njab.Columns.Add("dni_180_njab", "DNI");
            grilla_180_njab.Columns.Add("nombre_180_njab", "Nombre");
            grilla_180_njab.Columns.Add("apellido_180_njab", "Apellido");
            grilla_180_njab.Columns.Add("matricula_180_njab", "Matrícula");
            grilla_180_njab.Columns.Add("especialidad_180_njab", "Especialidades");
            grilla_180_njab.Columns[0].FillWeight = 16F;
            grilla_180_njab.Columns[1].FillWeight = 20F;
            grilla_180_njab.Columns[2].FillWeight = 20F;
            grilla_180_njab.Columns[3].FillWeight = 17F;
            grilla_180_njab.Columns[4].FillWeight = 27F;
            grilla_180_njab.SelectionChanged += gridProfesionales_SelectionChanged_180_njab;
            return grilla_180_njab;
        }

        private Panel CrearPanelSeccion_180_njab()
        {
            return new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
        }

        private Button CrearBotonPrincipal_180_njab(string texto_180_njab)
        {
            return new Button
            {
                BackColor = Color.FromArgb(35, 112, 130),
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 38,
                MinimumSize = new Size(158, 38),
                Text = texto_180_njab,
                UseVisualStyleBackColor = false
            };
        }

        private void ActualizarBotonAsignar_180_njab(bool habilitado_180_njab)
        {
            _btnAsignarJornada_180_njab.Enabled = habilitado_180_njab;
            _btnAsignarJornada_180_njab.BackColor = habilitado_180_njab
                ? Color.FromArgb(35, 112, 130)
                : Color.FromArgb(210, 220, 224);
            _btnAsignarJornada_180_njab.ForeColor = habilitado_180_njab
                ? Color.White
                : Color.FromArgb(108, 117, 125);
        }

        private void ActualizarEstadoGrilla_180_njab(bool hayProfesionales_180_njab)
        {
            _gridProfesionales_180_njab.Visible = hayProfesionales_180_njab;
            _lblSinProfesionales_180_njab.Visible = !hayProfesionales_180_njab;
            _layoutPrincipal_180_njab.RowStyles[1] = hayProfesionales_180_njab
                ? new RowStyle(SizeType.Percent, 100F)
                : new RowStyle(SizeType.Absolute, 210F);
        }

        private void CargarProfesionales_180_njab()
        {
            try
            {
                //Carga los profesionales y los muestra en la grilla.
                _profesionales_180_njab = _profesionalService_180_njab.Listar_180_njab() ?? new List<ProfesionalConsulta_180_njab>();
                _gridProfesionales_180_njab.Rows.Clear();
                foreach (ProfesionalConsulta_180_njab profesional_180_njab in _profesionales_180_njab)
                {
                    int indiceFila_180_njab = _gridProfesionales_180_njab.Rows.Add(
                        profesional_180_njab.Dni_180_njab ?? string.Empty,
                        profesional_180_njab.Nombre_180_njab,
                        profesional_180_njab.Apellido_180_njab,
                        profesional_180_njab.Matricula_180_njab,
                        profesional_180_njab.Especialidad_180_njab);
                    _gridProfesionales_180_njab.Rows[indiceFila_180_njab].Tag = profesional_180_njab;
                }

                if (_gridProfesionales_180_njab.Rows.Count == 0)
                {
                    ProfesionalSeleccionado_180_njab = null;
                    _lblEstado_180_njab.Text = "No hay profesionales previamente cargados para mostrar.";
                    _lblSinProfesionales_180_njab.Text = "No hay profesionales previamente cargados.\r\nLa lista depende de registros existentes en la base de datos.";
                    ActualizarBotonAsignar_180_njab(false);
                    ActualizarEstadoGrilla_180_njab(false);
                    return;
                }

                ActualizarEstadoGrilla_180_njab(true);
                _gridProfesionales_180_njab.ClearSelection();
                _gridProfesionales_180_njab.CurrentCell = null;
                ProfesionalSeleccionado_180_njab = null;
                ActualizarBotonAsignar_180_njab(false);
                ActualizarSeleccion_180_njab();
                _lblEstado_180_njab.Text = "Seleccioná un profesional para continuar.";

                if (!_autorizacionService_180_njab.TienePermiso(PermisosSistema.Administrador))
                {
                    _lblEstado_180_njab.Text = "Solo un administrador puede asignar y administrar jornadas.";
                    ActualizarBotonAsignar_180_njab(false);
                }
            }
            catch (Exception ex_180_njab)
            {
                _gridProfesionales_180_njab.Rows.Clear();
                ProfesionalSeleccionado_180_njab = null;
                _lblEstado_180_njab.Text = "No se pudo consultar la lista de profesionales.";
                _lblSinProfesionales_180_njab.Text = string.Format(
                    "No se pudo consultar la base de datos.\r\nDetalle: {0}",
                    ex_180_njab.Message);
                ActualizarBotonAsignar_180_njab(false);
                ActualizarEstadoGrilla_180_njab(false);
            }
        }

        private void gridProfesionales_SelectionChanged_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            ActualizarSeleccion_180_njab();
        }

        private void ActualizarSeleccion_180_njab()
        {
            if (_gridProfesionales_180_njab.CurrentRow == null || _gridProfesionales_180_njab.CurrentRow.Index < 0)
            {
                ProfesionalSeleccionado_180_njab = null;
                ActualizarBotonAsignar_180_njab(false);
                return;
            }

            ProfesionalConsulta_180_njab profesional_180_njab =
                _gridProfesionales_180_njab.CurrentRow.Tag as ProfesionalConsulta_180_njab;
            if (profesional_180_njab == null)
            {
                ProfesionalSeleccionado_180_njab = null;
                ActualizarBotonAsignar_180_njab(false);
                return;
            }

            //Guarda el profesional que selecciono el administrador
            ProfesionalSeleccionado_180_njab = profesional_180_njab;
            ActualizarBotonAsignar_180_njab(_autorizacionService_180_njab.TienePermiso(PermisosSistema.Administrador));
        }

        private void btnAsignarJornada_Click_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            //Conserva la seleccion para continuar con la jornada mas adelante
            ActualizarSeleccion_180_njab();
            if (ProfesionalSeleccionado_180_njab == null)
            {
                return;
            }

            _lblEstado_180_njab.Text = string.Format(
                "Profesional seleccionado para continuar: {0} {1} (matrícula {2}).",
                ProfesionalSeleccionado_180_njab.Nombre_180_njab,
                ProfesionalSeleccionado_180_njab.Apellido_180_njab,
                ProfesionalSeleccionado_180_njab.Matricula_180_njab);

            _navegar_180_njab(new JornadaProfesionalView_180_njab(ProfesionalSeleccionado_180_njab, _navegar_180_njab));
        }

        private void MostrarPantallaEnMismaVista_180_njab(UserControl pantalla_180_njab)
        {
            Controls.Clear();
            pantalla_180_njab.Dock = DockStyle.Fill;
            Controls.Add(pantalla_180_njab);
        }
    }
}
