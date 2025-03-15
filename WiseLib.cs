using DevExpress.CodeParser;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using Label = System.Windows.Forms.Label;
namespace WiseLib
{
    public partial class WiseLib : DevExpress.XtraEditors.XtraForm
    {
        public WiseLib()
        { InitializeComponent(); }

        public string selected_isbn = "0";
        public string _selected_isbn = "0";
        public string selected_member_no = "0";
        private DataTable allBooksDataTable;
        private DataTable filtered;

        private void WiseLib_load(object sender, EventArgs e)
        {
            main.TLOG("1","NULL","NULL");
            foreach (var panel in new[] { panel_member, panel_book_display, panel_connection, panel_book_search, panel_license, panel_grid, panel_right_buttons })
            { panel.Paint += main.Panel_Paint; } //PANEL PAINT
            this.BeginInvoke((Action)(() => { textEdit_member_no.Focus(); }));
            main.check_server_connection(simpleButton_current_connection, EventArgs.Empty); //check server 0ms
            main.check_connections(simpleButton_current_connection); //timer-5s
            main.check_connections(simpleButton_book_scanner); //timer-5s
            main.check_connections(simpleButton_card_scanner); //timer-5s
            load_random(sender, e); //load grid
            notif_panel();
            if (string.IsNullOrEmpty(main.library_name)) main.get_library_name();
            label_library_name.Text = main.library_name;
            label_library_name.Location = new Point(label_corazonthedev1.Left + (label_corazonthedev1.Width - label_library_name.Width) / 2,
            label_corazonthedev1.Top + (label_corazonthedev1.Height - label_library_name.Height) / 2 - 35);
            pictureBox_library_logo.Image = pictureBox_library_logo.Image = Image.FromFile(main.serverFolderPath+"logo.png");
            this.KeyPreview = true; this.KeyDown += new KeyEventHandler(shortcuts);
        }
        //------OTHER FUNCS--------------------------------------------------------------------------------------------------
        public int btn_count = 1;
        private void notif_panel()
        {
            string query = $"SELECT COUNT(*) FROM members WHERE return_date < DATE('now');";
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))//LICENSE
                    {
                        SimpleButton btn = new SimpleButton
                        {  Text = $"LICENSE 365 DAYS LEFT",
                            Size = new System.Drawing.Size(278, 41),
                            Cursor = Cursors.Hand,
                            Location = new System.Drawing.Point(1, 1)};
                        panel_notifications.Controls.Add(btn);
                        btn.Click += (s, e) => {open_github(null,null);};
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))//OVERDUE MEMBERS
                    {
                        long count = (long)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            
                            SimpleButton btn = new SimpleButton
                            {
                                Text = $"{count} Overdue Members",
                                Size = new System.Drawing.Size(278, 40),
                                Cursor = Cursors.Hand,
                                Location = new System.Drawing.Point(1, 41*btn_count)
                            };
                            panel_notifications.Controls.Add(btn);
                            btn.Click += (s, e) =>
                            {
                                this.Enabled = false;
                                members membersForm = new members();
                                membersForm.FormClosed += (ss, args) => this.Enabled = true; //activate when closed
                                membersForm.Show();
                                membersForm.simpleButton_view_overdue_members_Click(null, null);
                            };
                            btn_count++;
                        }
                    }
                    query = "SELECT COUNT(*) FROM members WHERE DATE(ban_date) = DATE('now');";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))//BAN ENDING
                    {
                        long count = (long)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            SimpleButton btn = new SimpleButton
                            {
                                Text = $"{count} Members' Ban Ending",
                                Size = new System.Drawing.Size(278, 40),
                                Cursor = Cursors.Hand,
                                Location = new System.Drawing.Point(1, 41*btn_count)
                            };
                            panel_notifications.Controls.Add(btn);
                            btn.Click += (s, e) =>
                            {
                                this.Enabled = false;
                                members membersForm = new members();
                                membersForm.FormClosed += (ss, args) => this.Enabled = true; //activate when closed
                                membersForm.Show();
                                membersForm.simpleButton_view_banned_members_Click(null, null);
                            };
                            btn_count++;
                        }
                    }
                    query = "SELECT COUNT(*) FROM reports WHERE SOLVED='0';";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))//ACTIVE REPORTS
                    {
                        long count = (long)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            SimpleButton btn = new SimpleButton
                            {
                                Text = $"{count} Active Reports",
                                Size = new System.Drawing.Size(278, 40),
                                Cursor = Cursors.Hand,
                                Location = new System.Drawing.Point(1, 41 * btn_count)
                            };
                            panel_notifications.Controls.Add(btn);
                            btn.Click += (s, e) =>
                            {
                                this.Enabled = false;
                                report reportForm = new report(0,"0");
                                reportForm.FormClosed += (ss, args) => this.Enabled = true; //activate when closed
                                reportForm.Show();
                                reportForm.accordionControlElement2_Click(reportForm.accordionControlElement2, e);
                            };
                            btn_count++;
                        }
                    }
                    if (simpleButton_card_scanner.Text.Contains("Not"))//CARD SCANNER 
                    {
                        SimpleButton btn = new SimpleButton
                        {
                            Text = $"CARD SCANNER DISCONNECTED",
                            Size = new System.Drawing.Size(278, 40),
                            Cursor = Cursors.Hand,
                            Location = new System.Drawing.Point(1, 41 * btn_count)
                        };
                        panel_notifications.Controls.Add(btn);
                        btn.Click += (s, e) =>
                        {
                            this.Enabled = false;
                            report reportForm = new report(0, "0");
                            reportForm.FormClosed += (ss, args) => this.Enabled = true; //activate when closed
                            reportForm.Show();
                            reportForm.accordionControlElement2_Click(reportForm.accordionControlElement2, e);
                        };
                        btn_count++;
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    XtraMessageBox.Show("notification panel load error", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void load_random(object sender, EventArgs e) //load grid by random
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

                        gridView1.FindFilterText = "";
                        label_grid_load_error_text.Visible = false; //ERR-MSG
                        gridView1.PopulateColumns(); //create custom column names
                        gridView1.Columns["page_count"].Caption = "page";
                        gridView1.Columns["year_of_publication"].Caption = "publication year";
                        gridView1.Columns["has_problem"].Caption = "problem";
                        string[] hiddenColumns = {"id", "ISBN", "image_s", "image_m", "image_l","date_added" };
                        foreach (var columnName in hiddenColumns) //hide columns
                        {
                            if (gridView1.Columns[columnName] != null)
                            { gridView1.Columns[columnName].Visible = false; }
                        }
                        gridView1.CustomColumnDisplayText += (_sender, _e) => //0 to no 1 to yes
                        {
                            if (_e.Column.FieldName == "taken" && _e.Value?.ToString() == "0") { _e.DisplayText = "no"; }
                            else if (_e.Column.FieldName == "taken" && _e.Value?.ToString() == "1") { _e.DisplayText = "yes"; }
                            if (_e.Column.FieldName == "missing" && _e.Value?.ToString() == "0") { _e.DisplayText = "no"; }
                            else if (_e.Column.FieldName == "missing" && _e.Value?.ToString() == "1") { _e.DisplayText = "yes"; }
                            if (_e.Column.FieldName == "has_problem" && _e.Value?.ToString() == "0") { _e.DisplayText = "no"; }
                            else if (_e.Column.FieldName == "has_problem" && _e.Value?.ToString() == "1") { _e.DisplayText = "yes"; }
                            
                        };
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
        private void display_right_panel_dynamic_buttons(bool condition)
        { new[] { simpleButton_up, simpleButton_down, simpleButton_right, simpleButton_left, simpleButton_view_book, simpleButton_view_author }.ToList().ForEach(button => button.Enabled = condition); }
        private void select_book_from_grid(object sender, EventArgs e)
        {
            panel_book_display.Enabled = true;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridView1.CalcHitInfo(gridControl1.PointToClient(Control.MousePosition));
            // LOAD\CREATE&VIEW BOOK IMG
            void load_book_status(int row_select_type)
            {
                selected_isbn = gridView1.GetRowCellValue(row_select_type, "ISBN")?.ToString(); //get isbn
                string missing_status = gridView1.GetRowCellValue(row_select_type, "missing")?.ToString(); 
                _selected_isbn = selected_isbn;
                //GET TEXTS
                //book author
                string author = gridView1.GetRowCellValue(row_select_type, "author")?.ToString();
                int maxLength = 30; //30char in every row
                List<string> _lines = new List<string>();
                for (int i = 0; i < author.Length; i += maxLength) //30 char parts
                { _lines.Add(author.Substring(i, Math.Min(maxLength, author.Length - i))); }
                label_book_author.Text = "Author: " + string.Join(Environment.NewLine, _lines);

                //book title 
                string title = gridView1.GetRowCellValue(row_select_type, "title")?.ToString();
                List<string> lines = new List<string>();
                for (int i = 0; i < title.Length; i += maxLength) //30 char parts
                {lines.Add(title.Substring(i, Math.Min(maxLength, title.Length - i)));}
                label_book_title.Text = "Title: " + string.Join(Environment.NewLine, lines);

                label_book_year.Text = "Publish year: " + gridView1.GetRowCellValue(row_select_type, "year_of_publication")?.ToString();
                label_book_publisher.Text = "Publisher: " + gridView1.GetRowCellValue(row_select_type, "publisher")?.ToString();
                label_book_status.Text = "Borrow Status: " + gridView1.GetRowCellValue(row_select_type, "taken")?.ToString();
                if (label_book_status.Text.Contains("0")) label_book_status.Text = "Borrow Status: No";
                else if (label_book_status.Text.Contains("1")) label_book_status.Text = "Borrow Status: Yes";
                //SET VISIBILITIES
                label_book_author.Visible = true;
                label_book_title.Visible = true;
                label_book_year.Visible = true;
                label_book_publisher.Visible = true;
                label_book_status.Visible = true;
                if (label_book_status.Text.Contains("Yes"))//BOOK ALREADY TAKEN
                {
                    simpleButton_view_book_main.Enabled = false;
                    label_borrowed_by_member.Visible = true;
                    simpleButton_lend.Visible = false; //NOT LENDABLE
                    string query = $"SELECT return_date FROM members WHERE borrowed_book_ISBN='{selected_isbn}';";
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
                                    label_book_return_date.Text = "Return Date: " + reader["return_date"].ToString();
                                    label_book_return_date.Visible = true;
                                }
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("Could not get return date info", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (label_book_status.Text.Contains("No")) //LENDABLE
                {
                    simpleButton_view_book_main.Enabled = true;
                    label_book_return_date.Visible = false;
                    simpleButton_lend.Visible = true;
                    label_borrowed_by_member.Visible = false;
                }
                if (missing_status.Contains("1"))
                {
                    simpleButton_lend.Enabled = false;
                    Label label = new Label
                    {
                        Name = "missingLabel",
                        Text = "BOOK IS MISSING",
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Location = new Point(
                            simpleButton_lend.Left + (simpleButton_lend.Width / 2),
                            simpleButton_lend.Top - 20)
                    };
                    label.Left -= label.Width / 2;
                    panel_book_display.Controls.Add(label);
                }
                else
                {
                    simpleButton_lend.Enabled = true;
                    Control lbl = panel_book_display.Controls["missingLabel"];
                    if (lbl != null)
                    {panel_book_display.Controls.Remove(lbl); lbl.Dispose(); }
                }
            }
            int clicked_row = hitInfo.RowHandle; //get row from MOUSE
            int selected_row = gridView1.FocusedRowHandle; //selected row KEY/BUTTON
            if (hitInfo.InRowCell || hitInfo.InRow)//MOUSE SELECT
            {load_book_status(clicked_row);}
            else //BUTTON SELECT
            {
                if (selected_row >= 0)
                { load_book_status(selected_row); }
                else
                { MessageBox.Show("No row is selected."); }
            }

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

                                            pictureBox_book_cover.Image = Image.FromStream(stream);
                                            if (pictureBox_book_cover.Image.Height > 2)
                                            {
                                                label_placeholder_book_title.Visible = false;
                                                label_placeholder_book_author.Visible = false;
                                                pictureBox_book_cover.Visible = true; //VIEW 
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
                    //finally
                    //{ create_book_cover_img(); }
                }
                try
                {
                    if (pictureBox_book_cover.Image.Height < 3) //IMG COULDN'T LOAD - CREATE BOOK COVER IMG
                    {create_book_cover_img();}
                }
                catch{ } //skip
            }
            void create_book_cover_img()//ON MAIN - CREATE BOOK COVER IMG
            { 
                string title = gridView1.GetRowCellValue(selected_row, "title")?.ToString(); //book title 
                int maxLength = 30; //30char in every row
                List<string> lines = new List<string>();
                for (int i = 0; i < title.Length; i += maxLength) //30 char parts
                { lines.Add(title.Substring(i, Math.Min(maxLength, title.Length - i))); }
                label_placeholder_book_title.Text = string.Join(Environment.NewLine, lines);

                string author = gridView1.GetRowCellValue(selected_row, "author")?.ToString();
                List<string> liness = new List<string>();
                for (int i = 0; i < author.Length; i += maxLength) //30 char parts
                { liness.Add(author.Substring(i, Math.Min(maxLength, author.Length - i))); }
                label_placeholder_book_author.Text = string.Join(Environment.NewLine, liness);

                label_placeholder_book_title.Visible = true;
                label_placeholder_book_author.Visible = true;
            }
        }
        private void display_book(string ISBN)
        {
            string query = $"SELECT title, author, year_of_publication, publisher, taken FROM books WHERE ISBN='{ISBN}';";
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
                            label_book_author.Text = "Author: " + reader["author"].ToString();
                            label_book_title.Text = "Title: " + reader["title"].ToString();
                            label_book_year.Text = "Publish Year: " + reader["year_of_publication"].ToString();
                            label_book_publisher.Text = "Publisher: " + reader["publisher"].ToString();
                            label_book_status.Text = "Borrow Status: " + reader["taken"].ToString();
                            //label_book_return_date.Text = "Return Date: " + reader["return_date"].ToString();
                            //label_book_return_date.Visible = true;
                            panel_book_display.Enabled = true;
                        }
                    }
                    if (label_book_status.Text.Contains("0")) label_book_status.Text = "Borrow Status: No";
                    else if (label_book_status.Text.Contains("1")) label_book_status.Text = "Borrow Status: Yes";
                    conn.Close();
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    XtraMessageBox.Show("Could not get info", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // LOAD\CREATE BOOK IMG
                var imageUrl = "";
                using (SQLiteConnection _conn = new SQLiteConnection(main.connectionString))
                {
                    List<string> img_list = new List<string> { "image_l", "image_m", "image_s" };
                    foreach (string img_x in img_list)
                    {
                        try
                        {
                            _conn.Open();
                            query = $"SELECT {img_x} FROM books WHERE ISBN= '{ISBN}'";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, _conn))
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
                                                pictureBox_book_cover.Visible = true; //VIEW BOOK IMG
                                                break;
                                            }
                                        }
                                    }
                                    catch (WebException ex)
                                    {
                                        if (img_x == img_list[img_list.Count - 1])
                                        { break; }
                                        main.LOG(ex);
                                        XtraMessageBox.Show("Book image web load error", "LOAD WEB ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            _conn.Close();
                        }
                        catch (Exception ex)
                        { main.LOG(ex); }
                    }
                }
                label_book_author.Visible = true;
                label_book_title.Visible = true;
                label_book_year.Visible = true;
                label_book_publisher.Visible = true;
                label_book_status.Visible = true;
            }
        }
        private void lend_book(object sender, EventArgs e) //LEND
        {
            var result = DialogResult;
            if (gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "has_problem")?.ToString() == "1")
            { result = XtraMessageBox.Show($"Are you sure to lend {label_book_title.Text} ?\nThis book has problem.", "Lending Book", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
            else { result = XtraMessageBox.Show($"Are you sure to lend {label_book_title.Text} ?", "Lending Book", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }
            if (result == DialogResult.Yes)
            {
                //transaction_no 1 = lend
                string query = $"INSERT INTO book_transactions(member_id,ISBN,borrow_date,return_date,transaction_type) VALUES('{selected_member_no}', '{selected_isbn}', '{DateTime.Now.ToString("yyyy-MM-dd")}', '{DateTime.Now.AddDays(15).ToString("yyyy-MM-dd")}', 1);";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    conn.Open();
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try //BOOK TRANSACTION
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {cmd.ExecuteNonQuery();}
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("Lend book Error " + ex.Message, "LEND ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback(); //undo
                        }
                        try //MEMBER BORROW STATUS = 1
                        {
                            query = $"UPDATE members SET borrow_status = '1',borrowed_book_ISBN = '{selected_isbn}', borrow_date = '{DateTime.Now.ToString("yyyy-MM-dd")}',return_date = '{DateTime.Now.AddDays(15).ToString("yyyy-MM-dd")}' WHERE member_id = '{selected_member_no}';";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {cmd.ExecuteNonQuery();}
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("Lend book Error " + ex.Message, "LEND ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback(); //undo
                        }
                        try //BOOK TAKEN = 1
                        {
                            query = $"UPDATE books SET taken = '1' WHERE ISBN = '{selected_isbn}';";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {cmd.ExecuteNonQuery();}
                            transaction.Commit(); //commit
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("Lend book Error " + ex.Message, "LEND ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback(); //undo
                        }
                    }
                    conn.Close();
                    load_random(sender, e); //re-load grid
                    display_book(selected_isbn);
                    simpleButton_lend.Visible = false;
                    simpleButton_search_Click(sender, e);
                    gridControl1.Enabled = false;
                    simpleButton_borrowed_book_view_Click(sender, e); //view borrowed book
                    XtraMessageBox.Show($"Book lended. Return date: {DateTime.Now.AddDays(15).ToString("yyyy-MM-dd")} ");
                    main.TLOG("5-0",selected_member_no,selected_isbn);
                }
            }
        }
        private void return_book(object sender, EventArgs e) //RETURN
        {
            var result = XtraMessageBox.Show("Are you sure to return book?", "Return",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {//transaction_no 2 = return
                string query = $"INSERT INTO book_transactions(member_id,ISBN,borrow_date,return_date,transaction_type) VALUES('{selected_member_no}', '{selected_isbn}', '{label_borrowed_book_borrow_date_text.Text}','{DateTime.Now.ToString("yyyy-MM-dd")}', 2);";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    conn.Open();

                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        //try //BOOK TRANSACTION
                        //{
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {cmd.ExecuteNonQuery();}
                        //}
                        //catch (Exception ex)
                        //{
                        //    main.LOG(ex);
                        //    XtraMessageBox.Show("Transaction Error " + ex.Message, "RETURN ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    transaction.Rollback(); //undo
                        //}
                        try //MEMBER BORROW STATUS = NO 
                        {
                            query = $"UPDATE members SET borrow_status = '0',borrowed_book_ISBN = NULL, borrow_date = NULL ,return_date = NULL WHERE member_id = '{selected_member_no}';";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("Update Status Error " + ex.Message, "RETURN ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback(); //undo
                        }
                        try //BOOK TAKEN = NO
                        {
                            query = $"UPDATE books SET taken = '0' WHERE ISBN = '{selected_isbn}';";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            transaction.Commit(); //commit
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("Return book Error " + ex.Message, "RETURN ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback(); //undo
                        }
                        conn.Close();
                        display_book(selected_isbn);
                        string sent_isbn = selected_isbn; //necessary
                        simpleButton_lend.Visible = true;
                        simpleButton_return_book.Visible = false;
                        simpleButton_search_Click(sender, e);
                        gridControl1.Enabled = true;
                        panel_book_search.Enabled = true;
                        main.TLOG("5-1", selected_member_no, selected_isbn);
                        if (Application.OpenForms["books"] is books booksForm) //update opened BOOKS form 
                        {
                            booksForm.simpleButton_clear_book_search_Click(sender,e);
                            booksForm.textEdit_search_book.Text = sent_isbn;
                            booksForm.simpleButton_search_book_Click(sender, e);
                            booksForm.simpleButton_view_book_Click(sender, e);
                            booksForm.Focus();
                        }
                        XtraMessageBox.Show("Book Returned Succesfully", "Book Return", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        load_random(null, null);
                    }
                }
            }
        }
        //------MEMBER SECTION-----------------------------------------------------------------------------------------------
        private void textEdit_member_no_KeyDown(object sender, KeyEventArgs e) //SEARCH MEMBER KEY.ENTER 
        {
            if ((e.KeyCode == Keys.Enter))
            { simpleButton_search_Click(sender, e); }
        }
        private void simpleButton_clear_member_search_Click(object sender, EventArgs e) //CLEAR INPUT
        {
            label_member_not_found.Visible = false;
            textEdit_member_no.Clear();
            textEdit_member_no.Focus();
        }
        public void simpleButton_search_Click(object sender, EventArgs e) //SEARCH MEMBER
        {
            selected_member_no = textEdit_member_no.Text.ToString().Trim(); //trim spaces, GET MEMBER_ID
            if (string.IsNullOrEmpty(selected_member_no) || selected_member_no.Any(char.IsLetter)) // EMPTY\CHAR
            { XtraMessageBox.Show("Invalid Member 0", "INVALID", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            {
                string query = $"SELECT member_name, member_surname, borrow_status, borrowed_book_ISBN, borrow_date, return_date, credit_point, ban_date FROM members WHERE member_id={selected_member_no};";
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
                                selected_isbn = reader["borrowed_book_ISBN"].ToString(); //GET ISBN
                                //LOAD INFO TEXTS NAV-PAGE-0
                                label_name_surname.Text = "Name: " + reader["member_name"].ToString() + " " + reader["member_surname"].ToString();
                                label_member_no.Text = $"Member No: {selected_member_no}";
                                label_borrow_status_text.Text = reader["borrow_status"].ToString(); // YES|NO
                                label_credit_point.Text = reader["credit_point"].ToString(); //credit point -1/0/1/2/3

                                if (label_credit_point.Text == "-1")
                                {
                                    label_banned.Text = "BANNED UNTIL " + reader["ban_date"].ToString();
                                    (label_banned.Visible, simpleButton_confirm_member.Enabled) = (true, false);
                                }

                                else (simpleButton_confirm_member.Enabled, label_banned.Visible) = (true, false);

                                if (label_borrow_status_text.Text.Contains("1")) //MEMBER ALREADY BORROWED ANY BOOK
                                { //GET BORROWED BOOK DATE-INFO
                                    string _query = $"SELECT author, title FROM books WHERE ISBN='{reader["borrowed_book_ISBN"].ToString()}';";
                                    using (SQLiteConnection _conn = new SQLiteConnection(main.connectionString))
                                    {
                                        try
                                        {
                                            _conn.Open();
                                            using (SQLiteCommand _cmd = new SQLiteCommand(_query, _conn))
                                            using (SQLiteDataReader _reader = _cmd.ExecuteReader())
                                            {
                                                if (_reader.Read())
                                                {
                                                    label_borrowed_book_return_date_text.Text = reader["return_date"].ToString();
                                                    label_borrowed_book_borrow_date_text.Text = reader["borrow_date"].ToString();
                                                }
                                            }
                                            _conn.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            main.LOG(ex);
                                            XtraMessageBox.Show("Could not get member borrow info " + ex, "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    //NAV-PAGE-1
                                    simpleButton_confirm_member.Visible = false; //HIDE CONFIRM
                                    simpleButton_borrowed_book_view.Visible = true; //SHOW VIEW BORROWED BOOK BUTTON
                                    panel_member_borrow_info.Visible = true; //SHOW BORROW PANEL
                                    navigationFrame_member.SelectedPageIndex = 1; //PAGE SWITCH
                                    simpleButton_borrowed_book_view.Focus();
                                    label_borrow_status_text.Text = "Yes";
                                }
                                else //MEMBER CAN BORROW 
                                {
                                    //visibilities
                                    //(simpleButton_return_book.Visible, label_borrow_status_text.Text, simpleButton_confirm_member.Visible, label_borrowed_book_title.Visible, label_borrowed_book_return_date.Visible, label_borrowed_book_return_date_text.Visible) = (false, "Yes", true, false, false, false);
                                    simpleButton_confirm_member.Visible = true; //SHOW CONFIRM
                                    simpleButton_borrowed_book_view.Visible = false; //HIDE VIEW BORROWED BOOK
                                    panel_member_borrow_info.Visible = false; //HIDE BORROW PANEL
                                    navigationFrame_member.SelectedPageIndex = 1; //PAGE SWITCH
                                    simpleButton_confirm_member.Focus();
                                    label_borrow_status_text.Text = "No";
                                }

                                //LOAD MEMBER IMG
                                string imagePath = $"{main.imgPath}{selected_member_no}.jpg";
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
                                conn.Close();
                            }
                            else { label_member_not_found.Visible = true; conn.Close(); textEdit_member_no.Focus(); } //ERR-MSG
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("Server error" + ex, "SERVER ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void keyboard_buttons(object sender, EventArgs e)
        {
            var button = sender as SimpleButton;
            try
            {
                if (button.ToolTip == "backspace") { textEdit_member_no.Text = textEdit_member_no.Text.Substring(0, textEdit_member_no.Text.Length - 1); } //member search
                else if (button.ToolTip == "backspace M") { textEdit1.Text = textEdit1.Text.Substring(0, textEdit1.Text.Length - 1); } //book search
                else if (button.ToolTip == "clear") { simpleButton_clear_member_search_Click(sender, e); }
                else if (button != null)
                { textEdit_member_no.Text += button.Text; }
            }
            catch { } //skip
        }
        private void simpleButton_scan_card_status_Click(object sender, EventArgs e) //SCAN CARD
        {
            XtraMessageBox.Show("this suppose to listen card scanner");//TODO listen card scanner
        }
        private void simpleButton_signup_Click(object sender, EventArgs e) //SIGN UP 
        {
            this.Enabled = false;
            signup signupForm = new signup();
            signupForm.FormClosed += (s, args) => this.Enabled = true; //activate when signup closed
            signupForm.FormClosed += (s, args) => textEdit_member_no.Focus(); //focus when signup closed
            signupForm.Show();
        }
        //------MEMBER SECTION > NAVPAGE MEMBER INFO------------------------------------------------------------------------------------
        private void simpleButton_confirm_member_Click(object sender, EventArgs e) //CONFIRM MEMBER
        {
            panel_book_search.Enabled = true;
            gridControl1.Enabled = true; //GRID BOT PANEL
            display_right_panel_dynamic_buttons(true); //right-panel-buttons
            textEdit1.Focus(); //search book
        }
        private void simpleButton_confirm_member_KeyDown(object sender, KeyEventArgs e) //BUTTON ENTERED
        { simpleButton_confirm_member_Click(sender, e); }
        public void simpleButton_borrowed_book_view_Click(object sender, EventArgs e) //VIEW BORROWED BOOK
        {
            display_book(selected_isbn); //borrowed book display
            simpleButton_return_book.Visible = true; //returnable
            simpleButton_return_book.Enabled = true; //returnable 
            simpleButton_lend.Visible = false; //non-landable
        }
        public void simpleButton_cancel_member_Click(object sender, EventArgs e) //CANCEL MEMBER
        {
            pictureBox_member_img.Image = Properties.Resources.person; //LOAD DEFAULT IMG
            navigationFrame_member.SelectedPageIndex = 0; //set nav-page-0
            panel_book_search.Enabled = false; //SEARCH
            gridControl1.Enabled = false; //GRID 
            simpleButton_return_book.Visible = false; //return book button
            panel_book_display.Enabled = false; //DISABLE BOOK-DISPLAY SECTION
            simpleButton_clear_member_search_Click(sender, e); //clear input
            display_right_panel_dynamic_buttons(false); //right-panel-buttons
        }
        private void simpleButton_view_selected_member_Click(object sender, EventArgs e)
        {
            simpleButton_members_Click(sender, e);
            members membersForm = Application.OpenForms["members"] as members;
            membersForm.simpleButton_clear_member_search_Click(sender, e);
            membersForm.textEdit_search_member.Text = selected_member_no;
            membersForm.simpleButton_search_member_Click(sender, e);
            membersForm.simpleButton_view_member_Click(sender, e);
        }
        //------BOOK SECTION--------------------------------------------------------------------------------------------------
        private void simpleButton_view_book_main_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            books booksForm = new books();
            booksForm.FormClosed += (s, args) => this.Enabled = true; //activate when closed
            
            //simpleButton_books_Click(sender, e);
            booksForm.textEdit_search_book.Clear();
            booksForm.textEdit_search_book.Text = _selected_isbn;
            booksForm.simpleButton_search_book_Click(sender, e);
            booksForm.simpleButton_up.PerformClick();
            booksForm.simpleButton_view_book_Click(sender, e);
            booksForm.Show();
        }
        //simpleButton_report_book2 - REPORT BOOK
        //simpleButton_return_book_Click - RETURN BOOK
        //simpleButton_lend_Click - LEND BOOK
        //------GRID SECTION--------------------------------------------------------------------------------------------------
        private void open_github(object sender, EventArgs e) { connection_status.open_github(sender, e); } //redirect to github
        private void simpleButton_clear_book_search_Click(object sender, EventArgs e) //BOOK SEARCH CLEAR
        {
            textEdit1.Clear();
            load_random(sender, e);
            label3_found.Visible = false;
        }
        private void textEdit1_KeyDown(object sender, KeyEventArgs e) //SEARCH-BOX ENTER-PRESSED
        {
            if (e.KeyCode == Keys.Enter)
            {
                simpleButton1_Click(sender, e);
                gridControl1.Focus();
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e) //SEARCH BOOK
        {
            string filter = textEdit1.Text.ToString();
            if (string.IsNullOrEmpty(filter)) //EMPTY INPUT
            { }//NOTHING
            else
            {
                string query = $"SELECT * FROM books WHERE title LIKE @filter OR author LIKE @filter LIMIT 150;";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            filtered = new DataTable();
                            adapter.Fill(filtered);
                            gridControl1.DataSource = filtered;
                        }
                    }
                    catch (Exception ex)
                    { main.LOG(ex); }
                }
                label3_found.Visible = true;
                label3_found.Text = $"{gridView1.RowCount} result found";
                gridView1.FindFilterText = filter;
                gridView1.RefreshData();
                gridControl1.Focus();
            }
        }
        //------GRID SECTION > GRID-------------------------------------------------------------------------------------------
        private void pictureBox_wiselib_logo_MouseDown(object sender, MouseEventArgs e)//backup not necessary probably
        { open_github(sender, e); }
        private void gridView1_DoubleClick(object sender, EventArgs e) { select_book_from_grid(sender, e); }//SELECT BOOK
        //--------------GRID SECTION > RIGHT BUTTON PANEL---------------------------------------------------------------------------
        private void simpleButton_members_Click(object sender, EventArgs e) //MEMBERS
        {
            this.Enabled = false;
            members membersForm = new members();
            membersForm.FormClosed += (s, args) => this.Enabled = true; //activate when signup closed
            membersForm.Show();
        }
        public void simpleButton_books_Click(object sender, EventArgs e) //BOOKS
        {
            this.Enabled = false;
            books booksForm = new books();
            booksForm.FormClosed += (s, args) => this.Enabled = true; //activate when closed
            booksForm.Show();
        }
        private void handle_arrow_buttons(object sender, EventArgs e)//ARROW BUTTONS
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
        public void simpleButton_view_book_Click(object sender, EventArgs e) //VIEW BOOK BUTTON
        { select_book_from_grid(sender, e); }
        private void simpleButton_view_author_Click(object sender, EventArgs e) //SEARCH AUTHOR
        {
            int selectedRowHandle = gridView1.FocusedRowHandle;
            textEdit1.Text = gridView1.GetRowCellValue(selectedRowHandle, "author")?.ToString();
            simpleButton1_Click(sender, e);
        }
        private void simpleButton_notifications_Click(object sender, EventArgs e) //NOTIFICATIONS
        {panel_notifications.Visible = !panel_notifications.Visible; } //show-hide

        private void simpleButton_history_Click(object sender, EventArgs e)
        {
            history historyForm = new history(4, ""); //member
            historyForm.FormClosed += (s, args) => this.Enabled = true; //enable when closed
            historyForm.ShowDialog();

            //Form form = new Form();
            //form.Size = new System.Drawing.Size(700, 700);
            //form.StartPosition = FormStartPosition.CenterScreen;
            //form.MaximizeBox = false;
            //form.MinimizeBox = false;

            //GridControl gridControl = new GridControl();
            //GridView gridView = new GridView(gridControl);
            //gridControl.MainView = gridView;
            //gridControl.Dock = DockStyle.Fill;
            //gridView.OptionsView.ColumnAutoWidth = true;
            //form.Controls.Add(gridControl);

            //form.Controls.Add(gridControl);
            //DataTable dt = new DataTable();
            //using (var conn = new SQLiteConnection(main.connectionString))
            //{
            //    conn.Open();
            //    using (var cmd = new SQLiteCommand("SELECT * FROM log_transactions ORDER BY transaction_id DESC", conn))
            //    using (var reader = cmd.ExecuteReader())
            //    {
            //        dt.Load(reader); 
            //    } 
            //}
            //foreach (DataRow row in dt.Rows)
            //{
            //    if (row["member_id"].ToString() == "NULL") row["member_id"] = string.Empty;
            //    if (row["book_isbn"].ToString() == "NULL") row["book_isbn"] = string.Empty;

            //    if (row["transaction_no"].ToString() == "0") { row["transaction_no"] = "Setting Connection Confirmed"; }
            //    if (row["transaction_no"].ToString() == "0-1") { row["transaction_no"] = "Setting Connection Confirmed"; }
            //    else if (row["transaction_no"].ToString() == "0-2") { row["transaction_no"] = "WiseLib Launch"; }
            //    else if (row["transaction_no"].ToString() == "0-3") { row["transaction_no"] = "WiseLib Launch"; }
            //    else if (row["transaction_no"].ToString() == "1") { row["transaction_no"] = "WiseLib Launch"; }

            //    else if (row["transaction_no"].ToString() == "2") { row["transaction_no"] = $"Member Added"; }
            //    else if (row["transaction_no"].ToString() == "2-1") { row["transaction_no"] = $"Member Deleted"; }
            //    else if (row["transaction_no"].ToString() == "2-2") { row["transaction_no"] = $"Member Credit Point Increased"; }
            //    else if (row["transaction_no"].ToString() == "2-3") { row["transaction_no"] = $"Member Credit Point Decreased"; }
            //    else if (row["transaction_no"].ToString() == "2-4") { row["transaction_no"] = $"Member Banned"; }
            //    else if (row["transaction_no"].ToString() == "2-5") { row["transaction_no"] = $"Member Card Regenerated"; }
            //    else if (row["transaction_no"].ToString() == "2-6") { row["transaction_no"] = $"Members Imported"; }

            //    else if (row["transaction_no"].ToString() == "3") { row["transaction_no"] = $"Book Added "; }
            //    else if (row["transaction_no"].ToString() == "3-1") { continue; } //get book info online
            //    else if (row["transaction_no"].ToString() == "3-2") { row["transaction_no"] = $"Book Deleted"; }
            //    else if (row["transaction_no"].ToString() == "3-3") { row["transaction_no"] = $"Book edited"; }
            //    else if (row["transaction_no"].ToString() == "3-4") { row["transaction_no"] = $"Book Set as MISSING"; }
            //    else if (row["transaction_no"].ToString() == "3-5") { row["transaction_no"] = $"Book Set as NOT MISSING"; }
            //    else if (row["transaction_no"].ToString() == "3-6") { row["transaction_no"] = $"Book Reported"; }
            //    else if (row["transaction_no"].ToString() == "3-6") { row["transaction_no"] = $"Books Imported"; }

            //    else if (row["transaction_no"].ToString() == "4-2") { row["transaction_no"] = $"Member Report Created"; }
            //    else if (row["transaction_no"].ToString() == "4-3") { row["transaction_no"] = $"Book Report Created"; }
            //    else if (row["transaction_no"].ToString() == "4-3") { row["transaction_no"] = $"Report Edited"; }
            //    else if (row["transaction_no"].ToString() == "4-4") { row["transaction_no"] = $"Report Set as SOLVED"; }
            //    else if (row["transaction_no"].ToString() == "4-5") { row["transaction_no"] = $"Report Set as NOT SOLVED"; }

            //    else if (row["transaction_no"].ToString() == "5-0") { row["transaction_no"] = $"Book Lended"; }
            //    else if (row["transaction_no"].ToString() == "4-5") { row["transaction_no"] = $"Book Returned"; }
            //}

            //gridControl.DataSource = dt;  
            //gridView.Columns["transaction_id"].Caption = "ID";
            //gridView.Columns["transaction_no"].Caption = "TRANSACTION";
            //gridView.Columns["transaction_date"].Caption = "Date";
            //gridView.Columns["member_id"].Caption = "Member ID";
            //gridView.Columns["book_isbn"].Caption= "Book ISBN";
            //gridView.BestFitColumns();
            //var WiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
            //main.copy_grid(WiseLibForm.gridView1, gridView);

            //form.ShowDialog();
        }
        private void simpleButton_statistics_Click(object sender, EventArgs e) //STATISTICS
        {
            this.Enabled = false;
            statistics statisticsForm = new statistics();
            statisticsForm.FormClosed += (s, args) => this.Enabled = true; //activate when closed
            statisticsForm.Show();
        }
        //-----CONNECTION SECTION BOTTOM-LEFT----------------------------------------------------------------------------------
        private void simpleButton_book_scanner_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            connection_status connection_statusForm = new connection_status(); //book connection status
            connection_statusForm.FormClosed += (s, args) => this.Enabled = true; //activate when connection_status closed
            connection_statusForm.Show();
        }
        private void simpleButton_card_scanner_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            connection_status connection_statusForm = new connection_status(); //card connection status
            connection_statusForm.FormClosed += (s, args) => this.Enabled = true; //activate when connection_status closed
            connection_statusForm.Show();
        }
        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { select_book_from_grid(sender, e); }
            else if (e.KeyCode == Keys.Escape)
            {
                textEdit1.Clear();
                load_random(sender, e);
                textEdit1.Focus();
            }
        }
        public void simpleButton_current_connection_Click(object sender, EventArgs e)//view connections
        {
            this.Enabled = false;
            connection_status connectionForm = new connection_status();
            connectionForm.FormClosed += (s, args) => this.Enabled = true; //activate when closed
            connectionForm.FormClosed += (s, args) => this.Focus(); //focus when closed
            connectionForm.Show();
        }

        private void simpleButton_reports_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            report reportForm = new report(0,"0");
            reportForm.FormClosed += (s, args) => this.Enabled = true; //activate when closed
            reportForm.FormClosed += (s, args) => this.Focus(); //focus when closed
            reportForm.Show();
        }

        private void WiseLib_FormClosing(object sender, FormClosingEventArgs e)//backup when exiting
        {
            connection_status cStatForm = new connection_status();
            cStatForm.simpleButton_backup_database_Click(null,null);
        }

        //---------------------------------------------------------------------------------------------------------------------

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
                {
                    Control lbl = parent.Controls[labelName];
                    if (lbl != null) parent.Controls.Remove(lbl);
                }
            }
        }
    }
}