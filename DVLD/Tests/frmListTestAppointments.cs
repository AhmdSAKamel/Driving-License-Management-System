using Driving_License_Management.Properties;
using Driving_License_Management.TestsFs;
using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management.Tests
{
    public partial class frmListTestAppointments : Form
    {
        public Action DataSaved;


        private DataTable _dtLicenseTestAppointments;
        private int _LocalDrivingLicenseApplicationID;
        private clsTestType.enTestType _TestType = clsTestType.enTestType.VisionTest;

        public frmListTestAppointments(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestType)
        {
            InitializeComponent();

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestType = TestType;
        }

        private void frmListTestAppointments_Load(object sender, EventArgs e)
        {
            _LoadTestTypeImageAndTitle();

            ctrlDrivingLicenseApplicationInfo1.LoadApplicationInfoByLocalDrivingAppID(_LocalDrivingLicenseApplicationID);
            
            _dtLicenseTestAppointments = clsTestAppointment.GetApplicationTestAppointmentsPerTestType(_LocalDrivingLicenseApplicationID, _TestType);

            dgvLicenseTestAppointments.DataSource = _dtLicenseTestAppointments.DefaultView;
            lblRecordsCount.Text = dgvLicenseTestAppointments.Rows.Count.ToString();

            bool doesHaveRows = (dgvLicenseTestAppointments.Rows.Count > 0);

            cmsApplications.Enabled = doesHaveRows;

            if (doesHaveRows)
            {
                dgvLicenseTestAppointments.Columns[0].HeaderText = "Appointment ID";
                dgvLicenseTestAppointments.Columns[0].Width = 150;

                dgvLicenseTestAppointments.Columns[1].HeaderText = "Appointment Date";
                dgvLicenseTestAppointments.Columns[1].Width = 200;

                dgvLicenseTestAppointments.Columns[2].HeaderText = "Paid Fees";
                dgvLicenseTestAppointments.Columns[2].Width = 150;

                dgvLicenseTestAppointments.Columns[3].HeaderText = "Is Locked";
                dgvLicenseTestAppointments.Columns[3].Width = 100;
            }

        }

        private void RefreshForm()
        {
            frmListTestAppointments_Load(null, null);
            DataSaved?.Invoke();
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _LoadTestTypeImageAndTitle()
        {
            switch (_TestType)
            {
                case clsTestType.enTestType.VisionTest:
                    {
                        lblTitle.Text = "Vision Test Appointments";
                        this.Text = lblTitle.Text;
                        pbTestTypeImage.Image = Resources.Vision_512;
                        break;
                    }

                case clsTestType.enTestType.WrittenTest:
                    {
                        lblTitle.Text = "Written Test Appointments";
                        this.Text = lblTitle.Text;
                        pbTestTypeImage.Image = Resources.Written_Test_512;
                        break;
                    }
                case clsTestType.enTestType.StreetTest:
                    {
                        lblTitle.Text = "Street Test Appointments";
                        this.Text = lblTitle.Text;
                        pbTestTypeImage.Image = Resources.driving_test_512;
                        break;
                    }
         
            }

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TestAppointmentID = (dgvLicenseTestAppointments.Rows.Count == 0 ||
                                     dgvLicenseTestAppointments.CurrentRow == null ||
                                     dgvLicenseTestAppointments.CurrentRow.Cells[0].Value == null)
                                     ? -1
                                     : (int)dgvLicenseTestAppointments.CurrentRow.Cells[0].Value;

            using (frmScheduleTest frm = new frmScheduleTest(_LocalDrivingLicenseApplicationID, _TestType, TestAppointmentID))
            {
                frm.DataUpdated += RefreshForm;
                frm.ShowDialog();
            }
        }

        private void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TestAppointmentID = (dgvLicenseTestAppointments.Rows.Count == 0 ||
                                     dgvLicenseTestAppointments.CurrentRow == null ||
                                     dgvLicenseTestAppointments.CurrentRow.Cells[0].Value == null)
                                     ? -1 : (int)dgvLicenseTestAppointments.CurrentRow.Cells[0].Value;


            using (frmTakeTest takeTest = new frmTakeTest(TestAppointmentID, _TestType))
            {
                takeTest.TestBeenTaken += RefreshForm;
                takeTest.ShowDialog();
            }
        }

        private void btnAddNewAppointment_Click(object sender, EventArgs e)
        {

            clsLocalDrivingLicenseApplication localDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_LocalDrivingLicenseApplicationID);

            if (localDrivingLicenseApplication.IsThereAnActiveScheduledTest(_TestType))
            {
                MessageBox.Show("Person Already have an active appointment for this test, You cannot add new appointment", 
                                "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // This should get information about the last test that a person sat from the same TestType. 
            clsTest LastTest = localDrivingLicenseApplication.GetLastTestPerTestType(_TestType);


            if (LastTest == null)   // That mean it's the first test to this localDrivinglicenseID for this test type.
            {
                using (frmScheduleTest frm1 = new frmScheduleTest(_LocalDrivingLicenseApplicationID, _TestType))
                {
                    frm1.DataUpdated += RefreshForm;
                    frm1.ShowDialog();
                }
                return;
            }

            // If person already passed the test s/he cannot retak it.
            // We can't make a schedule to a test that person already passed!
            if (LastTest.TestResult == true)
            {
                MessageBox.Show("This person already passed this test before, you can only retake faild test", 
                                 "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnAddNewAppointment.Enabled = false;
                return;
            }

            using (frmScheduleTest frm2 = new frmScheduleTest(LastTest.TestAppointmentInfo.LocalDrivingLicenseApplicationID, _TestType))
            {
                frm2.DataUpdated += RefreshForm;
                frm2.ShowDialog();
            }

        }
        

    }

}
