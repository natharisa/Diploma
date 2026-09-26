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
        private readonly DataGridView _gridProfesionales_180_njab;
        private readonly Button _btnAsignarJornada_180_njab;
        private readonly Label _lblEstado_180_njab;
        private List<ProfesionalConsulta_180_njab> _profesionales_180_njab;

        public ProfesionalConsulta_180_njab ProfesionalSeleccionado_180_njab { get; private set; }

        public ProfesionalesView_180_njab()
            : this(new ProfesionalApplicationService_180_njab())
        {
        }

        public ProfesionalesView_180_njab(ProfesionalApplicationService_180_njab profesionalService_180_njab)
        {
            _profesionalService_180_njab = profesionalService_180_njab;
            BackColor = Color.White;

            Label titulo_180_njab = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold),
                Location = new Point(24, 18),
                Name = "lblTituloProfesionales_180_njab",
                Text = "Profesionales"
            };
            Controls.Add(titulo_180_njab);

            _gridProfesionales_180_njab = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Location = new Point(24, 68),
                MultiSelect = false,
                Name = "gridProfesionales_180_njab",
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Size = new Size(900, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            _gridProfesionales_180_njab.Columns.Add("dni_180_njab", "DNI");
            _gridProfesionales_180_njab.Columns.Add("nombre_180_njab", "Nombre");
            _gridProfesionales_180_njab.Columns.Add("apellido_180_njab", "Apellido");
            _gridProfesionales_180_njab.Columns.Add("matricula_180_njab", "Matrícula");
            _gridProfesionales_180_njab.Columns.Add("especialidad_180_njab", "Especialidades");
            _gridProfesionales_180_njab.SelectionChanged += gridProfesionales_SelectionChanged_180_njab;
            Controls.Add(_gridProfesionales_180_njab);

            _btnAsignarJornada_180_njab = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(13, 110, 253),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Location = new Point(774, 440),
                Name = "btnAsignarJornada_180_njab",
                Size = new Size(150, 36),
                Text = "Asignar jornada",
                UseVisualStyleBackColor = false
            };
            _btnAsignarJornada_180_njab.Click += btnAsignarJornada_Click_180_njab;
            Controls.Add(_btnAsignarJornada_180_njab);

            _lblEstado_180_njab = new Label
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = false,
                Location = new Point(24, 440),
                Name = "lblEstadoProfesional_180_njab",
                Size = new Size(730, 36),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(_lblEstado_180_njab);

            CargarProfesionales_180_njab();
        }

        private void CargarProfesionales_180_njab()
        {
            //Carga los profesionales y los muestra en la grilla
            _gridProfesionales_180_njab.Rows.Clear();
            _profesionales_180_njab = _profesionalService_180_njab.Listar_180_njab();
            foreach (ProfesionalConsulta_180_njab profesional_180_njab in _profesionales_180_njab)
            {
                _gridProfesionales_180_njab.Rows.Add(
                    profesional_180_njab.Dni_180_njab ?? string.Empty,
                    profesional_180_njab.Nombre_180_njab,
                    profesional_180_njab.Apellido_180_njab,
                    profesional_180_njab.Matricula_180_njab,
                    profesional_180_njab.Especialidad_180_njab);
            }

            if (_gridProfesionales_180_njab.Rows.Count == 0)
            {
                _lblEstado_180_njab.Text = "No hay profesionales activos para mostrar.";
                _btnAsignarJornada_180_njab.Enabled = false;
            }
            else
            {
                _lblEstado_180_njab.Text = "Seleccioná un profesional para continuar.";
                _gridProfesionales_180_njab.Rows[0].Selected = true;
                ActualizarSeleccion_180_njab();
            }
        }

        private void gridProfesionales_SelectionChanged_180_njab(object sender_180_njab, System.EventArgs e_180_njab)
        {
            ActualizarSeleccion_180_njab();
        }

        private void ActualizarSeleccion_180_njab()
        {
            if (_gridProfesionales_180_njab.CurrentRow == null || _gridProfesionales_180_njab.CurrentRow.Index < 0)
            {
                ProfesionalSeleccionado_180_njab = null;
                _btnAsignarJornada_180_njab.Enabled = false;
                return;
            }

            int indice_180_njab = _gridProfesionales_180_njab.CurrentRow.Index;
            if (_profesionales_180_njab == null || indice_180_njab >= _profesionales_180_njab.Count)
            {
                ProfesionalSeleccionado_180_njab = null;
                _btnAsignarJornada_180_njab.Enabled = false;
                return;
            }

            //Guarda el profesional que selecciono el administrador
            ProfesionalSeleccionado_180_njab = _profesionales_180_njab[indice_180_njab];
            _btnAsignarJornada_180_njab.Enabled = true;
        }

        private void btnAsignarJornada_Click_180_njab(object sender_180_njab, System.EventArgs e_180_njab)
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
        }
    }
}
