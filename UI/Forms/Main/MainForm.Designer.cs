namespace UI
{
    partial class MainForm
    {
        /// <summary>
        /// Variable del disenador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se esten usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codigo generado por el Disenador de Windows Forms

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.bitacoraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.auditoriaCambiosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rolesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.idiomasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topPanel = new System.Windows.Forms.Panel();
            this.cmbIdiomas = new System.Windows.Forms.ComboBox();
            this.lblIdioma = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bitacoraToolStripMenuItem,
            this.auditoriaCambiosToolStripMenuItem,
            this.usuariosToolStripMenuItem,
            this.rolesToolStripMenuItem,
            this.idiomasToolStripMenuItem,
            this.salirToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1000, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // bitacoraToolStripMenuItem
            // 
            this.bitacoraToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.bitacoraToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.bitacoraToolStripMenuItem.Name = "bitacoraToolStripMenuItem";
            this.bitacoraToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.bitacoraToolStripMenuItem.Text = "Bitacora";
            this.bitacoraToolStripMenuItem.Click += new System.EventHandler(this.bitacoraToolStripMenuItem_Click);
            // 
            // auditoriaCambiosToolStripMenuItem
            // 
            this.auditoriaCambiosToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.auditoriaCambiosToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.auditoriaCambiosToolStripMenuItem.Name = "auditoriaCambiosToolStripMenuItem";
            this.auditoriaCambiosToolStripMenuItem.Size = new System.Drawing.Size(151, 24);
            this.auditoriaCambiosToolStripMenuItem.Text = "Auditoria cambios";
            this.auditoriaCambiosToolStripMenuItem.Click += new System.EventHandler(this.auditoriaCambiosToolStripMenuItem_Click);
            // 
            // usuariosToolStripMenuItem
            // 
            this.usuariosToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.usuariosToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.usuariosToolStripMenuItem.Name = "usuariosToolStripMenuItem";
            this.usuariosToolStripMenuItem.Size = new System.Drawing.Size(78, 24);
            this.usuariosToolStripMenuItem.Text = "Usuarios";
            this.usuariosToolStripMenuItem.Click += new System.EventHandler(this.usuariosToolStripMenuItem_Click);
            // rolesToolStripMenuItem
            // 
            this.rolesToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rolesToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.rolesToolStripMenuItem.Name = "rolesToolStripMenuItem";
            this.rolesToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.rolesToolStripMenuItem.Text = "Roles";
            this.rolesToolStripMenuItem.Click += new System.EventHandler(this.rolesToolStripMenuItem_Click);
            // 
            // idiomasToolStripMenuItem
            // 
            this.idiomasToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.idiomasToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.idiomasToolStripMenuItem.Name = "idiomasToolStripMenuItem";
            this.idiomasToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.idiomasToolStripMenuItem.Text = "Idiomas";
            this.idiomasToolStripMenuItem.Click += new System.EventHandler(this.idiomasToolStripMenuItem_Click);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.salirToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.salirToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.salirToolStripMenuItem.Text = "Cerrar sesión";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.topPanel.Controls.Add(this.cmbIdiomas);
            this.topPanel.Controls.Add(this.lblIdioma);
            this.topPanel.Controls.Add(this.lblUsuario);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 28);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1000, 48);
            this.topPanel.TabIndex = 1;
            // 
            // cmbIdiomas
            // 
            this.cmbIdiomas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbIdiomas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdiomas.FormattingEnabled = true;
            this.cmbIdiomas.Location = new System.Drawing.Point(820, 13);
            this.cmbIdiomas.Name = "cmbIdiomas";
            this.cmbIdiomas.Size = new System.Drawing.Size(164, 23);
            this.cmbIdiomas.TabIndex = 2;
            this.cmbIdiomas.SelectedIndexChanged += new System.EventHandler(this.cmbIdiomas_SelectedIndexChanged);
            // 
            // lblIdioma
            // 
            this.lblIdioma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIdioma.AutoSize = true;
            this.lblIdioma.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblIdioma.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.lblIdioma.Location = new System.Drawing.Point(756, 16);
            this.lblIdioma.Name = "lblIdioma";
            this.lblIdioma.Size = new System.Drawing.Size(46, 15);
            this.lblIdioma.TabIndex = 1;
            this.lblIdioma.Text = "Idioma";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.lblUsuario.Location = new System.Drawing.Point(16, 14);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(57, 19);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Usuario";
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.White;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 76);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(18);
            this.contentPanel.Size = new System.Drawing.Size(1000, 574);
            this.contentPanel.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Panel principal";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bitacoraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem auditoriaCambiosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usuariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rolesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem idiomasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.ComboBox cmbIdiomas;
        private System.Windows.Forms.Label lblIdioma;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Panel contentPanel;
    }
}
