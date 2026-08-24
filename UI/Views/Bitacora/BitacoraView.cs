using System;
using System.Collections.Generic;
using Application;
using Domain;
using System.Windows.Forms;
using Services;

namespace UI
{
    public partial class BitacoraView : LocalizedUserControl
    {
        private readonly BitacoraApplicationService _bitacoraService;
        private int _cantidadRegistros;

        public BitacoraView()
            : this(new BitacoraApplicationService())
        {
        }

        public BitacoraView(BitacoraApplicationService bitacoraService)
        {
            _bitacoraService = bitacoraService;
            InitializeComponent();
            ConfigurarTraducciones();
            CargarBitacora();
        }

        private void ConfigurarTraducciones()
        {
            lblTitulo.Tag = "AUDIT_TITLE";
            lblDescripcion.Tag = "AUDIT_DESCRIPTION";
            btnActualizar.Tag = "BTN_REFRESH";
            columnId.Tag = "GRID_ID";
            columnFecha.Tag = "GRID_DATE";
            columnIdUsuario.Tag = "GRID_USER_ID";
            columnUsuario.Tag = "GRID_USER";
            columnModulo.Tag = "GRID_MODULE";
            columnAccion.Tag = "GRID_ACTION";
            columnNivel.Tag = "GRID_LEVEL";
            columnDescripcion.Tag = "GRID_DESCRIPTION";
            columnEquipo.Tag = "GRID_DEVICE";
        }

        protected override void ApplyTranslations()
        {
            base.ApplyTranslations();
            ActualizarEstado();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        private void CargarBitacora()
        {
            listViewBitacora.BeginUpdate();
            listViewBitacora.Items.Clear();

            List<BitacoraRegistro> registros = _bitacoraService.Listar();

            foreach (BitacoraRegistro registro in registros)
            {
                ListViewItem item = new ListViewItem(registro.Id.ToString());
                item.SubItems.Add(registro.Fecha.ToString("dd/MM/yyyy HH:mm:ss"));
                item.SubItems.Add(registro.IdUsuario.HasValue ? registro.IdUsuario.Value.ToString() : string.Empty);
                item.SubItems.Add(registro.IdentificadorUsuario ?? string.Empty);
                item.SubItems.Add(registro.Modulo ?? string.Empty);
                item.SubItems.Add(registro.Accion ?? string.Empty);
                item.SubItems.Add(registro.Nivel ?? string.Empty);
                item.SubItems.Add(registro.Descripcion ?? string.Empty);
                item.SubItems.Add(registro.Equipo ?? string.Empty);

                listViewBitacora.Items.Add(item);
            }

            listViewBitacora.EndUpdate();
            _cantidadRegistros = registros.Count;
            ActualizarEstado();
        }

        private void ActualizarEstado()
        {
            lblEstado.Text = _cantidadRegistros == 0
                ? LanguageManager.Instance.Translate("AUDIT_EMPTY")
                : string.Format(LanguageManager.Instance.Translate("AUDIT_COUNT"), _cantidadRegistros);
        }
    }
}
