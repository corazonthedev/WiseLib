using DevExpress.DataProcessing.InMemoryDataProcessor.GraphGenerator;
using DevExpress.Utils;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraVerticalGrid.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using TextEdit = DevExpress.XtraEditors.TextEdit;
using ToolTip = System.Windows.Forms.ToolTip;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraPrinting.BarCode;
using Label = System.Windows.Forms.Label;
using DevExpress.XtraReports.Wizards;
using System.Threading;
using DevExpress.Pdf.Native;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.Templates;
using OfficeOpenXml.Drawing.Slicer.Style;
using DevExpress.XtraCharts.Native;
namespace WiseLib
{
    public partial class books : DevExpress.XtraEditors.XtraForm
    {
        public books()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(shortcuts);
        }
        private DataTable allBooksDataTable;
        private DataTable filtered;
        string image_s = "";
        string image_m = "";
        string image_l = "";
        private void books_Load(object sender, EventArgs e)
        {
            this.BeginInvoke((Action)(() => { textEdit_search_book.Focus(); }));
            foreach (var panel in new[] { panel1, panel2, panel3 })
            { panel.Paint += main.Panel_Paint; } //PANEL PAINT
            get_total_book();
            simpleButton_clear_book_search_Click(null, null);
            label_library_name.Text = main.library_name;
        }
        //-------------OTHER FUNCS-------------------------------------------------------------------------------------------
        private void shortcuts(object sender, KeyEventArgs e)//TODO         
        {
            if (navigationFrame1.SelectedPage == navigationPage1 && textEdit_search_book.Focused == false)
            {
                //ARROW KEYS----------------------------------------------------------------------
                if (e.KeyCode == Keys.Right) { simpleButton_right.PerformClick(); }
                if (e.KeyCode == Keys.Left) { simpleButton_left.PerformClick(); }
                if (e.KeyCode == Keys.Up) { simpleButton_up.PerformClick(); }
                if (e.KeyCode == Keys.Down) { simpleButton_down.PerformClick(); }
                //--------------------------------------------------------------------------------
                if (e.KeyCode == Keys.F) { textEdit_search_book.Focus(); }//F find book
                if (e.KeyCode == Keys.V) { simpleButton_view_book_Click(null, null); }//V view book
                if (e.KeyCode == Keys.E) { simpleButton_add_book_Click(null, null); }//E add book
                if (e.KeyCode == Keys.D) { simpleButton_delete_book_Click(null, null); }//D add book
                if (e.KeyCode == Keys.G) { simpleButton_copy_row_Click(null, null); }//G copy row
                if (e.KeyCode == Keys.H) { simpleButton_copy_cell_Click(null, null); } //H copy cell
                if (e.KeyCode == Keys.I) { simpleButton_import_books_Click(null, null); } //I import book
                //QUICK SEARCH KEYS-------------------------------------------------------------------------------------
                if (e.KeyCode == Keys.X) { simpleButton_showhide_extras_Click(null, null); }//X quick search show/hide
                if (e.KeyCode == Keys.R) { simpleButton_view_missing_books_Click(null, null); } //R view missing books
                if (e.KeyCode == Keys.T) { simpleButton_borrowed_books_Click(null, null); } //T view borrowed books
                if (e.KeyCode == Keys.Y) { simpleButton_view_overdue_books_Click(null, null); } //Y view overdue books
                if (e.KeyCode == Keys.U) { simpleButton_view_troubled_books_Click(null, null); } //U view troubled books
                if (e.KeyCode == Keys.O) { simpleButton_view_recently_added_books_Click(null, null); } //O view recently added books
                //SEARCH BAR KEYS-------------------------------------------------------------------------------------
                if (e.KeyCode == Keys.S) { simpleButton_paste_Click(null, null); } //S paste & search
                if (e.KeyCode == Keys.W) { simpleButton_clear_book_search_Click(null, null); } //W clear search
                //
                if (e.KeyCode == Keys.ControlKey) { ToggleShortcuts(); } //show-hide shortcuts
            }
        }
        private bool shortcutsVisible = false;
        private void ToggleShortcuts()
        {
            shortcutsVisible = !shortcutsVisible;
            var shortcuts = new (Control Button, string Key)[]
            {
                (simpleButton_view_book, "V"),
                (simpleButton_add_book, "E"),
                (simpleButton_delete_book, "D"),
                (simpleButton_copy_row, "G"),
                (simpleButton_copy_cell, "H"),
                (simpleButton_import_books, "I"),
                (simpleButton_showhide_extras, "X"),
                (simpleButton_view_missing_books, "R"),
                (simpleButton_borrowed_books, "T"),
                (simpleButton_view_overdue_books, "Y"),
                (simpleButton_view_troubled_books, "U"),
                (simpleButton_view_recently_added_books, "O"),
                (simpleButton_paste, "S"),
                (simpleButton_clear_book_search, "W")};

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
                {
                    Control lbl = parent.Controls[labelName];
                    if (lbl != null) parent.Controls.Remove(lbl);
                }
            }
        }
        private void get_total_book()
        {
            string query = "SELECT COUNT(*) FROM books";
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString)) //LOAD BOOK CATEGORIES
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        { label_total_book.Text += reader.GetInt32(0).ToString(); }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    XtraMessageBox.Show("Error getting total book", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void load_random() //load grid by random
        {
            if (main.database_connection_status)
            {
                string query = "SELECT * FROM books ORDER BY RANDOM() LIMIT 100;"; //ONLY DISPLAY
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                        allBooksDataTable = new DataTable();
                        adapter.Fill(allBooksDataTable);
                        gridControl1.DataSource = allBooksDataTable;
                        label_grid_load_error_text.Visible = false; //ERR-MSG
                        gridView1.FindFilterText = "";
                        gridView1.PopulateColumns(); //create custom column names
                        gridView1.Columns["title"].Caption = "Title";
                        gridView1.Columns["author"].Caption = "Author";
                        gridView1.Columns["year_of_publication"].Caption = "Year Of Publication";
                        gridView1.Columns["page_count"].Caption = "Page";
                        gridView1.Columns["publisher"].Caption = "Publisher";
                        gridView1.Columns["taken"].Caption = "Taken";
                        gridView1.Columns["missing"].Caption = "Missing";
                        gridView1.Columns["has_problem"].Caption = "Problem";
                        gridView1.Columns["date_added"].Caption = "Date Added";
                        string[] hiddenColumns = { "image_s", "image_m", "image_l" };
                        foreach (var columnName in hiddenColumns) //hide columns
                        {
                            if (gridView1.Columns[columnName] != null)
                            { gridView1.Columns[columnName].Visible = false; }
                        }
                        gridView1.CustomColumnDisplayText += (sender, e) => //0 to no 1 to yes
                        {
                            if (e.Column.FieldName == "taken" && e.Value?.ToString() == "0") { e.DisplayText = "no"; }
                            else if (e.Column.FieldName == "taken" && e.Value?.ToString() == "1") { e.DisplayText = "yes"; }
                            if (e.Column.FieldName == "missing" && e.Value?.ToString() == "0") { e.DisplayText = "no"; }
                            else if (e.Column.FieldName == "missing" && e.Value?.ToString() == "1") { e.DisplayText = "yes"; }
                            if (e.Column.FieldName == "has_problem" && e.Value?.ToString() == "0") { e.DisplayText = "no"; }
                            else if (e.Column.FieldName == "has_problem" && e.Value?.ToString() == "1") { e.DisplayText = "yes"; }
                        };
                        gridView1.BestFitColumns();
                        gridView1.RefreshData();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        label_grid_load_error_text.Visible = true; //ERR-MSG
                    }
                }
            }
            else { label_grid_load_error_text.Visible = true; } //ERR-MSG
        }
        private void keyboard_buttons(object sender, EventArgs e)//KEYBOARD
        {
            var button = sender as SimpleButton;
            if (button.ToolTip == "backspace") { if (textEdit_search_book.Text.Length > 0) { textEdit_search_book.Text = textEdit_search_book.Text.Substring(0, textEdit_search_book.Text.Length - 1); } }
            else if (button.ToolTip == "clear") { simpleButton_clear_book_search_Click(sender, e); }
            else if (button != null)
            { textEdit_search_book.Text += button.Text; }
        }
        //---------------NAVIGATION PAGE 1----------------------------------------------------------------------------------
        //default 1400, 830
        private void pictureBox_wiselib_logo_Click(object sender, EventArgs e)//redirect to github
        { connection_status.open_github(sender, e); }
        private void textEdit_search_book_KeyDown(object sender, KeyEventArgs e)//KEY ENTER-ESC
        {
            if (e.KeyCode == Keys.Enter) { simpleButton_search_book_Click(sender, e); }
            if (e.KeyCode == Keys.Escape) { gridView1.Focus(); }
        }
        private void simpleButton_paste_Click(object sender, EventArgs e)//PASTE
        {
            textEdit_search_book.Text = Clipboard.GetText().Trim();
            simpleButton_search_book_Click(sender, e);
        }
        public void simpleButton_clear_book_search_Click(object sender, EventArgs e)//CLEAR SEARCH
        {
            textEdit_search_book.Clear();
            load_random();
        }
        public void simpleButton_search_book_Click(object sender, EventArgs e)//SEARCH BUTTON
        {
            keyboard_panel.Visible = false;
            string filter = textEdit_search_book.Text.ToString();
            string query = "";
            if (string.IsNullOrEmpty(filter)) //EMPTY INPUT
            { }//NOTHING
            else if (filter.StartsWith("*1"))//MISSING BOOKS
            {
                query = $"SELECT * FROM books WHERE missing=1;";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                        filtered = new DataTable();
                        adapter.Fill(filtered);
                        gridControl1.DataSource = filtered;
                        conn.Close();
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                };
                gridView1.RefreshData();
                gridControl1.Focus();
            }
            else if (filter.StartsWith("*2"))//BORROWED BOOKS
            {
                query = $"SELECT * FROM books WHERE taken=1;";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                        filtered = new DataTable();
                        adapter.Fill(filtered);
                        gridControl1.DataSource = filtered;
                        conn.Close();
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
                gridView1.RefreshData();
                gridControl1.Focus();
            }
            else if (filter.StartsWith("*3"))//OVERDUE BOOKS
            {
                query = "SELECT ISBN FROM book_transactions WHERE return_date < DATE('now') AND transaction_type = 1;";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();

                        SQLiteDataAdapter isbnAdapter = new SQLiteDataAdapter(query, conn);
                        DataTable isbnTable = new DataTable();
                        isbnAdapter.Fill(isbnTable);

                        List<string> isbnList = new List<string>();
                        foreach (DataRow row in isbnTable.Rows) isbnList.Add(row["ISBN"].ToString());
                        if (isbnList.Count > 0)
                        {
                            string booksQuery = "SELECT * FROM books WHERE ISBN IN (" + string.Join(",", isbnList.Select(isbn => $"'{isbn}'")) + ")";

                            SQLiteDataAdapter booksAdapter = new SQLiteDataAdapter(booksQuery, conn);
                            DataTable booksTable = new DataTable();
                            booksAdapter.Fill(booksTable);

                            gridControl1.DataSource = booksTable;
                            gridView1.RefreshData();
                            gridControl1.Focus();
                        }
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
            }
            else if (filter.StartsWith("*4"))//TROUBLED BOOKS
            {
                query = "SELECT * FROM books WHERE has_problem=1";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                        filtered = new DataTable();
                        adapter.Fill(filtered);
                        gridControl1.DataSource = filtered;
                        conn.Close();
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
            }
            else if (filter.StartsWith("*5"))//LAST 100 BOOKS
            {
                query = "SELECT * FROM books ORDER BY id DESC LIMIT 100;";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                        filtered = new DataTable();
                        adapter.Fill(filtered);
                        gridControl1.DataSource = filtered;
                        conn.Close();
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
            }
            else //NORMAL SEARCH
            {
                query = $"SELECT * FROM books WHERE ISBN LIKE @filter OR title LIKE @filter OR author LIKE @filter OR year_of_publication LIKE @filter OR publisher LIKE @filter;";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            filtered = new DataTable();
                            adapter.Fill(filtered);
                            gridControl1.DataSource = filtered;
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
                gridView1.FindFilterText = filter;
                gridView1.RefreshData();
                gridControl1.Focus();
            }
        }
        private void simpleButton_show_hide_keyboard_Click(object sender, EventArgs e)//KEYBOARD SHOW-HIDE
        { keyboard_panel.Visible = !keyboard_panel.Visible; }
        //---------------QUICK SEARCHS-------------------------------------------------------------------------
        private void simpleButton_showhide_extras_Click(object sender, EventArgs e)
        { panel_extras.Visible = !panel_extras.Visible; }//show-hide
        private void simpleButton_view_missing_books_Click(object sender, EventArgs e)//MISSING BOOKS
        {
            simpleButton_clear_book_search_Click(sender, e);
            textEdit_search_book.Text = "*1";
            simpleButton_search_book_Click(sender, e);
        }
        private void simpleButton_borrowed_books_Click(object sender, EventArgs e)
        {
            simpleButton_clear_book_search_Click(sender, e);
            textEdit_search_book.Text = "*2";
            simpleButton_search_book_Click(sender, e);
        }
        private void simpleButton_view_overdue_books_Click(object sender, EventArgs e)
        {
            simpleButton_clear_book_search_Click(sender, e);
            textEdit_search_book.Text = "*3";
            simpleButton_search_book_Click(sender, e);
        }
        private void simpleButton_view_troubled_books_Click(object sender, EventArgs e)
        {
            simpleButton_clear_book_search_Click(sender, e);
            textEdit_search_book.Text = "*4";
            simpleButton_search_book_Click(sender, e);
        }
        private void simpleButton_view_recently_added_books_Click(object sender, EventArgs e)
        {
            simpleButton_clear_book_search_Click(sender, e);
            textEdit_search_book.Text = "*5";
            simpleButton_search_book_Click(sender, e);
        }
        //--------------RIGHT PANEL------------------------------------------------------------------------------------------
        private void simpleButton_add_book_Click(object sender, EventArgs e)//ADD
        {
            clear_add_textedit(sender, e);
            pictureBox_book_cover2.Image = null;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(852, 538); //res for book view
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point( //center
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
            if (comboBoxEdit_add_category.Properties.Items.Count == 0)
            {
                string query = "SELECT category FROM categories";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))//LOAD BOOK CATEGORIES
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            List<string> categories = new List<string>();
                            while (reader.Read()) categories.Add(reader["category"].ToString());
                            categories.Sort(); //alphabetic
                            comboBoxEdit_add_category.Properties.Items.AddRange(categories.ToArray()); //add to combobox
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("Error loading categories", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            navigationFrame1.SelectedPage = navigationPage5;
        }
        private void simpleButton_delete_book_Click(object sender, EventArgs e)//DELETE
        {
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row
            if (selectedRowHandle >= 0)
            {
                var selected_book_title = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[1]);
                var selected_book_author = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[2]);
                string selected_book_taken = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "taken")?.ToString();
                string selected_book_missing = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "missing")?.ToString();
                string selected_book_has_problem = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "has_problem")?.ToString();
                if (selected_book_taken == "1" || selected_book_missing == "1")
                { XtraMessageBox.Show("Can not delete taken/missed books.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                else
                {
                    DialogResult result = DialogResult.None;
                    if (selected_book_has_problem == "1")//has any reports
                    { result = XtraMessageBox.Show($"Are you sure to delete book {selected_book_author} - {selected_book_title} ?\nThis book has acitve report deleting book will also delete report.", "Delete Member", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
                    else
                    { result = XtraMessageBox.Show($"Are you sure to delete book {selected_book_author} - {selected_book_title} ?", "Delete Member", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }

                    if (result == DialogResult.Yes)
                    {
                        DialogResult _result = XtraMessageBox.Show($"Book {selected_book_author} - {selected_book_title} will be deleted.", "Delete Member", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (_result == DialogResult.Yes)
                        {
                            string query = $"DELETE FROM books WHERE ISBN = '{gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[1]).ToString()}'"; //1-id 2-isbn
                            using (var conn = new SQLiteConnection(main.connectionString))
                            {
                                try
                                {
                                    conn.Open();
                                    using (var command = new SQLiteCommand(query, conn))
                                    {
                                        command.ExecuteNonQuery();
                                        string _isbn = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[1]).ToString();
                                        main.TLOG("3-2", "", _isbn);

                                        var wiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                                        if (wiseLibForm != null)
                                        { wiseLibForm.simpleButton_cancel_member_Click(null, null); }

                                        gridView1.DeleteRow(selectedRowHandle); //delete from grid
                                        gridControl1.Refresh();
                                    }
                                    if (selected_book_has_problem == "1")//has any reports
                                    {
                                        string selected_book_isbn = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "ISBN")?.ToString();
                                        query = $"DELETE FROM reports WHERE ISBN='{selected_book_isbn}';";
                                        using (var cmmd = new SQLiteCommand(query, conn))
                                        { cmmd.ExecuteNonQuery(); }
                                    }
                                    conn.Close();
                                }
                                catch (Exception ex)
                                {
                                    main.LOG(ex);
                                    MessageBox.Show("Error deleting book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void simpleButton_view_book_Click(object sender, EventArgs e)//VIEW
        {
            textEdit_search_book.Clear();
            //try{ pictureBox_book_cover1.Image.Dispose(); }
            //catch { } //skip
            pictureBox_book_cover1.Image = null;

            ToolTip toolTip = new ToolTip();
            int selected_row = gridView1.FocusedRowHandle;

            var imageUrl = "";
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))//get book cover img
            {
                List<string> img_list = new List<string> { "image_l", "image_m", "image_s" };
                foreach (string img_x in img_list) //get img_l img_m img_s or create
                {
                    try
                    {
                        conn.Open();
                        string selectedISBN = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "ISBN")?.ToString();
                        string query = $"SELECT {img_x} FROM books WHERE ISBN= '{selectedISBN}'";
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

                                            pictureBox_book_cover1.Image = Image.FromStream(stream);
                                            if (pictureBox_book_cover1.Image.Height > 2)
                                            {
                                                label_placeholder_book_title.Visible = false;
                                                label_placeholder_book_author.Visible = false;
                                                pictureBox_book_cover1.Visible = true; //VIEW 
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
                void create_book_cover()//WHILE VIEW BOOK
                {
                    string title = gridView1.GetRowCellValue(selected_row, "title")?.ToString();
                    int maxLength = 25; //25char in every row
                    List<string> lines = new List<string>();
                    for (int i = 0; i < title.Length; i += maxLength) //30 char parts
                    { lines.Add(title.Substring(i, Math.Min(maxLength, title.Length - i))); }

                    string author2 = gridView1.GetRowCellValue(selected_row, "author")?.ToString();
                    List<string> liness = new List<string>();
                    for (int i = 0; i < author2.Length; i += maxLength) //30 char parts
                    { liness.Add(author2.Substring(i, Math.Min(maxLength, author2.Length - i))); }

                    label_placeholder_book_title.Text = string.Join(Environment.NewLine, lines);
                    label_placeholder_book_author.Text = string.Join(Environment.NewLine, liness);

                    toolTip.SetToolTip(label_placeholder_book_title, label_placeholder_book_title.Text); //tooltip
                    toolTip.SetToolTip(label_placeholder_book_author, label_placeholder_book_author.Text); //tooltip

                    label_placeholder_book_title.Visible = true;
                    label_placeholder_book_author.Visible = true;
                }
                try
                {
                    if (pictureBox_book_cover1.Image.Height < 3) create_book_cover(); //IMG COULDN'T LOAD - CREATE BOOK COVER IMG
                }
                catch (Exception ex)
                {
                    create_book_cover(); //necessary
                    main.LOG(ex);
                }
            }

            label_ISBN.Text = gridView1.GetRowCellValue(selected_row, "ISBN")?.ToString();
            label_title.Text = gridView1.GetRowCellValue(selected_row, "title")?.ToString();
            toolTip.SetToolTip(label_title, label_title.Text); //tooltip

            label_author.Text = gridView1.GetRowCellValue(selected_row, "author")?.ToString();
            toolTip.SetToolTip(label_author, label_author.Text); //tooltip

            label_year.Text = gridView1.GetRowCellValue(selected_row, "year_of_publication")?.ToString();
            toolTip.SetToolTip(label_year, label_year.Text); //tooltip

            label_publisher.Text = gridView1.GetRowCellValue(selected_row, "publisher")?.ToString();
            toolTip.SetToolTip(label_publisher, label_publisher.Text); //tooltip

            label_page_count.Text = gridView1.GetRowCellValue(selected_row, "page_count")?.ToString();
            toolTip.SetToolTip(label_page_count, label_page_count.Text); //tooltip

            label_category.Text = gridView1.GetRowCellValue(selected_row, "category")?.ToString();
            toolTip.SetToolTip(label_category, label_category.Text); //tooltip

            label_missing.Text = gridView1.GetRowCellValue(selected_row, "missing")?.ToString();

            if (label_missing.Text == "Yes") (simpleButton_delete_book2.Enabled, simpleButton_edit_book.Enabled) = (false, false);//missing
            else (simpleButton_delete_book2.Enabled, simpleButton_edit_book.Enabled) = (true, true);//not missing

            label_taken.Text = gridView1.GetRowCellValue(selected_row, "taken")?.ToString();
            if (label_taken.Text == "1") (simpleButton_delete_book.Enabled, simpleButton_delete_book2.Enabled, simpleButton_edit_book.Enabled) = (false, false, false);  //TAKEN BOOK CANNOT EDIT OR DELETE
            else if (label_taken.Text == "0") (simpleButton_delete_book.Enabled, simpleButton_delete_book2.Enabled, simpleButton_edit_book.Enabled) = (true, true, true);

            label_problem.Text = gridView1.GetRowCellValue(selected_row, "has_problem")?.ToString();
            if (label_problem.Text == "1")
            { simpleButton_report_book.Text = "view"; label_problem.ForeColor = Color.Red; }
            else { simpleButton_report_book.Text = "report"; label_problem.ForeColor = Color.White; }

            if (label_problem.Text == "1") label_problem.Text = "Yes";
            else label_problem.Text = "No";
            if (label_taken.Text == "1") label_taken.Text = "Yes";
            else label_taken.Text = "No";
            if (label_missing.Text == "1") label_missing.Text = "Yes";
            else label_missing.Text = "No";

            if (label_taken.Text == "Yes")//BOOK TAKEN
            {
                panel_taken.Visible = true;
                string query = $"SELECT borrow_date, return_date,member_id FROM book_transactions WHERE ISBN='{label_ISBN.Text}';";
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
                                //LOAD TEXTS
                                label_borrow_date.Text = reader["borrow_date"].ToString();
                                label_return_date.Text = reader["return_date"].ToString();
                                label_borrower_ID.Text = reader["member_id"].ToString();
                            }
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("Could not get borrowed book info", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else panel_taken.Visible = false;//BOOK NOT TAKEN

            navigationFrame1.SelectedPage = navigationPage2;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(852, 538); //res for book view
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point( //center
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
        }
        private void handle_arrow_buttons(object sender, EventArgs e)//ARROW BUTTON EVENTS
        {
            if (keyboard_panel.Visible) keyboard_panel.Visible = false;
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
            { Clipboard.SetText(cellValue); }

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
            label.Location = new Point(buttonLocation.X, buttonLocation.Y + 20);

            label.BringToFront();

            Task.Delay(2000).ContinueWith(t => { panel1.Invoke(new Action(() => panel1.Controls.Remove(label))); });//delete after 2s
        }
        private void simpleButton_import_books_Click(object sender, EventArgs e)//REPORT
        {
            importer importForm = new importer(1); //importer
            importForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed
            importForm.ShowDialog();
        }
        //---------------NAVIGATION PAGE 2----------------------------------------------------------------------------------
        //default size 852;538
        private void simpleButton_copy_ISBN_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label_ISBN.Text);

            var label = new Label
            {
                BackColor = Color.Gray,
                Text = $"ISBN COPIED {label_ISBN.Text}",
                Font = new Font("Tahoma", 16),
                ForeColor = Color.White,

                AutoSize = true,
            };
            navigationPage3.Controls.Add(label);

            Point buttonLocation = navigationPage3.PointToClient(simpleButton_copy_ISBN.Parent.PointToScreen(simpleButton_copy_ISBN.Location));
            label.Location = new Point(buttonLocation.X - 65, buttonLocation.Y + simpleButton_copy_ISBN.Height + 5);

            label.BringToFront();

            Task.Delay(2000).ContinueWith(t => { navigationPage3.Invoke(new Action(() => navigationPage3.Controls.Remove(label))); });//delete after 2s
        }
        private void simpleButton_copy_memberID_Click(object sender, EventArgs e)
        { Clipboard.SetText(label_borrower_ID.Text); }
        private void simpleButton_view_borrower_Click(object sender, EventArgs e)
        {
            var wiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();

            if (wiseLibForm != null)
            {
                wiseLibForm.Enabled = true;  // WiseLib formunu etkinleştir
                wiseLibForm.textEdit_member_no.Text = label_borrower_ID.Text;
                wiseLibForm.simpleButton_search_Click(sender, e);
                wiseLibForm.simpleButton_borrowed_book_view_Click(sender, e);
                wiseLibForm.BringToFront();  // WiseLib formunu ön plana getir
            }
        }
        public void simpleButton_cancel_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(1400, 830); //res for book view
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point( //center
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
            simpleButton_clear_book_search_Click(sender, e);
            navigationFrame1.SelectedPage = navigationPage1;
        }
        private void simpleButton_report_book_Click(object sender, EventArgs e)
        {
            if (simpleButton_report_book.Text == "view")
            {
                report reportForm = new report(20, label_ISBN.Text); //report
                reportForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed
                reportForm.ShowDialog();
            }
            else
            {
                report reportForm = new report(2, label_ISBN.Text); //report
                reportForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed
                reportForm.ShowDialog();
            }
        }
        private void simpleButton_delete_book2_Click(object sender, EventArgs e)//DELETE
        {
            string selected_book_has_problem = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "has_problem")?.ToString();
            DialogResult result = DialogResult.None;
            if (selected_book_has_problem == "1")
            { result = XtraMessageBox.Show($"Book {label_title.Text} Will Be Deleted\nThis book has acitve report deleting book will also delete report.", "Sure to Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
            else
            { result = XtraMessageBox.Show($"Book {label_title.Text} Will Be Deleted", "Sure to Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
            if (result == DialogResult.Yes)
            {
                string query = $"DELETE FROM BOOKS WHERE ISBN='{label_ISBN.Text}'";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                            main.TLOG("3-2", "", label_ISBN.Text);
                            XtraMessageBox.Show($"Book {label_title.Text} Deleted.");

                            var wiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                            if (wiseLibForm != null)
                            { wiseLibForm.simpleButton_cancel_member_Click(null, null); }
                            if (selected_book_has_problem == "1")//has any reports
                            {
                                string selected_book_isbn = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "ISBN")?.ToString();
                                query = $"DELETE FROM reports WHERE ISBN='{selected_book_isbn}';";
                                using (var cmmd = new SQLiteCommand(query, conn))
                                { cmmd.ExecuteNonQuery(); }
                            }
                            simpleButton_cancel_Click(sender, e);
                            simpleButton_clear_book_search_Click(sender, e);
                        }
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("Error Deleting Book");
                    }
                }
            }
        }
        private void simpleButton_book_history_Click(object sender, EventArgs e)
        {
            history historyForm = new history(2, label_ISBN.Text); //member
            historyForm.FormClosed += (s, args) => this.Enabled = true; //enable when closed
            historyForm.ShowDialog();
        }
        public void simpleButton_edit_book_Click(object sender, EventArgs e)//EDIT
        {
            textEdit_isbn.Text = label_ISBN.Text;
            textEdit_title.Text = label_title.Text;
            textEdit_author.Text = label_author.Text;
            dateTimePicker_publish_date1.Text = label_year.Text;
            textEdit_publisher.Text = label_publisher.Text;
            if (comboBoxEdit_category.Properties.Items.Count == 0)
            { string query = "SELECT category FROM categories";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))//LOAD BOOK CATEGORIES
                {
                    try
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            List<string> categories = new List<string>();
                            while (reader.Read()) categories.Add(reader["category"].ToString());
                            categories.Sort(); //alphabetic
                            comboBoxEdit_category.Properties.Items.AddRange(categories.ToArray()); //add to combobox
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("Error loading categories", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (comboBoxEdit_category.Properties.Items.Contains(label_category.Text)) comboBoxEdit_category.SelectedItem = label_category.Text;

            numericUpDown_page.Value = Convert.ToDecimal(label_page_count.Text);
            label_missing2.Text = label_missing.Text;

            navigationFrame2.SelectedPage = navigationPage4;
        }
        //---------------NAVIGATION PAGE 4----------------------------------------------------------------------------------
        private void textEdit_KeyPress_only_digit(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            { e.Handled = true; } //only number
        }
        private void textEdit_KeyPress_only_letter(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            { e.Handled = true; } //only letter
        }
        private void simpleButton_confirm_Click(object sender, EventArgs e)//UPDATE BOOK
        {
            if (new[] { textEdit_isbn.Text, textEdit_title.Text, textEdit_author.Text, dateTimePicker_publish_date1.Text, textEdit_publisher.Text, comboBoxEdit_category.Text, numericUpDown_page.Text, }.Any(string.IsNullOrEmpty) && label_missing2.Text == label_missing.Text)
            { XtraMessageBox.Show("Empty/Invalid Entry"); }
            else//UPDATE
            {
                var result = XtraMessageBox.Show("Book will be updated", "Sure to Update?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (label_missing2.Text == "Yes") label_missing2.Text = "1";
                    else if (label_missing2.Text == "NO") label_missing2.Text = "0";

                    void update_book(int category)
                    {
                        string query = "";
                        string missing = "";
                        if (label_missing2.Text.Contains("No")) missing = "0";
                        else missing = "1";

                        if (category == 0)//add without creating new category
                        { query = $"UPDATE books SET ISBN=@isbn,title=@title,author=@author,page_count='{numericUpDown_page.Text}',year_of_publication='{dateTimePicker_publish_date1.Text}',publisher=@publisher,missing='{missing}' WHERE ISBN='{label_ISBN.Text}';"; }
                        else if (category == 1)//add new category
                        { query = $"UPDATE books SET ISBN=@isbn,title=@title,author=@author,category=@category,year_of_publication='{dateTimePicker_publish_date1.Text}',publisher=@publisher,missing='{missing}' WHERE ISBN='{label_ISBN.Text}';"; }

                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                        {
                            try
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@isbn", textEdit_isbn.Text);
                                    cmd.Parameters.AddWithValue("@title", textEdit_title.Text);
                                    cmd.Parameters.AddWithValue("@author", textEdit_author.Text);
                                    cmd.Parameters.AddWithValue("@publisher", textEdit_publisher.Text);
                                    cmd.Parameters.AddWithValue("@category", comboBoxEdit_category.SelectedItem.ToString());
                                    conn.Open();
                                    int a = cmd.ExecuteNonQuery();
                                    if (!string.IsNullOrEmpty(image_l))
                                    {
                                        query = $"UPDATE books SET image_s=@image_s,image_m=@image_m,image_l=@image_l WHERE ISBN='{textEdit_isbn.Text}';";
                                        using (var _conn = new SQLiteConnection(main.connectionString))
                                        {
                                            try
                                            {
                                                _conn.Open();
                                                using (var _command = new SQLiteCommand(query, _conn))
                                                {
                                                    _command.Parameters.AddWithValue("@image_s", image_s);
                                                    _command.Parameters.AddWithValue("@image_m", image_m);
                                                    _command.Parameters.AddWithValue("@image_l", image_l);

                                                    int _result = _command.ExecuteNonQuery();
                                                    navigationFrame1.SelectedPage = navigationPage1;//view book
                                                    simpleButton_clear_book_search_Click(null, null);
                                                    textEdit_search_book.Text = textEdit_add_ISBN.Text;
                                                    simpleButton_search_book_Click(sender, e);
                                                    simpleButton_view_book_Click(sender, e);
                                                    main.TLOG("3-3", "", textEdit_isbn.Text);

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                XtraMessageBox.Show("book img adding error");
                                                main.LOG(ex);
                                            }
                                        }

                                    }

                                    foreach (Form form in Application.OpenForms)//refresh grid
                                    {
                                        if (form is WiseLib wiseLibForm)
                                        {
                                            wiseLibForm.load_random(sender, e);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                main.LOG(ex);
                                XtraMessageBox.Show("Error updating book");
                            }
                            //if (missing == "1")
                            //{
                            //    string _query = $"UPDATE books SET has_problem=1 WHERE ISBN='{label_ISBN.Text}';";
                            //    using (SQLiteConnection _conn = new SQLiteConnection(main.connectionString))
                            //    {
                            //        try
                            //        {
                            //            using (SQLiteCommand cmd = new SQLiteCommand(_query, _conn))
                            //            { _conn.Open(); int a = cmd.ExecuteNonQuery(); }
                            //        }
                            //        catch (Exception ex) { XtraMessageBox.Show("failed to update problem-missing status"); main.LOG(ex); }
                            //    }
                            //    report reportForm = new report(50, label_ISBN.Text); //report
                            //    reportForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed
                            //    reportForm.ShowDialog();
                            //}
                            //else if (missing == "0")
                            //{
                            //    string _query = $"UPDATE books SET has_problem=0 WHERE ISBN='{label_ISBN.Text}';";
                            //    using (SQLiteConnection _conn = new SQLiteConnection(main.connectionString))
                            //    {
                            //        try
                            //        {
                            //            using (SQLiteCommand cmd = new SQLiteCommand(_query, _conn))
                            //            { _conn.Open(); int a = cmd.ExecuteNonQuery(); }
                            //        }
                            //        catch (Exception ex) { XtraMessageBox.Show("failed to update problem-missing status"); main.LOG(ex); }
                            //    }

                            //    string _query2 = $"UPDATE reports SET SOLVED=1 WHERE ISBN='{label_ISBN.Text}';"; 
                            //    using (SQLiteConnection _conn = new SQLiteConnection(main.connectionString))
                            //    {
                            //        try
                            //        {
                            //            using (SQLiteCommand cmd = new SQLiteCommand(_query2, _conn))
                            //            { _conn.Open(); int a = cmd.ExecuteNonQuery(); }
                            //        }
                            //        catch (Exception ex) { XtraMessageBox.Show("failed to update problem-missing status"); main.LOG(ex); }
                            //    }
                            //}
                            //MISSING
                            if (missing_set && missing == "1")//set as MISSING
                            {
                                using (var conn1 = new SQLiteConnection(main.connectionString))
                                {
                                    conn1.Open();
                                    query = $"INSERT INTO reports(TYPE,ISBN,TITLE,DESC,SOLVED,CREATE_DATE) VALUES(2, '{label_ISBN.Text}','Missing','Missing',0,'{DateTime.Now.ToString("yyyy-MM-dd")}');";
                                    using (var cmd = new SQLiteCommand(query, conn1))
                                    { cmd.ExecuteNonQuery(); }
                                    main.TLOG("3-4", "", label_ISBN.Text);
                                    query = $"UPDATE books SET has_problem=1 WHERE ISBN='{label_ISBN.Text}'";
                                    using (var cmd = new SQLiteCommand(query, conn1))
                                    { cmd.ExecuteNonQuery(); }
                                }
                            }
                            else if (missing_set && missing == "0")//set as NOT missing
                            {
                                using (var conn1 = new SQLiteConnection(main.connectionString))
                                {
                                    conn1.Open();
                                    query = $"UPDATE reports SET SOLVED = 1, SOLVE_DATE = '{DateTime.Now.ToString("yyyy-MM-dd")}' WHERE ISBN = '{label_ISBN.Text}'";
                                    using (var cmd = new SQLiteCommand(query, conn1))
                                    { cmd.ExecuteNonQuery(); }
                                    main.TLOG("3-5", "", label_ISBN.Text);
                                    query = $"UPDATE books SET has_problem=0 WHERE ISBN='{label_ISBN.Text}'";
                                    using (var cmd = new SQLiteCommand(query, conn1))
                                    { cmd.ExecuteNonQuery(); }
                                }
                            }
                            XtraMessageBox.Show("Book updated.");
                            simpleButton_cancel_Click(sender, e);
                            simpleButton_clear_book_search_Click(sender, e);
                            textEdit_search_book.Text = label_ISBN.Text;
                            simpleButton_search_book_Click(sender, e);
                            simpleButton_view_book_Click(sender, e);
                            navigationFrame2.SelectedPage = navigationPage3;//view book
                        }
                    }
                    if (!comboBoxEdit_category.Properties.Items.Contains(comboBoxEdit_category.SelectedItem))//adding new category
                    {
                        string query = $"INSERT OR IGNORE INTO categories(category) VALUES(@category)";
                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                        {
                            try
                            {
                                conn.Open();
                                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                { cmd.Parameters.AddWithValue("@category", comboBoxEdit_category.Text); cmd.ExecuteNonQuery(); }
                            }
                            catch (Exception ex)
                            {
                                main.LOG(ex);
                                XtraMessageBox.Show("Category adding error");
                            }
                        }
                    }
                    //if (!comboBoxEdit_category.Properties.Items.Contains(comboBoxEdit_category.SelectedItem))//adding new category
                    //{
                    //    string query = $"INSERT OR IGNORE INTO categories(category) VALUES(@category)";
                    //    using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                    //    {
                    //        try
                    //        {
                    //            conn.Open();
                    //            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    //            {
                    //                cmd.Parameters.AddWithValue("@category", comboBoxEdit_category.Text);
                    //                cmd.ExecuteNonQuery(); 
                    //            }
                    //            update_book(0);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            main.LOG(ex);
                    //            XtraMessageBox.Show("Category adding error");
                    //        }
                    //    }
                    //}
                    //else 
                    update_book(1);
                    var openWiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                    openWiseLibForm.load_random(null, null);
                    if (openWiseLibForm != null && openWiseLibForm.selected_member_no != "0") { XtraMessageBox.Show("Please Re-Select lending book"); }
                }
            }
        }
        private void simpleButton_cancel2_Click(object sender, EventArgs e)
        {
            var result = XtraMessageBox.Show("Are you sure to Cancel?", "Sure to Cancel?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //navigationFrame2.SelectedPage = navigationPage3;
                simpleButton_clear_book_search_Click(sender, e);
                textEdit_search_book.Text = label_ISBN.Text;
                simpleButton_search_book_Click(sender, e);
                simpleButton_view_book_Click(sender, e);
                navigationFrame2.SelectedPage = navigationPage3;//view book
            }
        }
        private bool missing_set = false;
        private void simpleButton_set_missing_Click(object sender, EventArgs e)//MISSING 
        {
            if (label_missing.Text == "No")//set as MISSING
            {
                var result = XtraMessageBox.Show($"Book '{label_title.Text}' will be set as MISSING", "Missing Status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    label_missing2.Text = "Yes";
                    //using (var conn = new SQLiteConnection(main.connectionString))
                    //{
                    //    conn.Open();
                    //    string query = $"INSERT INTO reports(TYPE,ISBN,TITLE,DESC,SOLVED,CREATE_DATE) VALUES(2, '{label_ISBN.Text}','Missing','Missing',0,'{DateTime.Now.ToString("yyyy-MM-dd")}');";
                    //    using (var cmd = new SQLiteCommand(query, conn))
                    //    {cmd.ExecuteNonQuery();}
                    //main.TLOG("3-4", "", label_ISBN.Text);
                    missing_set = true;
                    //}
                }
            }
            else if (label_missing.Text == "Yes")//set as NOT missing
            {
                var result = XtraMessageBox.Show($"Book '{label_title.Text}' will be set as NOT missing", "Missing Status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    label_missing2.Text = "No";
                    //using (var conn = new SQLiteConnection(main.connectionString))
                    //{
                    //    conn.Open();
                    //    string query = $"UPDATE reports SET SOLVED = 1, SOLVE_DATE = '{DateTime.Now.ToString("yyyy-MM-dd")}' WHERE ID = {label_ISBN.Text}";
                    //    using (var cmd = new SQLiteCommand(query, conn))
                    //    { cmd.ExecuteNonQuery(); }
                    //main.TLOG("3-5", "", label_ISBN.Text);
                    missing_set = true;
                    //}
                }
            }
        }
        private async void simpleButton_get_info_online_Click(object sender, EventArgs e)//UPDATE
        {
            if (string.IsNullOrEmpty(textEdit_isbn.Text))
            { XtraMessageBox.Show("Enter ISBN", "No ISBN", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            {
                var result = XtraMessageBox.Show("Are your sure to update book info online?\nSome book info may be inaccurate, check info after fetching.", "Get book info online", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string isbn = textEdit_add_ISBN.Text;
                    progressBar2.Visible = true;
                    navigationPage4.Enabled = false;
                    progressBar2.Style = ProgressBarStyle.Marquee; //load animation
                    await Task.Run(() => get_book_info_by_isbn(label_ISBN.Text, 2));//update
                    navigationPage4.Enabled = true;
                    progressBar2.Visible = false;
                }
            }
        }
        //--------------NAVIGATION PAGE 4 OTHER FUNCS-----------------------------------------------------------------------
        private void clear_textedit(object sender, EventArgs e)
        {
            if (sender is SimpleButton btn)
            {
                if (btn.Name.Contains("ISBN")) textEdit_isbn.Clear();
                else if (btn.Name.Contains("title")) textEdit_title.Clear();
                else if (btn.Name.Contains("author")) textEdit_author.Clear();
                else if (btn.Name.Contains("year")) dateTimePicker_publish_date1.Clear();
                else if (btn.Name.Contains("publisher")) textEdit_publisher.Clear();
                else if (btn.Name.Contains("category")) comboBoxEdit_category.SelectedIndex = -1;
                else if (btn.Name.Contains("page")) numericUpDown_page.Value = 0;
            }
        }
        private void restore_textedit(object sender, EventArgs e)
        {
            if (sender is SimpleButton btn)
            {
                if (btn.Name.Contains("ISBN")) textEdit_isbn.Text = label_ISBN.Text;
                else if (btn.Name.Contains("title")) textEdit_title.Text = label_title.Text;
                else if (btn.Name.Contains("author")) textEdit_author.Text = label_author.Text;
                else if (btn.Name.Contains("year")) dateTimePicker_publish_date1.Text = label_year.Text;
                else if (btn.Name.Contains("publisher")) textEdit_publisher.Text = label_publisher.Text;
                else if (btn.Name.Contains("category")) comboBoxEdit_category.Text = label_category.Text;
                else if (btn.Name.Contains("page")) numericUpDown_page.Text = label_page_count.Text;
            }
        }
        //---------------------NAVIGATION PAGE 5-------------------------------------------------------------------------
        //default 852, 538
        private bool check_isbn()
        {
            bool result = false;
            if (textEdit_add_ISBN.Text.Length == 10)//ISBN-10
            {
                int sum = 0;
                for (int i = 0; i < 9; i++)
                {
                    int digit = int.Parse(textEdit_add_ISBN.Text[i].ToString());
                    sum += digit * (10 - i);
                }

                char checkDigitChar = textEdit_add_ISBN.Text[9];
                int checkDigit = (checkDigitChar == 'X') ? 10 : int.Parse(checkDigitChar.ToString());

                result = (sum + checkDigit) % 11 == 0;
            }
            else if (textEdit_add_ISBN.Text.Length == 13)//ISBN-13
            {
                int sum = 0;
                for (int i = 0; i < 12; i++)
                {
                    int digit = int.Parse(textEdit_add_ISBN.Text[i].ToString());
                    sum += (i % 2 == 0) ? digit : digit * 3;
                }

                int checkDigit = 10 - (sum % 10);
                if (checkDigit == 10) checkDigit = 0;

                result = checkDigit == int.Parse(textEdit_add_ISBN.Text[12].ToString());
            }
            else result = false;
            return result;
        }
        public void get_info_online(object title, object author, object category, object pages, object year, object publisher, int add_or_update)//1-add 2-update
        {
            if (add_or_update == 1) //ADD
            {
                textEdit_add_title.Text = title.ToString();
                textEdit_add_author.Text = author.ToString();
                comboBoxEdit_add_category.Text = category.ToString();
                numericUpDown_add_page.Text = pages.ToString();
                dateTimePicker_publish_date.Text = year.ToString();
                textEdit_add_publisher.Text = publisher.ToString();


                //LOAD IMG
                ToolTip toolTip = new ToolTip();
                List<string> img_list = new List<string> { image_l, image_m, image_s };
                foreach (string img_x in img_list) //get img_l img_m img_s or create
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(img_x) && img_x.Contains("http")) //LINK VALID
                        {
                            try
                            {
                                using (WebClient client = new WebClient())
                                {
                                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                                    byte[] imageData = client.DownloadData(img_x);
                                    using (MemoryStream stream = new MemoryStream(imageData))
                                    {

                                        pictureBox_book_cover2.Image = Image.FromStream(stream);
                                        if (pictureBox_book_cover2.Image.Height > 2)
                                        {
                                            label_placeholder_add_book_title.Visible = false;
                                            label_placeholder_add_book_author.Visible = false;
                                            pictureBox_book_cover2.Visible = true; //VIEW 
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
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
                void create_book_cover()//WHILE GETTING INFO ONLINE
                {
                    string title2 = title.ToString();
                    int maxLength = 20; //30char in every row
                    List<string> lines = new List<string>();
                    for (int i = 0; i < title2.Length; i += maxLength) //30 char parts
                    { lines.Add(title2.Substring(i, Math.Min(maxLength, title2.Length - i))); }

                    label_placeholder_add_book_title.Text = string.Join(Environment.NewLine, lines);
                    toolTip.SetToolTip(label_placeholder_add_book_title, title.ToString()); //tooltip
                    label_placeholder_add_book_author.Text = author.ToString();
                    toolTip.SetToolTip(label_placeholder_add_book_author, label_placeholder_add_book_author.Text); //tooltip

                    label_placeholder_add_book_title.Visible = true;
                    label_placeholder_add_book_author.Visible = true;
                }
                try
                {
                    if (pictureBox_book_cover2.Image.Height < 3) create_book_cover(); //IMG COULDN'T LOAD - CREATE BOOK COVER IMG
                }
                catch (Exception ex)
                {
                    create_book_cover(); //necessary
                    main.LOG(ex);
                }
            }
            else if (add_or_update == 2)//UPDATE
            {
                textEdit_title.Text = title.ToString();
                textEdit_author.Text = author.ToString();
                comboBoxEdit_category.Text = category.ToString();
                numericUpDown_page.Text = pages.ToString();
                dateTimePicker_publish_date1.Text = year.ToString();
                textEdit_publisher.Text = publisher.ToString();

                //LOAD IMG
                ToolTip toolTip = new ToolTip();
                List<string> img_list = new List<string> { image_l, image_m, image_s };
                foreach (string img_x in img_list) //get img_l img_m img_s or create
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(img_x) && img_x.Contains("http")) //LINK VALID
                        {
                            try
                            {
                                using (WebClient client = new WebClient())
                                {
                                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                                    byte[] imageData = client.DownloadData(img_x);
                                    using (MemoryStream stream = new MemoryStream(imageData))
                                    {

                                        pictureBox_book_cover1.Image = Image.FromStream(stream);
                                        if (pictureBox_book_cover1.Image.Height > 2)
                                        {
                                            label_placeholder_book_title.Visible = false;
                                            label_placeholder_book_author.Visible = false;
                                            pictureBox_book_cover1.Visible = true; //VIEW 
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
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
                void create_book_cover()//WHILE GETTING INFO ONLINE
                {
                    string title2 = title.ToString();
                    int maxLength = 20; //30char in every row
                    List<string> lines = new List<string>();
                    for (int i = 0; i < title2.Length; i += maxLength) //30 char parts
                    { lines.Add(title2.Substring(i, Math.Min(maxLength, title2.Length - i))); }

                    label_placeholder_book_title.Text = string.Join(Environment.NewLine, lines);
                    toolTip.SetToolTip(label_placeholder_book_title, title.ToString()); //tooltip

                    label_placeholder_book_author.Text = author.ToString();
                    toolTip.SetToolTip(label_placeholder_book_author, label_placeholder_book_author.Text); //tooltip

                    label_placeholder_book_title.Visible = true;
                    label_placeholder_book_author.Visible = true;
                }
                try
                {
                    if (pictureBox_book_cover1.Image.Height < 3) create_book_cover(); //IMG COULDN'T LOAD - CREATE BOOK COVER IMG
                }
                catch (Exception ex)
                {
                    create_book_cover(); //necessary
                    main.LOG(ex);
                }
            }
        }
        private void clear_add_textedit(object sender, EventArgs e)
        {
            if (sender is SimpleButton btn)
            {
                if (btn.Name.Contains("book"))
                {
                    textEdit_add_ISBN.Clear(); textEdit_add_title.Clear(); textEdit_add_author.Clear();
                    dateTimePicker_publish_date.ResetText(); textEdit_add_publisher.Clear();
                    comboBoxEdit_add_category.SelectedIndex = -1; numericUpDown_add_page.Value = 0;
                }
                if (btn.Name.Contains("ISBN")) textEdit_add_ISBN.Clear();
                else if (btn.Name.Contains("title")) textEdit_add_title.Clear();
                else if (btn.Name.Contains("author")) textEdit_add_author.Clear();
                else if (btn.Name.Contains("year")) dateTimePicker_publish_date.ResetText();
                else if (btn.Name.Contains("publisher")) textEdit_add_publisher.Clear();
                else if (btn.Name.Contains("category")) comboBoxEdit_add_category.SelectedIndex = -1;
                else if (btn.Name.Contains("page")) numericUpDown_add_page.Value = 0;
            }
        }
        private void textedit_leave(object sender, EventArgs e)
        {
            if (sender is TextEdit textEdit)
            { if (string.IsNullOrEmpty(textEdit.Text)) check_isbn(); }
        }
        private void simpleButton_paste_add_ISBN_Click(object sender, EventArgs e)//PASTE
        {
            textEdit_add_ISBN.Text = Clipboard.GetText();
            simpleButton_check_ISBN_Click(sender, e);
        }
        private void simpleButton_scan_barcode_Click(object sender, EventArgs e)
        {XtraMessageBox.Show("this suppose to use your barcode scanner"); }//TODOBARCODE
        private void simpleButton_check_ISBN_Click(object sender, EventArgs e)
        {
            bool result = check_isbn();

            if (result && sender != simpleButton_add_confirm) { XtraMessageBox.Show("ISBN Valid."); }
            else XtraMessageBox.Show("Invalid ISBN"); textEdit_add_ISBN.Focus();
        }
        private async void simpleButton_get_info_online2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit_add_ISBN.Text)) XtraMessageBox.Show("Enter ISBN", "No ISBN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var result = XtraMessageBox.Show("Are your sure to get book info online?\nSome book info may be inaccurate, check info after fetching.", "Get book info online", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string isbn = textEdit_add_ISBN.Text;
                    progressBar1.Visible = true;
                    panel_add_book.Enabled = false;
                    progressBar1.Style = ProgressBarStyle.Marquee; //load animation
                    await Task.Run(() => get_book_info_by_isbn(isbn, 1));//add
                    panel_add_book.Enabled = true;
                    progressBar1.Visible = false;
                    main.TLOG("3-1", "", textEdit_add_ISBN.Text);
                }
            }
        }
        private void simpleButton_add_confirm_Click(object sender, EventArgs e)
        {
            if (new[] { textEdit_add_ISBN.Text, textEdit_add_title.Text, textEdit_add_author.Text, dateTimePicker_publish_date.Text, textEdit_add_publisher.Text, comboBoxEdit_add_category.Text, numericUpDown_add_page.Text }.Any(string.IsNullOrEmpty))
            { XtraMessageBox.Show("empty"); }
            else
            {
                bool isbn_result = check_isbn();
                if (isbn_result)//ISBN VALID
                {
                    string query = "";
                    if (comboBoxEdit_add_category.SelectedIndex == -1)//ADDING NEW CATEGORY
                    {
                        query = $"INSERT OR IGNORE INTO categories(category) VALUES(@category)";
                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                        {
                            try
                            {
                                conn.Open();
                                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                { cmd.Parameters.AddWithValue("@category", comboBoxEdit_add_category.Text); cmd.ExecuteNonQuery(); }
                            }
                            catch (Exception ex)
                            {
                                main.LOG(ex);
                                XtraMessageBox.Show("Category adding error");
                            }
                        }
                    }

                    query = $"INSERT OR IGNORE INTO books (ISBN, title, author, year_of_publication, publisher, category, page_count, taken, missing, has_problem, date_added)" +
                    $" VALUES(@isbn,@title,@author,'{dateTimePicker_publish_date.DateTime.ToString("yyyy-MM-dd")}',@publisher,@category,'{numericUpDown_add_page.Text}',0,0,0,'{DateTime.Now.ToString("yyyy-MM-dd")}');";
                    using (var conn = new SQLiteConnection(main.connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (var command = new SQLiteCommand(query, conn))
                            {
                                command.Parameters.AddWithValue("@isbn", textEdit_add_ISBN.Text);
                                command.Parameters.AddWithValue("@title", textEdit_add_title.Text);
                                command.Parameters.AddWithValue("@author", textEdit_add_author.Text);
                                command.Parameters.AddWithValue("@publisher", textEdit_add_publisher.Text);
                                command.Parameters.AddWithValue("@category", comboBoxEdit_add_category.SelectedItem.ToString());
                                int result = command.ExecuteNonQuery();
                                if (result != 0)
                                {
                                    query = $"UPDATE books SET image_s=@image_s,image_m=@image_m,image_l=@image_l WHERE ISBN='{textEdit_add_ISBN.Text}';";
                                    using (var _conn = new SQLiteConnection(main.connectionString))
                                    {
                                        try
                                        {
                                            _conn.Open();
                                            using (var _command = new SQLiteCommand(query, _conn))
                                            {
                                                _command.Parameters.AddWithValue("@image_s", image_s);
                                                _command.Parameters.AddWithValue("@image_m", image_m);
                                                _command.Parameters.AddWithValue("@image_l", image_l);

                                                int _result = _command.ExecuteNonQuery();
                                                if (_result != 0) XtraMessageBox.Show("Book added");
                                                main.TLOG("3", "", textEdit_add_ISBN.Text);
                                                //navigationFrame1.SelectedPage = navigationPage1;//view book
                                                simpleButton_cancel_confirm_Click(null, null);
                                                textEdit_search_book.Text = string.Empty;
                                                textEdit_search_book.Text = textEdit_add_ISBN.Text;
                                                simpleButton_search_book_Click(sender, e);
                                                simpleButton_view_book_Click(sender, e);
                                                var openWiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                                                openWiseLibForm.load_random(null, null);
                                                if (openWiseLibForm != null && openWiseLibForm.selected_member_no != "0") { XtraMessageBox.Show("Please Re-Select lending book"); }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            XtraMessageBox.Show("book img adding error");
                                            main.LOG(ex);
                                        }
                                    }
                                }
                                else
                                {
                                    var _result = XtraMessageBox.Show("Book could not added: Already Exist\nDo you want to view book?", "Book Already Exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (_result == DialogResult.Yes)
                                    {
                                        simpleButton_cancel_confirm_Click(sender, e);
                                        simpleButton_clear_book_search_Click(sender, e);
                                        textEdit_search_book.Text = textEdit_add_ISBN.Text;
                                        simpleButton_search_book_Click(sender, e);
                                        simpleButton_view_book_Click(sender, e);
                                    }

                                }
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            MessageBox.Show("Error adding book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else XtraMessageBox.Show("ISBN Invalid");
            }
        }
        private void simpleButton_cancel_confirm_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(1400, 830); //res for book view
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point( //center
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
            navigationFrame1.SelectedPage = navigationPage1; //book main
        }
        private async Task get_book_info_by_isbn(string isbn, int add_or_update)//1-add 2-update
        {
            string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(responseBody);
                        var bookKey = $"ISBN:{isbn}";
                        if (json.ContainsKey(bookKey))
                        {
                            var book = json[bookKey];
                            object title = book["title"]?.ToString() ?? "Not Found";
                            object author = book["authors"]?[0]?["name"]?.ToString() ?? "Not Found";
                            object category = book["subjects"]?[0]?["name"]?.ToString() ?? "Unknown";
                            object pages = book["number_of_pages"]?.ToString() ?? "Not Found";
                            object year = book["publish_date"]?.ToString() ?? "Not Found";
                            object publisher = book["publishers"]?[0]?["name"]?.ToString() ?? "Not Found";

                            image_s = book["cover"]?["small"]?.ToString() ?? "NULL";
                            image_m = book["cover"]?["medium"]?.ToString() ?? "NULL";
                            image_l = book["cover"]?["large"]?.ToString() ?? "NULL";

                            this.Invoke((MethodInvoker)(() => get_info_online(title, author, category, pages, year, publisher, add_or_update)));//add
                        }
                        else XtraMessageBox.Show("Book could not found.");
                    }
                    else XtraMessageBox.Show($"Error: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    Console.WriteLine($"Error: {ex.Message}");
                }
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
                    ToolStripMenuItem btnDel = new ToolStripMenuItem("delete"); btnDel.Click += (s, ev) => { simpleButton_delete_book_Click(sender, e); };
                    ToolStripMenuItem btnView = new ToolStripMenuItem("view"); btnView.Click += (s, ev) => { simpleButton_view_book_Click(sender, e); };
                    ToolStripMenuItem btnCcell = new ToolStripMenuItem("copy cell"); btnCcell.Click += (s, ev) => { simpleButton_copy_cell_Click(sender, e); };
                    ToolStripMenuItem btnCrow = new ToolStripMenuItem("copy row"); btnCrow.Click += (s, ev) => { simpleButton_copy_row_Click(sender, e); };

                    menu.Items.Add(btnDel); menu.Items.Add(btnView); menu.Items.Add(btnCrow); menu.Items.Add(btnCcell); menu.Show(Cursor.Position);}}
        }
    }
}