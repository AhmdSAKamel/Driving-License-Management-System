using Driving_License_Management.General_Forms;
using Driving_License_Management.Global_Classes;
using DVLDBuiness;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;



namespace Driving_License_Management
{
    public partial class frmLogin : Form
    {
        private bool DataChanged = false;

        private string _KeyPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLDAppInfo";
        private string _valueName = "CurrentUserinDVLDApp";

        
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString))
            {
                try
                {
                    string value = Registry.GetValue(_KeyPath, _valueName, null) as string;

                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] currentuser = value.Split('#');

                        txtUserName.Text = currentuser[0];
                        txtPassword.Text = currentuser[1];

                        DataChanged = false;
                        chkRememberMe.Checked = true;

                        btnlogin.TabIndex = 0;
                        txtUserName.TabIndex = 1;
                        txtPassword.TabIndex = 2;
                        chkRememberMe.TabIndex = 3;
                    }

                }
                catch (Exception ex)
                {
                    LogUIExceptions.WriteExceptionToEventViewer(ex);
                }

            }
            else
            {
                btnlogin.Enabled = false;
                txtPassword.Enabled = false;
                txtUserName.Enabled = false;
                chkRememberMe.Enabled = false;

                MessageBox.Show("❌ Connection string is missing. Please complete your data.", "Missing Data",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                using (var frmConnection = new frmConnection())
                {
                    frmConnection.ShowDialog();
                }


                this.Close();
            }


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            DataChanged = true;
        }

        private void chkRememberMe_KeyPress(object sender, KeyPressEventArgs e)
        {
            chkRememberMe.Checked = !chkRememberMe.Checked;
        }

        private void OpenMainScreenForm()
        {
            using (frmMainScreen mainForm = new frmMainScreen())
            {
                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(txtUserName.Text) && !string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                LogUserIn();
            }
            else
            {
                MessageBox.Show("Please enter both username and password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void WriteUserInfoToRegistry(ref string UserNamre, string Password)
        {

            string dataValue = $"{UserNamre}#{Password}";

            try
            {
                Registry.SetValue(_KeyPath, _valueName, dataValue, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                LogUIExceptions.WriteExceptionToEventViewer(ex);
            }

        }

        private void DeleteStoredCredential()
        {

            string dataValue = "";

            try
            {
                Registry.SetValue(_KeyPath, _valueName, dataValue, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                LogUIExceptions.WriteExceptionToEventViewer(ex);
            }

        }

        private void LogUserIn()
        {
            var username = txtUserName.Text.Trim();
            var password = clsValidation.ComputeHash(txtPassword.Text);

            if (Regex.IsMatch(username, @"\b(delete|insert|update|drop|create|select)\b", RegexOptions.IgnoreCase) || Regex.IsMatch(password, @"\b(delete|insert|update|drop|create|select)\b", RegexOptions.IgnoreCase))
            {
                MessageBox.Show("SQL commands are not allowed in this input.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var currentUser = clsUser.FindByUsernameAndPassword(username, password);

            try
            {

                if (currentUser == null)
                {
                    MessageBox.Show("Invalid Username/Password.", "Wrong Credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!currentUser.isActive)
                {
                    MessageBox.Show("Your Account is Deactivated!", "Contact Your Admin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            catch (Exception ex)
            {
                LogUIExceptions.WriteExceptionToEventViewer(ex);
            }

            if (chkRememberMe.Checked && DataChanged)
            {
                WriteUserInfoToRegistry(ref username, txtPassword.Text);
            }

            if (!chkRememberMe.Checked)
            {
                DeleteStoredCredential();
            }

            clsGlobal.CurrentUser = currentUser;

            OpenMainScreenForm();
        }

    }

}
