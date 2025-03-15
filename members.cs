using DevExpress.Skins;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiseLib
{
    public partial class members : DevExpress.XtraEditors.XtraForm
    {
        public members()
        {InitializeComponent(); this.KeyPreview = true; this.KeyDown += new KeyEventHandler(shortcuts); }
        private int currentPage = 1;
        private int pageSize = 23; //23 row in each page
        private void members_Load(object sender, EventArgs e)
        {
            this.BeginInvoke((Action)(() => { this.Focus(); }));
            foreach (var panel in new[] { panel1, panel2, panel4, panel5, panel6 })
            { panel.Paint += main.Panel_Paint; }
            loadDataWithPagination(currentPage, pageSize, ""); //load all
        }
        //------OTHER FUNCS
        private void keyboard_buttons(object sender, EventArgs e)//KEYBOARD
        {
            var button = sender as SimpleButton;
            if (button.ToolTip == "backspace") { if (textEdit_search_member.Text.Length > 0) { textEdit_search_member.Text = textEdit_search_member.Text.Substring(0, textEdit_search_member.Text.Length - 1); } }
            else if (button.ToolTip == "clear") { simpleButton_clear_member_search_Click(sender, e); }
            else if (button != null)
            { textEdit_search_member.Text += button.Text; }
        }
        private void loadDataWithPagination(int pageNumber, int pageSize, string searchQuery = "")
        {
            if (main.database_connection_status)
            {
                int skip = (pageNumber - 1) * pageSize;
                string query;

                if (string.IsNullOrEmpty(searchQuery))
                { query = $"SELECT * FROM members LIMIT {pageSize} OFFSET {skip}"; } //get all
                else
                { query = $"SELECT * FROM members WHERE member_name OR member_surname OR member_id LIKE @searchquery LIMIT {pageSize} OFFSET {skip}"; }

                using (var conn = new SQLiteConnection(main.connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        try { cmd.Parameters.AddWithValue("@searchquery", searchQuery); }
                        catch { } //skip

                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        gridControl1.DataSource = dt;



                        gridView1.FindFilterText = "";
                        gridView1.PopulateColumns(); //create custom column names
                        gridView1.Columns["member_id"].Caption = "ID";
                        gridView1.Columns["member_name"].Caption = "name";
                        gridView1.Columns["member_surname"].Caption = "surname";
                        gridView1.Columns["borrow_status"].Caption = "borrow status";
                        gridView1.Columns["join_date"].Caption = "join date";
                        gridView1.Columns["credit_point"].Caption = "credit point";
                        gridView1.Columns["banned"].Caption = "ban";
                        gridView1.Columns["ban_date"].Caption = "ban date";
                        string[] hiddenColumns = { "borrowed_book_ISBN", "borrow_date", "return_date" };
                        foreach (var columnName in hiddenColumns) //hide columns
                        {
                            if (gridView1.Columns[columnName] != null)
                            { gridView1.Columns[columnName].Visible = false; }
                        }
                        gridView1.CustomColumnDisplayText += (sender, e) => //0 to no 1 to yes
                        {
                            if (e.Column.FieldName == "banned" && e.Value?.ToString() == "0") { e.DisplayText = "no"; }
                            else if (e.Column.FieldName == "banned" && e.Value?.ToString() == "1") { e.DisplayText = "yes"; }
                            if (e.Column.FieldName == "borrow_status" && e.Value?.ToString() == "0") { e.DisplayText = "no"; }
                            else if (e.Column.FieldName == "borrow_status" && e.Value?.ToString() == "1") { e.DisplayText = "yes"; }
                        };
                        gridView1.RefreshData();
                        gridView1.BestFitColumns();

                    }
                    conn.Close();
                }
            }
            else { label_grid_load_error_text.Visible = true; }
        }
        private void searchData(string searchQuery)
        { loadDataWithPagination(currentPage, pageSize, searchQuery); }
        private void check_score()
        {
            int current_score = int.Parse(label_member_score.Text);
            if (current_score == -1)
            { label_banned.Visible = true; }
            else
            { label_banned.Visible = false; }
            simpleButton_decrease_score.Enabled = current_score > -1;
            simpleButton_increase_score.Enabled = current_score < 3;
        }
        //-------------NAVIGATION PAGE 1------------------------------------------------------------------------------------
        private void pictureBox_wiselib_logo_Click(object sender, EventArgs e)//redirect to github
        { connection_status.open_github(sender, e); }
        private void textEdit_search_member_KeyDown(object sender, KeyEventArgs e) //KEY ENTER
        {
            if (e.KeyCode == Keys.Enter)
            { simpleButton_search_member_Click(sender, e); }
        }
        private void simpleButton_paste_Click(object sender, EventArgs e)//PASTE
        {
            textEdit_search_member.Text = Clipboard.GetText();
            simpleButton_search_member_Click(sender, e); //search
        }
        public void simpleButton_search_member_Click(object sender, EventArgs e)//SEARCH
        {
            keyboard_panel.Visible = false;
            string searchQuery = textEdit_search_member.Text;
            searchData(searchQuery);
        }
        public void simpleButton_clear_member_search_Click(object sender, EventArgs e) //CLEAR
        {
            textEdit_search_member.Clear();
            loadDataWithPagination(currentPage, pageSize, "");
        }
        private void simpleButton_show_hide_keyboard_Click(object sender, EventArgs e) //KEYBOARD
        { keyboard_panel.Visible = !keyboard_panel.Visible; }//show-hide keyboard
        //------RIGHT PANEL----------------------------------------------------------------------------------------
        private void simpleButton_add_member_Click(object sender, EventArgs e)//ADD MEMBER
        {
            this.Enabled = false;
            signup signupForm = new signup();
            signupForm.FormClosed += (s, args) => this.Enabled = true; //activate when signup closed
            signupForm.FormClosed += (s, args) => loadDataWithPagination(currentPage, pageSize);
            signupForm.Show();
        }
        private void simpleButton_delete_member_Click(object sender, EventArgs e) //DELETE MEMBER
        {
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row
            if (selectedRowHandle >= 0)
            {
                var selected_member_id = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[0]); //select first column
                var selected_member_name = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[1]);
                var selected_member_surname = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[2]);
                var selected_member_borrow_status = gridView1.GetRowCellValue(selectedRowHandle, "borrow_status")?.ToString();
                if (selected_member_borrow_status == "1" || selected_member_borrow_status == "Yes")
                { XtraMessageBox.Show("Can not delete member: Member has borrowed book", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                else
                {
                    DialogResult result = XtraMessageBox.Show($"Are you sure to delete member {selected_member_name} {selected_member_surname} ?", "Delete Member", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        DialogResult _result = XtraMessageBox.Show($"Member {selected_member_name} {selected_member_surname} will be deleted.", "Delete Member", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (_result == DialogResult.Yes)
                        {
                            string query = $"DELETE FROM members WHERE member_id = {selected_member_id}";
                            using (var conn = new SQLiteConnection(main.connectionString))
                            {
                                try
                                {
                                    conn.Open();
                                    using (var command = new SQLiteCommand(query, conn))
                                    {
                                        command.ExecuteNonQuery(); //delete from database
                                        gridView1.DeleteRow(selectedRowHandle); //delete from grid
                                        gridControl1.Refresh();
                                        try
                                        { pictureBox_member_img.Image.Dispose(); }
                                        catch { }//skip
                                        pictureBox_member_img.Image = Image.FromFile(main.cardPath + "cardexample.jpg");
                                        simpleButton_cancel.Enabled = true;
                                        label_banned.Visible = false;
                                        main.TLOG("2-1", selected_member_id.ToString(), "NULL");

                                        GC.Collect();
                                        GC.WaitForPendingFinalizers();
                                        File.Delete(Path.Combine(main.cardPath, selected_member_id + ".jpg"));//delete card img
                                        File.Delete(Path.Combine(main.imgPath, selected_member_id + ".jpg"));//delete member img
                                    }
                                    conn.Close();
                                    XtraMessageBox.Show("Member Deleted Succesfully");
                                }
                                catch (Exception ex)
                                {
                                    main.LOG(ex);
                                    MessageBox.Show("Error deleting member.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void simpleButton_view_member_Click(object sender, EventArgs e)//VIEW MEMBER
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog; //default 1400x830 for member view
            this.Size = new Size(1020, 430);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row

            if (selectedRowHandle >= 0) //display member card
            {
                var firstColumnValue = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[0]); //select member id
                try
                {pictureBox_member_img.Image = Image.FromFile(main.cardPath + firstColumnValue.ToString() + ".jpg");}
                catch (Exception ex)
                {main.LOG(ex); XtraMessageBox.Show("Get member card error", "Get Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);}
                navigationFrame1.SelectedPage = navigationPage2; //view member
            }
            else
            { MessageBox.Show("No row is selected."); }
            string query = "";
            int selected_member = gridView1.FocusedRowHandle;
            //BANNED
            if (gridView1.GetRowCellValue(selected_member, "banned")?.ToString() == "1" || gridView1.GetRowCellValue(selected_member, "banned")?.ToString() == "Yes" || gridView1.GetRowCellValue(selected_member, "credit_point")?.ToString() == "-1" || label_banned.Visible == true)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                    {
                        conn.Open();
                        query = $"SELECT ban_date FROM members WHERE member_id = '{gridView1.GetRowCellValue(selected_member, "member_id")?.ToString()}'";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                DateTime banDate = Convert.ToDateTime(result);
                                label_banned.Text = "BANNED UNTIL " + banDate.ToString("dd-MM-yyyy");
                            }
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    XtraMessageBox.Show("Could not get ban date", "Get error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                navigationFrame2.SelectedPage = navigationPage3; //borrow status
                label_member_score.Text = "-1";
                label_banned.Visible = true;
                new[] { simpleButton_set_member_score, simpleButton_regenerate_card, simpleButton_report_member, simpleButton_ban_member }.ToList().ForEach(btn => btn.Enabled = false);
                return;
            }
            else //NOT BANNED
            {
                label_banned.Visible = false;
                //enable buttons
                new[] { simpleButton_set_member_score, simpleButton_history, simpleButton_regenerate_card, simpleButton_report_member, simpleButton_ban_member }.ToList().ForEach(btn => btn.Enabled = true);
            }

            if (gridView1.GetRowCellValue(selected_member, "borrow_status")?.ToString() == "1" || gridView1.GetRowCellValue(selected_member, "borrow_status")?.ToString() == "Yes") //MEMBER ALREADY BORROWED BOOK
            {
                label_member_borrow_status.Text = "Borrowed Book: Yes";
                query = $"SELECT title, author FROM books WHERE ISBN='{gridView1.GetRowCellValue(selected_member, "borrowed_book_ISBN")?.ToString()}';";
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
                                label_borrowed_book_title.Text = "Book Title: " + reader["title"].ToString();
                                label_borrowed_book_author.Text = "Book Author: " + reader["author"].ToString();
                            }
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("Could not get book info", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                label_borrow_date.Text = "Borrow Date: " + gridView1.GetRowCellValue(selected_member, "borrow_date")?.ToString();
                label_return_date.Text = "Return Date: " + gridView1.GetRowCellValue(selected_member, "return_date")?.ToString();
            }
            else
            {
                label_member_borrow_status.Text = "Borrowed Book: No"; //MEMBER NOT BORROWED BOOK
                label_borrowed_book_title.Text = "Book Title: None";
                label_borrowed_book_author.Text = "Book Author: None";
                label_borrow_date.Text = "Borrow Date: None";
                label_return_date.Text = "Return Date: None";
            }
            label_member_score.Text = gridView1.GetRowCellValue(selected_member, "credit_point")?.ToString();

            query = $"SELECT ID FROM reports WHERE NO='{gridView1.GetRowCellValue(selected_member, "member_id")?.ToString()}' AND SOLVED=0;";
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) simpleButton_view_report.Visible = true;
                        else simpleButton_view_report.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    XtraMessageBox.Show("Could not get member report info", "GET ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                query = $"SELECT COUNT(*) FROM book_transactions WHERE member_id ='{gridView1.GetRowCellValue(selected_member, "member_id")?.ToString()}';";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    label_member_total_book_borrow.Text = "total books borrowed: " + count.ToString();
                    label_member_total_book_borrow.Left = simpleButton_cancel.Right - label_member_total_book_borrow.Width;
                }
            }
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
        private void simpleButton_import_members_Click(object sender, EventArgs e)//REPORT
        {
            this.Enabled = false;
            importer importerForm = new importer(2);
            importerForm.FormClosed += (s, args) => this.Enabled = true; //activate when signup closed
            importerForm.Show();
        }
        //------NAVIGATION PAGE 2-----------------------------------------------------------------------------------------
        private void simpleButton_set_member_score_Click(object sender, EventArgs e)//CREDIT
        {
            check_score();
            simpleButton_set_member_score.Visible = false;
            simpleButton_increase_score.Visible = true;
            simpleButton_decrease_score.Visible = true;
        }
        private void simpleButton_increase_score_Click(object sender, EventArgs e) //CREDIT INCREASE
        {
            DialogResult result = XtraMessageBox.Show($"Member score will be increased", "Increase Score", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int new_score = int.Parse(label_member_score.Text) + 1;
                label_member_score.Text = new_score.ToString();
                check_score();
                int selected_member = gridView1.FocusedRowHandle;
                string query = $"UPDATE members SET credit_point = '{new_score}' where member_id='{gridView1.GetRowCellValue(selected_member, "member_id")?.ToString()}';";
                using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                {
                    try
                    {
                        conn.Open();

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                            main.TLOG("2-2", gridView1.GetRowCellValue(selected_member, "member_id")?.ToString(), "NULL");
                        }
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("update credit Error " + ex.Message, "UPDATE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                check_score();
                simpleButton_set_member_score.Visible = true;
                simpleButton_increase_score.Visible = false;
                simpleButton_decrease_score.Visible = false;
            }
        }
        private void simpleButton_decrease_score_Click(object sender, EventArgs e)//CREDIT DECREASE
        {
            int new_score = int.Parse(label_member_score.Text) - 1;
            if (new_score == -1)
            {
                simpleButton_ban_member_Click(sender, e); simpleButton_set_member_score.Enabled = false;
                simpleButton_increase_score.Enabled = false; simpleButton_decrease_score.Enabled = false;
            } //BAN 
            else
            {
                DialogResult result = XtraMessageBox.Show($"Member score will be decreased", "Decrease Score", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    label_member_score.Text = new_score.ToString();
                    int selected_member = gridView1.FocusedRowHandle;
                    string query = $"UPDATE members SET credit_point = '{new_score}' where member_id='{gridView1.GetRowCellValue(selected_member, "member_id")?.ToString()}';";
                    using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                    {
                        try
                        {
                            conn.Open();

                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.ExecuteNonQuery();
                                main.TLOG("2-3", gridView1.GetRowCellValue(selected_member, "member_id")?.ToString(), "NULL");
                            }
                        }
                        catch (Exception ex)
                        {
                            main.LOG(ex);
                            XtraMessageBox.Show("update credit Error " + ex.Message, "UPDATE ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    check_score();
                    simpleButton_set_member_score.Visible = true;
                    simpleButton_increase_score.Visible = false;
                    simpleButton_decrease_score.Visible = false;
                }
            }
        }
        private void simpleButton_report_member_Click(object sender, EventArgs e)//REPORT
        {
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row
            report reportForm = new report(1, gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[0]).ToString());
            this.Hide();
            reportForm.FormClosed += (s, args) => this.Close(); //enable when closed
            reportForm.ShowDialog();
            //reportForm.simpleButton_view_member_book_Click(null, null);
        }
        private void simpleButton_regenerate_card_Click(object sender, EventArgs e)//REGENERATE
        {
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row
            if (selectedRowHandle >= 0)
            {
                var selected_member_id = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[0]); //select first column
                main.TLOG("2-5", selected_member_id.ToString(), "NULL");
                signup signupForm = new signup();
                signupForm.view_card(selected_member_id.ToString()); //view GENERATED card
                signupForm.FormClosed += (s, args) => this.Enabled = true;
                signupForm.ShowDialog();
            }
        }
        private void simpleButton_history_Click(object sender, EventArgs e)//HISTORY
        {
            int selected_member = gridView1.FocusedRowHandle;
            string member_id = gridView1.GetRowCellValue(selected_member, "member_id")?.ToString();
            history historyForm = new history(1, member_id); //member
            historyForm.FormClosed += (s, args) => this.Enabled = true; //enable when closed
            historyForm.ShowDialog();
        }
        private void simpleButton_member_delete_Click(object sender, EventArgs e) //DELETE ON SIDE-BAR
        {
            simpleButton_delete_member_Click(sender, e);
        }
        private void simpleButton_ban_member_Click(object sender, EventArgs e) //BAN
        {
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row
            var selected_member_borrow_status = gridView1.GetRowCellValue(selectedRowHandle, "borrow_status")?.ToString();
            if (selected_member_borrow_status == "1" || selected_member_borrow_status == "Yes")
            { XtraMessageBox.Show("Member can not banned: Member has borrowed book", "Member Ban Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            else
            {
                DialogResult result = XtraMessageBox.Show($"Member will be banned", "Ban member", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                        {
                            conn.Open();
                            string query = $"UPDATE members SET ban_date='{DateTime.Now.AddDays(15).ToString("yyyy-MM-dd")}', banned='1' WHERE member_id = '{gridView1.GetRowCellValue(selectedRowHandle, "member_id")?.ToString()}'";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.ExecuteNonQuery();
                                query = $"UPDATE MEMBERS SET credit_point=-1 WHERE member_id='{gridView1.GetRowCellValue(selectedRowHandle, "member_id")?.ToString()}'";
                                using (SQLiteCommand cmd2 = new SQLiteCommand(query, conn))
                                { cmd2.ExecuteNonQuery(); }
                                XtraMessageBox.Show($"Member banned untill {DateTime.Now.AddDays(15).ToString("yyyy-MM-dd")}");
                                main.TLOG("2-4", gridView1.GetRowCellValue(selectedRowHandle, "member_id")?.ToString(), "NULL");
                                simpleButton_cancel_Click(sender, e); //TEST
                                simpleButton_clear_member_search_Click(sender, e);
                                textEdit_search_member.Text = gridView1.GetRowCellValue(selectedRowHandle, "member_id")?.ToString();
                                simpleButton_search_member_Click(sender, e);
                                simpleButton_view_member_Click(sender, e);


                                //refresh book on WiseLib
                                WiseLib WlForm = Application.OpenForms["WiseLib"] as WiseLib;
                                if (WlForm.selected_member_no == gridView1.GetRowCellValue(selectedRowHandle, "member_id")?.ToString())
                                { WlForm.simpleButton_cancel_member_Click(sender, e); }
                            }
                            conn.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        main.LOG(ex);
                        XtraMessageBox.Show("member could not banned: " + ex.ToString());
                    }
                }
                else if (result == DialogResult.No) check_score();
            }
        }
        public void simpleButton_cancel_Click(object sender, EventArgs e)//CANCEL
        {
            this.Size = new Size(1400, 830); //default 800x700
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
            navigationFrame1.SelectedPage = navigationPage1;
            loadDataWithPagination(currentPage, pageSize, ""); //load all
            pictureBox_member_img.Image = Image.FromFile(main.cardPath + "cardexample.jpg");
        }
        public void simpleButton_view_overdue_members_Click(object sender, EventArgs e)
        {
            keyboard_panel.Visible = false;
            string query = "SELECT * FROM members WHERE return_date < @currentDate";
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@currentDate", DateTime.Now.ToString("yyyy-MM-dd"));

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            gridControl1.DataSource = dataTable;
                            gridView1.FindFilterText = "";
                            gridView1.PopulateColumns(); //create custom column names
                            gridView1.Columns["member_id"].Caption = "ID";
                            gridView1.Columns["member_name"].Caption = "name";
                            gridView1.Columns["member_surname"].Caption = "surname";
                            gridView1.Columns["borrow_status"].Caption = "borrow status";
                            gridView1.Columns["join_date"].Caption = "join date";
                            gridView1.Columns["credit_point"].Caption = "credit point";
                            gridView1.Columns["banned"].Caption = "ban";
                            gridView1.Columns["ban_date"].Caption = "ban date";
                            string[] hiddenColumns = { "borrowed_book_ISBN", "borrow_date", "return_date" };
                            foreach (var columnName in hiddenColumns) //hide columns
                            {
                                if (gridView1.Columns[columnName] != null)
                                { gridView1.Columns[columnName].Visible = false; }
                            }
                            gridView1.CustomColumnDisplayText += (a, ee) => //0 to no 1 to yes
                            {
                                if (ee.Column.FieldName == "banned" && ee.Value?.ToString() == "0") { ee.DisplayText = "no"; }
                                else if (ee.Column.FieldName == "banned" && ee.Value?.ToString() == "1") { ee.DisplayText = "yes"; }
                                if (ee.Column.FieldName == "borrow_status" && ee.Value?.ToString() == "0") { ee.DisplayText = "no"; }
                                else if (ee.Column.FieldName == "borrow_status" && ee.Value?.ToString() == "1") { ee.DisplayText = "yes"; }
                            };
                            gridView1.RefreshData();
                            gridView1.BestFitColumns();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching overdue members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void simpleButton_view_banned_members_Click(object sender, EventArgs e)
        {
            keyboard_panel.Visible = false;
            string query = "SELECT * FROM members WHERE banned=1";
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            gridControl1.DataSource = dataTable;
                            gridView1.FindFilterText = "";
                            gridView1.PopulateColumns(); //create custom column names
                            gridView1.Columns["member_id"].Caption = "ID";
                            gridView1.Columns["member_name"].Caption = "name";
                            gridView1.Columns["member_surname"].Caption = "surname";
                            gridView1.Columns["borrow_status"].Caption = "borrow status";
                            gridView1.Columns["join_date"].Caption = "join date";
                            gridView1.Columns["credit_point"].Caption = "credit point";
                            gridView1.Columns["banned"].Caption = "ban";
                            gridView1.Columns["ban_date"].Caption = "ban date";
                            string[] hiddenColumns = { "borrowed_book_ISBN", "borrow_date", "return_date" };
                            foreach (var columnName in hiddenColumns) //hide columns
                            {
                                if (gridView1.Columns[columnName] != null)
                                { gridView1.Columns[columnName].Visible = false; }
                            }
                            gridView1.CustomColumnDisplayText += (a, ee) => //0 to no 1 to yes
                            {
                                if (ee.Column.FieldName == "banned" && ee.Value?.ToString() == "0") { ee.DisplayText = "no"; }
                                else if (ee.Column.FieldName == "banned" && ee.Value?.ToString() == "1") { ee.DisplayText = "yes"; }
                                if (ee.Column.FieldName == "borrow_status" && ee.Value?.ToString() == "0") { ee.DisplayText = "no"; }
                                else if (ee.Column.FieldName == "borrow_status" && ee.Value?.ToString() == "1") { ee.DisplayText = "yes"; }
                            };
                            gridView1.RefreshData();
                            gridView1.BestFitColumns();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching banned members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private string memberid = string.Empty;
        private void simpleButton_view_report_Click(object sender, EventArgs e)
        {
            int selectedRowHandle = gridView1.FocusedRowHandle; //selected row
            var firstColumnValue = gridView1.GetRowCellValue(selectedRowHandle, gridView1.Columns[0]);//MEMBER NO
            memberid = firstColumnValue.ToString();
            report reportForm = new report(10, firstColumnValue.ToString()); //report
            reportForm.FormClosed += (se, args) => this.Enabled = true; //enable when closed
            reportForm.ShowDialog();
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitInfo = gridView1.CalcHitInfo(gridControl1.PointToClient(Control.MousePosition));
                if (hitInfo.InRow || hitInfo.InRowCell)
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripMenuItem btnDel = new ToolStripMenuItem("delete"); btnDel.Click += (s, ev) => { simpleButton_delete_member_Click(sender, e); };
                    ToolStripMenuItem btnView = new ToolStripMenuItem("view"); btnView.Click += (s, ev) => { simpleButton_view_member_Click(sender, e); };
                    ToolStripMenuItem btnCcell = new ToolStripMenuItem("copy cell"); btnCcell.Click += (s, ev) => { simpleButton_copy_cell_Click(sender, e); };
                    ToolStripMenuItem btnCrow = new ToolStripMenuItem("copy row"); btnCrow.Click += (s, ev) => { simpleButton_copy_row_Click(sender, e); };

                    menu.Items.Add(btnDel); menu.Items.Add(btnView); menu.Items.Add(btnCrow); menu.Items.Add(btnCcell); menu.Show(Cursor.Position);
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
