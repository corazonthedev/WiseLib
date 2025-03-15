namespace WiseLib
{
    partial class signup_camera
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(signup_camera));
            this.pictureBox_camera = new System.Windows.Forms.PictureBox();
            this.simpleButton_cancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_capture_photo = new DevExpress.XtraEditors.SimpleButton();
            this.comboBox_cameras = new System.Windows.Forms.ComboBox();
            this.simpleButton_tick_confirm = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_tick_cancel = new DevExpress.XtraEditors.SimpleButton();
            this.label_select_Error = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_camera)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_camera
            // 
            this.pictureBox_camera.Location = new System.Drawing.Point(28, 12);
            this.pictureBox_camera.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_camera.Name = "pictureBox_camera";
            this.pictureBox_camera.Size = new System.Drawing.Size(300, 490);
            this.pictureBox_camera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_camera.TabIndex = 0;
            this.pictureBox_camera.TabStop = false;
            // 
            // simpleButton_cancel
            // 
            this.simpleButton_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.simpleButton_cancel.Location = new System.Drawing.Point(28, 553);
            this.simpleButton_cancel.Name = "simpleButton_cancel";
            this.simpleButton_cancel.Size = new System.Drawing.Size(125, 27);
            this.simpleButton_cancel.TabIndex = 1;
            this.simpleButton_cancel.Text = "cancel";
            this.simpleButton_cancel.Click += new System.EventHandler(this.simpleButton_cancel_Click);
            // 
            // simpleButton_capture_photo
            // 
            this.simpleButton_capture_photo.Location = new System.Drawing.Point(132, 515);
            this.simpleButton_capture_photo.Name = "simpleButton_capture_photo";
            this.simpleButton_capture_photo.Size = new System.Drawing.Size(88, 23);
            this.simpleButton_capture_photo.TabIndex = 1;
            this.simpleButton_capture_photo.Text = "take photo";
            this.simpleButton_capture_photo.Click += new System.EventHandler(this.simpleButton_capture_photo_Click);
            // 
            // comboBox_cameras
            // 
            this.comboBox_cameras.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.comboBox_cameras.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBox_cameras.FormattingEnabled = true;
            this.comboBox_cameras.Location = new System.Drawing.Point(182, 553);
            this.comboBox_cameras.Name = "comboBox_cameras";
            this.comboBox_cameras.Size = new System.Drawing.Size(146, 27);
            this.comboBox_cameras.TabIndex = 3;
            this.comboBox_cameras.Text = "select camera";
            this.comboBox_cameras.SelectedIndexChanged += new System.EventHandler(this.comboBox_cameras_SelectedIndexChanged);
            // 
            // simpleButton_tick_confirm
            // 
            this.simpleButton_tick_confirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.simpleButton_tick_confirm.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.simpleButton_tick_confirm.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_tick_confirm.ImageOptions.SvgImage")));
            this.simpleButton_tick_confirm.Location = new System.Drawing.Point(102, 462);
            this.simpleButton_tick_confirm.Name = "simpleButton_tick_confirm";
            this.simpleButton_tick_confirm.Size = new System.Drawing.Size(26, 26);
            this.simpleButton_tick_confirm.TabIndex = 4;
            this.simpleButton_tick_confirm.Visible = false;
            this.simpleButton_tick_confirm.Click += new System.EventHandler(this.simpleButton_tick_confirm_Click);
            // 
            // simpleButton_tick_cancel
            // 
            this.simpleButton_tick_cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.simpleButton_tick_cancel.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.simpleButton_tick_cancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton_tick_cancel.ImageOptions.SvgImage")));
            this.simpleButton_tick_cancel.Location = new System.Drawing.Point(226, 462);
            this.simpleButton_tick_cancel.Name = "simpleButton_tick_cancel";
            this.simpleButton_tick_cancel.Size = new System.Drawing.Size(26, 26);
            this.simpleButton_tick_cancel.TabIndex = 4;
            this.simpleButton_tick_cancel.Visible = false;
            this.simpleButton_tick_cancel.Click += new System.EventHandler(this.simpleButton_tick_cancel_Click);
            // 
            // label_select_Error
            // 
            this.label_select_Error.AutoSize = true;
            this.label_select_Error.Location = new System.Drawing.Point(119, 236);
            this.label_select_Error.Name = "label_select_Error";
            this.label_select_Error.Size = new System.Drawing.Size(117, 19);
            this.label_select_Error.TabIndex = 5;
            this.label_select_Error.Text = "select a camera";
            // 
            // signup_camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.simpleButton_cancel;
            this.ClientSize = new System.Drawing.Size(359, 597);
            this.Controls.Add(this.label_select_Error);
            this.Controls.Add(this.simpleButton_tick_cancel);
            this.Controls.Add(this.simpleButton_tick_confirm);
            this.Controls.Add(this.comboBox_cameras);
            this.Controls.Add(this.simpleButton_capture_photo);
            this.Controls.Add(this.simpleButton_cancel);
            this.Controls.Add(this.pictureBox_camera);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("signup_camera.IconOptions.SvgImage")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "signup_camera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Take picture";
            this.Load += new System.EventHandler(this.signup_camera_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_camera)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_camera;
        private DevExpress.XtraEditors.SimpleButton simpleButton_cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton_capture_photo;
        private System.Windows.Forms.ComboBox comboBox_cameras;
        private DevExpress.XtraEditors.SimpleButton simpleButton_tick_confirm;
        private DevExpress.XtraEditors.SimpleButton simpleButton_tick_cancel;
        private System.Windows.Forms.Label label_select_Error;
    }
}