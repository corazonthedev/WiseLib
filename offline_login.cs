using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WiseLib
{
    public partial class offline_login : DevExpress.XtraEditors.XtraForm
    {
        public offline_login()
        {
            InitializeComponent();
        }
        public string jsonFilePath = "login.json";

        private void offline_login_Load(object sender, EventArgs e)
        {
            get_users_from_json();
        }

        private void get_users_from_json()
        {
            
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var users = JsonConvert.DeserializeObject<List<dynamic>>(jsonContent);
                foreach (var user in users)
                {
                    comboBoxEdit1_users.Properties.Items.Add(user.user_name.ToString());
                }
                if (comboBoxEdit1_users.Properties.Items.Count > 0)
                {
                    comboBoxEdit1_users.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            { main.LOG(ex); }
            
        }

        private void simpleButton1_login_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit1_password.Text) || (comboBoxEdit1_users.SelectedIndex < 0)) //INPUT NULL OR EMPTY 
            {
                XtraMessageBox.Show("Incorrect Username/Password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bool password_correct = check_username_password(comboBoxEdit1_users.Text, textEdit1_password.Text);
                if (password_correct)
                {
                    //START PROGRAM ONLINE
                    XtraMessageBox.Show("asd", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show("Incorrect Username/Password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool check_username_password(string username, string password)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath); //GET JSON 
                var users = JsonConvert.DeserializeObject<List<dynamic>>(jsonContent);
                foreach (var user in users) //CHEKC EACH
                {
                    if (user.user_name.ToString() == username && user.user_password.ToString() == password)
                    {
                        return true; //USERNAME/PASSWORD CORRECT
                        //START PROGRAM OFFLINE
                    }
                }
                return false; //USERNAME/PASSWORD INCORRECT
            }
            catch (Exception ex)
            {
                main.LOG(ex);
                return false;
            }
        }

    }
}
