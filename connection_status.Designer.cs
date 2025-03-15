namespace WiseLib
{
    partial class connection_status
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(connection_status));
            this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.simpleButton_select_database = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_backup_database = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_create_new_database = new DevExpress.XtraEditors.SimpleButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label_corazonthedev = new System.Windows.Forms.Label();
            this.pictureBox_github = new System.Windows.Forms.PictureBox();
            this.pictureBox_corazonthedev = new System.Windows.Forms.PictureBox();
            this.simpleButton_book_scanner = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_database = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_card_scanner = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_confirm = new DevExpress.XtraEditors.SimpleButton();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label_selectport2 = new System.Windows.Forms.Label();
            this.label_selectport1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_db_name = new System.Windows.Forms.Label();
            this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.pictureBox_library_logo = new System.Windows.Forms.PictureBox();
            this.textEdit_library_name = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.simpleButton_library_confirm = new DevExpress.XtraEditors.SimpleButton();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).BeginInit();
            this.navigationFrame1.SuspendLayout();
            this.navigationPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_github)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_corazonthedev)).BeginInit();
            this.navigationPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_library_logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_library_name.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // navigationFrame1
            // 
            this.navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
            this.navigationFrame1.Controls.Add(this.navigationPage1);
            this.navigationFrame1.Controls.Add(this.navigationPage2);
            this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
            this.navigationFrame1.Name = "navigationFrame1";
            this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.navigationPage1,
            this.navigationPage2});
            this.navigationFrame1.SelectedPage = this.navigationPage1;
            this.navigationFrame1.Size = new System.Drawing.Size(687, 401);
            this.navigationFrame1.TabIndex = 2;
            this.navigationFrame1.Text = "navigationFrame1";
            // 
            // navigationPage1
            // 
            this.navigationPage1.Controls.Add(this.simpleButton_select_database);
            this.navigationPage1.Controls.Add(this.simpleButton_backup_database);
            this.navigationPage1.Controls.Add(this.simpleButton_create_new_database);
            this.navigationPage1.Controls.Add(this.label2);
            this.navigationPage1.Controls.Add(this.label_corazonthedev);
            this.navigationPage1.Controls.Add(this.pictureBox_github);
            this.navigationPage1.Controls.Add(this.pictureBox_corazonthedev);
            this.navigationPage1.Controls.Add(this.simpleButton_book_scanner);
            this.navigationPage1.Controls.Add(this.simpleButton_database);
            this.navigationPage1.Controls.Add(this.simpleButton_card_scanner);
            this.navigationPage1.Controls.Add(this.simpleButton_confirm);
            this.navigationPage1.Controls.Add(this.comboBox2);
            this.navigationPage1.Controls.Add(this.comboBox1);
            this.navigationPage1.Controls.Add(this.label_selectport2);
            this.navigationPage1.Controls.Add(this.label_selectport1);
            this.navigationPage1.Controls.Add(this.label4);
            this.navigationPage1.Controls.Add(this.label3);
            this.navigationPage1.Controls.Add(this.label_db_name);
            this.navigationPage1.Name = "navigationPage1";
            this.navigationPage1.Size = new System.Drawing.Size(687, 401);
            // 
            // simpleButton_select_database
            // 
            this.simpleButton_select_database.Location = new System.Drawing.Point(202, 129);
            this.simpleButton_select_database.Name = "simpleButton_select_database";
            this.simpleButton_select_database.Size = new System.Drawing.Size(133, 23);
            this.simpleButton_select_database.TabIndex = 13;
            this.simpleButton_select_database.Text = "Select Database";
            this.simpleButton_select_database.Click += new System.EventHandler(this.simpleButton_select_database_Click);
            // 
            // simpleButton_backup_database
            // 
            this.simpleButton_backup_database.Location = new System.Drawing.Point(212, 71);
            this.simpleButton_backup_database.Name = "simpleButton_backup_database";
            this.simpleButton_backup_database.Size = new System.Drawing.Size(113, 43);
            this.simpleButton_backup_database.TabIndex = 12;
            this.simpleButton_backup_database.Text = "Backup\r\nDatabase";
            this.simpleButton_backup_database.Click += new System.EventHandler(this.simpleButton_backup_database_Click);
            // 
            // simpleButton_create_new_database
            // 
            this.simpleButton_create_new_database.Location = new System.Drawing.Point(212, 12);
            this.simpleButton_create_new_database.Name = "simpleButton_create_new_database";
            this.simpleButton_create_new_database.Size = new System.Drawing.Size(113, 43);
            this.simpleButton_create_new_database.TabIndex = 12;
            this.simpleButton_create_new_database.Text = "Create New \r\nDatabase";
            this.simpleButton_create_new_database.Click += new System.EventHandler(this.simpleButton_create_new_database_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(218, 277);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 23);
            this.label2.TabIndex = 11;
            this.label2.Text = "WiseLib";
            // 
            // label_corazonthedev
            // 
            this.label_corazonthedev.AutoSize = true;
            this.label_corazonthedev.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_corazonthedev.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_corazonthedev.Location = new System.Drawing.Point(45, 360);
            this.label_corazonthedev.Name = "label_corazonthedev";
            this.label_corazonthedev.Size = new System.Drawing.Size(168, 29);
            this.label_corazonthedev.TabIndex = 11;
            this.label_corazonthedev.Text = "corazonthedev";
            // 
            // pictureBox_github
            // 
            this.pictureBox_github.BackColor = System.Drawing.Color.White;
            this.pictureBox_github.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_github.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_github.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_github.Image")));
            this.pictureBox_github.Location = new System.Drawing.Point(216, 203);
            this.pictureBox_github.Name = "pictureBox_github";
            this.pictureBox_github.Size = new System.Drawing.Size(76, 74);
            this.pictureBox_github.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_github.TabIndex = 10;
            this.pictureBox_github.TabStop = false;
            // 
            // pictureBox_corazonthedev
            // 
            this.pictureBox_corazonthedev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_corazonthedev.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_corazonthedev.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_corazonthedev.Image")));
            this.pictureBox_corazonthedev.Location = new System.Drawing.Point(49, 203);
            this.pictureBox_corazonthedev.Name = "pictureBox_corazonthedev";
            this.pictureBox_corazonthedev.Size = new System.Drawing.Size(161, 157);
            this.pictureBox_corazonthedev.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_corazonthedev.TabIndex = 9;
            this.pictureBox_corazonthedev.TabStop = false;
            // 
            // simpleButton_book_scanner
            // 
            this.simpleButton_book_scanner.Appearance.Font = new System.Drawing.Font("Segoe MDL2 Assets", 45.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton_book_scanner.Appearance.Options.UseFont = true;
            this.simpleButton_book_scanner.Enabled = false;
            this.simpleButton_book_scanner.Location = new System.Drawing.Point(540, 240);
            this.simpleButton_book_scanner.Name = "simpleButton_book_scanner";
            this.simpleButton_book_scanner.Size = new System.Drawing.Size(134, 120);
            this.simpleButton_book_scanner.TabIndex = 8;
            // 
            // simpleButton_database
            // 
            this.simpleButton_database.Appearance.Font = new System.Drawing.Font("Segoe MDL2 Assets", 45.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton_database.Appearance.Options.UseFont = true;
            this.simpleButton_database.Enabled = false;
            this.simpleButton_database.Location = new System.Drawing.Point(49, 12);
            this.simpleButton_database.Name = "simpleButton_database";
            this.simpleButton_database.Size = new System.Drawing.Size(146, 140);
            this.simpleButton_database.TabIndex = 8;
            this.simpleButton_database.Tag = "";
            this.simpleButton_database.Text = "FirstDBcheck";
            // 
            // simpleButton_card_scanner
            // 
            this.simpleButton_card_scanner.Appearance.Font = new System.Drawing.Font("Segoe MDL2 Assets", 45.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton_card_scanner.Appearance.Options.UseFont = true;
            this.simpleButton_card_scanner.Enabled = false;
            this.simpleButton_card_scanner.Location = new System.Drawing.Point(540, 32);
            this.simpleButton_card_scanner.Name = "simpleButton_card_scanner";
            this.simpleButton_card_scanner.Size = new System.Drawing.Size(134, 120);
            this.simpleButton_card_scanner.TabIndex = 8;
            // 
            // simpleButton_confirm
            // 
            this.simpleButton_confirm.Location = new System.Drawing.Point(302, 360);
            this.simpleButton_confirm.Name = "simpleButton_confirm";
            this.simpleButton_confirm.Size = new System.Drawing.Size(104, 26);
            this.simpleButton_confirm.TabIndex = 7;
            this.simpleButton_confirm.Text = "confirm";
            this.simpleButton_confirm.Click += new System.EventHandler(this.simpleButton_confirm_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(369, 289);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(141, 27);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            this.comboBox2.SelectionChangeCommitted += new System.EventHandler(this.comboBox2_SelectionChangeCommitted);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(369, 79);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(141, 27);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.SelectionChangeCommitted += new System.EventHandler(this.comboBox1_SelectionChangeCommitted);
            // 
            // label_selectport2
            // 
            this.label_selectport2.AutoSize = true;
            this.label_selectport2.Location = new System.Drawing.Point(384, 267);
            this.label_selectport2.Name = "label_selectport2";
            this.label_selectport2.Size = new System.Drawing.Size(110, 19);
            this.label_selectport2.TabIndex = 5;
            this.label_selectport2.Text = "SELECT PORT";
            this.label_selectport2.Visible = false;
            // 
            // label_selectport1
            // 
            this.label_selectport1.AutoSize = true;
            this.label_selectport1.Location = new System.Drawing.Point(384, 57);
            this.label_selectport1.Name = "label_selectport1";
            this.label_selectport1.Size = new System.Drawing.Size(110, 19);
            this.label_selectport1.TabIndex = 5;
            this.label_selectport1.Text = "SELECT PORT";
            this.label_selectport1.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(537, 366);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "Book Scanner";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(538, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Card Scanner";
            // 
            // label_db_name
            // 
            this.label_db_name.AutoSize = true;
            this.label_db_name.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_db_name.Location = new System.Drawing.Point(26, 163);
            this.label_db_name.Name = "label_db_name";
            this.label_db_name.Size = new System.Drawing.Size(157, 19);
            this.label_db_name.TabIndex = 2;
            this.label_db_name.Text = "Database Connection";
            // 
            // navigationPage2
            // 
            this.navigationPage2.Controls.Add(this.label6);
            this.navigationPage2.Controls.Add(this.simpleButton_library_confirm);
            this.navigationPage2.Controls.Add(this.label5);
            this.navigationPage2.Controls.Add(this.label1);
            this.navigationPage2.Controls.Add(this.textEdit_library_name);
            this.navigationPage2.Controls.Add(this.pictureBox_library_logo);
            this.navigationPage2.Name = "navigationPage2";
            this.navigationPage2.Size = new System.Drawing.Size(687, 401);
            // 
            // pictureBox_library_logo
            // 
            this.pictureBox_library_logo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_library_logo.Location = new System.Drawing.Point(53, 97);
            this.pictureBox_library_logo.Name = "pictureBox_library_logo";
            this.pictureBox_library_logo.Size = new System.Drawing.Size(200, 200);
            this.pictureBox_library_logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_library_logo.TabIndex = 0;
            this.pictureBox_library_logo.TabStop = false;
            this.pictureBox_library_logo.Click += new System.EventHandler(this.pictureBox_library_logo_Click);
            // 
            // textEdit_library_name
            // 
            this.textEdit_library_name.Location = new System.Drawing.Point(332, 176);
            this.textEdit_library_name.Name = "textEdit_library_name";
            this.textEdit_library_name.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.textEdit_library_name.Properties.Appearance.Options.UseFont = true;
            this.textEdit_library_name.Size = new System.Drawing.Size(267, 32);
            this.textEdit_library_name.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(332, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Library Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(72, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(162, 33);
            this.label5.TabIndex = 2;
            this.label5.Text = "Library Logo";
            // 
            // simpleButton_library_confirm
            // 
            this.simpleButton_library_confirm.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.simpleButton_library_confirm.Appearance.Options.UseFont = true;
            this.simpleButton_library_confirm.Location = new System.Drawing.Point(479, 340);
            this.simpleButton_library_confirm.Name = "simpleButton_library_confirm";
            this.simpleButton_library_confirm.Size = new System.Drawing.Size(105, 35);
            this.simpleButton_library_confirm.TabIndex = 3;
            this.simpleButton_library_confirm.Text = "Confirm";
            this.simpleButton_library_confirm.Click += new System.EventHandler(this.simpleButton_library_confirm_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 19);
            this.label6.TabIndex = 4;
            this.label6.Text = "click to select logo";
            // 
            // connection_status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 401);
            this.Controls.Add(this.navigationFrame1);
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("connection_status.IconOptions.SvgImage")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "connection_status";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection";
            this.Load += new System.EventHandler(this.connection_status_Load);
            this.Shown += new System.EventHandler(this.connection_status_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).EndInit();
            this.navigationFrame1.ResumeLayout(false);
            this.navigationPage1.ResumeLayout(false);
            this.navigationPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_github)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_corazonthedev)).EndInit();
            this.navigationPage2.ResumeLayout(false);
            this.navigationPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_library_logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_library_name.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label label_selectport1;
        public System.Windows.Forms.Label label_selectport2;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        public DevExpress.XtraEditors.SimpleButton simpleButton_book_scanner;
        public DevExpress.XtraEditors.SimpleButton simpleButton_card_scanner;
        public DevExpress.XtraEditors.SimpleButton simpleButton_database;
        private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame1;
        private DevExpress.XtraBars.Navigation.NavigationPage navigationPage1;
        private System.Windows.Forms.Label label_db_name;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private DevExpress.XtraEditors.SimpleButton simpleButton_confirm;
        private System.Windows.Forms.PictureBox pictureBox_corazonthedev;
        private System.Windows.Forms.PictureBox pictureBox_github;
        private System.Windows.Forms.Label label_corazonthedev;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.SimpleButton simpleButton_create_new_database;
        private DevExpress.XtraEditors.SimpleButton simpleButton_select_database;
        public DevExpress.XtraEditors.SimpleButton simpleButton_backup_database;
        private DevExpress.XtraBars.Navigation.NavigationPage navigationPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit textEdit_library_name;
        private System.Windows.Forms.PictureBox pictureBox_library_logo;
        private DevExpress.XtraEditors.SimpleButton simpleButton_library_confirm;
        private System.Windows.Forms.Label label6;
    }
}