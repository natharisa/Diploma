using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Application;
using Domain;

namespace UI
{
    public class JornadaProfesionalView_180_njab : UserControl
    {
        private readonly ProfesionalConsulta_180_njab _profesional_180_njab;
        private readonly JornadaApplicationService_180_njab _jornadaService_180_njab;
        private readonly Action<UserControl> _navegar_180_njab;
        private readonly ComboBox _cmbDia_180_njab;
        private readonly DateTimePicker _dtpFecha_180_njab;
        private readonly ComboBox _cmbFranja_180_njab;
        private readonly NumericUpDown _nudDuracion_180_njab;
        private readonly ComboBox _cmbConsultorio_180_njab;
        private readonly DataGridView _gridJornadas_180_njab;
        private readonly Button _btnGuardar_180_njab;
        private readonly Button _btnCancelarEdicion_180_njab;
        private readonly Button _btnModificar_180_njab;
        private readonly Button _btnQuitar_180_njab;
        private readonly Button _btnVolver_180_njab;
        private readonly Label _lblEstado_180_njab;
        private JornadaProfesionalConsulta_180_njab _jornadaSeleccionada_180_njab;
        private bool _cargando_180_njab;
        private bool _modoEdicion_180_njab;

        public JornadaProfesionalView_180_njab(ProfesionalConsulta_180_njab profesional_180_njab, Action<UserControl> navegar_180_njab)
            : this(profesional_180_njab, navegar_180_njab, new JornadaApplicationService_180_njab())
        {
        }

        public JornadaProfesionalView_180_njab(ProfesionalConsulta_180_njab profesional_180_njab, Action<UserControl> navegar_180_njab, JornadaApplicationService_180_njab jornadaService_180_njab)
        {
            _profesional_180_njab = profesional_180_njab;
            _navegar_180_njab = navegar_180_njab;
            _jornadaService_180_njab = jornadaService_180_njab;
            BackColor = Color.FromArgb(245, 247, 250);
            Padding = new Padding(24, 20, 24, 18);
            Dock = DockStyle.Fill;

            TableLayoutPanel layoutPrincipal_180_njab = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 5,
                Dock = DockStyle.Fill,
                BackColor = BackColor,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPrincipal_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(layoutPrincipal_180_njab);

            TableLayoutPanel encabezado_180_njab = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Height = 74,
                Margin = new Padding(0, 0, 0, 14),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            encabezado_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            encabezado_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            FlowLayoutPanel textosEncabezado_180_njab = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };
            Label titulo_180_njab = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 21F, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 45, 56),
                Text = "Jornadas del profesional"
            };
            Label datosProfesional_180_njab = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(0, 5, 0, 0),
                Text = string.Format("{0} {1}  ·  DNI: {2}  ·  Matrícula: {3}",
                    _profesional_180_njab.Nombre_180_njab,
                    _profesional_180_njab.Apellido_180_njab,
                    _profesional_180_njab.Dni_180_njab ?? "Sin informar",
                    _profesional_180_njab.Matricula_180_njab)
            };
            textosEncabezado_180_njab.Controls.Add(titulo_180_njab);
            textosEncabezado_180_njab.Controls.Add(datosProfesional_180_njab);
            encabezado_180_njab.Controls.Add(textosEncabezado_180_njab, 0, 0);

            _btnVolver_180_njab = CrearBotonSecundario_180_njab("Volver", 110);
            _btnVolver_180_njab.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnVolver_180_njab.Margin = new Padding(16, 6, 0, 0);
            _btnVolver_180_njab.Click += Volver_180_njab;
            encabezado_180_njab.Controls.Add(_btnVolver_180_njab, 1, 0);
            layoutPrincipal_180_njab.Controls.Add(encabezado_180_njab, 0, 0);

            GroupBox formulario_180_njab = new GroupBox
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 76, 89),
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(14, 12, 14, 10),
                Text = "Asignar o modificar jornada"
            };
            TableLayoutPanel campos_180_njab = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            campos_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            campos_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            campos_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            campos_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            campos_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            campos_180_njab.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _cmbDia_180_njab = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top, Height = 28 };
            _cmbDia_180_njab.Items.AddRange(new object[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" });
            _cmbDia_180_njab.SelectedIndexChanged += SeleccionDeConfiguracionCambiada_180_njab;
            campos_180_njab.Controls.Add(CrearCampo_180_njab("Día de atención", _cmbDia_180_njab), 0, 0);

            _dtpFecha_180_njab = new DateTimePicker { Dock = DockStyle.Top, Format = DateTimePickerFormat.Short, Height = 28 };
            _dtpFecha_180_njab.ValueChanged += SeleccionDeConfiguracionCambiada_180_njab;
            campos_180_njab.Controls.Add(CrearCampo_180_njab("Fecha", _dtpFecha_180_njab), 1, 0);

            _cmbFranja_180_njab = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top, Height = 28, DisplayMember = "Descripcion_180_njab" };
            _cmbFranja_180_njab.DataSource = JornadaReglas_180_njab.ListarFranjas_180_njab();
            _cmbFranja_180_njab.SelectedIndexChanged += SeleccionDeConfiguracionCambiada_180_njab;
            campos_180_njab.Controls.Add(CrearCampo_180_njab("Franja horaria", _cmbFranja_180_njab), 2, 0);

            _nudDuracion_180_njab = new NumericUpDown { Dock = DockStyle.Top, Height = 28, Minimum = 1, Maximum = 240, Value = 30 };
            campos_180_njab.Controls.Add(CrearCampo_180_njab("Duración de los turnos (minutos)", _nudDuracion_180_njab), 0, 1);

            _cmbConsultorio_180_njab = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top, Height = 28, DisplayMember = "Nombre_180_njab", ValueMember = "IdConsultorio_180_njab" };
            campos_180_njab.Controls.Add(CrearCampo_180_njab("Consultorio disponible", _cmbConsultorio_180_njab), 1, 1);

            FlowLayoutPanel accionesFormulario_180_njab = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 7, 0, 0),
                BackColor = Color.Transparent
            };
            _btnGuardar_180_njab = CrearBotonPrincipal_180_njab("Asignar", 120);
            _btnGuardar_180_njab.Click += GuardarJornada_180_njab;
            accionesFormulario_180_njab.Controls.Add(_btnGuardar_180_njab);
            _btnCancelarEdicion_180_njab = CrearBotonSecundario_180_njab("Cancelar edición", 148);
            _btnCancelarEdicion_180_njab.Visible = false;
            _btnCancelarEdicion_180_njab.Click += CancelarEdicion_180_njab;
            accionesFormulario_180_njab.Controls.Add(_btnCancelarEdicion_180_njab);
            campos_180_njab.Controls.Add(accionesFormulario_180_njab, 0, 2);
            campos_180_njab.SetColumnSpan(accionesFormulario_180_njab, 3);
            formulario_180_njab.Controls.Add(campos_180_njab);
            layoutPrincipal_180_njab.Controls.Add(formulario_180_njab, 0, 1);

            Panel estadoContenedor_180_njab = new Panel
            {
                BackColor = Color.FromArgb(232, 243, 246),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Height = 42,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(12, 0, 12, 0)
            };
            _lblEstado_180_njab = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(35, 76, 89),
                TextAlign = ContentAlignment.MiddleLeft
            };
            estadoContenedor_180_njab.Controls.Add(_lblEstado_180_njab);
            layoutPrincipal_180_njab.Controls.Add(estadoContenedor_180_njab, 0, 2);

            GroupBox jornadas_180_njab = new GroupBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 76, 89),
                Margin = new Padding(0),
                Padding = new Padding(10, 12, 10, 10),
                Text = "Jornadas asignadas"
            };
            _gridJornadas_180_njab = CrearGrillaJornadas_180_njab();
            jornadas_180_njab.Controls.Add(_gridJornadas_180_njab);
            layoutPrincipal_180_njab.Controls.Add(jornadas_180_njab, 0, 3);

            TableLayoutPanel accionesJornadas_180_njab = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Height = 54,
                Margin = new Padding(0, 12, 0, 0),
                Padding = new Padding(0)
            };
            accionesJornadas_180_njab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            FlowLayoutPanel accionesEdicion_180_njab = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 7, 0, 0)
            };
            _btnModificar_180_njab = CrearBotonSecundario_180_njab("Modificar selección", 156);
            _btnModificar_180_njab.Enabled = false;
            _btnModificar_180_njab.Click += ModificarJornada_180_njab;
            accionesEdicion_180_njab.Controls.Add(_btnModificar_180_njab);
            _btnQuitar_180_njab = CrearBotonPeligro_180_njab("Quitar asignación", 150);
            _btnQuitar_180_njab.Enabled = false;
            _btnQuitar_180_njab.Click += QuitarAsignacion_180_njab;
            accionesEdicion_180_njab.Controls.Add(_btnQuitar_180_njab);
            accionesJornadas_180_njab.Controls.Add(accionesEdicion_180_njab, 0, 0);
            layoutPrincipal_180_njab.Controls.Add(accionesJornadas_180_njab, 0, 4);

            Load += CargarVista_180_njab;
        }

        private DataGridView CrearGrillaJornadas_180_njab()
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
                Padding = new Padding(9, 0, 9, 0)
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
                Padding = new Padding(9, 0, 9, 0)
            };
            grilla_180_njab.Columns.Add("dia_180_njab", "Día");
            grilla_180_njab.Columns.Add("fecha_180_njab", "Fecha");
            grilla_180_njab.Columns.Add("franja_180_njab", "Franja horaria");
            grilla_180_njab.Columns.Add("consultorio_180_njab", "Consultorio");
            grilla_180_njab.Columns.Add("duracion_180_njab", "Duración");
            grilla_180_njab.Columns[0].FillWeight = 12F;
            grilla_180_njab.Columns[1].FillWeight = 14F;
            grilla_180_njab.Columns[2].FillWeight = 23F;
            grilla_180_njab.Columns[3].FillWeight = 18F;
            grilla_180_njab.Columns[4].FillWeight = 15F;
            grilla_180_njab.SelectionChanged += JornadaSeleccionada_180_njab;
            return grilla_180_njab;
        }

        private Panel CrearCampo_180_njab(string etiqueta_180_njab, Control control_180_njab)
        {
            Panel campo_180_njab = new Panel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 12, 9),
                Padding = new Padding(0)
            };
            Label etiquetaControl_180_njab = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Height = 22,
                Text = etiqueta_180_njab
            };
            control_180_njab.Margin = new Padding(0);
            campo_180_njab.Controls.Add(control_180_njab);
            campo_180_njab.Controls.Add(etiquetaControl_180_njab);
            return campo_180_njab;
        }

        private Button CrearBotonPrincipal_180_njab(string texto_180_njab, int ancho_180_njab)
        {
            Button boton_180_njab = CrearBotonBase_180_njab(texto_180_njab, ancho_180_njab);
            boton_180_njab.BackColor = Color.FromArgb(35, 112, 130);
            boton_180_njab.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 91, 106);
            return boton_180_njab;
        }

        private Button CrearBotonSecundario_180_njab(string texto_180_njab, int ancho_180_njab)
        {
            Button boton_180_njab = CrearBotonBase_180_njab(texto_180_njab, ancho_180_njab);
            boton_180_njab.BackColor = Color.White;
            boton_180_njab.FlatAppearance.BorderColor = Color.FromArgb(126, 150, 158);
            boton_180_njab.FlatAppearance.BorderSize = 1;
            boton_180_njab.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 243, 246);
            boton_180_njab.ForeColor = Color.FromArgb(35, 76, 89);
            return boton_180_njab;
        }

        private Button CrearBotonPeligro_180_njab(string texto_180_njab, int ancho_180_njab)
        {
            Button boton_180_njab = CrearBotonBase_180_njab(texto_180_njab, ancho_180_njab);
            boton_180_njab.BackColor = Color.White;
            boton_180_njab.FlatAppearance.BorderColor = Color.FromArgb(170, 91, 91);
            boton_180_njab.FlatAppearance.BorderSize = 1;
            boton_180_njab.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 238, 238);
            boton_180_njab.ForeColor = Color.FromArgb(145, 61, 61);
            return boton_180_njab;
        }

        private Button CrearBotonBase_180_njab(string texto_180_njab, int ancho_180_njab)
        {
            return new Button
            {
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Height = 36,
                MinimumSize = new Size(ancho_180_njab, 36),
                Text = texto_180_njab,
                UseVisualStyleBackColor = false,
                Margin = new Padding(0, 0, 10, 0)
            };
        }

        private void CargarVista_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            DateTime fechaInicial_180_njab = CalcularFechaInicial_180_njab();
            _dtpFecha_180_njab.Value = fechaInicial_180_njab;
            _cmbDia_180_njab.SelectedIndex = (int)fechaInicial_180_njab.DayOfWeek - 1;
            CargarJornadas_180_njab();
            ActualizarDisponibilidad_180_njab();
        }

        private void ActualizarDisponibilidad_180_njab()
        {
            if (_cargando_180_njab || _cmbDia_180_njab.SelectedIndex < 0 || !(_cmbFranja_180_njab.SelectedItem is JornadaFranjaOpcion_180_njab))
            {
                return;
            }

            DayOfWeek dia_180_njab = (DayOfWeek)(_cmbDia_180_njab.SelectedIndex + 1);
            if (_dtpFecha_180_njab.Value.DayOfWeek != dia_180_njab)
            {
                _cmbConsultorio_180_njab.DataSource = null;
                _lblEstado_180_njab.Text = "La fecha no coincide con el día seleccionado; corregí una de las dos opciones.";
                return;
            }

            try
            {
                JornadaFranjaOpcion_180_njab franja_180_njab = (JornadaFranjaOpcion_180_njab)_cmbFranja_180_njab.SelectedItem;
                int idJornadaExcluir_180_njab = _modoEdicion_180_njab && _jornadaSeleccionada_180_njab != null ? _jornadaSeleccionada_180_njab.IdJornada_180_njab : 0;
                List<ConsultorioConsulta_180_njab> consultorios_180_njab = _jornadaService_180_njab.ListarConsultoriosDisponibles_180_njab(_dtpFecha_180_njab.Value.Date, franja_180_njab.Codigo_180_njab, idJornadaExcluir_180_njab);
                _cmbConsultorio_180_njab.DataSource = consultorios_180_njab;
                if (consultorios_180_njab.Count == 0)
                {
                    _lblEstado_180_njab.Text = "No hay consultorios disponibles para esa fecha y franja. Podés elegir otra fecha o franja.";
                    return;
                }

                _lblEstado_180_njab.Text = "Consultorio disponible. Revisá la selección y presioná Asignar.";
            }
            catch (Exception ex_180_njab)
            {
                _cmbConsultorio_180_njab.DataSource = null;
                _lblEstado_180_njab.Text = string.Format(
                    "No se pudieron consultar los consultorios disponibles. Detalle: {0}",
                    ex_180_njab.Message);
            }
        }

        private void GuardarJornada_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            JornadaSolicitud_180_njab solicitud_180_njab = ConstruirSolicitud_180_njab();
            JornadaOperacionResultado_180_njab resultado_180_njab = _modoEdicion_180_njab
                ? _jornadaService_180_njab.Modificar_180_njab(solicitud_180_njab)
                : _jornadaService_180_njab.Asignar_180_njab(solicitud_180_njab);
            if (!resultado_180_njab.Exito_180_njab)
            {
                MessageBox.Show(this, resultado_180_njab.Mensaje_180_njab, "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(this, resultado_180_njab.Mensaje_180_njab, "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CancelarEdicion_180_njab(null, EventArgs.Empty);
            CargarJornadas_180_njab();
            ActualizarDisponibilidad_180_njab();
        }

        private void ModificarJornada_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            if (_jornadaSeleccionada_180_njab == null)
            {
                MessageBox.Show(this, "Seleccioná una jornada para modificar.", "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            JornadaOperacionResultado_180_njab turnos_180_njab = _jornadaService_180_njab.TieneTurnosVigentes_180_njab(_jornadaSeleccionada_180_njab.IdJornada_180_njab);
            if (!turnos_180_njab.Exito_180_njab)
            {
                MessageBox.Show(this, turnos_180_njab.Mensaje_180_njab, "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _modoEdicion_180_njab = true;
            _cargando_180_njab = true;
            _dtpFecha_180_njab.Value = _jornadaSeleccionada_180_njab.Fecha_180_njab;
            _cmbDia_180_njab.SelectedIndex = (int)_jornadaSeleccionada_180_njab.Fecha_180_njab.DayOfWeek - 1;
            _cmbFranja_180_njab.SelectedIndex = _jornadaSeleccionada_180_njab.HoraInicio_180_njab == new TimeSpan(8, 0, 0) ? 0 : 1;
            _nudDuracion_180_njab.Value = Math.Max(_nudDuracion_180_njab.Minimum, Math.Min(_nudDuracion_180_njab.Maximum, _jornadaSeleccionada_180_njab.DuracionTurnoMinutos_180_njab));
            _cargando_180_njab = false;
            ActualizarDisponibilidad_180_njab();
            SeleccionarConsultorio_180_njab(_jornadaSeleccionada_180_njab.IdConsultorio_180_njab);
            _btnGuardar_180_njab.Text = "Guardar cambios";
            _btnCancelarEdicion_180_njab.Visible = true;
            _lblEstado_180_njab.Text = "Editando la jornada seleccionada. Volvé a validar antes de guardar.";
        }

        private void QuitarAsignacion_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            if (_jornadaSeleccionada_180_njab == null)
            {
                MessageBox.Show(this, "Seleccioná una jornada para quitar.", "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            JornadaOperacionResultado_180_njab resultado_180_njab = _jornadaService_180_njab.QuitarAsignacion_180_njab(_jornadaSeleccionada_180_njab.IdJornada_180_njab, _profesional_180_njab.Id_180_njab);
            if (!resultado_180_njab.Exito_180_njab)
            {
                MessageBox.Show(this, resultado_180_njab.Mensaje_180_njab, "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(this, resultado_180_njab.Mensaje_180_njab, "Jornada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarJornadas_180_njab();
            ActualizarDisponibilidad_180_njab();
        }

        private void Volver_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            if (_navegar_180_njab != null)
            {
                _navegar_180_njab(new ProfesionalesView_180_njab(_navegar_180_njab));
            }
        }

        private void CancelarEdicion_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            _modoEdicion_180_njab = false;
            _jornadaSeleccionada_180_njab = null;
            _btnGuardar_180_njab.Text = "Asignar";
            _btnCancelarEdicion_180_njab.Visible = false;
            if (_cmbConsultorio_180_njab.Items.Count > 0)
            {
                _cmbConsultorio_180_njab.SelectedIndex = 0;
            }
        }

        private void JornadaSeleccionada_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            _jornadaSeleccionada_180_njab = _gridJornadas_180_njab.CurrentRow == null
                ? null
                : _gridJornadas_180_njab.CurrentRow.Tag as JornadaProfesionalConsulta_180_njab;
            _btnModificar_180_njab.Enabled = _jornadaSeleccionada_180_njab != null && !_modoEdicion_180_njab;
            _btnQuitar_180_njab.Enabled = _jornadaSeleccionada_180_njab != null && !_modoEdicion_180_njab;
        }

        private JornadaSolicitud_180_njab ConstruirSolicitud_180_njab()
        {
            JornadaFranjaOpcion_180_njab franja_180_njab = _cmbFranja_180_njab.SelectedItem as JornadaFranjaOpcion_180_njab;
            ConsultorioConsulta_180_njab consultorio_180_njab = _cmbConsultorio_180_njab.SelectedItem as ConsultorioConsulta_180_njab;
            return new JornadaSolicitud_180_njab
            {
                IdJornada_180_njab = _modoEdicion_180_njab && _jornadaSeleccionada_180_njab != null ? _jornadaSeleccionada_180_njab.IdJornada_180_njab : 0,
                IdProfesional_180_njab = _profesional_180_njab.Id_180_njab,
                IdConsultorio_180_njab = consultorio_180_njab == null ? 0 : consultorio_180_njab.IdConsultorio_180_njab,
                Fecha_180_njab = _dtpFecha_180_njab.Value.Date,
                Dia_180_njab = _cmbDia_180_njab.SelectedIndex < 0 ? DayOfWeek.Sunday : (DayOfWeek)(_cmbDia_180_njab.SelectedIndex + 1),
                Franja_180_njab = franja_180_njab == null ? (JornadaFranja_180_njab)(-1) : franja_180_njab.Codigo_180_njab,
                DuracionTurnoMinutos_180_njab = (int)_nudDuracion_180_njab.Value
            };
        }

        private void CargarJornadas_180_njab()
        {
            _jornadaSeleccionada_180_njab = null;
            _btnModificar_180_njab.Enabled = false;
            _btnQuitar_180_njab.Enabled = false;
            _gridJornadas_180_njab.Rows.Clear();
            List<JornadaProfesionalConsulta_180_njab> jornadas_180_njab;
            try
            {
                jornadas_180_njab = _jornadaService_180_njab.ListarJornadas_180_njab(_profesional_180_njab.Id_180_njab);
            }
            catch (Exception ex_180_njab)
            {
                _lblEstado_180_njab.Text = string.Format(
                    "No se pudieron consultar las jornadas asignadas. Detalle: {0}",
                    ex_180_njab.Message);
                return;
            }
            foreach (JornadaProfesionalConsulta_180_njab jornada_180_njab in jornadas_180_njab)
            {
                int indiceFila_180_njab = _gridJornadas_180_njab.Rows.Add(
                    jornada_180_njab.Dia_180_njab,
                    jornada_180_njab.Fecha_180_njab.ToString("dd/MM/yyyy"),
                    jornada_180_njab.Franja_180_njab,
                    jornada_180_njab.Consultorio_180_njab,
                    jornada_180_njab.DuracionTurnoMinutos_180_njab + " min");
                _gridJornadas_180_njab.Rows[indiceFila_180_njab].Tag = jornada_180_njab;
            }

            _gridJornadas_180_njab.ClearSelection();
            _gridJornadas_180_njab.CurrentCell = null;

            _lblEstado_180_njab.Text = jornadas_180_njab.Count == 0
                ? "No hay jornadas activas asignadas a este profesional."
                : "Seleccioná una jornada para modificarla o quitar su asignación.";
        }

        private void SeleccionDeConfiguracionCambiada_180_njab(object sender_180_njab, EventArgs e_180_njab)
        {
            ActualizarDisponibilidad_180_njab();
        }

        private void SeleccionarConsultorio_180_njab(int idConsultorio_180_njab)
        {
            foreach (object item_180_njab in _cmbConsultorio_180_njab.Items)
            {
                ConsultorioConsulta_180_njab consultorio_180_njab = item_180_njab as ConsultorioConsulta_180_njab;
                if (consultorio_180_njab != null && consultorio_180_njab.IdConsultorio_180_njab == idConsultorio_180_njab)
                {
                    _cmbConsultorio_180_njab.SelectedItem = consultorio_180_njab;
                    return;
                }
            }
        }

        private DateTime CalcularFechaInicial_180_njab()
        {
            DateTime fecha_180_njab = DateTime.Today;
            while (fecha_180_njab.DayOfWeek == DayOfWeek.Sunday)
            {
                fecha_180_njab = fecha_180_njab.AddDays(1);
            }
            return fecha_180_njab;
        }
    }
}
