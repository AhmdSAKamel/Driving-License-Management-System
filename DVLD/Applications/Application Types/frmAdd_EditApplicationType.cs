using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Driving_License_Management.Applications
{
    public partial class frmAdd_EditApplicationType : Form
    {
        public Action DataUpdated;

        public enum Mode { AddNew, Update}

        Mode _Mode;
        

        bool DidDataChange = false;
        int appTypeID = -1;

        clsApplicationType app;


        public frmAdd_EditApplicationType(int ApplicationTypeID)
        {
            InitializeComponent();

            appTypeID = ApplicationTypeID;

            _Mode = Mode.Update;
        }

        public frmAdd_EditApplicationType()
        {
            InitializeComponent();

            _Mode = Mode.AddNew;
        }

        private void frmAdd_EditApplicationType_Load(object sender, EventArgs e)
        {

            if (_Mode == Mode.Update)
            {
                app = clsApplicationType.FindApplTypeByID((byte)appTypeID);
                SetAppTypeInfo();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetAppTypeInfo()
        {
            if (app != null)
            {
                lblappID.Text = app.ApplicationTypeID.ToString();

                txtAppTitle.Text = app.Title;

                txtAppFees.Text = app.Fees.ToString();

                DidDataChange = false;
            }

        }

        private void txtAppTitle_TextChanged(object sender, EventArgs e)
        {
            DidDataChange = true;
        }

        private void txtAppFees_TextChanged(object sender, EventArgs e)
        {
            DidDataChange = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (DidDataChange)
            {
                if (this.ValidateChildren())
                {
                    app.Title = txtAppTitle.Text;
                    app.Fees = decimal.Parse(txtAppFees.Text);

                    if (app.Save())
                    {
                        MessageBox.Show("Data saved SUCCESSFULLY.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DataUpdated?.Invoke();

                        DidDataChange = false;
                    }

                }
                else
                {
                    MessageBox.Show("Please ensure that all required data is complete before saving.", "Incomplete Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            else
            {
                MessageBox.Show("No changes were made to the data. Please modify the information before saving.", "No Data Changes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void txtAppTitle_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtAppTitle.Text))
            {
                e.Cancel = true;
                txtAppTitle.Focus();
                errorProvider1.SetError(txtAppTitle, "Title Should have a title.");
            }
            else
            {
                errorProvider1.Clear();
                e.Cancel = false;
            }

        }

        private void txtAppFees_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtAppFees.Text))
            {
                e.Cancel = true;
                txtAppTitle.Focus();
                errorProvider1.SetError(txtAppFees, "Fees Should have a Value.");
            }
            else
            {
                errorProvider1.Clear();
                e.Cancel = false;
            }

            if (!decimal.TryParse(txtAppFees.Text, out decimal fee))
            {
                e.Cancel = true;
                txtAppTitle.Focus();
                errorProvider1.SetError(txtAppFees, "Fees Should have a valid amount.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.Clear();
            }

        }
        
    }

}
