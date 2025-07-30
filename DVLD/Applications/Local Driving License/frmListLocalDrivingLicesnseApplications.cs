using Driving_License_Management.Driving_Licenses;
using Driving_License_Management.Licenses;
using Driving_License_Management.Licenses.Local_Driving_License;
using Driving_License_Management.Properties;
using Driving_License_Management.Tests;
using Driving_License_Management.TestsFs;
using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management.Applications.Local_Driving_License
{

    public partial class frmListLocalDrivingLicesnseApplications : Form
    {
        private DataTable _dtAllLocalDrivingLicenseApplications;//= clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();

        DataTable LicenseClasses;

        string[] byStatusstrings = { "All", "New", "Canceled", "Completed" };


        int LocalDLAppID;
        int TotalPassedTests;
        int LicenseID;
        bool hasValidLicense;

        const int AppStatusColumnIndex = 6;
        const int TotalPassedTestsColumnIndex = 5;
        const int LicenseIDColumnIndex = 7;

        StringBuilder filterString = new StringBuilder();


        public enum FilterChoices
        {
            None = 0,
            LDLAppID,
            DrivingClass,
            NationalNo,
            FullName,
            ApplicationDate,
            PassedTests,
            Status
        }
        private FilterChoices _filterBy;

        public frmListLocalDrivingLicesnseApplications()
        {
            InitializeComponent();

            // To find Control!

            //Control foundControl = this.Controls.Find("ctrlFilter1", true).FirstOrDefault();
            //if (foundControl != null)
            //{
            //    MessageBox.Show("Control is found!");
            //}
        }

        private void frmListLocalDrivingLicesnseApplications_Load(object sender, EventArgs e)
        {
            _dtAllLocalDrivingLicenseApplications = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();


            dgvLocalDrivingLicenseApplications.DataSource = _dtAllLocalDrivingLicenseApplications.DefaultView;

            //dgvLocalDrivingLicenseApplications.Columns["LicenseID"].Visible = false;

            SetlblRecord();

            ctrlFilter1.FillcbWithData(ref _dtAllLocalDrivingLicenseApplications, "None");

            LicenseClasses = clsLicenseClass.GetAllLicenseClasses();
        }

        private void SetlblRecord()
        {
            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
        }

        private void RefreshForm()
        {
            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CleardgvFilterResults()
        {
            _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = null;
            SetlblRecord();
        }

        private void ctrlFilter1_OnFilterInputByCB_1()
        {
            PerformFilter();
        }

        private void ctrlFilter1_OnFilterInput_1()
        {
            //timer1.Tick += timer1_Tick; // Ensure event is assigned only once
            timer1.Start(); // Start the timer
            timer1.Stop();

            PerformFilter();
        }

        private void ctrlFilter1_OncbChosen_1()
        {
            _filterBy = (FilterChoices)ctrlFilter1.SelectedcbIndex;


            ctrlFilter1.isFilterByCB = false;
            ctrlFilter1.SetCBFilter = false;
            ctrlFilter1.isFilterByIntegerType = false;


            switch (_filterBy)
            {
                case FilterChoices.DrivingClass:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByCB = true;
                        ctrlFilter1.TurntxtFilterInputIntoCB(ref LicenseClasses, 0, "All");
                        break;
                    }

                case FilterChoices.Status:
                    {
                        CleardgvFilterResults();
                        ctrlFilter1.isFilterByCB = true;
                        ctrlFilter1.TurntxtFilterInputIntoCB(byStatusstrings);
                        break;
                    }

                case FilterChoices.LDLAppID:
                case FilterChoices.PassedTests:
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

        private void showDetailsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            using (frmLocalDrivingLicenseApplicationInfo frm = new frmLocalDrivingLicenseApplicationInfo((int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value))
            {
                frm.ShowDialog();
            }
        }

        private void btnAddNewApp_Click(object sender, EventArgs e)
        {
            using (frmAddUpdateLocalDrivingLicenseApp frmAddNewLDL = new frmAddUpdateLocalDrivingLicenseApp())
            {
                frmAddNewLDL.AppAdded += RefreshForm;

                frmAddNewLDL.ShowDialog();
            }

        }

        private void editToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            LocalDLAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            using (frmAddUpdateLocalDrivingLicenseApp frmUpdateLDL = new frmAddUpdateLocalDrivingLicenseApp(LocalDLAppID))
            {
                frmUpdateLDL.AppAdded += RefreshForm;
                frmUpdateLDL.ShowDialog();
            }

        }

        private void DeleteApplicationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure do want to delete this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;


            LocalDLAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDLAppID);

            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.Delete())
                {
                    MessageBox.Show("Application Deleted Successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshForm();
                }
                else
                {
                    MessageBox.Show("Could not delete applicatoin, other data depends on it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void CancelApplicaitonToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            
            if (MessageBox.Show("Are you sure do want to cancel this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;


            LocalDLAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication CurrentLocalDrivingLicenseApp = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDLAppID);

            if (CurrentLocalDrivingLicenseApp != null)
            {
                if (CurrentLocalDrivingLicenseApp.CancelApplication())
                {
                    MessageBox.Show("Application Cancelled Successfully.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //refresh the form again.
                    RefreshForm();
                }
                else
                {
                    MessageBox.Show("Could not cancel applicatoin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void cmsApplications_Opening(object sender, CancelEventArgs e)
        {

            LocalDLAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication CurrentLocalDrivingLicenseApp = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDLAppID);

            bool isNew = (CurrentLocalDrivingLicenseApp.ApplicationStatus == clsApplication.enApplicationStatus.New);
            bool isCompleted = (CurrentLocalDrivingLicenseApp.ApplicationStatus == clsApplication.enApplicationStatus.Completed);


            TotalPassedTests = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[5].Value;

            hasValidLicense = CurrentLocalDrivingLicenseApp.IsLicenseIssued();

            issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = (TotalPassedTests > 2) && !hasValidLicense && isNew;//|| isCompleted;

            showLicenseToolStripMenuItem.Enabled = hasValidLicense;

            editToolStripMenuItem.Enabled = !hasValidLicense && isNew && (TotalPassedTests == 0);
            //ScheduleTestsMenue.Enabled = !hasValidLicense;

            CancelApplicaitonToolStripMenuItem.Enabled = isNew;

            DeleteApplicationToolStripMenuItem.Enabled = isNew;


            bool PassedVisionTest = CurrentLocalDrivingLicenseApp.DoesPassTestType(clsTestType.enTestType.VisionTest); ;
            bool PassedWrittenTest = CurrentLocalDrivingLicenseApp.DoesPassTestType(clsTestType.enTestType.WrittenTest);
            bool PassedStreetTest = CurrentLocalDrivingLicenseApp.DoesPassTestType(clsTestType.enTestType.StreetTest);

            ScheduleTestsMenue.Enabled = (!PassedVisionTest || !PassedWrittenTest || !PassedStreetTest) && isNew;


            if (ScheduleTestsMenue.Enabled)
            {
                toolStripMenuItem1.Enabled = !PassedVisionTest;

                toolStripMenuItem2.Enabled = PassedVisionTest && !PassedWrittenTest;

                toolStripMenuItem3.Enabled = PassedVisionTest && PassedWrittenTest && !PassedStreetTest;
            }

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
                        case FilterChoices.DrivingClass:
                            filterString = new StringBuilder(("[" + ctrlFilter1.cbChosenString + "]" + " = " + "'" + ctrlFilter1.CBFilterString + "'"));
                            break;

                        case FilterChoices.Status:
                            filterString = new StringBuilder(("[Status] =  " + "'" + ctrlFilter1.CBFilterString + "'"));
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
                if (_filterBy == FilterChoices.ApplicationDate)
                {

                    if (_filterBy == FilterChoices.ApplicationDate && DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dt))
                    {
                        filterString = new StringBuilder(dt.TimeOfDay != TimeSpan.Zero ? $"[Application Date] = '{dt:yyyy-MM-dd HH:mm:ss}'"
                            : $"[Application Date] >= '#{dt:MM/dd/yyyy}#' AND [Application Date] < '#{dt.AddDays(1):MM/dd/yyyy}#'");

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
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = filterString.ToString();
            }
            catch (EvaluateException ex)
            {
                MessageBox.Show($"Invalid search pattern. Please revise your input. {ex.Message}",
                                 "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetlblRecord();
        }

        private void ScheduleTest(clsTestType.enTestType TestType)
        {
            int LocalDrivingLicenseApplicationID = (dgvLocalDrivingLicenseApplications.Rows.Count == 0 ||
                                                    dgvLocalDrivingLicenseApplications.CurrentRow == null ||
                                                    dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value == null)
                                                    ? -1 : (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            using (frmListTestAppointments ListTestAppointment = new frmListTestAppointments(LocalDrivingLicenseApplicationID, TestType))
            {
                ListTestAppointment.DataSaved += RefreshForm;

                ListTestAppointment.ShowDialog();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ScheduleTest(clsTestType.enTestType.VisionTest);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ScheduleTest(clsTestType.enTestType.WrittenTest);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ScheduleTest(clsTestType.enTestType.StreetTest);
        }

        private void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            using (frmIssueDriverLicenseFirstTime issueDrivingLicense = new frmIssueDriverLicenseFirstTime(LocalDrivingLicenseApplicationID))
            {
                issueDrivingLicense.LicenseIssued += RefreshForm;
                issueDrivingLicense.ShowDialog();
            }

        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            int LicenseID = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID).GetActiveLicenseID();

            if (LicenseID != -1)
            {
                frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
                frm.ShowDialog();

            }
            else
            {
                MessageBox.Show("No License Found!", "No License", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string nationalNo = (dgvLocalDrivingLicenseApplications.CurrentRow.Cells[2].Value).ToString();

            using (frmShowPersonLicenseHistory showLicenseHistory = new frmShowPersonLicenseHistory(nationalNo))
            {
                showLicenseHistory.InformationUpdated += RefreshForm;
                showLicenseHistory.ShowDialog();
            }

        }

        
    }

}
