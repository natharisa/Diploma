namespace UI
{
    partial class Registro
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

        /// <summary>
        /// Metodo necesario para admitir el Disenador. No se puede modificar
        /// el contenido de este metodo con el editor de codigo.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTitulo = new System.Windows.Forms.Label();
            this.labelSubtitulo = new System.Windows.Forms.Label();
            this.labelNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.labelApellido = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.labelUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.labelEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.labelPass = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.btnCrearCuenta = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.labelTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.labelTitulo.Location = new System.Drawing.Point(36, 24);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(178, 32);
            this.labelTitulo.TabIndex = 0;
            this.labelTitulo.Text = "Crear usuario";
            // 
            // labelSubtitulo
            // 
            this.labelSubtitulo.AutoSize = true;
            this.labelSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.labelSubtitulo.Location = new System.Drawing.Point(39, 63);
            this.labelSubtitulo.Name = "labelSubtitulo";
            this.labelSubtitulo.Size = new System.Drawing.Size(191, 17);
            this.labelSubtitulo.TabIndex = 1;
            this.labelSubtitulo.Text = "Completa los datos del registro";
            // 
            // labelNombre
            // 
            this.labelNombre.AutoSize = true;
            this.labelNombre.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(70)))));
            this.labelNombre.Location = new System.Drawing.Point(39, 109);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(56, 17);
            this.labelNombre.TabIndex = 2;
            this.labelNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.White;
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombre.Location = new System.Drawing.Point(42, 130);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(168, 25);
            this.txtNombre.TabIndex = 0;
            // 
            // labelApellido
            // 
            this.labelApellido.AutoSize = true;
            this.labelApellido.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelApellido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(70)))));
            this.labelApellido.Location = new System.Drawing.Point(233, 109);
            this.labelApellido.Name = "labelApellido";
            this.labelApellido.Size = new System.Drawing.Size(58, 17);
            this.labelApellido.TabIndex = 4;
            this.labelApellido.Text = "Apellido";
            // 
            // txtApellido
            // 
            this.txtApellido.BackColor = System.Drawing.Color.White;
            this.txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtApellido.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtApellido.Location = new System.Drawing.Point(236, 130);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(168, 25);
            this.txtApellido.TabIndex = 1;
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(70)))));
            this.labelUser.Location = new System.Drawing.Point(39, 182);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(50, 17);
            this.labelUser.TabIndex = 6;
            this.labelUser.Text = "Usuario";
            // 
            // txtUser
            // 
            this.txtUser.BackColor = System.Drawing.Color.White;
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUser.Location = new System.Drawing.Point(42, 203);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(168, 25);
            this.txtUser.TabIndex = 2;
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.labelEmail.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(70)))));
            this.labelEmail.Location = new System.Drawing.Point(233, 182);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(41, 17);
            this.labelEmail.TabIndex = 8;
            this.labelEmail.Text = "Email";
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.White;
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.Location = new System.Drawing.Point(236, 203);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(168, 25);
            this.txtEmail.TabIndex = 3;
            // 
            // labelPass
            // 
            this.labelPass.AutoSize = true;
            this.labelPass.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelPass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(70)))));
            this.labelPass.Location = new System.Drawing.Point(39, 256);
            this.labelPass.Name = "labelPass";
            this.labelPass.Size = new System.Drawing.Size(74, 17);
            this.labelPass.TabIndex = 10;
            this.labelPass.Text = "Contrasena";
            // 
            // txtPass
            // 
            this.txtPass.BackColor = System.Drawing.Color.White;
            this.txtPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPass.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPass.Location = new System.Drawing.Point(42, 277);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(362, 25);
            this.txtPass.TabIndex = 4;
            // 
            // btnCrearCuenta
            // 
            this.btnCrearCuenta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(178)))), ((int)(((byte)(120)))));
            this.btnCrearCuenta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCrearCuenta.FlatAppearance.BorderSize = 0;
            this.btnCrearCuenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearCuenta.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCrearCuenta.ForeColor = System.Drawing.Color.White;
            this.btnCrearCuenta.Location = new System.Drawing.Point(236, 332);
            this.btnCrearCuenta.Name = "btnCrearCuenta";
            this.btnCrearCuenta.Size = new System.Drawing.Size(168, 36);
            this.btnCrearCuenta.TabIndex = 6;
            this.btnCrearCuenta.Text = "Crear cuenta";
            this.btnCrearCuenta.UseVisualStyleBackColor = false;
            this.btnCrearCuenta.Click += new System.EventHandler(this.btnCrearCuenta_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(212)))), ((int)(((byte)(218)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.btnCancelar.Location = new System.Drawing.Point(42, 332);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(168, 36);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // Registro
            // 
            this.AcceptButton = this.btnCrearCuenta;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(448, 402);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCrearCuenta);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.labelPass);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.labelEmail);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.labelApellido);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.labelNombre);
            this.Controls.Add(this.labelSubtitulo);
            this.Controls.Add(this.labelTitulo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Registro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registro de usuario";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Label labelSubtitulo;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label labelApellido;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label labelPass;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnCrearCuenta;
        private System.Windows.Forms.Button btnCancelar;
    }
}
