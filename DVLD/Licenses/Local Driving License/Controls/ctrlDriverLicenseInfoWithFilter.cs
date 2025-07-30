using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Driving_License_Management.Licenses.Local_Driving_License.Controls
{
    public partial class ctrlDriverLicenseInfoWithFilter : UserControl
    {
        // Define a custom event handler delegate with parameters
        public event Action<int> OnLicenseSelected;
        // Create a protected method to raise the event with a parameter
        protected virtual void PersonSelected(int LicenseID)
        {
            Action<int> handler = OnLicenseSelected;
            if (handler != null)
            {
                handler(LicenseID); // Raise the event with the parameter
            }

        }


        private int _LicenseID = -1;

        private bool _FilterEnabled = true;

        public bool FilterEnabled
        {
            get
            {
                return _FilterEnabled;
            }
            set
            {
                _FilterEnabled = value;
                gbFilters.Enabled = _FilterEnabled;
            }
        }
        public int LicenseID
        {
            get { return ctrlDriverLicenseInfo1.LicenseID; }
        }

        public clsLicense SelectedLicenseInfo
        { get { return ctrlDriverLicenseInfo1.SelectedLicenseInfo; } }


        public ctrlDriverLicenseInfoWithFilter()
        {
            InitializeComponent();
        }

        public void txtLicenseIDFocus()
        {
            txtLicenseID.Focus();
        }

        public void LoadLicenseInfo(int LicenseID)
        {

            txtLicenseID.Text = LicenseID.ToString();

            ctrlDriverLicenseInfo1.LoadInfo(LicenseID);
            _LicenseID = ctrlDriverLicenseInfo1.LicenseID;

            if (ctrlDriverLicenseInfo1.SelectedLicenseInfo != null && FilterEnabled)
            {
                OnLicenseSelected?.Invoke(_LicenseID);

                //if (OnLicenseSelected != null && FilterEnabled)
                //{
                //    OnLicenseSelected(_LicenseID);
                //}
            }

        }

        private void txtLicenseID_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This line restricts input to only numeric characters and control keys.
            //char.IsDigit(e.KeyChar): Allows numbers(0 - 9).
            //char.IsControl(e.KeyChar): Allows control keys(like Backspace, Enter, and Tab).

            //The !(NOT operator) ensures that all non-digit and non-control characters are blocked by setting e.Handled = true.
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);



            if (e.KeyChar == (char)13)
            {
                btnFind.PerformClick();
            }

            // (char)13 represents the ASCII code for the Enter key.
            // When Enter is pressed, btnFind.PerformClick(); programmatically clicks the button, triggering its event handler.
            // This allows users to press Enter to search instead of clicking manually.

        }

        private void btnFind_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", 
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                txtLicenseID.Focus();
                return;
            }

            _LicenseID = int.Parse(txtLicenseID.Text);
            LoadLicenseInfo(_LicenseID);
        
        }

        private void txtLicenseID_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtLicenseID.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtLicenseID, "This field is required!");
            }
            else
            {
                errorProvider1.SetError(txtLicenseID, null);
            }
        }


    }
}
