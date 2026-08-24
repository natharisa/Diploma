namespace UI
{
    partial class RolesView
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
            this.groupBoxRoles = new System.Windows.Forms.GroupBox();
            this.treeViewRoles = new System.Windows.Forms.TreeView();
            this.groupBoxRoles.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(18, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(76, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Roles";
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescripcion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDescripcion.Location = new System.Drawing.Point(20, 58);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(302, 19);
            this.lblDescripcion.TabIndex = 1;
            this.lblDescripcion.Text = "Pantalla base para asignacion y gestion de roles.";
            // 
            // groupBoxRoles
            // 
            this.groupBoxRoles.Controls.Add(this.treeViewRoles);
            this.groupBoxRoles.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxRoles.Location = new System.Drawing.Point(24, 98);
            this.groupBoxRoles.Name = "groupBoxRoles";
            this.groupBoxRoles.Size = new System.Drawing.Size(420, 286);
            this.groupBoxRoles.TabIndex = 2;
            this.groupBoxRoles.TabStop = false;
            this.groupBoxRoles.Text = "Estructura de roles";
            // 
            // treeViewRoles
            // 
            this.treeViewRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewRoles.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.treeViewRoles.Location = new System.Drawing.Point(3, 21);
            this.treeViewRoles.Name = "treeViewRoles";
            this.treeViewRoles.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            new System.Windows.Forms.TreeNode("Administrador", new System.Windows.Forms.TreeNode[] {
            new System.Windows.Forms.TreeNode("Gestion de permisos"),
            new System.Windows.Forms.TreeNode("Gestion de roles")}),
            new System.Windows.Forms.TreeNode("Auditor", new System.Windows.Forms.TreeNode[] {
            new System.Windows.Forms.TreeNode("Consulta de bitacora")})});
            this.treeViewRoles.Size = new System.Drawing.Size(414, 262);
            this.treeViewRoles.TabIndex = 0;
            // 
            // RolesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBoxRoles);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.lblTitulo);
            this.Name = "RolesView";
            this.Size = new System.Drawing.Size(900, 520);
            this.groupBoxRoles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.GroupBox groupBoxRoles;
        private System.Windows.Forms.TreeView treeViewRoles;
    }
}
