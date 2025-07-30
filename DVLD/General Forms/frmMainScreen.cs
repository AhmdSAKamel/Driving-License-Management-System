using System;
using System.Windows.Forms;
using Driving_License_Management.Forms;
using Driving_License_Management.UserFs;
using Driving_License_Management.TestsFs;
using Driving_License_Management.Global_Classes;
using Driving_License_Management.Driving_Licenses;
using Driving_License_Management.Applications.Local_Driving_License;
using Driving_License_Management.Applications.Renew_Local_License;
using Driving_License_Management.Drivers;
using Driving_License_Management.Licenses.Detain_License;
using Driving_License_Management.Applications.Release_Detained_License;
using Driving_License_Management.Applications.International_License;
using Driving_License_Management.General_Forms;




namespace Driving_License_Management
{

    public partial class frmMainScreen : Form
    {
         
        public frmMainScreen()
        {
            InitializeComponent();
        }

        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frmpeople = new frmPeople())
            {
                frmpeople.ShowDialog();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            using (var frmUser = new frmUsers())
            {
                frmUser.ShowDialog();
            }

        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form frmLogging = new frmLogin())
            {
                clsGlobal.CurrentUser = null;

                this.Hide();
                frmLogging.ShowDialog();
                this.Close();
            }

        }

        private void btnCurrentUserInfo_Click(object sender, EventArgs e)
        {
            using (frmShowUserDetails showUserDetails = new frmShowUserDetails())
            {
                showUserDetails.ShowDialog();
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            using (frmChangeUserPassword changePassword = new frmChangeUserPassword())
            {
                changePassword.ShowDialog();
            }
        }

        private void manageApplicationTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmApplicationTypes Applications = new frmApplicationTypes())
            {
                Applications.ShowDialog();
            }
        }

        private void manageTestTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmTestTypes frmtest = new frmTestTypes())
            {
                frmtest.ShowDialog();
            }
        }

        private void localLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAddUpdateLocalDrivingLicenseApp addNewLocalLicenseApp = new frmAddUpdateLocalDrivingLicenseApp())
            {
                addNewLocalLicenseApp.ShowDialog();
            }
        }

        private void localDrivingLicenseApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmListLocalDrivingLicesnseApplications frmLocalDrivingApplications = new frmListLocalDrivingLicesnseApplications())
            {
                frmLocalDrivingApplications.ShowDialog();
            }
        }

        private void retakeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmListLocalDrivingLicesnseApplications frmLocalDrivingApplications = new frmListLocalDrivingLicesnseApplications())
            {
                frmLocalDrivingApplications.ShowDialog();
            }
        }

        private void renewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmRenewLocalDrivingLicenseApplication renewLocalLicense = new frmRenewLocalDrivingLicenseApplication())
            {
                renewLocalLicense.ShowDialog();
            }
        }

        private void replacementForLostOrDamagedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmReplaceLostOrDamagedLicenseApplication Replace = new frmReplaceLostOrDamagedLicenseApplication())
            {
                Replace.ShowDialog();
            }

        }

        private void btnDrivers_Click(object sender, EventArgs e)
        {
            using (frmListDrivers listDrivers = new frmListDrivers() )
            {
                listDrivers.ShowDialog();
            }

        }

        private void manageDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmListDetainedLicense ListOfdetainedLicenses = new frmListDetainedLicense())
            {
                ListOfdetainedLicenses.ShowDialog();
            }
        }

        private void detainLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmDetainLicenseApplication detainLicense = new frmDetainLicenseApplication())
            {
                detainLicense.ShowDialog();
            }

        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmReleaseDetainedLicense ReseaLicense = new frmReleaseDetainedLicense())
            {
                ReseaLicense.ShowDialog();
            }

        }
         
        private void releaseDetainedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmReleaseDetainedLicense ReseaLicense = new frmReleaseDetainedLicense())
            {
                ReseaLicense.ShowDialog();
            }
        }

        private void internationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(frmNewInternationalLicenseApplication NewInternationalLicense = new frmNewInternationalLicenseApplication())
            {
                NewInternationalLicense.ShowDialog();
            }
        }

        private void internationalLicensesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmListInternationalLicesnseApplications InternationalLicenseList = new frmListInternationalLicesnseApplications())
            {
                InternationalLicenseList.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (frmConnection frmconString = new frmConnection())
            {
                frmconString.ShowDialog();
            }

            this.Close();
        }
    }

}
