namespace WiseLib
{
    partial class offline_login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxEdit1_users = new DevExpress.XtraEditors.ComboBoxEdit();
            this.textEdit1_password = new DevExpress.XtraEditors.TextEdit();
            this.checkEdit1_show_pass = new DevExpress.XtraEditors.CheckEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label_password = new System.Windows.Forms.Label();
            this.label_user = new System.Windows.Forms.Label();
            this.simpleButton1_login = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1_users.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1_password.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1_show_pass.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxEdit1_users
            // 
            this.comboBoxEdit1_users.Location = new System.Drawing.Point(255, 38);
            this.comboBoxEdit1_users.Name = "comboBoxEdit1_users";
            this.comboBoxEdit1_users.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit1_users.Size = new System.Drawing.Size(202, 26);
            this.comboBoxEdit1_users.TabIndex = 0;
            // 
            // textEdit1_password
            // 
            this.textEdit1_password.Location = new System.Drawing.Point(255, 123);
            this.textEdit1_password.Name = "textEdit1_password";
            this.textEdit1_password.Size = new System.Drawing.Size(126, 26);
            this.textEdit1_password.TabIndex = 1;
            // 
            // checkEdit1_show_pass
            // 
            this.checkEdit1_show_pass.Location = new System.Drawing.Point(401, 124);
            this.checkEdit1_show_pass.Name = "checkEdit1_show_pass";
            this.checkEdit1_show_pass.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.checkEdit1_show_pass.Properties.Appearance.Options.UseFont = true;
            this.checkEdit1_show_pass.Properties.Caption = "Show";
            this.checkEdit1_show_pass.Size = new System.Drawing.Size(56, 20);
            this.checkEdit1_show_pass.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "long majestic historic library location and name";
            // 
            // label_password
            // 
            this.label_password.AutoSize = true;
            this.label_password.Location = new System.Drawing.Point(256, 97);
            this.label_password.Name = "label_password";
            this.label_password.Size = new System.Drawing.Size(76, 19);
            this.label_password.TabIndex = 3;
            this.label_password.Text = "Password";
            // 
            // label_user
            // 
            this.label_user.AutoSize = true;
            this.label_user.Location = new System.Drawing.Point(258, 12);
            this.label_user.Name = "label_user";
            this.label_user.Size = new System.Drawing.Size(41, 19);
            this.label_user.TabIndex = 3;
            this.label_user.Text = "User";
            // 
            // simpleButton1_login
            // 
            this.simpleButton1_login.Location = new System.Drawing.Point(292, 187);
            this.simpleButton1_login.Name = "simpleButton1_login";
            this.simpleButton1_login.Size = new System.Drawing.Size(118, 23);
            this.simpleButton1_login.TabIndex = 4;
            this.simpleButton1_login.Text = "login";
            this.simpleButton1_login.Click += new System.EventHandler(this.simpleButton1_login_Click);
            // 
            // offline_login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 268);
            this.Controls.Add(this.simpleButton1_login);
            this.Controls.Add(this.label_user);
            this.Controls.Add(this.label_password);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkEdit1_show_pass);
            this.Controls.Add(this.textEdit1_password);
            this.Controls.Add(this.comboBoxEdit1_users);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "offline_login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "offline";
            this.Load += new System.EventHandler(this.offline_login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1_users.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1_password.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1_show_pass.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1_users;
        private DevExpress.XtraEditors.TextEdit textEdit1_password;
        private DevExpress.XtraEditors.CheckEdit checkEdit1_show_pass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_password;
        private System.Windows.Forms.Label label_user;
        private DevExpress.XtraEditors.SimpleButton simpleButton1_login;
    }
}