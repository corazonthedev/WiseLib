using AForge.Video.DirectShow;
using DevExpress.Drawing;
using DevExpress.XtraEditors;
using System;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace WiseLib
{
    public partial class signup : DevExpress.XtraEditors.XtraForm
    {
        bool img_chosen = false;
        Timer timer = new Timer();
        
        public signup()
        { InitializeComponent(); }

        private void signup_Load(object sender, EventArgs e)
        {
            if (!Application.OpenForms.OfType<members>().Any())
            {
                navigationFrame1.SelectedPage = navigationPage2;
            }
            this.BeginInvoke((Action)(() => { textEdit_name.Focus(); })); //form wait&focus 
            set_timer();
            set_keydown_events();
        }
        private void set_timer()
        {
            timer.Interval = 250; //CHECK PHOTO IN 250ms
            timer.Tick += Timer_Tick_check_photo;
            timer.Start();
        }
        private void Timer_Tick_check_photo(object sender, EventArgs e) //FOR LABEL VISIBILITY
        {
            if (!img_chosen)
            { label_select_photo.Visible = true; }
            else
            { label_select_photo.Visible = false; }
        }
        //BUTTON EVENTS
        private void pictureBox_member_photo_Click(object sender, EventArgs e) //SELECT PHOTO
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                dialog.Title = "Select Member Photo";
                try
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox_member_photo.Image = Image.FromFile(dialog.FileName);
                        pictureBox_member_photo.SizeMode = PictureBoxSizeMode.StretchImage;
                        img_chosen = true;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is System.OutOfMemoryException) { XtraMessageBox.Show("Too large photo."); } //bigger than full-hd
                    else { main.LOG(ex); }
                }
            }
        }
        private void label_select_photo_Click(object sender, EventArgs e) //SELECT PHOTO TEXT CLICK
        { pictureBox_member_photo_Click(sender, e); }
        private void simpleButton_open_camera_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            signup_camera signup_camera_Form = new signup_camera();
            signup_camera_Form.FormClosed += (s, args) => this.Enabled = true; // activate when camera closed
            try
            {
                if (signup_camera.taken_pic != null)
                {
                    signup_camera_Form.FormClosed += (s, args) => pictureBox_member_photo.Image = signup_camera.taken_pic; //inherit taken_pic
                    img_chosen = true;
                }
            }
            catch (Exception ex)
            { main.LOG(ex); }
            signup_camera_Form.Show();
        }
        private void simpleButton_confirm_Click(object sender, EventArgs e) //CONFIRM
        {
            if (img_chosen)
            {
                if (!string.IsNullOrEmpty(textEdit_name.Text) && !string.IsNullOrEmpty(textEdit_surname.Text)) //EMPTY CHECK
                {
                    if (!textEdit_name.Text.Any(char.IsDigit) && !textEdit_name.Text.Any(char.IsDigit)) //DIGIT CHECK
                    {
                        textEdit_name.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(textEdit_name.Text.ToLower());
                        textEdit_surname.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(textEdit_surname.Text.ToLower());
                        string _query = $"SELECT member_name,member_surname FROM members WHERE member_name='{textEdit_name.Text}' AND member_surname='{textEdit_surname.Text}';";
                        using (SQLiteConnection conn1 = new SQLiteConnection(main.connectionString))
                        {
                            try
                            {
                                conn1.Open();
                                using (SQLiteCommand cmd1 = new SQLiteCommand(_query, conn1))
                                {
                                    object result = cmd1.ExecuteScalar();

                                    if (result != null)
                                    { XtraMessageBox.Show("Member already signed up", "Member exist", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                                    else
                                    {
                                        string query = $"INSERT INTO members('member_name','member_surname','join_date','borrow_status','credit_point','banned') VALUES('{textEdit_name.Text}', '{textEdit_surname.Text}', '{DateTime.Now.ToString("yyyy-MM-dd")}', 'no', 3, 0);";
                                        using (SQLiteConnection _conn = new SQLiteConnection(main.connectionString))
                                        {
                                            try
                                            {
                                                _conn.Open();
                                                using (SQLiteCommand _cmd = new SQLiteCommand(query, _conn))
                                                {
                                                    _cmd.ExecuteNonQuery();
                                                }

                                                _conn.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                main.LOG(ex);
                                                XtraMessageBox.Show("Create Member error " + ex.Message, "CREATE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        query = $"SELECT member_id FROM members WHERE member_name='{textEdit_name.Text}' AND member_surname='{textEdit_surname.Text}' AND join_date='{DateTime.Now.ToString("yyyy-MM-dd")}';";
                                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                                        {
                                            try
                                            {
                                                conn.Open();
                                                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                                using (SQLiteDataReader reader = cmd.ExecuteReader())
                                                {
                                                    if (reader.Read())
                                                    {
                                                        string member_id = reader["member_id"].ToString();
                                                        main.TLOG("2",member_id,"NULL");
                                                        conn.Close();
                                                        save_img_w_res(pictureBox_member_photo.Image, member_id);
                                                        GenerateCard(textEdit_name.Text, textEdit_surname.Text, member_id);
                                                    }
                                                }
                                                conn.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                main.LOG(ex);
                                                XtraMessageBox.Show("Member ID generate error " + ex.Message, "GENERATE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                }
                                conn1.Close();
                            }
                            catch (Exception ex)
                            {
                                main.LOG(ex);
                                XtraMessageBox.Show("Check Member error " + ex.Message, "CHECK ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    { XtraMessageBox.Show("Invalid Name/Surnmae", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                else
                { XtraMessageBox.Show("Enter Name/Surnmae", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
            { XtraMessageBox.Show("Select member image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void simpleButton_cancel_Click(object sender, EventArgs e) //CANCEL
        {
            DialogResult result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) //CHECK
            { this.Close(); }
        }
        private void simpleButton_clear_name_Click(object sender, EventArgs e) //CLEAR NAME
        { textEdit_name.Clear(); }
        private void simpleButton_clear_surname_Click(object sender, EventArgs e) //CLEAR SURNAME
        { textEdit_surname.Clear(); }
        public void simpleButton_clear_img_Click(object sender, EventArgs e) //CLEAR IMG - LOAD DEFAULT
        {
            pictureBox_member_photo.Image = Properties.Resources.person;
            img_chosen = false;
        }
        private void simpleButton_print_card_Click(object sender, EventArgs e) //CARDVIEW - PRINT 
        {
            Image imageToPrint = pictureBox_card_preview.Image; //select img

            PrintDocument printDocument = new PrintDocument(); //create print document
            printDocument.PrintPage += (s, args) =>
            {
                if (imageToPrint != null)
                { args.Graphics.DrawImage(imageToPrint, 0, 0, imageToPrint.Width, imageToPrint.Height); } //print img
            };

            PrintPreviewDialog previewDialog = new PrintPreviewDialog(); //SHOW PRINT PREVIEW
            previewDialog.Document = printDocument;
            this.Close(); //CLOSE SIGN-UP
            previewDialog.WindowState = FormWindowState.Maximized;
            previewDialog.ShowDialog();
        }
        //TEXTEDIT TAB/ENTER KEY EVENTS
        private void Control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
            if (sender == textEdit_name && e.KeyCode == Keys.Tab) textEdit_surname.Focus();
            else if (sender == textEdit_surname && e.KeyCode == Keys.Tab) simpleButton_confirm.Focus();
            else if (sender == textEdit_surname && e.KeyCode == Keys.Enter) simpleButton_confirm_Click(sender, e);
            else if (sender == simpleButton_confirm && e.KeyCode == Keys.Tab) simpleButton_cancel.Focus();
            else if (sender == simpleButton_cancel && e.KeyCode == Keys.Tab) textEdit_name.Focus();
        }
        private void set_keydown_events()
        {
            textEdit_name.PreviewKeyDown += Control_PreviewKeyDown;
            textEdit_surname.PreviewKeyDown += Control_PreviewKeyDown;
            simpleButton_confirm.PreviewKeyDown += Control_PreviewKeyDown;
            simpleButton_cancel.PreviewKeyDown += Control_PreviewKeyDown;
        }
        //LAST STEPS
        public static void save_img_w_res(Image originalImage, string member_id) //SAVE IMG WITH RESOLUTION & MEMBER NO
        {
            try
            {
                using (Bitmap resizedImage = new Bitmap(330, 490))
                {
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.DrawImage(originalImage, 0, 0, resizedImage.Width, resizedImage.Height);
                    }

                    string filePath = Path.Combine(main.appPath + "server\\imgs\\", $"{member_id}.jpg");
                    resizedImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg); //save img
                }
            }
            catch (Exception ex)
            {
                main.LOG(ex);
                XtraMessageBox.Show("Image save error " + ex.Message, "SAVE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void GenerateCard(string member_name, string member_surname, string member_id)
        {
            try
            {
                (int width, int height) = (587, 387);

                using (Bitmap bitmap = new Bitmap(width, height)) //create new bitmap
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // High graphic settings
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        g.Clear(Color.White); // White background

                        Pen blackPen = new Pen(Color.Black, 2); // Pen for border
                        Font textFont = new Font("Arial", 14, FontStyle.Regular); // Font for text
                        Brush textBrush = Brushes.Black; // Color for text

                        g.DrawRectangle(blackPen, 20, 20, width - 50, height - 50); // Outer card border

                        Rectangle photoBox = new Rectangle(45, 45, 206, 265); // Photo box in left
                        g.DrawRectangle(blackPen, photoBox);

                        if (pictureBox_member_photo.Image != null) // Get image in photo box
                        {
                            g.DrawImage(pictureBox_member_photo.Image, photoBox);
                        }

                        // Card texts
                        g.DrawString("Name: " + member_name, textFont, textBrush, new PointF(260, 60));
                        g.DrawString("Surname: " + member_surname, textFont, textBrush, new PointF(260, 100));
                        g.DrawString("Member ID: " + member_id, textFont, textBrush, new PointF(260, 140));

                        // Library text
                        g.DrawString(main.library_name, textFont, textBrush, new PointF(200, height - 65));


                        if (File.Exists(main.serverFolderPath+"logo.png")) // Check if file exists
                        {
                            Image logoImage = Image.FromFile(main.serverFolderPath + "logo.png"); // Load image from file
                            int squareSize = 120; // Size of the square
                            Rectangle squareBox = new Rectangle(width - 190, (height - squareSize) / 2 + 42, squareSize, squareSize); // Position of the square8

                            g.DrawRectangle(blackPen, squareBox); // Draw square border
                            g.DrawImage(logoImage, squareBox); // Draw image inside the square
                        }
                        else
                        {
                            // If the logo image is not found, display an error message (optional)
                            g.DrawString("Logo not found", textFont, Brushes.Red, new PointF(width - 120, (height - 20) / 2));
                        }
                    }
                    //save bitmap card img
                    string outputPath = $"{main.appPath}server\\imgs\\cards\\{member_id}.jpg";
                    bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //load card img to preview
                    pictureBox_card_preview.Image = (Bitmap)bitmap.Clone();
                    pictureBox_card_preview.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox_card_preview.Visible = true;
                    simpleButton_print_card.Visible = true; //print button
                    navigationFrame1.SelectedPage = navigationPage1;
                    bitmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                main.LOG(ex);
                XtraMessageBox.Show("Card generate error " + ex.Message, "GENERATE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void view_card(string member_id)
        {
            navigationFrame1.SelectedPage = navigationPage1;
            pictureBox_card_preview.Image = Image.FromFile(main.cardPath + member_id + ".jpg");   
            pictureBox_card_preview.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_card_preview.Visible = true;
            simpleButton_print_card.Visible = true; //print button            
        }

        private void textEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            { e.Handled = true; } //only letter
        }
    }
    
}