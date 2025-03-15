namespace WiseLib
{
    partial class signup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(signup));
            this.simpleButton_confirm = new DevExpress.XtraEditors.SimpleButton();
            this.label_name = new System.Windows.Forms.Label();
            this.label_surname = new System.Windows.Forms.Label();
            this.simpleButton_cancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_clear_name = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_clear_surname = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_clear_img = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_print_card = new DevExpress.XtraEditors.SimpleButton();
            this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.pictureBox_card_preview = new System.Windows.Forms.PictureBox();
            this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.label_select_photo = new System.Windows.Forms.Label();
            this.pictureBox_member_photo = new System.Windows.Forms.PictureBox();
            this.simpleButton_open_camera = new DevExpress.XtraEditors.SimpleButton();
            this.textEdit_name = new DevExpress.XtraEditors.TextEdit();
            this.textEdit_surname = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).BeginInit();
            this.navigationFrame1.SuspendLayout();
            this.navigationPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_card_preview)).BeginInit();
            this.navigationPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_member_photo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_name.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_surname.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButton_confirm
            // 
            this.simpleButton_confirm.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.simpleButton_confirm.Appearance.Options.UseFont = true;
            this.simpleButton_confirm.Location = new System.Drawing.Point(306, 247);
            this.simpleButton_confirm.Name = "simpleButton_confirm";
            this.simpleButton_confirm.Size = new System.Drawing.Size(83, 28);
            this.simpleButton_confirm.TabIndex = 2;
            this.simpleButton_confirm.Text = "confirm";
            this.simpleButton_confirm.Click += new System.EventHandler(this.simpleButton_confirm_Click);
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_name.Location = new System.Drawing.Point(248, 29);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(59, 23);
            this.label_name.TabIndex = 3;
            this.label_name.Text = "Name";
            // 
            // label_surname
            // 
            this.label_surname.AutoSize = true;
            this.label_surname.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label_surname.Location = new System.Drawing.Point(248, 125);
            this.label_surname.Name = "label_surname";
            this.label_surname.Size = new System.Drawing.Size(86, 23);
            this.label_surname.TabIndex = 3;
            this.label_surname.Text = "Surname";
            // 
            // simpleButton_cancel
            // 
            this.simpleButton_cancel.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.simpleButton_cancel.Appearance.Options.UseFont = true;
            this.simpleButton_cancel.Location = new System.Drawing.Point(409, 247);
            this.simpleButton_cancel.Name = "simpleButton_cancel";
            this.simpleButton_cancel.Size = new System.Drawing.Size(83, 28);
            this.simpleButton_cancel.TabIndex = 2;
            this.simpleButton_cancel.Text = "cancel";
            this.simpleButton_cancel.Click += new System.EventHandler(this.simpleButton_cancel_Click);
            // 
            // simpleButton_clear_name
            // 
            this.simpleButton_clear_name.Appearance.ForeColor = System.Drawing.Color.Red;
            this.simpleButton_clear_name.Appearance.Options.UseForeColor = true;
            this.simpleButton_clear_name.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_clear_name.ImageOptions.SvgImage")));
            this.simpleButton_clear_name.ImageOptions.SvgImageSize = new System.Drawing.Size(35, 35);
            this.simpleButton_clear_name.Location = new System.Drawing.Point(409, 58);
            this.simpleButton_clear_name.Name = "simpleButton_clear_name";
            this.simpleButton_clear_name.Size = new System.Drawing.Size(42, 30);
            this.simpleButton_clear_name.TabIndex = 4;
            this.simpleButton_clear_name.ToolTip = "Clear Name";
            this.simpleButton_clear_name.Click += new System.EventHandler(this.simpleButton_clear_name_Click);
            // 
            // simpleButton_clear_surname
            // 
            this.simpleButton_clear_surname.Appearance.ForeColor = System.Drawing.Color.Red;
            this.simpleButton_clear_surname.Appearance.Options.UseForeColor = true;
            this.simpleButton_clear_surname.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_clear_surname.ImageOptions.SvgImage")));
            this.simpleButton_clear_surname.ImageOptions.SvgImageSize = new System.Drawing.Size(35, 35);
            this.simpleButton_clear_surname.Location = new System.Drawing.Point(409, 154);
            this.simpleButton_clear_surname.Name = "simpleButton_clear_surname";
            this.simpleButton_clear_surname.Size = new System.Drawing.Size(42, 30);
            this.simpleButton_clear_surname.TabIndex = 4;
            this.simpleButton_clear_surname.ToolTip = "Clear Surname";
            this.simpleButton_clear_surname.Click += new System.EventHandler(this.simpleButton_clear_surname_Click);
            // 
            // simpleButton_clear_img
            // 
            this.simpleButton_clear_img.Appearance.ForeColor = System.Drawing.Color.Red;
            this.simpleButton_clear_img.Appearance.Options.UseForeColor = true;
            this.simpleButton_clear_img.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_clear_img.ImageOptions.SvgImage")));
            this.simpleButton_clear_img.ImageOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.simpleButton_clear_img.Location = new System.Drawing.Point(195, 12);
            this.simpleButton_clear_img.Name = "simpleButton_clear_img";
            this.simpleButton_clear_img.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.simpleButton_clear_img.Size = new System.Drawing.Size(25, 20);
            this.simpleButton_clear_img.TabIndex = 4;
            this.simpleButton_clear_img.ToolTip = "Clear image";
            this.simpleButton_clear_img.Click += new System.EventHandler(this.simpleButton_clear_img_Click);
            // 
            // simpleButton_print_card
            // 
            this.simpleButton_print_card.Cursor = System.Windows.Forms.Cursors.Hand;
            this.simpleButton_print_card.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_print_card.ImageOptions.SvgImage")));
            this.simpleButton_print_card.Location = new System.Drawing.Point(459, 225);
            this.simpleButton_print_card.Name = "simpleButton_print_card";
            this.simpleButton_print_card.Size = new System.Drawing.Size(38, 46);
            this.simpleButton_print_card.TabIndex = 7;
            this.simpleButton_print_card.ToolTip = "print";
            this.simpleButton_print_card.Visible = false;
            this.simpleButton_print_card.Click += new System.EventHandler(this.simpleButton_print_card_Click);
            // 
            // navigationFrame1
            // 
            this.navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
            this.navigationFrame1.Controls.Add(this.navigationPage1);
            this.navigationFrame1.Controls.Add(this.navigationPage2);
            this.navigationFrame1.Location = new System.Drawing.Point(12, 10);
            this.navigationFrame1.Name = "navigationFrame1";
            this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.navigationPage1,
            this.navigationPage2});
            this.navigationFrame1.SelectedPage = this.navigationPage2;
            this.navigationFrame1.Size = new System.Drawing.Size(515, 290);
            this.navigationFrame1.TabIndex = 8;
            this.navigationFrame1.Text = "navigationFrame1";
            // 
            // navigationPage1
            // 
            this.navigationPage1.Caption = "navigationPage1";
            this.navigationPage1.Controls.Add(this.simpleButton_print_card);
            this.navigationPage1.Controls.Add(this.pictureBox_card_preview);
            this.navigationPage1.Name = "navigationPage1";
            this.navigationPage1.Size = new System.Drawing.Size(515, 290);
            // 
            // pictureBox_card_preview
            // 
            this.pictureBox_card_preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_card_preview.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_card_preview.Name = "pictureBox_card_preview";
            this.pictureBox_card_preview.Size = new System.Drawing.Size(515, 290);
            this.pictureBox_card_preview.TabIndex = 6;
            this.pictureBox_card_preview.TabStop = false;
            this.pictureBox_card_preview.Visible = false;
            // 
            // navigationPage2
            // 
            this.navigationPage2.Caption = "navigationPage2";
            this.navigationPage2.Controls.Add(this.label_select_photo);
            this.navigationPage2.Controls.Add(this.simpleButton_clear_img);
            this.navigationPage2.Controls.Add(this.pictureBox_member_photo);
            this.navigationPage2.Controls.Add(this.simpleButton_open_camera);
            this.navigationPage2.Controls.Add(this.simpleButton_clear_surname);
            this.navigationPage2.Controls.Add(this.textEdit_name);
            this.navigationPage2.Controls.Add(this.textEdit_surname);
            this.navigationPage2.Controls.Add(this.simpleButton_clear_name);
            this.navigationPage2.Controls.Add(this.simpleButton_confirm);
            this.navigationPage2.Controls.Add(this.label_surname);
            this.navigationPage2.Controls.Add(this.simpleButton_cancel);
            this.navigationPage2.Controls.Add(this.label_name);
            this.navigationPage2.Name = "navigationPage2";
            this.navigationPage2.Size = new System.Drawing.Size(515, 290);
            // 
            // label_select_photo
            // 
            this.label_select_photo.AutoSize = true;
            this.label_select_photo.BackColor = System.Drawing.Color.White;
            this.label_select_photo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_select_photo.ForeColor = System.Drawing.Color.Black;
            this.label_select_photo.Location = new System.Drawing.Point(41, 239);
            this.label_select_photo.Name = "label_select_photo";
            this.label_select_photo.Size = new System.Drawing.Size(151, 19);
            this.label_select_photo.TabIndex = 8;
            this.label_select_photo.Text = "Click to select photo";
            this.label_select_photo.Click += new System.EventHandler(this.label_select_photo_Click);
            // 
            // pictureBox_member_photo
            // 
            this.pictureBox_member_photo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_member_photo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_member_photo.Image = global::WiseLib.Properties.Resources.person;
            this.pictureBox_member_photo.Location = new System.Drawing.Point(14, 10);
            this.pictureBox_member_photo.Name = "pictureBox_member_photo";
            this.pictureBox_member_photo.Size = new System.Drawing.Size(206, 265);
            this.pictureBox_member_photo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_member_photo.TabIndex = 0;
            this.pictureBox_member_photo.TabStop = false;
            this.pictureBox_member_photo.Click += new System.EventHandler(this.pictureBox_member_photo_Click);
            // 
            // simpleButton_open_camera
            // 
            this.simpleButton_open_camera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.simpleButton_open_camera.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.simpleButton_open_camera.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_open_camera.ImageOptions.SvgImage")));
            this.simpleButton_open_camera.Location = new System.Drawing.Point(234, 234);
            this.simpleButton_open_camera.Name = "simpleButton_open_camera";
            this.simpleButton_open_camera.Size = new System.Drawing.Size(51, 41);
            this.simpleButton_open_camera.TabIndex = 9;
            this.simpleButton_open_camera.ToolTip = "Take a picture\r\n";
            this.simpleButton_open_camera.ToolTipTitle = "Camera";
            this.simpleButton_open_camera.Click += new System.EventHandler(this.simpleButton_open_camera_Click);
            // 
            // textEdit_name
            // 
            this.textEdit_name.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textEdit_name.Location = new System.Drawing.Point(248, 58);
            this.textEdit_name.Name = "textEdit_name";
            this.textEdit_name.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.textEdit_name.Properties.Appearance.Options.UseFont = true;
            this.textEdit_name.Size = new System.Drawing.Size(155, 30);
            this.textEdit_name.TabIndex = 1;
            this.textEdit_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEdit_KeyPress);
            // 
            // textEdit_surname
            // 
            this.textEdit_surname.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textEdit_surname.Location = new System.Drawing.Point(248, 154);
            this.textEdit_surname.Name = "textEdit_surname";
            this.textEdit_surname.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.textEdit_surname.Properties.Appearance.Options.UseFont = true;
            this.textEdit_surname.Size = new System.Drawing.Size(155, 30);
            this.textEdit_surname.TabIndex = 1;
            this.textEdit_surname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEdit_KeyPress);
            // 
            // signup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 315);
            this.Controls.Add(this.navigationFrame1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("signup.IconOptions.SvgImage")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "signup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sign Up";
            this.Load += new System.EventHandler(this.signup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).EndInit();
            this.navigationFrame1.ResumeLayout(false);
            this.navigationPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_card_preview)).EndInit();
            this.navigationPage2.ResumeLayout(false);
            this.navigationPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_member_photo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_name.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_surname.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_member_photo;
        private DevExpress.XtraEditors.TextEdit textEdit_name;
        private DevExpress.XtraEditors.TextEdit textEdit_surname;
        private DevExpress.XtraEditors.SimpleButton simpleButton_confirm;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label_surname;
        private DevExpress.XtraEditors.SimpleButton simpleButton_cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_clear_name;
        private DevExpress.XtraEditors.SimpleButton simpleButton_clear_surname;
        private System.Windows.Forms.PictureBox pictureBox_card_preview;
        private DevExpress.XtraEditors.SimpleButton simpleButton_print_card;
        private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame1;
        private DevExpress.XtraBars.Navigation.NavigationPage navigationPage1;
        private DevExpress.XtraEditors.SimpleButton simpleButton_open_camera;
        private DevExpress.XtraBars.Navigation.NavigationPage navigationPage2;
        private System.Windows.Forms.Label label_select_photo;
        public DevExpress.XtraEditors.SimpleButton simpleButton_clear_img;
    }
}