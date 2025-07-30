using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Driving_License_Management.Licenses;
using Driving_License_Management.PeopleFs;






namespace Driving_License_Management.Drivers
{
    public partial class frmListDrivers : Form
    {

        private DataTable _dtAllDrivers;

        StringBuilder filterString = new StringBuilder();
        string[] strings = { "All", "Active", "Inactive" };

        public enum FilterChoices
        {
            None = 0,
            byDrivderID,
            byPersonID,
            byNationalNo,
            byFullName,
            byDate,
            byActiveLicense,
        }
        private FilterChoices _filterBy;

        public enum FilterCBChoice
        {
            All = 0,
            Active,
            Inactive
        }
        private FilterCBChoice _filterCBChoice = new FilterCBChoice();


        public frmListDrivers()
        {
            InitializeComponent();
        }

        private void frmListDrivers_Load(object sender, EventArgs e)
        {
            _dtAllDrivers = clsDriver.GetAllDrivers();
            dgvDrivers.DataSource = _dtAllDrivers.DefaultView;
            
            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();

            ctrlFilter1.FillcbWithData(ref _dtAllDrivers, "None");


            if (dgvDrivers.Rows.Count > 0)
            {
                //dgvDrivers.Columns[0].HeaderText = "Driver ID";
                dgvDrivers.Columns[0].Width = 120;

                //dgvDrivers.Columns[1].HeaderText = "Person ID";
                dgvDrivers.Columns[1].Width = 120;

                //dgvDrivers.Columns[2].HeaderText = "National No.";
                dgvDrivers.Columns[2].Width = 140;

                //dgvDrivers.Columns[3].HeaderText = "Full Name";
                dgvDrivers.Columns[3].Width = 320;

                //dgvDrivers.Columns[4].HeaderText = "Date";
                dgvDrivers.Columns[4].Width = 170;

                //dgvDrivers.Columns[5].HeaderText = "Active Licenses";
                dgvDrivers.Columns[5].Width = 150;
            
            }

            cmsDrivers.Enabled = dgvDrivers.Rows.Count > 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _dtAllDrivers.Dispose();
            this.Close();
        }

        private void ctrlFilter1_OncbChosen()
        {

            _filterBy = (FilterChoices)ctrlFilter1.SelectedcbIndex;


            ctrlFilter1.isFilterByCB = false;
            ctrlFilter1.SetCBFilter = false;
            ctrlFilter1.isFilterByIntegerType = false;


            switch (_filterBy)
            {

                case FilterChoices.byActiveLicense:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByCB = true;
                        ctrlFilter1.TurntxtFilterInputIntoCB(strings);
                        break;
                    }

                case FilterChoices.byDrivderID:
                case FilterChoices.byPersonID:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByIntegerType = true;
                        ctrlFilter1.txtFilterValue = "";
                        break;
                    }


            }

            ctrlFilter1.SettxtFilter = (_filterBy != FilterChoices.None && !ctrlFilter1.isFilterByCB);

            PerformFilter();

        }

        private void ctrlFilter1_OnFilterInput()
        {

            PerformFilter();
        }

        private void ctrlFilter1_OnFilterInputByCB()
        {
            _filterCBChoice = (FilterCBChoice)ctrlFilter1.SelectedCBFilterIndex;

            PerformFilter();
        }

        private void PerformFilter()
        {

            if (!ctrlFilter1.isFilterByCB && (string.IsNullOrWhiteSpace(ctrlFilter1.txtFilterValue) || _filterBy == FilterChoices.None))
            {
                CleardgvFilterResults();
                return;
            }

            if (ctrlFilter1.isFilterByCB)
            {
                if (ctrlFilter1.SelectedCBFilterIndex > 0)
                {
                    switch (_filterBy)
                    {
                        case FilterChoices.byActiveLicense:

                            switch (_filterCBChoice)
                            {
                                case FilterCBChoice.Active:
                                    {
                                        filterString = new StringBuilder("[Active Licenses] = 1");
                                        break;
                                    }
                                case FilterCBChoice.Inactive:
                                    {
                                        filterString = new StringBuilder("[Active Licenses] = 0");
                                        break;
                                    }
                            
                            }

                            break;
                        
                    }

                }
                else
                {
                    filterString = new StringBuilder("");
                }

            }
            else
            {
                if (_filterBy == FilterChoices.byDate)
                {
                    if (DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dt))
                    {
                        filterString = new StringBuilder(dt.TimeOfDay != TimeSpan.Zero ? $"[Date] = '{dt:yyyy-MM-dd HH:mm:ss}'"
                            : $"[Date] >= '#{dt:MM/dd/yyyy}#' AND [Date] < '#{dt.AddDays(1):MM/dd/yyyy}#'");
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
                _dtAllDrivers.DefaultView.RowFilter = filterString.ToString();
            }
            catch (EvaluateException ex)
            {
                MessageBox.Show($"Invalid search pattern. Please revise your input. {ex.Message}",
                    "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetlblRecord();
        }

        private void CleardgvFilterResults()
        {
            _dtAllDrivers.DefaultView.RowFilter = null;
            SetlblRecord();
        }

        private void SetlblRecord()
        {
            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
        }

        private void issueInternationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet.");
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int personID = (int)dgvDrivers.CurrentRow.Cells[1].Value;

            using (frmShowDetails showDetails = new frmShowDetails(personID))
            {
                showDetails.ShowDialog();
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int personID = (int)dgvDrivers.CurrentRow.Cells[1].Value;

            using (frmShowPersonLicenseHistory showPersonLicenseHistory = new frmShowPersonLicenseHistory(personID))
            {
                showPersonLicenseHistory.ShowDialog();
            }

        }



    }

}
