using Driving_License_Management.Applications.International_License;
using Driving_License_Management.Licenses.Local_Driving_License;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management.Licenses.Controls
{
    public partial class ctrlDriverLicenses : UserControl
    {
        
        private int _DriverID;
        private clsDriver _Driver;
        private DataTable _dtDriverLocalLicensesHistory;
        private DataTable _dtDriverInternationalLicensesHistory;

        public ctrlDriverLicenses()
        {
            InitializeComponent();
        }

        private void _LoadLocalLicenseInfo()
        {

            _dtDriverLocalLicensesHistory = clsDriver.GetLicenses(_DriverID);

            //if (_dtDriverLocalLicensesHistory == null)
            //    return;

            dgvLocalLicensesHistory.DataSource = _dtDriverLocalLicensesHistory.DefaultView;
            lblLocalLicensesRecords.Text = dgvLocalLicensesHistory.Rows.Count.ToString();

            if (dgvLocalLicensesHistory.Rows.Count > 0)
            {
                dgvLocalLicensesHistory.Columns[0].HeaderText = "Lic.ID";
                dgvLocalLicensesHistory.Columns[0].Width = 110;

                dgvLocalLicensesHistory.Columns[1].HeaderText = "App.ID";
                dgvLocalLicensesHistory.Columns[1].Width = 110;

                dgvLocalLicensesHistory.Columns[2].HeaderText = "Class Name";
                dgvLocalLicensesHistory.Columns[2].Width = 270;

                dgvLocalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvLocalLicensesHistory.Columns[3].Width = 170;

                dgvLocalLicensesHistory.Columns[4].HeaderText = "Expiration Date";
                dgvLocalLicensesHistory.Columns[4].Width = 170;

                dgvLocalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvLocalLicensesHistory.Columns[5].Width = 110;

            }


            cmsLocalLicenseHistory.Enabled = (dgvLocalLicensesHistory.Rows.Count > 0);

        }

        private void _LoadInternationalLicenseInfo()
        {

            _dtDriverInternationalLicensesHistory = clsDriver.GetInternationalLicenses(_DriverID);

            if (_dtDriverInternationalLicensesHistory == null)
                return;

            dgvInternationalLicensesHistory.DataSource = _dtDriverInternationalLicensesHistory.DefaultView;
            lblInternationalLicensesRecords.Text = dgvInternationalLicensesHistory.Rows.Count.ToString();

            if (dgvInternationalLicensesHistory.Rows.Count > 0)
            {
                dgvInternationalLicensesHistory.Columns[0].HeaderText = "Int.License ID";
                dgvInternationalLicensesHistory.Columns[0].Width = 160;

                dgvInternationalLicensesHistory.Columns[1].HeaderText = "Application ID";
                dgvInternationalLicensesHistory.Columns[1].Width = 130;

                dgvInternationalLicensesHistory.Columns[2].HeaderText = "L.License ID";
                dgvInternationalLicensesHistory.Columns[2].Width = 130;

                dgvInternationalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvInternationalLicensesHistory.Columns[3].Width = 180;

                dgvInternationalLicensesHistory.Columns[4].HeaderText = "Expiration Date";
                dgvInternationalLicensesHistory.Columns[4].Width = 180;

                dgvInternationalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvInternationalLicensesHistory.Columns[5].Width = 120;

            }

            cmsInterenationalLicenseHistory.Enabled = (dgvInternationalLicensesHistory.Rows.Count > 0);
                
        }

        public void LoadInfo(int DriverID)
        {
            _DriverID = DriverID;
            _Driver = clsDriver.FindByDriverID(_DriverID);

            if (_Driver == null)
            {
                MessageBox.Show("There is no driver with id = " + _DriverID, "Not Found Driver!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _LoadLocalLicenseInfo();
            _LoadInternationalLicenseInfo();
        }

        public void LoadInfoByPersonID(int PersonID)
        {

            _Driver = clsDriver.FindByPersonID(PersonID);
            _DriverID = _Driver.DriverID;

            if (_Driver == null)
            {
                MessageBox.Show("There is no driver linked with person ID = " + PersonID, "Driver Not found", 
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            _LoadLocalLicenseInfo();
            _LoadInternationalLicenseInfo();
        }

        private void showLicenseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalLicensesHistory.Rows.Count == 0)
                return;

            int LicenseID = (int)dgvLocalLicensesHistory.CurrentRow.Cells[0].Value;
            
            using (frmShowLicenseInfo showLicenseInfo = new frmShowLicenseInfo(LicenseID))
            {
                showLicenseInfo.ShowDialog();
            }

        }

        private void InternationalLicenseHistorytoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvInternationalLicensesHistory.Rows.Count == 0)
                return;

            int InternationalLicenseID = (int)dgvInternationalLicensesHistory.CurrentRow.Cells[0].Value;

            using (frmShowPersonLicenseHistory History = new frmShowPersonLicenseHistory(InternationalLicenseID))
            {
                History.ShowDialog();
            }

        }

        public void Clear()
        {
            if (_dtDriverInternationalLicensesHistory == null || _dtDriverLocalLicensesHistory == null)
                return;

            _dtDriverLocalLicensesHistory.Clear();
            _dtDriverInternationalLicensesHistory.Clear();
        }


    }

}
