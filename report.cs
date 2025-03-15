using DevExpress.CodeParser;
using DevExpress.Data.Mask.Internal;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Label = System.Windows.Forms.Label;
using NavigationPane = DevExpress.XtraBars.Navigation.NavigationPage;
namespace WiseLib
{
    public partial class report : DevExpress.XtraEditors.XtraForm
    {
        private int report_type; //0 general //1 members //2 books //99 missing //10 member report view //20 book report view 
        private string reported_no; //ISBN or MEMBER_ID
        long last_insert_id = 0; //LAST PROCESS
        public report(int _report_type, string _reported_no)
        {
            InitializeComponent(); report_type = _report_type;reported_no = _reported_no;
            this.KeyPreview = true; this.KeyDown += new KeyEventHandler(shortcuts);
        }
        private void report_Load(object sender, EventArgs e)
        {
            var WiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
            main.copy_grid(WiseLibForm.gridView1, gridView1);
            if (report_type == 0 && reported_no == "0") //ALL REPORTS
            {
                navigationFrame1.SelectedPage = navigationPage1;
                accordionControlElement1_Click(accordionControlElement1, e);
                foreach (var panel in new[] { accordionControl1 })
                { panel.Paint += main.Panel_Paint; }
            }
            else if (report_type == 1)//REPORT MEMBER
            {
                //LOAD FORM
                navigationFrame1.SelectedPage = navigationPage_report_member; this.Size = new Size(750, 425); this.Text = "Report Member"; //new size 
                this.StartPosition = FormStartPosition.Manual; this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Location = new Point( //center
                    (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                    (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
                //LOAD MEMBER INFO
                label_member_id.Text += reported_no;
                using (var connection = new SQLiteConnection(main.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT member_name,member_surname FROM members WHERE member_id = {reported_no};";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            { label_member_name.Text += reader["member_name"].ToString() + " " + reader["member_surname"]; }
                        }
                    }
                }
                //LOAD MEMBER IMG
                string imagePath = $"{main.imgPath}{reported_no}.jpg";
                using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(imagePath)))  //full path used?
                {
                    if (stream.Length > 0)
                    {
                        pictureBox_member_img.Image = Image.FromStream(stream);
                        pictureBox_member_img.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    { XtraMessageBox.Show("Member photo could not load", "LOAD ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
            else if (report_type == 2)//REPORT BOOK
            {
                //LOAD FORM
                navigationFrame1.SelectedPage = navigationPage_report_book; this.Size = new Size(750, 425); this.Text = "Report Book"; //new size 
                this.StartPosition = FormStartPosition.Manual; this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.Location = new Point( //center
                    (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                    (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
                //LOAD BOOK INFO
                label_book_isbn.Text += reported_no;
                using (var connection = new SQLiteConnection(main.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT title, author FROM books WHERE ISBN = '{reported_no}';";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                label_book_name.Text += reader["title"].ToString();
                                label_book_author.Text += reader["author"].ToString();
                            }
                        }
                    }
                }
                //LOAD BOOK IMG
                var imageUrl = "";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))//get book cover img
                {
                    List<string> img_list = new List<string> { "image_l", "image_m", "image_s" };
                    foreach (string img_x in img_list) //get img_l img_m img_s or create
                    {
                        try
                        {
                            conn.Open();

                            string isbn = new string(label_book_isbn.Text.Where(char.IsDigit).ToArray());

                            string query = $"SELECT {img_x} FROM books WHERE ISBN= '{isbn}'";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                imageUrl = cmd.ExecuteScalar()?.ToString();
                                if (!string.IsNullOrEmpty(imageUrl) && imageUrl.Contains("http")) //LINK VALID
                                {
                                    try
                                    {
                                        using (WebClient client = new WebClient())
                                        {
                                            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                                            byte[] imageData = client.DownloadData(imageUrl);
                                            using (MemoryStream stream = new MemoryStream(imageData))
                                            {

                                                pictureBox_book_cover.Image = Image.FromStream(stream);
                                                if (pictureBox_book_cover.Image.Height > 2)
                                                {
                                                    label_placeholder_book_title.Visible = false;
                                                    label_placeholder_book_author.Visible = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch (WebException ex)
                                    {
                                        main.LOG(ex);
                                        XtraMessageBox.Show("Book image web load error", "LOAD WEB ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        { main.LOG(ex); }
                    }
                    void create_book_cover()
                    {
                        string title = label_book_name.Text;
                        int maxLength = 20; //30char in every row
                        List<string> lines = new List<string>();
                        for (int i = 0; i < title.Length; i += maxLength) //30 char parts
                        { lines.Add(title.Substring(i, Math.Min(maxLength, title.Length - i))); }

                        System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
                        label_placeholder_book_title.Text = string.Join(Environment.NewLine, lines);
                        toolTip.SetToolTip(label_placeholder_book_title, label_placeholder_book_title.Text); //tooltip

                        label_placeholder_book_author.Text = label_book_author.Text;
                        toolTip.SetToolTip(label_placeholder_book_author, label_placeholder_book_author.Text); //tooltip

                        label_placeholder_book_title.Visible = true;
                        label_placeholder_book_author.Visible = true;
                    }
                    try
                    {
                        if (pictureBox_book_cover.Image.Height < 3) create_book_cover(); //IMG COULDN'T LOAD - CREATE BOOK COVER IMG
                    }
                    catch (Exception ex)
                    {
                        create_book_cover(); //necessary
                        main.LOG(ex);
                    }
                }
            }
            else if (report_type == 20)//VIEW BOOK REPORT
            {
                navigationFrame1.SelectedPage = navigationPage1;
                accordionControlElement2_Click(accordionControlElement2, e);

                string reported_book_report_no = string.Empty;
                using (var connection = new SQLiteConnection(main.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT ID FROM reports WHERE ISBN='{reported_no}';";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            { reported_book_report_no = reader["ID"].ToString(); }
                        }
                    }
                }
                gridView1.ApplyFindFilter(reported_book_report_no);
                simpleButton_up.PerformClick();
                simpleButton_view_Click(reported_book_report_no, e);
            }
            else if (report_type == 10)//VIEW MEMBER REPORT
            {
                navigationFrame1.SelectedPage = navigationPage1;
                accordionControlElement2_Click(accordionControlElement2, e);
                string reported_member_report_no = string.Empty;
                using (var connection = new SQLiteConnection(main.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT ID FROM reports WHERE NO='{reported_no}';";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            { reported_member_report_no = reader["ID"].ToString(); }
                        }
                    }
                }
                gridView1.ApplyFindFilter(reported_member_report_no);
                simpleButton_up.PerformClick();
                simpleButton_view_Click(reported_member_report_no, e);
                 
            }
            else if (report_type == 50) //CREATE AUTO MISSING REPORT
            {
                //LOAD FORM
                navigationFrame1.SelectedPage = navigationPage_report_book;
                //LOAD BOOK INFO
                label_book_isbn.Text += reported_no;
                using (var connection = new SQLiteConnection(main.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT title, author FROM books WHERE ISBN = '{reported_no}';";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                label_book_name.Text += reader["title"].ToString();
                                label_book_author.Text += reader["author"].ToString();
                            }
                        }
                    }
                }
                textEdit_book_title.Text = "Missing";
                memoEdit_book_desc.Text = "Missing";
                simpleButton_report_book_Click(null, null);
            }
            gridView1.Columns["SOLVE_DATE"].Caption = "SOLVE DATE";
            gridView1.Columns["CREATE_DATE"].Caption = "CREATE DATE";
        }
        private void navigation_arrows(object sender, EventArgs e)
        {
            SimpleButton button = sender as SimpleButton;
            if (button != null)
            {
                if (button.Name.Contains("up")) gridView1.MovePrev();
                else if (button.Name.Contains("down")) gridView1.MoveNext();
                else if (button.Name.Contains("right"))
                {
                    int selectedRowHandle = gridView1.FocusedRowHandle;
                    GridColumn selectedColumn = gridView1.FocusedColumn;
                    int columnIndex = gridView1.VisibleColumns.IndexOf(selectedColumn);
                    if (columnIndex == gridView1.VisibleColumns.Count - 1)
                    { gridView1.FocusedColumn = gridView1.VisibleColumns[0]; } //first
                    else
                    { gridView1.FocusedColumn = gridView1.VisibleColumns[columnIndex + 1]; } //right
                }
                else if (button.Name.Contains("left"))
                {
                    int selectedRowHandle = gridView1.FocusedRowHandle;
                    GridColumn selectedColumn = gridView1.FocusedColumn;
                    int columnIndex = gridView1.VisibleColumns.IndexOf(selectedColumn);
                    if (columnIndex == 0)
                    { gridView1.FocusedColumn = gridView1.VisibleColumns[gridView1.VisibleColumns.Count - 1]; } //last
                    else
                    { gridView1.FocusedColumn = gridView1.VisibleColumns[columnIndex - 1]; } //left
                }
            }
        }
        private void accordionControlElement1_Click(object sender, EventArgs e)//ALL REPORTS
        {
            foreach (var item in accordionControl1.Elements)
            {
                item.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#383838");
                item.Appearance.Normal.ForeColor = Color.White;}
            var clickedItem = sender as AccordionControlElement; 
            if (clickedItem != null)
            {
                clickedItem.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#888888");  
                clickedItem.Appearance.Normal.ForeColor = Color.White; }
            navigationFrame1.SelectedPage = navigationPage1;

            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM reports"; 
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable); 
                        gridControl1.DataSource = dataTable;
                        gridView1.RefreshData();
                    }
                }
            }
        }
        public void accordionControlElement2_Click(object sender, EventArgs e)//ACTIVE REPORTS
        {
            foreach (var item in accordionControl1.Elements)
            {
                item.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#383838");
                item.Appearance.Normal.ForeColor = Color.White;}
            var clickedItem = sender as AccordionControlElement;
            if (clickedItem != null)
            {
                clickedItem.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#888888");
                clickedItem.Appearance.Normal.ForeColor = Color.White;}
            navigationFrame1.SelectedPage = navigationPage1;

            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM reports WHERE SOLVED=0";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        gridControl1.DataSource = dataTable;
                    }
                }
            }
        }
        private void accordionControlElement3_Click(object sender, EventArgs e)//SOLVED REPORTS
        {
            foreach (var item in accordionControl1.Elements)
            {
                item.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#383838");
                item.Appearance.Normal.ForeColor = Color.White;}
            var clickedItem = sender as AccordionControlElement;
            if (clickedItem != null)
            {
                clickedItem.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#888888");
                clickedItem.Appearance.Normal.ForeColor = Color.White;}
            navigationFrame1.SelectedPage = navigationPage1;

            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM reports WHERE SOLVED=1";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        gridControl1.DataSource = dataTable;
                    }
                }
            }
        }
        private void accordionControlElement4_Click(object sender, EventArgs e)//MISSING REPORTS
        {
            foreach (var item in accordionControl1.Elements)
            {
                item.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#383838");
                item.Appearance.Normal.ForeColor = Color.White;}
            var clickedItem = sender as AccordionControlElement;
            if (clickedItem != null)
            {
                clickedItem.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#888888");
                clickedItem.Appearance.Normal.ForeColor = Color.White;}
            navigationFrame1.SelectedPage = navigationPage1;
            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM reports WHERE TYPE=99";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        gridControl1.DataSource = dataTable;
                    }
                }
            }
        }
        //-----ALL REPORTS--------------------------------------------------------------------------------------------------------------
        bool adding = false;
        private void simpleButton_add_Click(object sender, EventArgs e)
        {
            navigationFrame2.SelectedPage = navigationPage_edit;
            panel1.Enabled = false;
            adding = true;
            comboBoxEdit_edit_reportype.Properties.Items.Add("Book Report");
            comboBoxEdit_edit_reportype.Properties.Items.Add("Member Report");
            dateEdit_edit_create_date.EditValue = $"{DateTime.Now.ToString("dd-MM-yyyy")}";
            dateEdit_edit_solve_date.EditValue = null;

            comboBoxEdit_edit_member_isbn.SelectedIndex = 0;
            comboBoxEdit_edit_reportype.SelectedIndex = 0;

            textEdit_edit_title.Text = string.Empty;
            textEdit_edit_reportno.Text = string.Empty;
            memoEdit1.Text = string.Empty;
            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT COALESCE(MAX(id), 0) + 1 FROM reports", connection))
                {   int newId = Convert.ToInt32(command.ExecuteScalar());
                    textEdit_edit_reportno.Text=newId.ToString();}}
        }
        private void simpleButton_view_Click(object sender, EventArgs e)
        {
            string report_no = string.Empty;
            if (sender is string) report_no = sender.ToString(); 
            else
            {
                int selected_row = gridView1.FocusedRowHandle;
                report_no = gridView1.GetRowCellValue(selected_row, "ID")?.ToString();
            }
            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM reports WHERE ID={report_no}";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            label_report_no.Text = reader["ID"].ToString();
                            string report_type = reader["TYPE"].ToString();

                            if (report_type == "1") report_type = "Member Report";
                            else if (report_type == "2") report_type = "Book Report";
                            else if (report_type == "99") report_type = "Missing Report";
                            label_report_type.Text = report_type;

                            if (reader["NO"].ToString() == "")
                            { label_report_type_text.Text = "Reported Book ISBN: ";
                              label_reported_isbn_no.Text = reader["ISBN"].ToString();
                              simpleButton_copy_member_isbn.ToolTip = "Copy ISBN";
                                simpleButton_view_member_book.ToolTip = "View Book";}

                            if (reader["ISBN"].ToString() == "")
                            { label_report_type_text.Text = "Reported Member ID: ";
                                label_reported_isbn_no.Text = reader["NO"].ToString();
                                simpleButton_copy_member_isbn.ToolTip = "Copy Member ID";
                                simpleButton_view_member_book.ToolTip = "View Member";}


                            label_title.Text = reader["TITLE"].ToString();


                            string create_date = DateTime.ParseExact(reader["CREATE_DATE"].ToString(), "yyyy-MM-dd", null).ToString("dd-MM-yyyy");
                            label_create_date.Text = create_date;
                            if (reader["SOLVED"].ToString() == "1")
                            {
                                string solve_date = DateTime.ParseExact(reader["SOLVE_DATE"].ToString(), "yyyy-MM-dd", null).ToString("dd-MM-yyyy");
                                label_solve_date.Text = solve_date;
                            }
                            
                            label_desc.Text = reader["DESC"].ToString();

                            if (reader["SOLVED"].ToString() == "0") label_status.Text = "Active";
                            else if (reader["SOLVED"].ToString() == "1") label_status.Text = "Solved";

                            if (reader["SOLVE_DATE"].ToString() == "") label_solve_date.Text = "Not Solved Yet";
                            else label_solve_date.Text = reader["SOLVE_DATE"].ToString();

                        }
                        panel1.Enabled = false;
                        navigationFrame2.SelectedPage = navigationPage_view;
                    }
                }
            }

        }
        private void simpleButton_edit_Click(object sender, EventArgs e)
        {
            if (sender is SimpleButton btn && btn.Name == "simpleButton_edit2") { } //nothing
            else simpleButton_view_Click(sender, e);

            textEdit_edit_reportno.Text = label_report_no.Text;

            comboBoxEdit_edit_reportype.Properties.Items.Clear();
            comboBoxEdit_edit_reportype.Properties.Items.Add("Book Report");
            comboBoxEdit_edit_reportype.Properties.Items.Add("Member Report");

            label_edit_report_type_text.Text = label_report_type_text.Text;

            if (label_report_type.Text.Contains("Book"))
            {
                comboBoxEdit_edit_reportype.SelectedIndex = 0;
                simpleButton18.ToolTip = "Restore Book";
            }
            else if (label_report_type.Text.Contains("Member"))
            {
                comboBoxEdit_edit_reportype.SelectedIndex = 1;
                simpleButton18.ToolTip = "Restore Member";
            }

            comboBoxEdit_edit_member_isbn.Properties.Items.Clear();
            comboBoxEdit_edit_member_isbn.Properties.Items.Add(label_reported_isbn_no.Text);
            if (label_report_type.Text.Contains("Member")) comboBoxEdit_edit_member_isbn.Properties.Items.Add("Select Member");
            else if (label_report_type.Text.Contains("Book")) comboBoxEdit_edit_member_isbn.Properties.Items.Add("Select Book");

            comboBoxEdit_edit_member_isbn.SelectedIndex = 0;

            dateEdit_edit_create_date.EditValue = DateTime.ParseExact(label_create_date.Text, "dd-MM-yyyy", null);
            dateEdit_edit_create_date.Enabled = false;
            dateEdit_edit_create_date.EditValue = DateTime.ParseExact(label_create_date.Text, "dd-MM-yyyy", null);
            dateEdit_edit_solve_date.Enabled = false;

            textEdit_edit_title.Text = label_title.Text;

            label_edit_report_status.Text = label_status.Text;

            memoEdit1.Text = label_desc.Text;

            navigationFrame2.SelectedPage = navigationPage_edit;
            panel1.Enabled = false;
        }
        private void simpleButton_delete_Click(object sender, EventArgs e)
        {
            var view = gridView1;
            if (view.FocusedRowHandle < 0) return; 

            object idValue = view.GetRowCellValue(view.FocusedRowHandle, "ID"); 

            var result = XtraMessageBox.Show($"Are you sure to delete report {idValue} ?", "Delete report", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (idValue != null)
                {
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        string query = $"DELETE FROM reports WHERE ID={idValue}";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            object member_no = view.GetRowCellValue(view.FocusedRowHandle, "NO");
                            object isbn = view.GetRowCellValue(view.FocusedRowHandle, "ISBN");

                            int a = command.ExecuteNonQuery();
                            if (a > 0) 
                            {
                                XtraMessageBox.Show("Report Deleted"); view.DeleteRow(view.FocusedRowHandle); main.TLOG("4-4", label_report_no.Text, label_report_no.Text);
                                object report_type = view.GetRowCellValue(view.FocusedRowHandle, "TYPE");
                                if (report_type.ToString() == "1") //Book
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        
                                        query = $"UPDATE books SET has_problem = 0 WHERE ISBN='{isbn.ToString()}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { a = cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                else if (report_type.ToString() == "2") //Member
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        
                                        query = $"UPDATE members SET credit_point=credit_point+1 WHERE member_id='{member_no.ToString()}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { a = cmnd.ExecuteNonQuery(); }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //------REPORT MEMBER-----------------------------------------------------------------------------------------------------------
        private void simpleButton_report_member_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit_title.Text))
            { XtraMessageBox.Show("Please enter TITLE","",MessageBoxButtons.OK,MessageBoxIcon.Error); }
            else if (string.IsNullOrEmpty(memoEdit_desc.Text))
            { XtraMessageBox.Show("Please enter DESCRIPTION", "", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            {
                var result = XtraMessageBox.Show("Are you sure to create new report?","Create new report",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        string query = $"INSERT INTO reports(TYPE,NO,TITLE,DESC,SOLVED,CREATE_DATE) VALUES({report_type}, '{reported_no}',@title,@desc,0,'{DateTime.Now.ToString("yyyy-MM-dd")}');";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@title", textEdit_title.Text);
                            command.Parameters.AddWithValue("@desc", memoEdit_desc.Text);
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0) 
                            {
                                last_insert_id = connection.LastInsertRowId;

                                navigationFrame1.SelectedPage = navigationPage1;
                                accordionControlElement2_Click(accordionControlElement2, e);
                                gridView1.ApplyFindFilter(last_insert_id.ToString());
                                simpleButton_view_Click(sender, e);
                                this.Size = new Size(1220,750); //default size
                                this.Location = new Point( //center
                                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
                                main.TLOG("4-1",reported_no,"");
                            }
                        }
                        query = $"UPDATE members SET credit_point=credit_point-1 WHERE member_id='{reported_no}';";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            int a = command.ExecuteNonQuery();
                            if (a > 0) XtraMessageBox.Show("Member point decreased");
                        }
                        connection.Close();
                    }
                }
            }
        }
        private void simpleButton_cancel_member_Click(object sender, EventArgs e)
        {
            var membersForm = Application.OpenForms.OfType<members>().FirstOrDefault();
            var booksForm = Application.OpenForms.OfType<books>().FirstOrDefault();

            var result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) 
            {
                if (membersForm != null) //back to members
                {   result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes) { this.Close(); }}
                else if (booksForm != null) //back to members
                {   result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes) { this.Close(); }}
                else {navigationFrame1.SelectedPage = navigationPage1; this.Size = new Size(1220, 750); } //back to main
            }
        }
        //-----REPORT BOOK--------------------------------------------------------------------------------------------------------------

        //-----VIEW SINGLE REPORT-------------------------------------------------------------------------------------------------------    
        private void simpleButton_copy_report_no_Click(object sender, EventArgs e)//COPY REPORT NO
        {
            Clipboard.SetText(label_report_no.Text);
            var label = new Label
            {
                BackColor = Color.Gray,
                Text = $"REPORT NO COPIED {label_report_no.Text}",
                Font = new Font("Tahoma", 16),
                ForeColor = Color.White,

                AutoSize = true,
            };
            navigationPage_view.Controls.Add(label);

            Point buttonLocation = navigationPage_view.PointToClient(simpleButton_copy_report_no.Parent.PointToScreen(simpleButton_copy_report_no.Location));
            label.Location = new Point(buttonLocation.X - 65, buttonLocation.Y + simpleButton_copy_report_no.Height + 5);

            label.BringToFront();

            Task.Delay(2000).ContinueWith(t => { navigationPage_view.Invoke(new Action(() => navigationPage_view.Controls.Remove(label))); });//delete after 2s
        }
        public void simpleButton_view_member_book_Click(object sender, EventArgs e)//VIEW MEMBER/BOOK
        {
            if (label_report_type.Text.Contains("Book") || label_report_type.Text.Contains("Missing")) 
            {
                books booksForm = new books(); //books
                booksForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed

                booksForm.textEdit_search_book.Text = label_reported_isbn_no.Text;
                booksForm.simpleButton_search_book_Click(sender, e);
                booksForm.simpleButton_view_book_Click(sender, e);

                booksForm.ShowDialog();
            }
            else if (label_report_type.Text.Contains("Member"))
            {
                members membersForm = new members(); //books
                membersForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed
                
                membersForm.textEdit_search_member.Text = label_reported_isbn_no.Text;
                membersForm.simpleButton_search_member_Click(sender, e);
                membersForm.simpleButton_view_member_Click(sender, e);
                membersForm.ShowDialog();
            }
        }
        private void simpleButton_copy_member_isbn_Click(object sender, EventArgs e)//COPY ISBN/NO
        {
            Clipboard.SetText(label_reported_isbn_no.Text);

            var label = new Label { };
            if (label_report_type.Text.Contains("Book") || label_report_type.Text.Contains("Missing"))
            {
                label = new Label
                {
                    BackColor = Color.Gray,
                    Text = $"ISBN NO COPIED {label_reported_isbn_no.Text}",
                    Font = new Font("Tahoma", 16),
                    ForeColor = Color.White,

                    AutoSize = true,
                };
            }
            else if (label_report_type.Text.Contains("Member"))
            {
                label = new Label
                {
                    BackColor = Color.Gray,
                    Text = $"MEMBER ID COPIED {label_reported_isbn_no.Text}",
                    Font = new Font("Tahoma", 16),
                    ForeColor = Color.White,

                    AutoSize = true,
                };
            }
            navigationPage_view.Controls.Add(label);

            Point buttonLocation = navigationPage_view.PointToClient(simpleButton_copy_member_isbn.Parent.PointToScreen(simpleButton_copy_member_isbn.Location));
            label.Location = new Point(buttonLocation.X - 65, buttonLocation.Y + simpleButton_copy_member_isbn.Height + 5);

            label.BringToFront();

            Task.Delay(2000).ContinueWith(t => { navigationPage_view.Invoke(new Action(() => navigationPage_view.Controls.Remove(label))); });//delete after 2s

        }
        private void simpleButton_delete2_Click(object sender, EventArgs e)//DELETE SINGLE
        {
            var result = XtraMessageBox.Show("Are you sure to delete this report?", "Deleting report", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        string query = $"DELETE FROM reports WHERE id = {label_report_no.Text}"; // Silme sorgusu
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                if (label_report_type.Text.Contains("Book")) //Book
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        query = $"UPDATE books SET has_problem = 0 WHERE ISBN='{label_reported_isbn_no.Text}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                else if (label_report_type.Text.Contains("Member")) //Member
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        query = $"UPDATE members SET credit_point=credit_point+1 WHERE member_id='{label_reported_isbn_no.Text}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                XtraMessageBox.Show("Report deleted", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                simpleButton_back_Click(sender, e);
                                main.TLOG("4-4", label_report_no.Text, label_report_no.Text);
                            }
                            else XtraMessageBox.Show("Error deleting report", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Error deleting report", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    main.LOG(ex);
                }

            }
        }
        private void simpleButton_edit2_Click(object sender, EventArgs e)//EDIT
        {
            simpleButton_edit_Click(sender, e);
            navigationFrame2.SelectedPage = navigationPage_edit;
            panel1.Enabled = false;
        }
        private void simpleButton_print_Click(object sender, EventArgs e)//PRINT
        {
            XtraMessageBox.Show("this suppose to print out your report as a4 page");//TODO
        }
        private void simpleButton_back_Click(object sender, EventArgs e)//BACK
        {
            accordionControlElement1_Click(accordionControlElement1, e);
            navigationFrame2.SelectedPage = navigationPage_grid;
            panel1.Enabled = true;
            gridView1.FindFilterText = string.Empty;
        }
        private void simpleButton_set_status_Click(object sender, EventArgs e)//SET STATUS
        {
            if (label_status.Text == "Active")
            {
                var result = XtraMessageBox.Show("Are you sure to set report status as SOLVED ?","Set report status",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.Yes) 
                {

                    try
                    {
                        using (var connection = new SQLiteConnection(main.connectionString))
                        {
                            connection.Open();
                            string query = $"UPDATE reports SET SOLVED = 1, SOLVE_DATE = '{DateTime.Now.ToString("yyyy-MM-dd")}' WHERE ID = {label_report_no.Text}";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                int a = command.ExecuteNonQuery();
                                if (label_report_type.Text.Contains("Book")) //Book
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        query = $"UPDATE books SET has_problem = 0 WHERE ISBN='{label_reported_isbn_no.Text}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                else if (label_report_type.Text.Contains("Member")) //Member
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        query = $"UPDATE members SET credit_point=credit_point+1 WHERE member_id='{label_reported_isbn_no.Text}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                if (a > 0) XtraMessageBox.Show("Report status set as SOLVED");
                                main.TLOG("4-5", label_report_no.Text, label_report_no.Text);
                                label_status.Text = "Solved";
                                label_solve_date.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            }
                        }
                    }
                    catch (Exception ex) { main.LOG(ex); }
                }
            }
            else if (label_status.Text == "Solved")
            {
                var result = XtraMessageBox.Show("Are you sure to set report status as ACTIVE ?", "Set report status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var connection = new SQLiteConnection(main.connectionString))
                        {
                            connection.Open();
                            string query = $"UPDATE reports SET SOLVED = 0, SOLVE_DATE = NULL WHERE ID = {label_report_no.Text}";

                            using (var command = new SQLiteCommand(query, connection))
                            {
                                int a = command.ExecuteNonQuery();
                                if (a > 0) XtraMessageBox.Show("Report status set as ACTIVE");
                                if (label_report_type.Text.Contains("Book")) //Book
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        query = $"UPDATE books SET has_problem = 1 WHERE ISBN='{label_reported_isbn_no.Text}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                else if (label_report_type.Text.Contains("Member")) //Member
                                {
                                    using (var cn1 = new SQLiteConnection(main.connectionString))
                                    {
                                        cn1.Open();
                                        query = $"UPDATE members SET credit_point=-1 WHERE member_id='{label_reported_isbn_no.Text}'";
                                        using (var cmnd = new SQLiteCommand(query, cn1))
                                        { cmnd.ExecuteNonQuery(); }
                                    }
                                }
                                main.TLOG("4-6", label_report_no.Text, label_report_no.Text);
                                label_status.Text = "Active";
                                label_solve_date.Text = "Not Solved Yet";
                            }
                        }
                    }
                    catch (Exception ex) { main.LOG(ex); }
                }
            }
        }
        //-----EDIT SINGLE----------------------------------------------------------------------------------------------------------------
        private void clear_textedit(object sender, EventArgs e)
        {
            if (sender is SimpleButton btn)
            {
                if (btn.ToolTip.Contains("No")) textEdit_edit_reportno.Clear();

                else if (btn.ToolTip.Contains("Create")) dateEdit_edit_create_date.Text = "00-01-0000";
                else if (btn.ToolTip.Contains("Solve")) dateEdit_edit_solve_date.Text = "00-00-0000";

                else if (btn.ToolTip.Contains("Title")) textEdit_edit_title.Clear();
                else if (btn.ToolTip.Contains("Description")) memoEdit1.Clear();
            }
        }
        private void restore_textedit(object sender, EventArgs e)
        {
            if (sender is SimpleButton btn)
            {
                if (btn.ToolTip.Contains("No")) textEdit_edit_reportno.Text = label_report_no.Text;
                else if (btn.ToolTip.Contains("Type"))
                {
                    if (label_report_type.Text.Contains("Book")) comboBoxEdit_edit_reportype.SelectedIndex = 0;
                    else if (label_report_type.Text.Contains("Member")) comboBoxEdit_edit_reportype.SelectedIndex = 1;
                }

                else if (btn.ToolTip.Contains("Create")) dateEdit_edit_create_date.Text = label_create_date.Text;
                else if (btn.ToolTip.Contains("Solve")) dateEdit_edit_solve_date.Text = label_solve_date.Text;

                else if (btn.ToolTip.Contains("Title")) textEdit_edit_title.Text = label_title.Text;
                else if (btn.ToolTip.Contains("Description")) memoEdit1.Text = label_desc.Text;

                else if (btn.ToolTip.Contains("Book")) comboBoxEdit_edit_member_isbn.SelectedItem = label_reported_isbn_no.Text;
                else if (btn.ToolTip.Contains("Member")) comboBoxEdit_edit_member_isbn.SelectedItem = label_reported_isbn_no.Text;
            }
        }
        private void simpleButton_edit_cancel_Click(object sender, EventArgs e)
        {
            navigationFrame2.SelectedPage = navigationPage_grid;
            panel1.Enabled = true;
        }
        private void simpleButton_edit_report_status_Click(object sender, EventArgs e)
        {
            if (label_edit_report_status.Text == "Active")
            {
                var result = XtraMessageBox.Show("Are you sure to set report status as SOLVED ?", "Set report status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) label_edit_report_status.Text = "Solved";
                simpleButton_edit_report_solve_date_Click(sender, e);
            }
            else if (label_edit_report_status.Text == "Solved")
            {
                var result = XtraMessageBox.Show("Are you sure to set report status as ACTIVE ?", "Set report status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) label_edit_report_status.Text = "Active";
                dateEdit_edit_solve_date.Clear();
                dateEdit_edit_solve_date.Enabled = false;
            }
        }
        private void simpleButton_edit_delete_Click(object sender, EventArgs e)
        {simpleButton_delete2_Click(sender, e);}
        private void simpleButton_copy_row_Click(object sender, EventArgs e)
        {
            var rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle >= 0)
            {
                var rowValues = gridView1.Columns
                    .Select(col => gridView1.GetRowCellValue(rowHandle, col)?.ToString())
                    .Where(val => val != null);

                Clipboard.SetText(string.Join("\t", rowValues));
            }

            var label = new Label
            {
                BackColor = Color.Gray,
                Text = $"ROW\nCOPIED",
                Font = new Font("Tahoma", 14),
                ForeColor = Color.White,
                AutoSize = true,
            };
            panel1.Controls.Add(label);
            Point buttonLocation = panel1.PointToClient(simpleButton_copy_row.Parent.PointToScreen(simpleButton_copy_row.Location));
            label.Location = new Point(buttonLocation.X, buttonLocation.Y + simpleButton_copy_row.Height + 5);

            label.BringToFront();

            Task.Delay(2000).ContinueWith(t => { panel1.Invoke(new Action(() => panel1.Controls.Remove(label))); });//delete after 2s
        }
        private void simpleButton_copy_cell_Click(object sender, EventArgs e)
        {
            var cellValue = gridView1.GetFocusedValue()?.ToString();
            if (!string.IsNullOrEmpty(cellValue))
            {Clipboard.SetText(cellValue);}

            var label = new Label
            {
                BackColor = Color.Gray,
                Text = $"CELL\nCOPIED",
                Font = new Font("Tahoma", 14),
                ForeColor = Color.White,
                AutoSize = true,
            };
            panel1.Controls.Add(label);
            Point buttonLocation = panel1.PointToClient(simpleButton_copy_cell.Parent.PointToScreen(simpleButton_copy_cell.Location));
            label.Location = new Point(buttonLocation.X, buttonLocation.Y+20);

            label.BringToFront();

            Task.Delay(2000).ContinueWith(t => { panel1.Invoke(new Action(() => panel1.Controls.Remove(label))); });//delete after 2s
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "SOLVED" && e.Value?.ToString() == "0") { e.DisplayText = "no"; }
            else if (e.Column.FieldName == "SOLVED" && e.Value?.ToString() == "1") { e.DisplayText = "yes"; }

            if (e.Column.FieldName == "TYPE" && e.Value?.ToString() == "1") { e.DisplayText = "Member Report"; }
            else if (e.Column.FieldName == "TYPE" && e.Value?.ToString() == "2") { e.DisplayText = "Book Report"; }
            else if (e.Column.FieldName == "TYPE" && e.Value?.ToString() == "99") { e.DisplayText = "Missing Report"; }
        }
        private void dynamic_selector(string member_or_book)
        {
            Form form = new Form();
            form.Size = new System.Drawing.Size(400, 700);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;   

            DevExpress.XtraGrid.GridControl gridControl = new DevExpress.XtraGrid.GridControl();
            gridControl.Dock = DockStyle.Fill;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridControl.MainView = gridView;
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsCustomization.AllowGroup = false;
            gridView.OptionsFind.AllowFindPanel = true;
            gridView.OptionsFind.AlwaysVisible = true;
            form.Controls.Add(gridControl);

            string query = string.Empty;
            if (member_or_book == "Member")
            { query = "SELECT member_id, member_name, member_surname, banned FROM members"; form.Text = "Select Member"; }
            else if (member_or_book == "Book")
            { query = "SELECT ISBN,title, author FROM books"; form.Text = "Select Book"; }

            using (var connection = new SQLiteConnection(main.connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable); 
                        gridControl.DataSource = dataTable;
                    }
                }
            }

            gridView.DoubleClick += (senderr, ee) => //double click event
            {
                if (member_or_book == "Member")
                {
                    string member_id = gridView.GetFocusedRowCellValue("member_id")?.ToString();

                    var items = comboBoxEdit_edit_member_isbn.Properties.Items;
                    //if (items.Count > 1) items.Insert(items.Count - 1, member_id); //add as last 2nd
                    comboBoxEdit_edit_member_isbn.Properties.Items.Add(member_id);

                    comboBoxEdit_edit_member_isbn.SelectedItem = member_id.ToString();
                    form.Close();
                }
                else if (member_or_book == "Book")
                {
                    string book_isbn = gridView.GetFocusedRowCellValue("ISBN")?.ToString();

                    var items = comboBoxEdit_edit_member_isbn.Properties.Items;
                    //if (items.Count > 1) items.Insert(items.Count - 1, book_isbn); //add as last 2nd
                    comboBoxEdit_edit_member_isbn.Properties.Items.Add(book_isbn);

                    comboBoxEdit_edit_member_isbn.SelectedItem = book_isbn.ToString();
                    form.Close();
                }
            };
            form.ShowDialog();
        }
        private void comboBoxEdit_edit_member_isbn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_edit_member_isbn.SelectedIndex != 0)
            {
                if (comboBoxEdit_edit_member_isbn.SelectedItem.ToString().Contains("Book")) dynamic_selector("Book");
                else if (comboBoxEdit_edit_member_isbn.SelectedItem.ToString().Contains("Member")) dynamic_selector("Member");
            } 
        }
        private void comboBoxEdit_edit_member_isbn_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboBoxEdit_edit_member_isbn.SelectedItem.ToString().Contains("Member")) dynamic_selector("Member");
            else if (comboBoxEdit_edit_member_isbn.SelectedItem.ToString().Contains("Book")) dynamic_selector("Book");
        }
        private void simpleButton_edit_report_create_date_Click(object sender, EventArgs e)
        {
            var result = XtraMessageBox.Show($"Are you sure to edit CREATE date?", "Delete report", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                dateEdit_edit_create_date.Enabled = true;
                dateEdit_edit_create_date.ShowPopup();
            }
        }
        private void simpleButton_edit_report_solve_date_Click(object sender, EventArgs e)
        {
            var result = XtraMessageBox.Show($"Are you sure to edit SOLVE date?", "Delete report", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                dateEdit_edit_solve_date.Enabled = true;
                dateEdit_edit_solve_date.ShowPopup();
            }   
        }
        private void simpleButton_edit_confirm_Click(object sender, EventArgs e)
        {
            void add()
            {
                try
                {
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        string r_type, no, solved, solve_date, isbn;
                        r_type = no = solved = solve_date = isbn = string.Empty;

                        if (comboBoxEdit_edit_reportype.Text.Contains("Member"))
                        { r_type = "1"; no = comboBoxEdit_edit_member_isbn.SelectedItem.ToString(); }
                        else if (comboBoxEdit_edit_reportype.Text.Contains("Book"))
                        { r_type = "2"; isbn = comboBoxEdit_edit_member_isbn.SelectedItem.ToString(); }

                        if (label_edit_report_status.Text.Contains("Solved"))
                        { solved = "1"; solve_date = dateEdit_edit_solve_date.DateTime.ToString("yyyy-MM-dd"); }
                        else
                        { solved = "0"; solve_date = ""; }

                        string query = "INSERT INTO reports (TYPE, NO, ISBN, TITLE, DESC, SOLVED, CREATE_DATE, SOLVE_DATE) " +
                                       "VALUES (@type, @no, @isbn, @title, @desc, @solved, @create_date, @solve_date)";

                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@type", r_type);
                            command.Parameters.AddWithValue("@no", string.IsNullOrEmpty(no) ? (object)DBNull.Value : no);
                            command.Parameters.AddWithValue("@isbn", string.IsNullOrEmpty(isbn) ? (object)DBNull.Value : isbn);
                            command.Parameters.AddWithValue("@title", textEdit_edit_title.Text);
                            command.Parameters.AddWithValue("@desc", memoEdit1.Text);
                            command.Parameters.AddWithValue("@solved", solved);
                            command.Parameters.AddWithValue("@create_date", dateEdit_edit_create_date.DateTime.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@solve_date", string.IsNullOrEmpty(solve_date) ? (object)DBNull.Value : solve_date);

                            int a = command.ExecuteNonQuery();
                            if (a > 0)
                            {
                                XtraMessageBox.Show("SUCCESS");
                                accordionControlElement1_Click(accordionControlElement1, e);
                                gridView1.ApplyFindFilter(textEdit_edit_reportno.Text);
                                simpleButton_view_Click(sender, e);
                                main.TLOG("4-2", "", "");
                            }
                        }
                        if (!string.IsNullOrEmpty(isbn))
                        {
                            string has_problem = solved == "1" ? "0" : "1";
                            query = $"UPDATE books SET has_problem={has_problem} WHERE ISBN='{isbn}'";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                int a = command.ExecuteNonQuery();
                                if (a > 0) XtraMessageBox.Show("UPDATED");
                            }
                        }
                    }
                }
                catch (Exception ex) { main.LOG(ex); XtraMessageBox.Show("FAILED TO ADD"); }
            }
            void update()
            {
                try
                {
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        string r_type, no, solved, solve_date, isbn;
                        r_type = no = solved = solve_date = isbn = string.Empty;

                        if (comboBoxEdit_edit_reportype.Text.Contains("Member"))
                        { r_type = "1"; no = comboBoxEdit_edit_member_isbn.SelectedItem.ToString(); }
                        else if (comboBoxEdit_edit_reportype.Text.Contains("Book"))
                        { r_type = "2"; isbn = comboBoxEdit_edit_member_isbn.SelectedItem.ToString(); }

                        if (label_edit_report_status.Text.Contains("Solved"))
                        { solved = "1"; solve_date = dateEdit_edit_solve_date.DateTime.ToString("yyyy-MM-dd"); }
                        else
                        { solved = "0"; solve_date = ""; }

                        string query = $"UPDATE reports SET ID={textEdit_edit_reportno.Text}, TYPE='{r_type}', NO='{no}'," +
                            $" TITLE=@title, DESC=@desc, SOLVED='{solved}', CREATE_DATE='{dateEdit_edit_create_date.DateTime.ToString("yyyy-MM-dd")}'," +
                            $" SOLVE_DATE='{solve_date}',";

                        if (!string.IsNullOrEmpty(no)) query += $"NO = {no} WHERE ID={label_report_no.Text}";
                        else if (!string.IsNullOrEmpty(isbn)) query += $"ISBN = {isbn} WHERE ID={label_report_no.Text}";


                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@title", textEdit_edit_title.Text);
                            command.Parameters.AddWithValue("@desc", memoEdit1.Text);
                            int a = command.ExecuteNonQuery();
                            if (a > 0)
                            {
                                XtraMessageBox.Show("SUCCES");
                                accordionControlElement1_Click(accordionControlElement1, e);
                                gridView1.ApplyFindFilter(textEdit_edit_reportno.Text);
                                simpleButton_view_Click(sender, e);
                                main.TLOG("4-3", label_report_no.Text, label_report_no.Text);
                            }
                        }
                        if (!string.IsNullOrEmpty(isbn))
                        {
                            string has_problem = string.Empty;
                            if (solved == "1") has_problem = "0";
                            else has_problem = "1";

                            query = $"UPDATE books SET has_problem={has_problem} WHERE ISBN='{isbn}';";
                            using (var command = new SQLiteCommand(query, connection))
                            {int a= command.ExecuteNonQuery();
                                if (a > 0) XtraMessageBox.Show("updtd");
                            }
                        }
                    }
                }
                catch (Exception ex) { main.LOG(ex); XtraMessageBox.Show("FAILED TO UPDATE"); }
            } 
            var result = XtraMessageBox.Show("Are you sure to update report?", "Update Report", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (new[] { textEdit_edit_reportno.Text, comboBoxEdit_edit_reportype.Text, comboBoxEdit_edit_member_isbn.Text,
                dateEdit_edit_create_date.Text, textEdit_edit_title.Text, memoEdit1.Text, }.Any(string.IsNullOrEmpty))
                { XtraMessageBox.Show("EMPTY"); }
                else
                {
                    if (textEdit_edit_reportno.Text != label_report_no.Text)
                    {
                        using (var connection = new SQLiteConnection(main.connectionString))
                        {
                            connection.Open();
                            string query = $"SELECT ID FROM reports WHERE ID = {textEdit_edit_reportno.Text} ;";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {   XtraMessageBox.Show($"Please enter another Report NO\nAlready exist report no:{textEdit_edit_reportno.Text}", "Invalid Report No", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;}
                                else { if (adding) { add(); } else { update(); } }
                            }
                        }
                    }
                    else { if (adding) { add(); } else { update(); } }
                }
            }
        }
        private void textEdit_edit_reportno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            { e.Handled = true; } //only number
        }
        private void comboBoxEdit_edit_reportype_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxEdit_edit_member_isbn.Properties.Items.Clear();
            if (comboBoxEdit_edit_reportype.SelectedIndex.ToString() == "0")
            { 
                label_edit_report_type_text.Text = "Reported Book ISBN:";
                comboBoxEdit_edit_member_isbn.Properties.Items.Add("Select Book");
            }
            else if (comboBoxEdit_edit_reportype.SelectedIndex.ToString() == "1") 
            {
                label_edit_report_type_text.Text = "Reported Member ID:";
                comboBoxEdit_edit_member_isbn.Properties.Items.Add("Select Member");
            }
            comboBoxEdit_edit_member_isbn.SelectedIndex = 0;
        }
        private void simpleButton_report_book_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit_book_title.Text))
            { XtraMessageBox.Show("Please enter TITLE", "", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else if (string.IsNullOrEmpty(memoEdit_book_desc.Text))
            { XtraMessageBox.Show("Please enter DESCRIPTION", "", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            {
                var result = (DialogResult?)null;
                if (sender is null) result = DialogResult.Yes;
                else 
                result = XtraMessageBox.Show("Are you sure to create new report?", "Create new report", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        string query = $"INSERT INTO reports(TYPE,ISBN,TITLE,DESC,SOLVED,CREATE_DATE) VALUES({report_type}, '{reported_no}',@title,@desc,0,'{DateTime.Now.ToString("yyyy-MM-dd")}');";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@title", textEdit_book_title.Text);
                            command.Parameters.AddWithValue("@desc", memoEdit_book_desc.Text);
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                last_insert_id = connection.LastInsertRowId;
                                if (sender is null) { } //skip
                                else {navigationFrame1.SelectedPage = navigationPage1;
                                accordionControlElement2_Click(accordionControlElement2, e);
                                gridView1.ApplyFindFilter(last_insert_id.ToString());
                                simpleButton_view_Click(sender, e);
                                this.Size = new Size(1220, 750); //default size
                                    this.Location = new Point( //center
                                    (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                    (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
                                }
                            }
                        }
                        query = $"UPDATE books SET has_problem=1 WHERE ISBN ='{reported_no}'";
                        using (var command = new SQLiteCommand(query, connection))
                        {command.ExecuteNonQuery(); main.TLOG("4-2","",reported_no); }
                        connection.Close();
                    }
                }
                var WiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault(); //refresh main
                if (WiseLibForm != null) { WiseLibForm.load_random(null,null); }
                var bookForm = Application.OpenForms.OfType<books>().FirstOrDefault(); //refresh book view
                if (bookForm != null) { bookForm.simpleButton_cancel_Click(null, null); bookForm.simpleButton_view_book_Click(null, null); }
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitInfo = gridView1.CalcHitInfo(gridControl1.PointToClient(Control.MousePosition));
                if (hitInfo.InRow || hitInfo.InRowCell)
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripMenuItem btnEdit = new ToolStripMenuItem("delete"); btnEdit.Click += (s, ev) => { simpleButton_edit_Click(sender, e); };
                    ToolStripMenuItem btnDel = new ToolStripMenuItem("view"); btnDel.Click += (s, ev) => { simpleButton_delete_Click(sender, e); };
                    ToolStripMenuItem btnCcell = new ToolStripMenuItem("copy cell"); btnCcell.Click += (s, ev) => { simpleButton_copy_cell_Click(sender, e); };
                    ToolStripMenuItem btnCrow = new ToolStripMenuItem("copy row"); btnCrow.Click += (s, ev) => { simpleButton_copy_row_Click(sender, e); };

                    menu.Items.Add(btnDel); menu.Items.Add(btnEdit); menu.Items.Add(btnCrow); menu.Items.Add(btnCcell); menu.Show(Cursor.Position);
                }
            }
        }
        private void shortcuts(object sender, KeyEventArgs e)//TODO         
        {
            //if (navigationFrame1.SelectedPage == navigationPage1 && textEdit_search_book.Focused == false)
            //{
            //    //ARROW KEYS----------------------------------------------------------------------
            //    if (e.KeyCode == Keys.Right) { simpleButton_right.PerformClick(); }
            //    if (e.KeyCode == Keys.Left) { simpleButton_left.PerformClick(); }
            //    if (e.KeyCode == Keys.Up) { simpleButton_up.PerformClick(); }
            //    if (e.KeyCode == Keys.Down) { simpleButton_down.PerformClick(); }
            //    //--------------------------------------------------------------------------------
            //    if (e.KeyCode == Keys.F) { textEdit_search_book.Focus(); }//F find book
            //    if (e.KeyCode == Keys.V) { simpleButton_view_book_Click(null, null); }//V view book
            //    if (e.KeyCode == Keys.E) { simpleButton_add_book_Click(null, null); }//E add book
            //    if (e.KeyCode == Keys.D) { simpleButton_delete_book_Click(null, null); }//D add book
            //    if (e.KeyCode == Keys.G) { simpleButton_copy_row_Click(null, null); }//G copy row
            //    if (e.KeyCode == Keys.H) { simpleButton_copy_cell_Click(null, null); } //H copy cell
            //    if (e.KeyCode == Keys.I) { simpleButton_import_books_Click(null, null); } //I import book
            //    //QUICK SEARCH KEYS-------------------------------------------------------------------------------------
            //    if (e.KeyCode == Keys.X) { simpleButton_showhide_extras_Click(null, null); }//X quick search show/hide
            //    if (e.KeyCode == Keys.R) { simpleButton_view_missing_books_Click(null, null); } //R view missing books
            //    if (e.KeyCode == Keys.T) { simpleButton_borrowed_books_Click(null, null); } //T view borrowed books
            //    if (e.KeyCode == Keys.Y) { simpleButton_view_overdue_books_Click(null, null); } //Y view overdue books
            //    if (e.KeyCode == Keys.U) { simpleButton_view_troubled_books_Click(null, null); } //U view troubled books
            //    if (e.KeyCode == Keys.O) { simpleButton_view_recently_added_books_Click(null, null); } //O view recently added books
            //    //SEARCH BAR KEYS-------------------------------------------------------------------------------------
            //    if (e.KeyCode == Keys.S) { simpleButton_paste_Click(null, null); } //S paste & search
            //    if (e.KeyCode == Keys.W) { simpleButton_clear_book_search_Click(null, null); } //W clear search
            //    //
            //    if (e.KeyCode == Keys.ControlKey) { ToggleShortcuts(); } //show-hide shortcuts
            //}
        }
        private bool shortcutsVisible = false;
        private void ToggleShortcuts()
        {
            shortcutsVisible = !shortcutsVisible;
            var shortcuts = new (Control Button, string Key)[]
            {
                //(simpleButton_view_book, "V"),
                //(simpleButton_add_book, "E"),
                //(simpleButton_delete_book, "D"),
                //(simpleButton_copy_row, "G"),
                //(simpleButton_copy_cell, "H"),
                //(simpleButton_import_books, "I"),
                //(simpleButton_showhide_extras, "X"),
                //(simpleButton_view_missing_books, "R"),
                //(simpleButton_borrowed_books, "T"),
                //(simpleButton_view_overdue_books, "Y"),
                //(simpleButton_view_troubled_books, "U"),
                //(simpleButton_view_recently_added_books, "O"),
                //(simpleButton_paste, "S"),
                //(simpleButton_clear_book_search, "W")
            };

            foreach (var shortcut in shortcuts)
            {
                Control parent = shortcut.Button.Parent;
                string labelName = "shortcut_" + shortcut.Key;
                if (shortcutsVisible)
                {
                    if (parent.Controls[labelName] == null)
                    {
                        Label shortcutLabel = new Label
                        {
                            Text = shortcut.Key,
                            Font = new Font("Arial", 12, FontStyle.Bold),
                            ForeColor = Color.White,
                            BackColor = Color.DarkGray,
                            AutoSize = true,
                            Location = new Point(shortcut.Button.Left, shortcut.Button.Top - 7),
                            Name = labelName
                        };
                        parent.Controls.Add(shortcutLabel);
                        shortcutLabel.BringToFront();
                    }
                }
                else
                {   Control lbl = parent.Controls[labelName];
                    if (lbl != null) parent.Controls.Remove(lbl);}
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var membersForm = Application.OpenForms.OfType<members>().FirstOrDefault();
            var booksForm = Application.OpenForms.OfType<books>().FirstOrDefault();

            var result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (membersForm != null) //back to members
                {
                    result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes) { this.Close(); }
                }
                else if (booksForm != null) //back to members
                {
                    result = XtraMessageBox.Show("Are you sure to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes) { this.Close(); }
                }
                else { navigationFrame1.SelectedPage = navigationPage1; this.Size = new Size(1220, 750); } //back to main
            }
        }
    }
}

