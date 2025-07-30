using DVLDBuiness;
using System;
using System.Windows.Forms;

namespace Driving_License_Management.TestsFs
{
    public partial class frmEditTestType : Form
    {

        public Action DataUpdated;

        clsTestType testType;
        bool DidDataChange = false;


        public frmEditTestType(byte TestTypeID)
        {
            InitializeComponent();

            testType = clsTestType.FindTestTypeByTestTypeID(TestTypeID);

            if (testType != null)
            {
                RefreshForm();
            }
            else
            {
                MessageBox.Show(
                    "This Application Type can't be found!", "Error",
                        MessageBoxButtons.OK,   MessageBoxIcon.Error);              
                
                //this.Close();
            }

        }

        private void RefreshForm()
        {
            lblappID.Text = testType.ID.ToString();

            txtTestTitle.Text = testType.Title.ToString();

            txtTestDescription.Text = testType.Description.ToString();

            txtFee.Text = testType.Fees.ToString();

            DidDataChange = false;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void txtAppTitle_TextChanged(object sender, System.EventArgs e)
        {
            DidDataChange = true;

        }

        private void txtAppDescription_TextChanged(object sender, System.EventArgs e)
        {
            DidDataChange = true;

        }

        private void txtAppFees_TextChanged(object sender, System.EventArgs e)
        {
            DidDataChange = true;

        }

        private void txtAppTitle_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if (string.IsNullOrEmpty(txtTestTitle.Text))
            {
                e.Cancel = true;
                txtTestTitle.Focus();
                errorProvider1.SetError(txtTestTitle, "Title Should have a title.");
            }
            else
            {
                errorProvider1.Clear();
                e.Cancel = false;
            }

        }

        private void txtAppDescription_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtTestDescription.Text))
            {
                e.Cancel = true;
                txtTestTitle.Focus();
                errorProvider1.SetError(txtTestDescription, "Title Should have a title.");
            }
            else
            {
                errorProvider1.Clear();
                e.Cancel = false;
            }

        }

        private void txtAppFees_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtFee.Text))
            {
                e.Cancel = true;
                txtTestTitle.Focus();
                errorProvider1.SetError(txtFee, "Fees Should have a Value.");
            }
            else
            {
                errorProvider1.Clear();
                e.Cancel = false;
            }

            if (!double.TryParse(txtFee.Text, out double fee))
            {
                e.Cancel = true;
                txtTestTitle.Focus();
                errorProvider1.SetError(txtFee, "Fees Should have a valid amount.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.Clear();
            }

        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            
            if (DidDataChange)
            {
                if (ValidateAndUpdateApplication())
                {
                    if (testType.Save())
                    {
                        MessageBox.Show("Data saved SUCCESSFULLY.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DataUpdated?.Invoke();

                        DidDataChange = false;
                    }

                }
                else
                {
                    return;
                }

            }
            else
            {
                MessageBox.Show("No changes were made to the data. Please modify the information before saving.", "No Data Changes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private bool ValidateAndUpdateApplication()
        {

            if (string.IsNullOrEmpty(txtTestTitle.Text))
            {
                MessageBox.Show("Please ensure that all required data is complete before saving.", "Incomplete Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                testType.Title = txtTestTitle.Text;
            }


            if (string.IsNullOrEmpty(txtTestDescription.Text))
            {
                MessageBox.Show("Please ensure that all required data is complete before saving.", "Incomplete Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                testType.Description = txtTestDescription.Text;
            }


            if (string.IsNullOrEmpty(txtFee.Text))
            {
                MessageBox.Show("Please ensure that all required data is complete before saving.", "Incomplete Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (double.TryParse(txtFee.Text, out double fee))
            {
                testType.Fees = (decimal)fee;
            }
            else
            {
                MessageBox.Show("Please enter a valid amount for the application fees.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;

        }

    }

}
