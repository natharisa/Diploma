namespace UI
{
    partial class BitacoraView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.listViewBitacora = new System.Windows.Forms.ListView();
            this.columnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFecha = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnIdUsuario = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUsuario = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnModulo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnAccion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnNivel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDescripcion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnEquipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(18, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(117, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Bitacora";
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescripcion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDescripcion.Location = new System.Drawing.Point(20, 58);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(320, 19);
            this.lblDescripcion.TabIndex = 1;
            this.lblDescripcion.Text = "Aca vas a poder consultar los eventos del sistema.";
            // 
            // listViewBitacora
            // 
            this.listViewBitacora.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnId,
            this.columnFecha,
            this.columnIdUsuario,
            this.columnUsuario,
            this.columnModulo,
            this.columnAccion,
            this.columnNivel,
            this.columnDescripcion,
            this.columnEquipo});
            this.listViewBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewBitacora.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.listViewBitacora.FullRowSelect = true;
            this.listViewBitacora.GridLines = true;
            this.listViewBitacora.HideSelection = false;
            this.listViewBitacora.Location = new System.Drawing.Point(24, 97);
            this.listViewBitacora.Name = "listViewBitacora";
            this.listViewBitacora.Size = new System.Drawing.Size(852, 354);
            this.listViewBitacora.TabIndex = 2;
            this.listViewBitacora.UseCompatibleStateImageBehavior = false;
            this.listViewBitacora.View = System.Windows.Forms.View.Details;
            // 
            // columnId
            // 
            this.columnId.Text = "Id";
            this.columnId.Width = 55;
            // 
            // columnFecha
            // 
            this.columnFecha.Text = "Fecha";
            this.columnFecha.Width = 150;
            // 
            // columnIdUsuario
            // 
            this.columnIdUsuario.Text = "Id usuario";
            this.columnIdUsuario.Width = 80;
            // 
            // columnUsuario
            // 
            this.columnUsuario.Text = "Usuario";
            this.columnUsuario.Width = 170;
            // 
            // columnModulo
            // 
            this.columnModulo.Text = "Modulo";
            this.columnModulo.Width = 110;
            // 
            // columnAccion
            // 
            this.columnAccion.Text = "Accion";
            this.columnAccion.Width = 120;
            // 
            // columnNivel
            // 
            this.columnNivel.Text = "Nivel";
            this.columnNivel.Width = 105;
            // 
            // columnDescripcion
            // 
            this.columnDescripcion.Text = "Descripcion";
            this.columnDescripcion.Width = 260;
            // 
            // columnEquipo
            // 
            this.columnEquipo.Text = "Equipo";
            this.columnEquipo.Width = 150;
            // 
            // lblEstado
            // 
            this.lblEstado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblEstado.Location = new System.Drawing.Point(24, 464);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(127, 15);
            this.lblEstado.TabIndex = 3;
            this.lblEstado.Text = "Cargando bitacora...";
            // 
            // btnActualizar
            // 
            this.btnActualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(752, 53);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(124, 32);
            this.btnActualizar.TabIndex = 4;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // BitacoraView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.listViewBitacora);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.lblTitulo);
            this.Name = "BitacoraView";
            this.Size = new System.Drawing.Size(900, 520);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.ListView listViewBitacora;
        private System.Windows.Forms.ColumnHeader columnId;
        private System.Windows.Forms.ColumnHeader columnFecha;
        private System.Windows.Forms.ColumnHeader columnIdUsuario;
        private System.Windows.Forms.ColumnHeader columnUsuario;
        private System.Windows.Forms.ColumnHeader columnModulo;
        private System.Windows.Forms.ColumnHeader columnAccion;
        private System.Windows.Forms.ColumnHeader columnNivel;
        private System.Windows.Forms.ColumnHeader columnDescripcion;
        private System.Windows.Forms.ColumnHeader columnEquipo;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button btnActualizar;
    }
}
