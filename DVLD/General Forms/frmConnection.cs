using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management.General_Forms
{
    public partial class frmConnection : Form
    {
        public frmConnection()
        {
            InitializeComponent();
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {

            string server = txtServerName.Text.Trim();
            string db = txtDataBaseName.Text.Trim();
            string user = txtUserName.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(txtServerName.Text) || string.IsNullOrWhiteSpace(txtDataBaseName.Text) ||
                   string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                List<string> missingFields = new List<string>();

                if (string.IsNullOrWhiteSpace(txtServerName.Text)) missingFields.Add("Server");
                if (string.IsNullOrWhiteSpace(txtDataBaseName.Text)) missingFields.Add("Database");
                if (string.IsNullOrWhiteSpace(txtUserName.Text)) missingFields.Add("Username");
                if (string.IsNullOrWhiteSpace(txtPassword.Text)) missingFields.Add("Password");

                string message = "Please fill in the following field(s):\n" + string.Join("\n", missingFields);
                MessageBox.Show(message, "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Optional: set focus to the first missing field
                if (missingFields.Contains("Server")) txtServerName.Focus();
                else if (missingFields.Contains("Database")) txtDataBaseName.Focus();
                else if (missingFields.Contains("Username")) txtUserName.Focus();
                else if (missingFields.Contains("Password")) txtPassword.Focus();

            }

            // ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

            string newConnection = $"Server={server};Initial Catalog={db};User Id={user};Password={pass};";

            // Optional: Test connection before saving
            using (SqlConnection conn = new SqlConnection(newConnection))
            {
                try
                {
                    conn.Open();
                    conn.Close();

                    // ✅ Safely save to YourApp.exe.config
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    // 🛡 Check if "MyDB" entry exists and update or add it
                    if (config.ConnectionStrings.ConnectionStrings["MyDB"] == null)
                    {
                        config.ConnectionStrings.ConnectionStrings.Add(
                            new ConnectionStringSettings("MyDB", newConnection));
                    }
                    else
                    {
                        config.ConnectionStrings.ConnectionStrings["MyDB"].ConnectionString = newConnection;
                    }

                    // ✅ Save and refresh the config section
                    config.Save(ConfigurationSaveMode.Full);  // You can also use Modified if preferred
                    ConfigurationManager.RefreshSection("connectionStrings");

                    MessageBox.Show("✅ Connection string saved successfully.", "Successful Operation",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Failed to connect: {ex.Message}", "FAILED Operation",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

    }

}
