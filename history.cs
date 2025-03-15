using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace WiseLib
{
    public partial class history : DevExpress.XtraEditors.XtraForm
    {
        public history(int _get_type, string id)
        {
            InitializeComponent();
            get_type = _get_type;
            member_id = id;
            book_isbn = id;
        }
        int get_type; //1=member 2=book
        string member_id; //1
        string book_isbn; //2
        private async void history_Load(object sender, EventArgs e)
        {
            if (get_type == 1) //member
            {
                this.Text = "Member Log";
                progressBar1.Visible = true; progressBar1.Style = ProgressBarStyle.Marquee; //load animation
                await Task.Run(() => load_member_history(member_id)); //dynamic panel loads
                progressBar1.Visible = false;}
            else if (get_type == 2) //book
            {
                this.Text = "Book Log";
                progressBar1.Visible = true; progressBar1.Style = ProgressBarStyle.Marquee; //load animation
                await Task.Run(() => load_book_history(book_isbn)); //dynamic panel loads
                progressBar1.Visible = false;}
            else if (get_type == 3)
            {
                this.Text = "Select Database";
                progressBar1.Visible = true; progressBar1.Style = ProgressBarStyle.Marquee; //load animation
                await Task.Run(() => load_database_history()); //dynamic panel loads
                progressBar1.Visible = false;}
            else if (get_type == 4) //log
            {
                this.Text = "Transactions Log";
                progressBar1.Visible = true; progressBar1.Style = ProgressBarStyle.Marquee; //load animation
                await Task.Run(() => load_log_transactions()); //dynamic panel loads
                progressBar1.Visible = false;}}
        private void load_member_history(string memberId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                conn.Open();
                string query = $"SELECT ISBN, borrow_date, return_date, transaction_type FROM book_transactions WHERE member_id = '{memberId}' ORDER BY id ASC;";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        (int yPosition, int xPosition, int panelCount) = (10,10,0);
                        while (reader.Read())
                        {
                            string isbn = reader["ISBN"].ToString();
                            string transactionType = reader["transaction_type"].ToString() == "1" ? "Borrow" : "Return";
                            string date = transactionType == "Borrow" ? reader["borrow_date"].ToString() : reader["return_date"].ToString();
                            string return_date = reader["return_date"].ToString();
                            //get book info
                            string title = "", author = "";
                            using (SQLiteConnection bookConn = new SQLiteConnection(main.connectionString))
                            {
                                bookConn.Open();
                                string bookQuery = $"SELECT title, author FROM books WHERE ISBN = '{isbn}' ORDER BY id";
                                using (SQLiteCommand bookCmd = new SQLiteCommand(bookQuery, bookConn))
                                {using (SQLiteDataReader bookReader = bookCmd.ExecuteReader())
                                 {if (bookReader.Read()) {title = bookReader["title"].ToString(); author = bookReader["author"].ToString();}}}}
                            add_member_dynamic_panel(title, author, date, transactionType, ref xPosition, ref yPosition, ref panelCount, return_date);}}}}}
        string actual_borrow_date = string.Empty;
        private void add_member_dynamic_panel(string title, string author, string date, string transactionType, ref int xPosition, ref int yPosition, ref int panelCount, string _return_date)
        {
            Panel panel = new Panel
            {Size = new Size(300, 100), Location = new Point(xPosition, yPosition), BorderStyle = BorderStyle.FixedSingle};
            using (GraphicsPath path = new GraphicsPath())
            {   int radius = 20;
                path.AddArc(0, 0, radius, radius, 180, 90); //left top
                path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90); //right top
                path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90); //right bot
                path.AddArc(0, panel.Height - radius, radius, radius, 90, 90); //left bot
                path.CloseFigure();
                panel.Region = new Region(path); //border radius}
                Label lblTransaction = new Label
                { Text = $"{transactionType}", Location = new Point(10, 70), AutoSize = true };
                ToolTip toolTip = new ToolTip();
                Label lblTitle = new Label
                { Text = $"Title: {title}", Location = new Point(10, 10), AutoSize = true, };
                toolTip.SetToolTip(lblTitle, lblTitle.Text); //tooltip
                Label lblAuthor = new Label
                { Text = $"Author: {author}", Location = new Point(10, 30), AutoSize = true };
                toolTip.SetToolTip(lblAuthor, lblAuthor.Text); //tooltip
                Label lblDate = new Label
                { Location = new Point(10, 50), AutoSize = true };
                if (transactionType == "Borrow")
                { panel.BackColor = Color.BlueViolet; lblDate.Text = $"{date} --- {_return_date}"; actual_borrow_date = date; }
                else //returned
                {   panel.BackColor = Color.Green; lblDate.Text = _return_date;
                    DateTime return_date = DateTime.Parse(date);
                    DateTime actual_borrow_date2 = DateTime.Parse(actual_borrow_date);
                    int daysDifference = (return_date.Date - actual_borrow_date2.Date).Days;
                    if (daysDifference > 15) lblTransaction.Text = $"Returned in {daysDifference} days   {daysDifference - 15} days penalty";
                    else lblTransaction.Text = $"Returned in {daysDifference} days";}

                panel.Controls.Add(lblTransaction); panel.Controls.Add(lblTitle); panel.Controls.Add(lblAuthor); panel.Controls.Add(lblDate); panelCount++;

                if (panelCount % 2 == 0){ xPosition = 10; yPosition += 110; }//2 placed ,new line
                else { xPosition = 320; }
                if (panel_left.InvokeRequired) panel_left.Invoke(new Action(() => panel_left.Controls.Add(panel)));
                else panel_left.Controls.Add(panel);}}
        private void load_book_history(string isbn)
        {
            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {   conn.Open();
                string query = $"SELECT member_id, borrow_date, return_date, transaction_type FROM book_transactions WHERE ISBN = '{isbn}' ORDER BY id ASC";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        (int yPosition,int xPosition,int panelCount) = (10,10,0);

                        while (reader.Read())
                        {
                            string memberId = reader["member_id"].ToString();
                            string transactionType = reader["transaction_type"].ToString() == "1" ? "Borrow" : "Return";
                            string date = transactionType == "Borrow" ? reader["borrow_date"].ToString() : reader["return_date"].ToString();
                            string return_date = reader["return_date"].ToString();

                            string memberName = ""; //get member info
                            using (SQLiteConnection memberConn = new SQLiteConnection(main.connectionString))
                            {   memberConn.Open();
                                string memberQuery = $"SELECT member_name,member_surname FROM members WHERE member_id = '{memberId}'";
                                using (SQLiteCommand memberCmd = new SQLiteCommand(memberQuery, memberConn))
                                {   using (SQLiteDataReader memberReader = memberCmd.ExecuteReader())
                                    {   if (memberReader.Read())
                                        { memberName = memberReader["member_name"].ToString() + " " + memberReader["member_surname"].ToString(); }}}}
                            add_book_dynamic_panel(memberName, date, transactionType, ref xPosition, ref yPosition, ref panelCount, return_date);}}}}}
        private void add_book_dynamic_panel(string memberName, string date, string transactionType, ref int xPosition, ref int yPosition, ref int panelCount, string _return_date)
        {   Panel panel = new Panel
            {Size = new Size(300, 100), Location = new Point(xPosition, yPosition), BorderStyle = BorderStyle.FixedSingle};

            Label lblTransaction = new Label
            {Text = $"{transactionType}",Location = new Point(10, 70),AutoSize = true};
            Label lblMember = new Label
            {Text = $"Member: {memberName}",Location = new Point(10, 10),AutoSize = true};
            Label lblDate = new Label
            {Location = new Point(10, 50),AutoSize = true};

            if (transactionType == "Borrow")
            {   panel.BackColor = Color.BlueViolet;
                lblDate.Text = $"{date} --- {_return_date}";
                actual_borrow_date = date;}
            else // Returned
            {   panel.BackColor = Color.Green; lblDate.Text = _return_date; lblTransaction.Text = "Returned";
                DateTime return_date = DateTime.Parse(date); DateTime actual_borrow_date2 = DateTime.Parse(actual_borrow_date);
                int daysDifference = (return_date.Date - actual_borrow_date2.Date).Days;
                if (daysDifference > 15) { lblTransaction.Text = $"Returned in {daysDifference} days   {daysDifference - 15} days penalty"; }
                else { lblTransaction.Text = $"Returned in {daysDifference} days"; }}
            
            panel.Controls.Add(lblTransaction); panel.Controls.Add(lblMember);panel.Controls.Add(lblDate);panelCount++;

            if (panelCount % 2 == 0) { xPosition = 10; yPosition += 110; } //2 placed, next line
            else { xPosition = 320; }
            if (panel_left.InvokeRequired) { panel_left.Invoke(new Action(() => panel_left.Controls.Add(panel))); }
            else { panel_left.Controls.Add(panel); }}
        private void load_database_history()
        {
            if (Directory.Exists(main.backupPath))
            {   string[] files = Directory.GetFiles(main.backupPath);
                (int xPosition,int yPosition ,int panelCount )= (10,10,0);

                foreach (string file in files)
                {   string fileName = Path.GetFileName(file); add_database_dynamic_panel(fileName, ref xPosition, ref yPosition, ref panelCount);}}
            else
            {MessageBox.Show("Backups folder not found.");}}
        private void add_database_dynamic_panel(string fileName, ref int xPosition, ref int yPosition, ref int panelCount)
        {
            Panel panel = new Panel
            {
                Size = new Size(300, 50),
                Location = new Point(xPosition, yPosition),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(48, 48, 48)
            };

            Label lblFileName = new Label
            {
                Text = fileName,
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(lblFileName);
            panelCount++;

            panel.Click += (s, e) =>
            {
                var result = XtraMessageBox.Show($"Are you sure to select\nDatabase {fileName}?\nThis will override current Database!", "Select Database", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string sourceFile = Path.Combine(main.backupPath, fileName);
                        string targetDir = main.serverFolderPath;
                        string targetFile = Path.Combine(targetDir, fileName);

                        File.Delete(main.serverPath); //delete
                        File.Move(sourceFile, main.serverPath); //rename and move

                        XtraMessageBox.Show("Database Succesfully selected. Restarting..", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Restart();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                        main.LOG(ex);
                    }
                }
            };

            if (panelCount % 2 == 0)
            {
                xPosition = 10;
                yPosition += 60;
            }
            else
            {
                xPosition = 320;
            }

            if (panel_left.InvokeRequired)
                panel_left.Invoke(new Action(() => panel_left.Controls.Add(panel)));
            else
                panel_left.Controls.Add(panel);
        }
        private void load_log_transactions()
        {

            using (SQLiteConnection conn = new SQLiteConnection(main.connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM log_transactions ORDER BY transaction_id DESC";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    int xPosition = 10;
                    int yPosition = 10;
                    int panelCount = 0;

                    while (reader.Read())
                    {
                        string transactionId = reader["transaction_id"].ToString();
                        string transactionNo = reader["transaction_no"].ToString();
                        string real_transactionNo = reader["transaction_no"].ToString();
                        string transactionDate = reader["transaction_date"].ToString();
                        string memberId = reader["member_id"].ToString() == "NULL" ? string.Empty : reader["member_id"].ToString();
                        string bookIsbn = reader["book_isbn"].ToString() == "NULL" ? string.Empty : reader["book_isbn"].ToString();

                        transactionNo = GetTransactionDescription(transactionNo);

                        add_log_dynamic_panel(transactionId, real_transactionNo, transactionNo, transactionDate, memberId, bookIsbn, ref yPosition, ref panelCount, ref xPosition);
                    }
                }
            }
        }
        private string GetTransactionDescription(string transactionNo)
        {
            switch (transactionNo)
            {
                case "0":
                case "0-1": return "Setting Connection Confirmed";
                case "0-2": return "New Database Created";
                case "0-3": return "Database Backup";
                case "0-4": return "Database Selected";
                case "1": return "WiseLib Launch";
                case "2": return "Member Added";
                case "2-1": return "Member Deleted";
                case "2-2": return "Member Credit Point Increased";
                case "2-3": return "Member Credit Point Decreased";
                case "2-4": return "Member Banned";
                case "2-5": return "Member Card Regenerated";
                case "2-6": return "Members Imported";
                case "3": return "Book Added";
                case "3-1": return "Book info online";
                case "3-2": return "Book Deleted";
                case "3-3": return "Book Edited";
                case "3-4": return "Book Set as MISSING";
                case "3-5": return "Book Set as NOT MISSING";
                case "3-6": return "Book Reported";
                case "4-2": return "Member Report Created";
                case "4-3": return "Book Report Created";
                case "4-4": return "Report Set as SOLVED";
                case "4-5": return "Report Set as NOT SOLVED";
                case "5-0": return "Book Lended";
                default: return "Unknown Transaction";
            }
        }
        private void add_log_dynamic_panel(string transactionId, string real_transactionNo, string transactionNo, string transactionDate, string memberId, string bookIsbn, ref int yPosition, ref int panelCount, ref int xPosition)
        {
            Panel panel = new Panel
            {
                Size = new Size(300, 100),
                Location = new Point(xPosition, yPosition),
                BorderStyle = BorderStyle.FixedSingle
            };

            if (real_transactionNo.StartsWith("0")) panel.BackColor = Color.Red;
            else if (real_transactionNo.StartsWith("1")) panel.BackColor = Color.LawnGreen;
            else if (real_transactionNo.StartsWith("2")) panel.BackColor = Color.Green;
            else if (real_transactionNo.StartsWith("3")) panel.BackColor = Color.Blue;
            else if (real_transactionNo.StartsWith("4")) panel.BackColor = Color.Orange;
            else if (real_transactionNo.StartsWith("5")) panel.BackColor = Color.LightSeaGreen;

            ToolTip toolTip = new ToolTip();

            void AddLabelWithTooltip(string text, Point location)
            {
                Label label = new Label
                {
                    Text = text,
                    Location = location,
                    AutoSize = true,
                    ForeColor = Color.Black
                };
                toolTip.SetToolTip(label, text);
                panel.Controls.Add(label);
            }

            AddLabelWithTooltip($"ID: {transactionId}", new Point(5, 10));
            AddLabelWithTooltip($"Transaction: {transactionNo}", new Point(5, 30));
            AddLabelWithTooltip($"Date: {transactionDate}", new Point(5, 50));

            if (!string.IsNullOrEmpty(memberId))
                AddLabelWithTooltip($"Member ID: {memberId}", new Point(5, 70));

            if (!string.IsNullOrEmpty(bookIsbn))
                AddLabelWithTooltip($"Book ISBN: {bookIsbn}", new Point(5, 73));

            panelCount++;

            panel.Click += (s, e) => MessageBox.Show($"Transaction ID: {transactionId}\nTransaction: {transactionNo}");

            if (panelCount % 2 == 0)
            {
                xPosition = 10;
                yPosition += 110;
            }
            else
            {
                xPosition = 320;
            }

            if (panel_left.InvokeRequired) panel_left.Invoke(new Action(() => panel_left.Controls.Add(panel)));
            else panel_left.Controls.Add(panel);
        }
    }
}
