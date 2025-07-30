using Driving_License_Management.Licenses;
using Driving_License_Management.Licenses.Detain_License;
using Driving_License_Management.Licenses.Local_Driving_License;
using Driving_License_Management.PeopleFs;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using static Driving_License_Management.UserControls.ctrlPersonCardWithFilter;

namespace Driving_License_Management.Applications.Release_Detained_License
{
    public partial class frmListDetainedLicense : Form
    {

        private DataTable _dtDetainedLicenses;

        private string[] strings = { "All", "Released", "Unreleased" };
        private enum FilterChoices
        {
            None = 0,
            DetainID,
            LicenseID,
            DetainDate,
            IsRelease,
            FineFees,
            ReleaseDate,
            NationalNo,
            FullName,
            ReleaseAppID
        }
        FilterChoices filterChoice;

        StringBuilder filterString = new StringBuilder(); 
        
        public frmListDetainedLicense()
        {
            InitializeComponent();
        }

        private void frmListDetainedLicense_Load(object sender, EventArgs e)
        {
            
            _dtDetainedLicenses = clsDetainedLicense.GetAllDetainedLicenses();


            dgvDetainedLicenses.DataSource = _dtDetainedLicenses.DefaultView;
            lblTotalRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();

            if (dgvDetainedLicenses.Rows.Count > 0)
            {
                //dgvDetainedLicenses.Columns[0].HeaderText = "D.ID";
                dgvDetainedLicenses.Columns[0].Width = 70;

                //dgvDetainedLicenses.Columns[1].HeaderText = "L.ID";
                dgvDetainedLicenses.Columns[1].Width = 70;

                //dgvDetainedLicenses.Columns[2].HeaderText = "D.Date";
                dgvDetainedLicenses.Columns[2].Width = 120;

                //dgvDetainedLicenses.Columns[3].HeaderText = "Is Released";
                dgvDetainedLicenses.Columns[3].Width = 80;

                //dgvDetainedLicenses.Columns[4].HeaderText = "Fine Fees";
                dgvDetainedLicenses.Columns[4].Width = 90;

                //dgvDetainedLicenses.Columns[5].HeaderText = "Release Date";
                dgvDetainedLicenses.Columns[5].Width = 120;

                //dgvDetainedLicenses.Columns[6].HeaderText = "N.No.";
                dgvDetainedLicenses.Columns[6].Width = 70;

                //dgvDetainedLicenses.Columns[7].HeaderText = "Full Name";
                dgvDetainedLicenses.Columns[7].Width = 270;

                //dgvDetainedLicenses.Columns[8].HeaderText = "Rlease App.ID";
                dgvDetainedLicenses.Columns[8].Width = 130;
            }

            ctrlFilter1.FillcbWithData(ref _dtDetainedLicenses, "None");

            cmsApplications.Enabled = dgvDetainedLicenses.Rows.Count > 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetlblRecord()
        {
            lblTotalRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void RefreshForm()
        {
            frmListDetainedLicense_Load(null, null);
        }

        private void CleardgvFilterResults()
        {
            _dtDetainedLicenses.DefaultView.RowFilter = null;
            SetlblRecord();
        }

        private void ctrlFilter1_OnFilterInputByCB()
        {
            PerformFilter();
        }

        private void ctrlFilter1_OnFilterInput()
        {
            timer1.Start(); // Start the timer
            timer1.Stop();

            PerformFilter();
        }

        private void ctrlFilter1_OncbChosen()
        {

            filterChoice = (FilterChoices)ctrlFilter1.SelectedcbIndex;


            ctrlFilter1.isFilterByCB = false;
            ctrlFilter1.SetCBFilter = false;
            ctrlFilter1.isFilterByIntegerType = false;


            switch (filterChoice)
            {
                case FilterChoices.IsRelease:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByCB = true;
                        ctrlFilter1.TurntxtFilterInputIntoCB(strings);
                        break;
                    }


                case FilterChoices.ReleaseAppID:
                case FilterChoices.LicenseID:
                case FilterChoices.DetainID:
                case FilterChoices.FineFees:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByIntegerType = true;
                        ctrlFilter1.txtFilterValue = "";
                        break;
                    }

            }

            ctrlFilter1.SettxtFilter = (filterChoice != FilterChoices.None && !ctrlFilter1.isFilterByCB);

            PerformFilter();


        }

        private void PerformFilter()
        {

            if (!ctrlFilter1.isFilterByCB && (string.IsNullOrWhiteSpace(ctrlFilter1.txtFilterValue) || filterChoice == FilterChoices.None))
            {
                CleardgvFilterResults();
                return;
            }


            if (ctrlFilter1.isFilterByCB)
            {

                if (ctrlFilter1.SelectedCBFilterIndex > 0)
                {
                    byte filterindex = (byte)(ctrlFilter1.SelectedCBFilterIndex - 1);

                    switch (ctrlFilter1.CBFilterString)
                    {

                        case "Released":
                            {
                                filterString = new StringBuilder(("[" + ctrlFilter1.cbChosenString + "]" + "=" + 1));
                                break;
                            }
                        case "Unreleased":
                            {
                                filterString = new StringBuilder(("[" + ctrlFilter1.cbChosenString + "]" + "=" + 0));
                                break;
                            }

                    }


                    switch (filterChoice)
                    {



                        case FilterChoices.IsRelease:
                            break;
                    }

                }
                else
                {
                    CleardgvFilterResults();
                    return;
                }

            }
            else
            {
                if (filterChoice == FilterChoices.DetainDate || filterChoice == FilterChoices.ReleaseDate)
                {

                    switch (filterChoice)
                    {
                        case FilterChoices.DetainDate:
                            {
                                if (DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dt))
                                {
                                    filterString = new StringBuilder(dt.TimeOfDay != TimeSpan.Zero ? $"[D.Date] = '{dt:yyyy-MM-dd HH:mm:ss}'"
                                                 : $"[D.Date] >= '#{dt:MM/dd/yyyy}#' AND [D.Date] < '#{dt.AddDays(1):MM/dd/yyyy}#'");
                                }

                                break;
                            }
                        case FilterChoices.ReleaseDate:
                            {
                                if (DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dt))
                                {
                                    filterString = new StringBuilder(dt.TimeOfDay != TimeSpan.Zero ? $"[Release Date] = '{dt:yyyy-MM-dd HH:mm:ss}'"
                                                 : $"[Release Date] >= '#{dt:MM/dd/yyyy}#' AND [Release Date] < '#{dt.AddDays(1):MM/dd/yyyy}#'");
                                }

                                break;
                            }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ctrlFilter1.txtFilterValue))
                    {
                        filterString = new StringBuilder(ctrlFilter1.StringToExecuteFilter());
                    }
                }

            }

            try
            {
                _dtDetainedLicenses.DefaultView.RowFilter = filterString.ToString();
            }
            catch (EvaluateException ex)
            {
                MessageBox.Show($"Invalid search pattern. Please revise your input. {ex.Message}",
                    "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetlblRecord();
        }

        private void btnReleaseDetainedLicense_Click(object sender, EventArgs e)
        {
            using (frmReleaseDetainedLicense ReleaseLicense = new frmReleaseDetainedLicense())
            {
                ReleaseLicense.LicenseReleased += RefreshForm;
                ReleaseLicense.ShowDialog();
            }
        }

        private void btnDetain_Click(object sender, EventArgs e)
        {
            using (frmDetainLicenseApplication detainLicense = new frmDetainLicenseApplication())
            {
                detainLicense.LicenseDetained += RefreshForm;
                detainLicense.ShowDialog();
            }


        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(dgvDetainedLicenses.CurrentRow.Cells[1].Value.ToString(), out int LicenseID))
                return;

            //LicenseID = (int)dgvDetainedLicenses.CurrentRow.Cells[1].Value;
            int PersonID = clsLicense.Find(LicenseID).DriverInfo.PersonID;

            using (frmShowDetails showPersonDetails = new frmShowDetails(PersonID))
            {
                showPersonDetails.ShowDialog();
            }
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(dgvDetainedLicenses.CurrentRow.Cells[1].Value.ToString(), out int LicenseID))
                return;

            frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
            {
                frm.ShowDialog();
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(dgvDetainedLicenses.CurrentRow.Cells[1].Value.ToString(), out int LicenseID))
                return;

            int PersonID = clsLicense.Find(LicenseID).DriverInfo.PersonID;

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(PersonID);
            {
                frm.ShowDialog();
            }
        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(dgvDetainedLicenses.CurrentRow.Cells[1].Value.ToString(), out int LicenseID))
                return;

            using (frmReleaseDetainedLicense ReleaseLicense = new frmReleaseDetainedLicense(LicenseID))
            {
                ReleaseLicense.LicenseReleased += RefreshForm;
                ReleaseLicense.ShowDialog();
            }
        }

        private void cmsApplications_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            releaseDetainedLicenseToolStripMenuItem.Enabled = !(bool)dgvDetainedLicenses.CurrentRow.Cells[3].Value;
        }

    }


}
