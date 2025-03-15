using DevExpress.CodeParser;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Native;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.Templates;
using DevExpress.Pdf.Native.BouncyCastle.Asn1;
using DevExpress.Utils;
using System.Globalization;
namespace WiseLib
{
    public partial class importer : DevExpress.XtraEditors.XtraForm
    {
        public importer(int _book_or_member)
        {
            InitializeComponent();
            int book_or_member = _book_or_member;
            if (book_or_member == 1) simpleButton_import_books_Click(null, null);
            if (book_or_member == 2) simpleButton_import_members_Click(null, null);
        }
        bool import_result = false;
        private void simpleButton_import_books_Click(object sender, EventArgs e)
        {
            label_page_name.Text = "IMPORT BOOKS";

            label16.Text = "X";
            label16.Cursor = Cursors.Hand;
            label16.Click += (s, se) =>
            { comboBoxEdit_category.SelectedIndex = -1; }; //CLEAR 


            navigationFrame1.SelectedPage = navigationPage_import;
        }
        private void simpleButton_import_members_Click(object sender, EventArgs e)
        {
            label_page_name.Text = "IMPORT MEMBERS";

            //set labels
            //member id
            label1.Visible = true;
            comboBoxEdit_memberid.Visible = true;
            label19.Visible = true;
            label2.Text = "NAME";
            label3.Text = "SURNAME";
            label4.Text = "BORROWED";
            label12.Visible = false;
            label5.Text = "B-ISBN";
            toolTipController1.SetToolTip(label5, "BORROWED BOOK ISBN");
            label13.Visible = false;
            label6.Text = "B-DATE";
            toolTipController1.SetToolTip(label6, "BORROW DATE");
            label14.Visible = false;
            label7.Text = "R-DATE";
            toolTipController1.SetToolTip(label7, "RETURN DATE");
            label15.Visible = false;
            label8.Text = "JOIN DATE";

            simpleButton_get_img_online.Visible = false;
            simpleButton_check_books.Text = "Check Members";
            simpleButton_add_books.Text = "Add Members";

            navigationFrame1.SelectedPage = navigationPage_import;

        }
        //------NAVPAGE IMPORT BOOKS-------------------------------------------------------------------------------------------------
        private string selectedFilePath;
        List<string> columnNames = new List<string>();
        private void simpleButton_select_excel_Click(object sender, EventArgs e)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Excel Dosyasını Seçin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName; // Dosya yolunu kaydediyoruz
                    label_excel_name.Text = openFileDialog.FileName;

                    columnNames.Clear();

                    //clear grids
                    gridControl1.DataSource = null;
                    gridView1.Columns.Clear();
                    gridControl1.RefreshDataSource();
                    gridControl2.DataSource = null;
                    gridView2.Columns.Clear();
                    gridControl2.RefreshDataSource();
                    //clear pages
                    comboBoxEdit_excel_pages.Properties.Items.Clear();
                    //clear comboboxes
                    if (label_page_name.Text.Contains("MEMBERS")) comboBoxEdit_memberid.Properties.Items.Clear();
                    comboBoxEdit_isbn.Properties.Items.Clear();
                    comboBoxEdit_title.Properties.Items.Clear();
                    comboBoxEdit_author.Properties.Items.Clear();
                    comboBoxEdit_page.Properties.Items.Clear();
                    comboBoxEdit_publisher.Properties.Items.Clear();
                    comboBoxEdit_publish_date.Properties.Items.Clear();
                    comboBoxEdit_category.Properties.Items.Clear();
                    //clear select
                    if (label_page_name.Text.Contains("MEMBERS")) comboBoxEdit_memberid.SelectedIndex = -1;
                    comboBoxEdit_isbn.SelectedIndex = -1;
                    comboBoxEdit_title.SelectedIndex = -1;
                    comboBoxEdit_author.SelectedIndex = -1;
                    comboBoxEdit_page.SelectedIndex = -1;
                    comboBoxEdit_publisher.SelectedIndex = -1;
                    comboBoxEdit_publish_date.SelectedIndex = -1;
                    comboBoxEdit_category.SelectedIndex = -1;

                    using (var package = new ExcelPackage(new FileInfo(selectedFilePath)))
                    {
                        foreach (var worksheet in package.Workbook.Worksheets) //get pages
                        {
                            comboBoxEdit_excel_pages.Properties.Items.Add(worksheet.Name);
                        }

                        if (package.Workbook.Worksheets.Count > 0) //get pages
                        {
                            var worksheet = package.Workbook.Worksheets.First();
                            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                            {
                                string columnName = worksheet.Cells[1, col].Text;
                                if (!string.IsNullOrEmpty(columnName))
                                {columnNames.Add(columnName); }
                            }
                        }
                    }
                    foreach (var columnName in columnNames) //add columns
                    {
                        if (label_page_name.Text.Contains("MEMBERS")) comboBoxEdit_memberid.Properties.Items.Add(columnName);
                        comboBoxEdit_isbn.Properties.Items.Add(columnName);
                        comboBoxEdit_title.Properties.Items.Add(columnName);
                        comboBoxEdit_author.Properties.Items.Add(columnName);
                        comboBoxEdit_page.Properties.Items.Add(columnName);
                        comboBoxEdit_publisher.Properties.Items.Add(columnName);
                        comboBoxEdit_publish_date.Properties.Items.Add(columnName);
                        comboBoxEdit_category.Properties.Items.Add(columnName);
                    }

                    comboBoxEdit_excel_pages.Enabled = true;
                    panel_select.Enabled = true;
                    comboBoxEdit_excel_pages.SelectedIndex = 0;
                    comboBoxEdit_excel_pages_SelectedIndexChanged(sender, e);
                    StartComboBoxCheck();
                }
            }
        }
        private Timer checkComboBoxesTimer;
        private void StartComboBoxCheck()
        {
            checkComboBoxesTimer = new Timer();
            checkComboBoxesTimer.Interval = 1000; // 1 saniye
            checkComboBoxesTimer.Tick += CheckComboBoxes;
            checkComboBoxesTimer.Start();
        }
        private void CheckComboBoxes(object sender, EventArgs e)
        {
            if (label_page_name.Text.Contains("BOOK"))
            {
                bool comboBox1Filled = comboBoxEdit_isbn.SelectedIndex != -1;
                bool comboBox2Filled = comboBoxEdit_title.SelectedIndex != -1;
                bool comboBox3Filled = comboBoxEdit_page.SelectedIndex != -1;
                bool comboBox4Filled = comboBoxEdit_author.SelectedIndex != -1;
                bool comboBox5Filled = comboBoxEdit_publish_date.SelectedIndex != -1;
                bool comboBox6Filled = comboBoxEdit_publisher.SelectedIndex != -1;
                if (comboBox1Filled && comboBox2Filled && comboBox3Filled && comboBox4Filled && comboBox5Filled && comboBox6Filled)
                {
                    simpleButton_preview.Enabled = true;
                }
                else simpleButton_preview.Enabled = false;
            }
            else //MEBMERS
            {
                bool comboBox1Filled = comboBoxEdit_memberid.SelectedIndex != -1;
                bool comboBox2Filled = comboBoxEdit_isbn.SelectedIndex != -1;
                bool comboBox3Filled = comboBoxEdit_title.SelectedIndex != -1;
                bool comboBox6Filled = comboBoxEdit_category.SelectedIndex != -1;
                if (comboBox1Filled && comboBox2Filled && comboBox3Filled && comboBox6Filled)
                {
                    simpleButton_preview.Enabled = true;
                }
                else simpleButton_preview.Enabled = false;
            }
        }
        private void comboBoxEdit_excel_pages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_excel_pages.SelectedIndex >= 0 && !string.IsNullOrEmpty(selectedFilePath))
            {
                //clear grids
                gridControl1.DataSource = null;
                gridView1.Columns.Clear();
                gridControl1.RefreshDataSource();
                gridControl2.DataSource = null;
                gridView2.Columns.Clear();
                gridControl2.RefreshDataSource();
                //clear comboboxes
                comboBoxEdit_isbn.Properties.Items.Clear();
                comboBoxEdit_title.Properties.Items.Clear();
                comboBoxEdit_author.Properties.Items.Clear();
                comboBoxEdit_page.Properties.Items.Clear();
                comboBoxEdit_publisher.Properties.Items.Clear();
                comboBoxEdit_publish_date.Properties.Items.Clear();
                comboBoxEdit_category.Properties.Items.Clear();
                //clear select
                comboBoxEdit_isbn.SelectedIndex = -1;
                comboBoxEdit_title.SelectedIndex = -1;
                comboBoxEdit_author.SelectedIndex = -1;
                comboBoxEdit_page.SelectedIndex = -1;
                comboBoxEdit_publisher.SelectedIndex = -1;
                comboBoxEdit_publish_date.SelectedIndex = -1;
                comboBoxEdit_category.SelectedIndex = -1;

                string selectedSheetName = comboBoxEdit_excel_pages.SelectedItem.ToString();

                using (var package = new ExcelPackage(new FileInfo(selectedFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[selectedSheetName];
                    DataTable dt = new DataTable();

                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        string columnName = worksheet.Cells[1, col].Value?.ToString()?.Trim() ?? $"Column{col}";

                        comboBoxEdit_isbn.Properties.Items.Add(columnName);//
                        comboBoxEdit_title.Properties.Items.Add(columnName);
                        comboBoxEdit_author.Properties.Items.Add(columnName);
                        comboBoxEdit_page.Properties.Items.Add(columnName);
                        comboBoxEdit_publisher.Properties.Items.Add(columnName);
                        comboBoxEdit_publish_date.Properties.Items.Add(columnName);
                        comboBoxEdit_category.Properties.Items.Add(columnName);

                        dt.Columns.Add(columnName);
                    }

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        DataRow dataRow = dt.NewRow();

                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value;
                            string columnName = dt.Columns[col - 1].ColumnName.ToUpper();

                            if (columnName.Contains("DATE")) // Sütun adı DATE içeriyorsa
                            {
                                if (cellValue is double numericValue) // Excel tarih formatı (OADate)
                                    dataRow[col - 1] = DateTime.FromOADate(numericValue).ToString("dd.MM.yyyy");
                                else if (DateTime.TryParse(cellValue?.ToString(), out DateTime parsedDate)) // String olarak geldiyse
                                    dataRow[col - 1] = parsedDate.ToString("dd.MM.yyyy");
                                else
                                    dataRow[col - 1] = cellValue; // Tarih değilse aynen ekle
                            }
                            else
                            {
                                dataRow[col - 1] = cellValue ?? DBNull.Value;
                            }
                        }

                        dt.Rows.Add(dataRow);
                    }
                    gridControl1.DataSource = dt;
                }
            }
        }
        private void simpleButton_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void simpleButton_preview_Click(object sender, EventArgs e)
        {
            if (label_page_name.Text.Contains("BOOK")) //BOOKS
            {
                gridControl2.DataSource = null;
                gridView2.Columns.Clear();
                gridControl2.RefreshDataSource();
                string selectedColumn1 = comboBoxEdit_isbn.SelectedItem.ToString().Trim();
                string selectedColumn2 = comboBoxEdit_title.SelectedItem.ToString().Trim();
                string selectedColumn3 = comboBoxEdit_author.SelectedItem.ToString().Trim();
                string selectedColumn4 = comboBoxEdit_page.SelectedItem.ToString().Trim();
                string selectedColumn5 = comboBoxEdit_publisher.SelectedItem.ToString().Trim();
                string selectedColumn6 = comboBoxEdit_publish_date.SelectedItem.ToString().Trim();

                string selectedColumn7 = string.Empty;
                if (comboBoxEdit_category.SelectedIndex != -1)
                { selectedColumn7 = comboBoxEdit_category.SelectedItem.ToString(); }

                DataTable sourceTable = (DataTable)gridControl1.DataSource;
                DataTable newTable = new DataTable();

                newTable.Columns.Add("ISBN", typeof(string));
                newTable.Columns.Add("TITLE", typeof(string));
                newTable.Columns.Add("AUTHOR", typeof(string));
                newTable.Columns.Add("PAGE", typeof(string));
                newTable.Columns.Add("PUBLISHER", typeof(string));
                newTable.Columns.Add("PUBLISH-DATE", typeof(string));

                if (comboBoxEdit_category.SelectedIndex != -1) newTable.Columns.Add("CATEGORY", typeof(string));

                newTable.Columns.Add("STATE", typeof(string));

                foreach (DataRow row in sourceTable.Rows)
                {
                    DataRow newRow = newTable.NewRow();
                    newRow["ISBN"] = row[selectedColumn1];
                    newRow["TITLE"] = row[selectedColumn2];
                    newRow["AUTHOR"] = row[selectedColumn3];
                    newRow["PAGE"] = row[selectedColumn4];
                    newRow["PUBLISHER"] = row[selectedColumn5];
                    newRow["PUBLISH-DATE"] = row[selectedColumn6];
                    if (comboBoxEdit_category.SelectedIndex != -1) newRow["CATEGORY"] = row[selectedColumn7];

                    //newRow[selectedColumn0] = row[selectedColumn0];
                    newTable.Rows.Add(newRow);
                }
                gridControl2.DataSource = newTable;
                gridView2.Columns["ISBN"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                panel_interact.Enabled = true;
            }
            else //MEMBERS
            {
                gridControl2.DataSource = null;
                gridView2.Columns.Clear();
                gridControl2.RefreshDataSource();
                string selectedColumn0 = comboBoxEdit_memberid.SelectedItem.ToString().Trim();
                string selectedColumn1 = comboBoxEdit_isbn.SelectedItem.ToString().Trim();
                string selectedColumn2 = comboBoxEdit_title.SelectedItem.ToString().Trim();
                string selectedColumn3 = string.Empty; string selectedColumn4 = string.Empty; string selectedColumn5 = string.Empty; string selectedColumn6 = string.Empty;
                try
                {
                    selectedColumn3 = comboBoxEdit_author.SelectedItem.ToString().Trim();
                    selectedColumn4 = comboBoxEdit_page.SelectedItem.ToString().Trim();
                    selectedColumn5 = comboBoxEdit_publisher.SelectedItem.ToString().Trim();
                    selectedColumn6 = comboBoxEdit_publish_date.SelectedItem.ToString().Trim();

                }
                catch { } //skip

                string selectedColumn7 = comboBoxEdit_category.SelectedItem.ToString();

                DataTable sourceTable = (DataTable)gridControl1.DataSource;
                DataTable newTable = new DataTable();

                newTable.Columns.Add("ID", typeof(string));
                newTable.Columns.Add("NAME", typeof(string));
                newTable.Columns.Add("SURNAME", typeof(string));
                newTable.Columns.Add("BORROWED", typeof(string));
                newTable.Columns.Add("BORROWED ISBN", typeof(string));
                newTable.Columns.Add("BORROW DATE", typeof(string));
                newTable.Columns.Add("RETURN DATE", typeof(string));
                newTable.Columns.Add("JOIN DATE", typeof(string));

                newTable.Columns.Add("STATE", typeof(string));

                foreach (DataRow row in sourceTable.Rows)
                {
                    DataRow newRow = newTable.NewRow();
                    newRow["ID"] = row[selectedColumn0];
                    newRow["NAME"] = row[selectedColumn1];
                    newRow["SURNAME"] = row[selectedColumn2];
                    try
                    {
                        newRow["BORROWED"] = row[selectedColumn3];
                        newRow["BORROWED ISBN"] = row[selectedColumn4];
                        newRow["BORROW DATE"] = row[selectedColumn5];
                        newRow["RETURN DATE"] = row[selectedColumn6];
                    }
                    catch { }

                    newRow["JOIN DATE"] = row[selectedColumn7];

                    //newRow[selectedColumn0] = row[selectedColumn0];
                    newTable.Rows.Add(newRow);
                }
                gridControl2.DataSource = newTable;
                gridView2.Columns["ID"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                panel_interact.Enabled = true;
                simpleButton_get_img_online.Visible = false;
            }
        }
        bool already_exist = false;
        private void simpleButton_check_books_Click(object sender, EventArgs e)
        {
            if (label_page_name.Text.Contains("BOOKS"))
            {
                if (gridControl2.DataSource is DataTable dataTable)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string isbn = row["ISBN"].ToString();

                        if (!string.IsNullOrEmpty(isbn)) //NO EMPTY ISBN
                        {
                            if (CheckIsbnInDatabase(isbn))
                            {
                                row["STATE"] = "Already Exist";
                                already_exist = true;
                            }
                            else { row["STATE"] = "Not Found"; }
                        }
                        else row["STATE"] = "NO ISBN";
                        gridControl2.RefreshDataSource();
                    }
                }
            }
            else
            {
                if (gridControl2.DataSource is DataTable dataTable2)
                {
                    foreach (DataRow row in dataTable2.Rows)
                    {
                        string id = row["ID"].ToString();

                        if (!string.IsNullOrEmpty(id)) //NO EMPTY ID
                        {
                            if (check_member_in_db(id))
                            {
                                row["STATE"] = "Already Exist";
                                already_exist = true;
                            }
                            else { row["STATE"] = "Not Found"; }
                        }
                        else row["STATE"] = "NO ID";
                    }
                    gridControl2.RefreshDataSource();
                }
            }
        }
        private bool CheckIsbnInDatabase(string isbn)
        {
            // Veritabanı bağlantısını ayarla
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                conn.Open();
                string query = $"SELECT COUNT(*) FROM books WHERE ISBN = @ISBN";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ISBN", isbn);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        private bool check_member_in_db(string id)
        {
            // Veritabanı bağlantısını ayarla
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                conn.Open();
                string query = $"SELECT COUNT(*) FROM members WHERE member_id= @ID";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        private void simpleButton_add_books_Click(object sender, EventArgs e)
        {
            int import_count = 0;
            if (label_page_name.Text.Contains("BOOK"))//BOOKS
            {
                if (already_exist)
                {
                    var result = XtraMessageBox.Show("Update existing books?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                        {
                            try
                            {
                                conn.Open();
                                for (int i = 0; i < gridView2.RowCount; i++)
                                {
                                    string state = gridView2.GetRowCellValue(i, "STATE")?.ToString();
                                    if (state == "Already Exist" || state == "ALREADY ADDED")
                                    {
                                        string isbn = gridView2.GetRowCellValue(i, "ISBN")?.ToString();
                                        string title = gridView2.GetRowCellValue(i, "TITLE")?.ToString();
                                        string author = gridView2.GetRowCellValue(i, "AUTHOR")?.ToString();
                                        string page = gridView2.GetRowCellValue(i, "PAGE")?.ToString();
                                        string publisher = gridView2.GetRowCellValue(i, "PUBLISHER")?.ToString();
                                        string publishDate = gridView2.GetRowCellValue(i, "PUBLISH-DATE")?.ToString();

                                        string category = "unknown";
                                        if (comboBoxEdit_category.SelectedIndex != -1)
                                        { category = gridView2.GetRowCellValue(i, "CATEGORY")?.ToString(); }

                                        string query = @"UPDATE books 
                                    SET title = @title, 
                                        author = @author, 
                                        page_count = @page, 
                                        publisher = @publisher, 
                                        year_of_publication = @publishDate, 
                                        category = @category 
                                    WHERE isbn = @isbn";
                                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                        {
                                            cmd.Parameters.AddWithValue("@title", title);
                                            cmd.Parameters.AddWithValue("@author", author);
                                            cmd.Parameters.AddWithValue("@page", page);
                                            cmd.Parameters.AddWithValue("@publisher", publisher);
                                            cmd.Parameters.AddWithValue("@publishDate", publishDate);
                                            cmd.Parameters.AddWithValue("@category", category);
                                            cmd.Parameters.AddWithValue("@isbn", isbn);

                                            int res = cmd.ExecuteNonQuery();
                                            if (res > 0) { gridView2.SetRowCellValue(i, "STATE", "UPDATED"); main.TLOG("3-7", "", ""); import_result = true; import_count++; }
                                            else gridView2.SetRowCellValue(i, "STATE", "NOT UPDATED");
                                        }
                                    }
                                }
                                XtraMessageBox.Show($"total imported {import_count}"); 
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                    {
                        conn.Open();

                        string insertQuery = "INSERT INTO books (ISBN, title, author, page_count, year_of_publication, publisher, category," +
                            "taken, missing, has_problem, date_added) VALUES (@ISBN, @Title, @Author, @Page_count, @Year_of_publication, " +
                            $"@Publisher, @Category, 0, 0, 0, '{DateTime.Now.ToString("yyyy-MM-dd")}');";
                        using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                        {
                            DataTable dataTable = (DataTable)gridControl2.DataSource;
                            foreach (DataRow row in dataTable.Rows)
                            {
                                string state = row["STATE"].ToString();
                                if (state == "Already Exist" || state == "UPDATED")
                                { continue; }
                                else
                                {
                                    string isbn = row["ISBN"]?.ToString().Trim();
                                    string title = row["TITLE"]?.ToString().Trim();
                                    string author = row["AUTHOR"]?.ToString().Trim();
                                    string page = row["PAGE"]?.ToString().Trim();
                                    string date = row["PUBLISH-DATE"]?.ToString().Trim();
                                    string publisher = row["PUBLISHER"]?.ToString().Trim();
                                    string category = "unknown";
                                    if (comboBoxEdit_category.SelectedIndex != -1)
                                    { category = row["CATEGORY"]?.ToString().Trim(); }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(isbn) && !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(author)
                                            && !string.IsNullOrEmpty(page) && !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(publisher))
                                        {
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.AddWithValue("@ISBN", isbn);
                                            cmd.Parameters.AddWithValue("@Title", title);
                                            cmd.Parameters.AddWithValue("@Author", author);
                                            cmd.Parameters.AddWithValue("@Page_count", page);
                                            cmd.Parameters.AddWithValue("@Year_of_publication", date);
                                            cmd.Parameters.AddWithValue("@Publisher", publisher);

                                            cmd.Parameters.AddWithValue("@Category", category);

                                            int res = cmd.ExecuteNonQuery();
                                            if (res != 0) { row["STATE"] = "ADDED"; main.TLOG("3-7","",""); import_result = true; }
                                            else row["STATE"] = "NOT ADDED";
                                        }
                                        else row["STATE"] = "INVALID DATA";
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is System.Data.SQLite.SQLiteException) row["STATE"] = "ALREADY ADDED"; //TODO may not be accurate
                                        else row["STATE"] = "ERROR";
                                        main.LOG(ex + " \nQUERY: " + insertQuery);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else //MEMBERS
            {
                if (already_exist)
                {
                    var result = XtraMessageBox.Show("Update existing members?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                        {
                            try
                            {
                                conn.Open();
                                for (int i = 0; i < gridView2.RowCount; i++)
                                {
                                    string state = gridView2.GetRowCellValue(i, "STATE")?.ToString();
                                    if (state == "Already Exist" || state == "ALREADY ADDED")
                                    {
                                        string id = gridView2.GetRowCellValue(i, "ID")?.ToString();
                                        string name = gridView2.GetRowCellValue(i, "NAME")?.ToString();
                                        string surname = gridView2.GetRowCellValue(i, "SURNAME")?.ToString();
                                        string borrowed = gridView2.GetRowCellValue(i, "BORROWED")?.ToString();
                                        string b_isbn = gridView2.GetRowCellValue(i, "BORROWED ISBN")?.ToString();
                                        string b_date = gridView2.GetRowCellValue(i, "BORROW DATE")?.ToString();
                                        string return_date = gridView2.GetRowCellValue(i, "RETURN DATE")?.ToString();
                                        string join_date = gridView2.GetRowCellValue(i, "JOIN DATE")?.ToString();

                                        string query = @"UPDATE members 
                                    SET 
                                        ID=@id,
                                        member_name = @name, 
                                        member_surname = @s_name, 
                                        borrow_status = @b_stat, 
                                        borrowed_book_ISBN = @b_book_isbn, 
                                        borrow_date = @b_date, 
                                        return_date = @r_date,
                                        join_date = @j_date
                                    WHERE id = @id";
                                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                        {
                                            cmd.Parameters.AddWithValue("@id", id);
                                            cmd.Parameters.AddWithValue("@name", name);
                                            cmd.Parameters.AddWithValue("@s_name", surname);
                                            cmd.Parameters.AddWithValue("@b_stat", borrowed);
                                            cmd.Parameters.AddWithValue("@b_book_isbn", b_isbn);
                                            cmd.Parameters.AddWithValue("@b_date", b_date);
                                            cmd.Parameters.AddWithValue("@r_date", return_date);
                                            cmd.Parameters.AddWithValue("@join_date", join_date);

                                            int res = cmd.ExecuteNonQuery();
                                            if (res > 0) { gridView2.SetRowCellValue(i, "STATE", "UPDATED"); main.TLOG("2-6","", ""); import_result = true; }
                                            else gridView2.SetRowCellValue(i, "STATE", "NOT UPDATED");

                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
                    {
                        conn.Open();

                        string insertQuery = "INSERT INTO members (member_id, member_name, member_surname, borrow_status, borrowed_book_ISBN," +
                            "borrow_date, return_date, join_date, credit_point, banned) VALUES (@id, @name, @s_name, @b_stat, @b_book_isbn, " +
                            $"@b_date, @r_date, @j_date, 3, 0);";
                        using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                        {
                            DataTable dataTable = (DataTable)gridControl2.DataSource;
                            foreach (DataRow row in dataTable.Rows)
                            {
                                string state = row["STATE"].ToString();
                                if (state == "Already Exist" || state == "UPDATED")
                                { continue; }
                                else
                                {
                                    string id = row["ID"]?.ToString().Trim();
                                    string name = row["NAME"]?.ToString().Trim();
                                    string surname = row["SURNAME"]?.ToString().Trim();
                                    string borrowed = row["BORROWED"]?.ToString().Trim();
                                    string b_isbn = row["BORROWED ISBN"]?.ToString().Trim();
                                    string b_date = row["BORROW DATE"]?.ToString().Trim();
                                    string return_date = row["RETURN DATE"]?.ToString().Trim();
                                    string join_date = row["JOIN DATE"]?.ToString().Trim();

                                    if (string.IsNullOrEmpty(borrowed) || borrowed == "0") {borrowed = "0";}

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(join_date))
                                        {
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.AddWithValue("@id", id);
                                            cmd.Parameters.AddWithValue("@name", name);
                                            cmd.Parameters.AddWithValue("@s_name", surname);
                                            cmd.Parameters.AddWithValue("@b_stat", borrowed);
                                            cmd.Parameters.AddWithValue("@b_book_isbn", string.IsNullOrWhiteSpace(b_isbn) ? (object)DBNull.Value : b_isbn);
                                            cmd.Parameters.AddWithValue("@b_date", string.IsNullOrWhiteSpace(b_date) ? (object)DBNull.Value : b_date);
                                            cmd.Parameters.AddWithValue("@r_date", string.IsNullOrWhiteSpace(return_date) ? (object)DBNull.Value : return_date);
                                            cmd.Parameters.AddWithValue("@j_date", join_date);

                                            int res = cmd.ExecuteNonQuery();
                                            if (res != 0) { row["STATE"] = "ADDED"; main.TLOG("2-6", "", ""); import_result = true; }
                                            else row["STATE"] = "NOT ADDED";
                                        }
                                        else row["STATE"] = "INVALID DATA";
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is System.Data.SQLite.SQLiteException) row["STATE"] = "ALREADY ADDED"; //TODO may not be accurate
                                        else row["STATE"] = "ERROR";
                                        main.LOG(ex + " \nQUERY: " + insertQuery);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private async void simpleButton_get_img_online_Click(object sender, EventArgs e)
        {
            var result = XtraMessageBox.Show("Added images may be inaccurate. Sure to continue?", "Add Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    string isbn = gridView2.GetRowCellValue(i, "ISBN").ToString();
                    bool success = await get_book_image_by_isbn(isbn);
                    gridView2.SetRowCellValue(i, "STATE", success ? "IMG ADDED" : "IMG NOT ADDED");
                }
            }
        }
        private async Task<bool> get_book_image_by_isbn(string isbn)
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
                            string image_s = book["cover"]?["small"]?.ToString() ?? "NULL";
                            string image_m = book["cover"]?["medium"]?.ToString() ?? "NULL";
                            string image_l = book["cover"]?["large"]?.ToString() ?? "NULL";

                            bool result = update_book_imgs(isbn, image_s, image_m, image_l);
                            return result;
                        }
                    }
                    Console.WriteLine($"Book with ISBN {isbn} could not be found.");
                }
                catch (Exception ex)
                {
                    main.LOG(ex);
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return false;
        }
        private bool update_book_imgs(string isbn, string img_s, string img_m, string img_l)
        {
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE books SET image_s = @img_s, image_m = @img_m, image_l = @img_l WHERE isbn = @isbn";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@img_s", img_s);
                        cmd.Parameters.AddWithValue("@img_m", img_m);
                        cmd.Parameters.AddWithValue("@img_l", img_l);
                        cmd.Parameters.AddWithValue("@isbn", isbn);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }
        private void importer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (import_result)
            {
                var WiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                var BooksForm = Application.OpenForms.OfType<books>().FirstOrDefault();
                if (BooksForm != null) { BooksForm.Close(); WiseLibForm.simpleButton_books_Click(null, null); }
            }
        }
    }
}