using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.CodeParser;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit.Model;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static DevExpress.XtraEditors.Mask.MaskSettings;
namespace WiseLib
{
    public static class main
    {
        public static void LOG(object message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(appPath + "log.log", true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message?.ToString() ?? "null"}");
                }
            }
            catch (Exception)
            { XtraMessageBox.Show("Error with LOGING"); }
        }
        public static void TLOG(string tno,string member_id,string isbn)
        {
            if (string.IsNullOrEmpty(member_id)) member_id = "NULL";
            if (string.IsNullOrEmpty(isbn)) member_id = "NULL";
            try
            {
                using (var connection = new SQLiteConnection(main.connectionString)) //TRACT
                {
                    connection.Open();
                    string query = $"INSERT INTO log_transactions(transaction_no,transaction_date,member_id,book_isbn) VALUES ('{tno}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{member_id}','{isbn}');";
                    using (var command = new SQLiteCommand(query, connection))
                    {command.ExecuteNonQuery();}
                }
            }
            catch (Exception ex)
            {
                if (first_setup) return;
                XtraMessageBox.Show("Error with TRANSACTION LOGING\n"+ex);
                LOG(ex); 
            }
        }
        
        public static bool database_connection_status = false;

        public static string card_scanner_port = null;
        public static bool card_scanner_connection_status = false;

        public static string book_scanner_port = null;
        public static bool book_scanner_connection_status = false;

        public static bool first_setup = false;

        static bool book_scanner_notification = false;
        static bool card_scanner_notification = false;

        public static string library_name = string.Empty;

        public static string appPath = Directory.GetParent(Application.StartupPath).Parent.FullName + "\\"; //---C:\\Users\\akami\\Desktop\\WiseLib\\
        //public static string serverFolderPath = appPath + "server\\"; //---C:\\Users\\akami\\Desktop\\WiseLib\\server\\
        public static string serverPath = serverFolderPath + "wldb.db"; //---C:\\Users\\akami\\Desktop\\WiseLib\\server\\wldb.db
        //public static string imgPath = serverFolderPath + "imgs\\"; //---C:\\Users\\akami\\Desktop\\WiseLib\\server\\imgs\\
        //public static string cardPath = imgPath + "cards\\"; //---C:\\Users\\akami\\Desktop\\WiseLib\\server\\imgs\\cards\\
        public static string connectionString = string.Empty;

        public static string serverFolderPath = string.Empty;
        //public static string serverPath = string.Empty;
        public static string imgPath = string.Empty;
        public static string cardPath = string.Empty;
        public static string backupPath = string.Empty;
        //public static string connectionString = string.Empty;

        
        public static void check_config(Form form)
        {
            if (!File.Exists("config.ini")) //config.ini
            {
                first_setup = true;

                File.Create("config.ini").Dispose();

                File.WriteAllText("config.ini",
                "[CardScanner]" + Environment.NewLine +
                "CardScannerPort=" + Environment.NewLine +

                "[BookScanner]" + Environment.NewLine +
                "BookScannerPort=");
                if (!Directory.Exists(appPath + "server")) //server folder & server 
                {
                    Directory.CreateDirectory(appPath + "server"); //folder
                    serverFolderPath = appPath + "server\\";

                    SQLiteConnection.CreateFile(serverFolderPath + "wldb.db"); //server
                    serverPath = serverFolderPath + "wldb.db";

                    main.connectionString = $"Data Source={serverPath};Version=3;";

                    string sqlFilePath = appPath + "createnew.sql";
                    string sqlCommands = File.ReadAllText(sqlFilePath);
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        using (var command = new SQLiteCommand(sqlCommands, connection))
                        { command.ExecuteNonQuery(); }
                    }

                    Directory.CreateDirectory(appPath + "imgs");//folder
                    imgPath = serverFolderPath + "imgs\\";

                    Directory.CreateDirectory(imgPath + "cards\\");//folder
                    cardPath = imgPath + "cards\\";
                    //LASTODO download card_example from github rep to server/cards folder
                    Directory.CreateDirectory(serverFolderPath + "backups");//folder
                    backupPath = serverFolderPath + "backups\\";

                    connection_status cStatForm = new connection_status();
                    cStatForm.first_set_up();
                    cStatForm.ShowDialog();
                }
            }
            else //launch
            {
                string[] lines = File.ReadAllLines("config.ini");
                foreach (string line in lines)
                {
                    if (line.StartsWith("LibraryName=") && line.Length == 12)
                    {
                        connection_status cStatForm = new connection_status();
                        cStatForm.first_set_up();
                    }
                }
                serverFolderPath = appPath + "server\\";
                serverPath = serverFolderPath + "wldb.db";
                main.backupPath = serverFolderPath + "backups\\"; ;
                main.connectionString = $"Data Source={serverPath};Version=3;";
                imgPath = serverFolderPath + "imgs\\";
                cardPath = imgPath + "cards\\";
                main.get_library_name();
                Application.Run(new WiseLib());
            }
        }
        public static void update_config(Form form, bool wiselib_launch)
        {
            try
            {
                string[] lines = File.ReadAllLines("config.ini");

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("[CardScanner]"))
                    {
                        if (i + 1 < lines.Length && lines[i + 1].StartsWith("CardScannerPort="))
                        { lines[i + 1] = $"CardScannerPort={card_scanner_port}"; }
                    }

                    if (lines[i].StartsWith("[BookScanner]"))
                    {
                        if (i + 1 < lines.Length && lines[i + 1].StartsWith("BookScannerPort="))
                        { lines[i + 1] = $"BookScannerPort={book_scanner_port}"; }
                    }
                }
                File.WriteAllLines("config.ini", lines);
                first_setup = true;
                if (wiselib_launch)
                {
                    WiseLib WiseLibForm = new WiseLib();
                    form.Hide();
                    WiseLibForm.ShowDialog();
                    form.Close();
                }
                else
                { form.Close(); }
            }
            catch (Exception ex)
            {main.LOG(ex);}
        }
        public static void get_library_name()
        {
            string[] lines = File.ReadAllLines("config.ini");
            library_name = "";
            foreach (string line in lines)
            { if (line.StartsWith("LibraryName=")) library_name = line.Split('=')[1].Trim(); }
        }
        public static bool scanner_status = false;
        public static Timer timer = new Timer();
        public static void check_connections(SimpleButton target_button)
        {
            //check_server_connection(target_button, EventArgs.Empty); //first check 0ms
            set_timer(target_button);
        }
        public static void set_timer(SimpleButton target_button)
        {
            timer.Interval = 5000; //every 5000ms/5sec
            if (target_button.ToolTip == "Server Connection") timer.Tick += (sender, e) => check_server_connection(target_button, e); 
            else if (target_button.ToolTip == "Card Scanner") timer.Tick += (sender, e) => check_card_scanner(target_button); 
            else if (target_button.ToolTip == "Book Scanner") timer.Tick += (sender, e) => check_book_scanner(target_button); 
            timer.Start();
        }
        public static void check_server_connection(SimpleButton target_button, EventArgs e)
        {
            try
            {
                if (!File.Exists(main.serverPath)) //server exist
                {
                    if (target_button.Text == "FirstDBcheck") target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 48); //on connection_status
                    else target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 20);
                    target_button.Text = "\uE839"; // Unicode EthernetError
                    target_button.ForeColor = Color.Red; //RED
                    database_connection_status = false;
                    //XtraMessageBox.Show("Could not connected to server", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (var conn = new SQLiteConnection(main.connectionString))
                {
                    conn.Open();
                    if (target_button.Text == "FirstDBcheck") target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 48); //on connection_status
                    else target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 20);
                    target_button.Text = "\uE839"; //unicode Ethernet
                    target_button.ForeColor = Color.Green; //GREEN
                    conn.Close();
                    database_connection_status = true;
                }
            }
            catch (Exception ex)
            {
                main.LOG(ex);
                if (target_button.Text == "FirstDBcheck") target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 48); //on connection_status
                else target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 20);
                target_button.Text = "\uE839"; //unicode EthernetError
                target_button.ForeColor = Color.Red; //RED
                database_connection_status = false;
                //XtraMessageBox.Show("Could not connected to server","Connection Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void check_card_scanner(SimpleButton target_button)
        {
            return; //TODO
            //try scanning while checking
            SerialPort serialPort = new SerialPort(card_scanner_port, 9600); //check CARD SCANNER

            try //CONNECTED
            {
                serialPort.Open();
                //XtraMessageBox.Show("barcode scanner connected");
                target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 18); 
                target_button.Text = "\uEE6F\nConnected"; // Unicode GenericScan
                target_button.ForeColor = Color.Green; //GREEN
                target_button.Cursor = Cursors.Hand; //cursor
                card_scanner_connection_status = true;
                foreach (Form form in Application.OpenForms) //if wiselib open
                {
                    if (form is WiseLib) 
                    {
                        WiseLib activeWiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault(); //current active wiselib form
                                                                                                              //card logo
                        activeWiseLibForm.simpleButton_scan_card_status.Appearance.Font = new Font("Segoe MDL2 Assets", 46);
                        activeWiseLibForm.simpleButton_scan_card_status.Text = "\uEF40"; // Unicode ChipCardCreditCardReader
                        activeWiseLibForm.simpleButton_scan_card_status.ForeColor = Color.Green; //GREEN
                        activeWiseLibForm.simpleButton_scan_card_status.Cursor = Cursors.Hand; //cursor
                                                                                               //text
                        activeWiseLibForm.label_card_scan_active.Text = "Card Scan Active"; //TEXT
                        activeWiseLibForm.label_card_scan_active.ForeColor = Color.Green; //GREEN 
                    }
                }
               
                //serialPort.DataReceived += (sender, e) =>
                //{
                //    string data = serialPort.ReadExisting();
                //    XtraMessageBox.Show("barcode data: " + data);
                //};
                //XtraMessageBox.Show("scanning");
            }
            catch (Exception ex) //NOT CONNECTED
            {
                main.LOG(ex);
                //XtraMessageBox.Show("Barcode Scanner could not connected: " + ex.Message);
                target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 18);
                target_button.Text = "\uEE6F\nNot Connected"; // Unicode GenericScan
                target_button.ForeColor = Color.Red; //RED

                WiseLib activeWiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                foreach (Form form in Application.OpenForms) //if wiselib open
                {
                    if (form is WiseLib)
                    {
                        activeWiseLibForm.simpleButton_scan_card_status.Appearance.Font = new Font("Segoe MDL2 Assets", 46);
                        activeWiseLibForm.simpleButton_scan_card_status.Text = "\uEF40"; // Unicode ChipCardCreditCardReader
                        activeWiseLibForm.simpleButton_scan_card_status.ForeColor = Color.Red; //GREEN
                        activeWiseLibForm.simpleButton_scan_card_status.Cursor = Cursors.WaitCursor; //cursor
                                                                                                     //text
                        activeWiseLibForm.label_card_scan_active.Text = "Card Scan DeActive"; //TEXT
                        activeWiseLibForm.label_card_scan_active.ForeColor = Color.Red; //GREEN 
                    }
                }
                target_button.Cursor = Cursors.WaitCursor; //cursor
                card_scanner_connection_status = false;

                if (!card_scanner_notification)//show only once
                {
                    SimpleButton btn = new SimpleButton //BOOK SCANNER DISCONNECTED NOTIFICATION
                    {
                        Text = $"BOOK SCANNER DISCONNECTED",
                        Size = new System.Drawing.Size(278, 40),
                        ForeColor = Color.Red,
                        Cursor = Cursors.Hand,
                        Location = new System.Drawing.Point(1, 41 * activeWiseLibForm.btn_count)
                    };
                    activeWiseLibForm.panel_notifications.Controls.Add(btn);
                    btn.Click += (s, e) =>
                    { activeWiseLibForm.simpleButton_current_connection_Click(null, null); };
                    activeWiseLibForm.btn_count++;
                    card_scanner_notification = true;
                }

            }
            finally
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
        }
        public static void check_book_scanner(SimpleButton target_button)
        {
            return; //TODO
            //try scanning while checking
            SerialPort serialPort = new SerialPort(book_scanner_port, 9600); //check BOOK SCANNER port
            try
            {
                serialPort.Open();
                target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 18);
                target_button.Text = "\uEE6F\nConnected"; // Unicode GenericScan
                target_button.ForeColor = Color.Green; //GREEN
                target_button.Cursor = Cursors.Hand; //cursor
                book_scanner_connection_status = true;
                //XtraMessageBox.Show("book scanner connected");

                //serialPort.DataReceived += (sender, e) =>
                //{
                //    string data = serialPort.ReadExisting();
                //    XtraMessageBox.Show("barcode data: " + data);
                //};
                //XtraMessageBox.Show("scanning");
            }
            catch (Exception ex)
            {
                main.LOG(ex);
                //XtraMessageBox.Show("book Scanner could not connected: " + ex.Message);
                target_button.Appearance.Font = new Font("Segoe MDL2 Assets", 18);
                target_button.Text = "\uEE6F\nNot Connected"; // Unicode GenericScan
                target_button.ForeColor = Color.Red; //RED
                target_button.Cursor = Cursors.WaitCursor; //cursor
                book_scanner_connection_status = false;

                if (!book_scanner_notification)
                {
                    WiseLib activeWiseLibForm = Application.OpenForms.OfType<WiseLib>().FirstOrDefault();
                    SimpleButton btn = new SimpleButton
                    {
                        Text = $"CARD SCANNER DISCONNECTED",
                        Size = new System.Drawing.Size(278, 40),
                        ForeColor = Color.Red,
                        Cursor = Cursors.Hand,
                        Location = new System.Drawing.Point(1, 41 * activeWiseLibForm.btn_count)
                    };
                    activeWiseLibForm.panel_notifications.Controls.Add(btn);
                    btn.Click += (s, e) =>
                    { activeWiseLibForm.simpleButton_current_connection_Click(null, null); };
                    activeWiseLibForm.btn_count++;
                    book_scanner_notification = true;
                }
            }
            finally
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
        }
        public static void Panel_Paint(object sender, PaintEventArgs e) //BORDERS
        {
            Control panel = (Control)sender;
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, panel.Width - 1, panel.Height - 1));
            }
        }
        public static void copy_grid(GridView source_grid, GridView target_grid)
        {
            using (var ms = new MemoryStream())
            {
                source_grid.SaveLayoutToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                target_grid.RestoreLayoutFromStream(ms);
            }
        }
    }
}

