using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using DevExpress.XtraRichEdit.Model;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace WiseLib
{
    public partial class connection_status : DevExpress.XtraEditors.XtraForm
    {
        public connection_status()
        {InitializeComponent();}  
        private void connection_status_Load(object sender, EventArgs e)
        {
            if (main.first_setup == false)
            {
                ListActiveCOMPorts();
                //main.check_config(this); //check COM port connections

                simpleButton_backup_database.Enabled = true;
                simpleButton_create_new_database.Enabled = true;
                simpleButton_select_database.Enabled = true;

                string[] lines = File.ReadAllLines("config.ini");
                string cardScannerPort = "";
                string bookScannerPort = "";

                foreach (string line in lines)
                {
                    if (line.StartsWith("CardScannerPort=")) cardScannerPort = line.Split('=')[1].Trim();
                    if (line.StartsWith("BookScannerPort=")) bookScannerPort = line.Split('=')[1].Trim();
                }
                if (comboBox1.Items.Contains(cardScannerPort)) comboBox1.SelectedItem = cardScannerPort;
                if (comboBox2.Items.Contains(bookScannerPort)) comboBox2.SelectedItem = bookScannerPort;
                main.check_server_connection(simpleButton_database, e); //check db
                main.check_card_scanner(simpleButton_card_scanner);
                main.check_book_scanner(simpleButton_book_scanner);
                label_db_name.Text = main.serverPath;
            }
        }
        //FIRST LOAD----------------------------------------------------------------------------------------
        public void first_set_up()
        {
            if (main.first_setup) //first setup
            {
                simpleButton_backup_database.Enabled = false;
                simpleButton_create_new_database.Enabled = false;
                simpleButton_select_database.Enabled = false;
                ListActiveCOMPorts();
                (comboBox1.SelectedIndex, comboBox2.SelectedIndex) = (0, 0);
                (simpleButton_book_scanner.Text, simpleButton_card_scanner.Text) = ("\uEE6F", "\uEF40");
                main.check_server_connection(simpleButton_database, null);
                if (simpleButton_database.Text.Contains("\uE839")) { label_db_name.Text = "Database Connected"; label_db_name.ForeColor = Color.Green; }
                else { label_db_name.Text = "Database NOT Connected"; label_db_name.ForeColor = Color.Red; }
                main.TLOG("0", "", "");
                main.get_library_name();
            }
        }
        private void connection_status_Shown(object sender, EventArgs e)
        { if (main.first_setup) XtraMessageBox.Show("This is for first time setting up, select your ports and confirm.\n If you don't have one select 'COM debug' ", "WiseLib By corazonthedev Setting up", MessageBoxButtons.OK, MessageBoxIcon.Information); } //first setup
        //REGULAR LOAD--------------------------------------------------------------------------------------
        private void ListActiveCOMPorts()
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            { comboBox1.Items.Add(port); }

            comboBox2.Items.Clear();
            string[] _ports = SerialPort.GetPortNames();
            foreach (string port in _ports)
            { comboBox2.Items.Add(port); }
            comboBox1.Items.Add("COM debug");
            comboBox2.Items.Add("COM debug");
            if (comboBox1.Items.Count < 0 || comboBox2.Items.Count < 0)
            { MessageBox.Show("No COM ports found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void simpleButton_create_new_database_Click(object sender, EventArgs e)
        {
            var result = XtraMessageBox.Show("Are you sure to create new Database?\n" +
                "This will override existing Database. Backup is suggested.", "Create New Database",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string dbPath = main.serverPath;
                string sqlFilePath = main.appPath+"createnew.sql";
                try
                {
                    if (File.Exists(dbPath))
                    {File.Delete(dbPath);}

                    SQLiteConnection.CreateFile(dbPath);

                    string sqlCommands = File.ReadAllText(sqlFilePath);
                    using (var connection = new SQLiteConnection(main.connectionString))
                    {
                        connection.Open();
                        using (var command = new SQLiteCommand(sqlCommands, connection))
                        { command.ExecuteNonQuery(); }
                    }
                    XtraMessageBox.Show("New Database created Succesfully, WiseLib will Restart.","Create Succes",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    main.TLOG("0-2","","");
                    Application.Restart();
                }
                catch (Exception ex)
                {XtraMessageBox.Show(ex.ToString());main.LOG(ex);}
            }
        }
        public void simpleButton_backup_database_Click(object sender, EventArgs e)
        {
            try
            {
                string sourceFilePath = main.serverPath;
                string destinationFolderPath = main.backupPath;
                string newFileName = "wldb" + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".db";
                string destinationFilePath = Path.Combine(destinationFolderPath, newFileName);
                File.Copy(sourceFilePath, destinationFilePath, true);
                if (sender != null) XtraMessageBox.Show("Backup Server Completed.");
                main.TLOG("0-3", "", "");
            }
            catch(Exception ex)
            {main.LOG(ex); XtraMessageBox.Show("Failed to Backup Server");}
        }
        private void simpleButton_select_database_Click(object sender, EventArgs e)
        {
            history historyForm = new history(3, null); //member
            historyForm.FormClosed += (s, args) => this.Enabled = true; //enable when closed
            historyForm.ShowDialog();
            main.TLOG("0-4", "", "");
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //main.check_card_scanner(simpleButton_card_scanner);
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //main.check_book_scanner(simpleButton_book_scanner);
        }
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (main.first_setup) //regular
            {
                main.card_scanner_port = comboBox1.SelectedItem?.ToString();
                main.check_card_scanner(simpleButton_card_scanner);
            }
        }
        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (main.first_setup) //regular load
            {
                main.book_scanner_port = comboBox2.SelectedItem?.ToString();
                main.check_book_scanner(simpleButton_book_scanner);
            }
        }
        public static void open_github(object sender, EventArgs e)
        {
            string url = "https://github.com/corazonthedev"; //lasttodo
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true //open in browser
                });
            }
            catch (Exception ex)
            {MessageBox.Show($"failed to open link: {ex.Message}"); main.LOG(ex); }
        }
        private void simpleButton_confirm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.SelectedItem?.ToString()) && !string.IsNullOrEmpty(comboBox2.SelectedItem?.ToString()))
            {navigationFrame1.SelectedPage = navigationPage2;}
            else
            { XtraMessageBox.Show("Please Select PORTS");}
        }
        //----------NAVIGATIONPAGE 2-------------------------------------------------------------------------------------------------------------------
        private void pictureBox_library_logo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {pictureBox_library_logo.Image = Image.FromFile(ofd.FileName); File.Copy(ofd.FileName, main.serverFolderPath + "logo.png", true); label6.Visible = false; }}
        }
        private void simpleButton_library_confirm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textEdit_library_name.ToString()) && pictureBox_library_logo.Image != null)
            {
                main.card_scanner_port = comboBox1.SelectedItem?.ToString();
                main.book_scanner_port = comboBox2.SelectedItem?.ToString();
                if (main.first_setup)
                {
                    string content = $"[LibraryName]{Environment.NewLine}LibraryName={textEdit_library_name.Text}"; File.AppendAllText("config.ini", content);
                    XtraMessageBox.Show("Success!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); main.get_library_name();
                    bool isWiseLibFormOpen = Application.OpenForms.OfType<WiseLib>().Any();
                    main.update_config(this, !isWiseLibFormOpen); main.TLOG("0-1", "", "");
                }
            }
            else { XtraMessageBox.Show("Enter your library name/logo"); }
        }
    }
}
