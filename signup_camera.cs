using AForge.Video;
using AForge.Video.DirectShow;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WiseLib
{
    public partial class signup_camera : DevExpress.XtraEditors.XtraForm
    {
        public signup_camera()
        {
            InitializeComponent();
        }

        private FilterInfoCollection videoDevices; //camera list
        private VideoCaptureDevice videoSource; //source
        private Bitmap capturedImage;
        public static Image taken_pic; //will be used in signup.cs 

        private void signup_camera_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice); //get input devices
            if (videoDevices.Count == 0)
            {
                XtraMessageBox.Show("CAMERA NOT FOUND", "CAMERA ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (FilterInfo device in videoDevices) //list cameras
            { comboBox_cameras.Items.Add(device.Name); }
        }
        private void simpleButton_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            var signupForm = Application.OpenForms["signup"] as signup;
            signupForm.simpleButton_clear_img_Click(sender,e);
        }
            
        private void comboBox_cameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning) //stop if working
            { videoSource.SignalToStop(); videoSource.WaitForStop(); }
            try
            {
                videoSource = new VideoCaptureDevice(videoDevices[comboBox_cameras.SelectedIndex].MonikerString); //start new camera
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
                label_select_Error.Visible = false;
                if (!videoSource.IsRunning)
                { XtraMessageBox.Show("CAMERA COULD NOT STARTED", "CAMERA ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch (Exception ex)
            { main.LOG(ex); }
        }
        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs) //CAMERA VIEW
        {
            Bitmap originalFrame = (Bitmap)eventArgs.Frame.Clone();
            originalFrame.RotateFlip(RotateFlipType.Rotate90FlipNone); //flip 90deg
            pictureBox_camera.Image = originalFrame; //show in picbox
        }
        private void simpleButton_capture_photo_Click(object sender, EventArgs e) //TAKE PHOTO
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                capturedImage = (Bitmap)pictureBox_camera.Image.Clone();
                Bitmap finalImage = new Bitmap(capturedImage, new Size(300, 490)); //re-size img
                (taken_pic, pictureBox_camera.Image) = (finalImage, finalImage); //set
                (simpleButton_capture_photo.Enabled, simpleButton_tick_confirm.Visible, simpleButton_tick_cancel.Visible) = (false, true, true); //set
            }
            else
            { XtraMessageBox.Show("CAMERA NOT WORKING", "CAMERA ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            if (videoSource != null && videoSource.IsRunning)
            { videoSource.SignalToStop(); videoSource.WaitForStop(); }  //stop camera
        }
        private void simpleButton_tick_confirm_Click(object sender, EventArgs e) //CONFIRM
        {
            if (taken_pic != null) { this.Close(); } //END
            else { XtraMessageBox.Show("NO PICTURE TAKEN", "PICTURE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void simpleButton_tick_cancel_Click(object sender, EventArgs e) //CANCEL - FORM.CLOSE
        {
            (simpleButton_capture_photo.Enabled, simpleButton_tick_confirm.Visible, simpleButton_tick_cancel.Visible) = (true, false, false);
            comboBox_cameras_SelectedIndexChanged(sender, e);
        }
    }
}
