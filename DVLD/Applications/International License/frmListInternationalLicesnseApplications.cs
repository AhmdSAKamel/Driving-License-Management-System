using Driving_License_Management.Licenses;
using Driving_License_Management.Licenses.International_Licenses;
using Driving_License_Management.PeopleFs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Driving_License_Management.UserControls.ctrlPersonCardWithFilter;

namespace Driving_License_Management.Applications.International_License
{
    public partial class frmListInternationalLicesnseApplications : Form
    {
        private DataTable _dtInternationalLicenseApplications;

        StringBuilder filterString = new StringBuilder();
        
        private string []strings = { "All","Active", "Inactive" }; 

        enum FilterChoices
        {
            None = 0,
            int_LicenseID,
            ApplicationID,
            DriverID,
            L_LicenseID,
            IssueDate,
            ExpirationDate,
            IsActive
        }
        FilterChoices _filterChoice;

        public frmListInternationalLicesnseApplications()
        {
            InitializeComponent();
        }

        private void frmListInternationalLicesnseApplications_Load(object sender, EventArgs e)
        {

            _dtInternationalLicenseApplications = clsInternationalLicense.GetAllInternationalLicenses();

            dgvInternationalLicenses.DataSource = _dtInternationalLicenseApplications;
            lblInternationalLicensesRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();

            if (dgvInternationalLicenses.Rows.Count > 0)
            {
                //dgvInternationalLicenses.Columns[0].HeaderText = "Int.License ID";
                dgvInternationalLicenses.Columns[0].Width = 70;

                //dgvInternationalLicenses.Columns[1].HeaderText = "Application ID";
                dgvInternationalLicenses.Columns[1].Width = 70;

                //dgvInternationalLicenses.Columns[2].HeaderText = "Driver ID";
                dgvInternationalLicenses.Columns[2].Width = 60;

                //dgvInternationalLicenses.Columns[3].HeaderText = "L.License ID";
                dgvInternationalLicenses.Columns[3].Width = 60;

                //dgvInternationalLicenses.Columns[4].HeaderText = "Issue Date";
                dgvInternationalLicenses.Columns[4].Width = 100;

                //dgvInternationalLicenses.Columns[5].HeaderText = "Expiration Date";
                dgvInternationalLicenses.Columns[5].Width = 100;

                //dgvInternationalLicenses.Columns[6].HeaderText = "Is Active";
                dgvInternationalLicenses.Columns[6].Width = 85;
            }

            cmsApplications.Enabled = (dgvInternationalLicenses.Rows.Count > 0);

            ctrlFilter1.FillcbWithData(ref _dtInternationalLicenseApplications, "None");

            SetlblRecord();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetlblRecord()
        {
            lblInternationalLicensesRecords.Text = _dtInternationalLicenseApplications.Rows.Count.ToString();
        }

        private void RefreshForm()
        {
            frmListInternationalLicesnseApplications_Load(null, null);
        }

        private void CleardgvFilterResults()
        {
            _dtInternationalLicenseApplications.DefaultView.RowFilter = null;
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
            _filterChoice = (FilterChoices)ctrlFilter1.SelectedcbIndex;


            ctrlFilter1.isFilterByCB = false;
            ctrlFilter1.SetCBFilter = false;
            ctrlFilter1.isFilterByIntegerType = false;


            switch (_filterChoice)
            {
                case FilterChoices.IsActive:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByCB = true;
                        ctrlFilter1.TurntxtFilterInputIntoCB(strings);
                        break;
                    }


                case FilterChoices.int_LicenseID:
                case FilterChoices.L_LicenseID:
                case FilterChoices.ApplicationID:
                case FilterChoices.DriverID:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByIntegerType = true;
                        ctrlFilter1.txtFilterValue = "";
                        break;
                    }

            }

            ctrlFilter1.SettxtFilter = (_filterChoice != FilterChoices.None && !ctrlFilter1.isFilterByCB);

            PerformFilter();
        }

        private void PerformFilter()
        {

            if (!ctrlFilter1.isFilterByCB && (string.IsNullOrWhiteSpace(ctrlFilter1.txtFilterValue) || _filterChoice == FilterChoices.None))
            {
                CleardgvFilterResults();
                return;
            }

            if (ctrlFilter1.isFilterByCB)
            {
                if (ctrlFilter1.SelectedCBFilterIndex > 0)
                {
                    //byte filterindex = (byte)(ctrlFilter1.SelectedCBFilterIndex - 1);

                    switch (ctrlFilter1.SelectedCBFilterIndex)
                    {
                        case 1:
                            filterString = new StringBuilder(("[" + ctrlFilter1.cbChosenString + "]" + "=" + 1));
                            break;
                        case 2:
                            filterString = new StringBuilder(("[" + ctrlFilter1.cbChosenString + "]" + "=" + 0));
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
                if (_filterChoice == FilterChoices.IssueDate || _filterChoice == FilterChoices.ExpirationDate)
                {

                    switch (_filterChoice)
                    {
                        case FilterChoices.ExpirationDate:
                            {
                                if (DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dt))
                                {
                                    filterString = new StringBuilder(dt.TimeOfDay != TimeSpan.Zero ? $"[Expiration Date] = '{dt:yyyy-MM-dd HH:mm:ss}'"
                                                 : $"[Expiration Date] >= '#{dt:MM/dd/yyyy}#' AND [Expiration Date] < '#{dt.AddDays(1):MM/dd/yyyy}#'");
                                }

                                break;
                            }
                        case FilterChoices.IssueDate:
                            {
                                if (DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dt))
                                {
                                    filterString = new StringBuilder(dt.TimeOfDay != TimeSpan.Zero ? $"[Issue Date] = '{dt:yyyy-MM-dd HH:mm:ss}'"
                                                 : $"[Issue Date] >= '#{dt:MM/dd/yyyy}#' AND [Issue Date] < '#{dt.AddDays(1):MM/dd/yyyy}#'");
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
                _dtInternationalLicenseApplications.DefaultView.RowFilter = filterString.ToString();
            }
            catch (EvaluateException ex)
            {
                MessageBox.Show($"Invalid search pattern. Please revise your input. {ex.Message}",
                                 "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetlblRecord();
        }

        private void PesonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DriverID = (int)dgvInternationalLicenses.CurrentRow.Cells[2].Value;

            int PersonID = clsDriver.FindByDriverID(DriverID).PersonID;

            frmShowDetails showDetails = new frmShowDetails(PersonID);
            {
                showDetails.ShowDialog();
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int InternationalLicenseID = (int)dgvInternationalLicenses.CurrentRow.Cells[0].Value;

            using (frmShowInternationalLicenseInfo showInternationalLicenseDetails = new frmShowInternationalLicenseInfo(InternationalLicenseID))
            {
                showInternationalLicenseDetails.ShowDialog();
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int DriverID = (int)dgvInternationalLicenses.CurrentRow.Cells[2].Value;
            int PersonID = clsDriver.FindByDriverID(DriverID).PersonID;

            using (frmShowPersonLicenseHistory showPersonLicenseHistory = new frmShowPersonLicenseHistory(PersonID))
            {
                showPersonLicenseHistory.ShowDialog();
            }
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            using (frmNewInternationalLicenseApplication addNewInternationalLicense = new frmNewInternationalLicenseApplication())
            {
                addNewInternationalLicense.LicenseAdded += RefreshForm;
                addNewInternationalLicense.ShowDialog();
            }
        }


    }


}
