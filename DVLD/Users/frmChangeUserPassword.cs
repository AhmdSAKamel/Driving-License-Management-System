using Driving_License_Management.Global_Classes;
using Driving_License_Management.PeopleFs;
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

namespace Driving_License_Management.UserFs
{
    public partial class frmChangeUserPassword : Form
    {
        clsUser _User = new clsUser();

        public Action Yes;

        public frmChangeUserPassword(int PersonID)
        {
            InitializeComponent();
            
            _User = clsUser.FindByPersonID(PersonID);
        }

        public frmChangeUserPassword()
        {
            InitializeComponent();

            _User = clsGlobal.CurrentUser;
        }

        private void frmChangeUserPassword_Load(object sender, EventArgs e)
        {

            lblUserID.Text = _User.UserID.ToString();
            lblUserName.Text = _User.UserName.ToString();

            if (_User.isActive)
            {
                lblActiveStatus.Text = "Yes";
            }
            else
            {
                lblActiveStatus.Text = "No";
            }

            ctrlPersonDetails1.LoadPersonInfo(_User.PersonID);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCurrentPassword_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtCurrentPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtCurrentPassword, "Username cannot be blank");
                return;
            }
            else
            {
                errorProvider1.SetError(txtCurrentPassword, null);
            }


            if (_User.Password != clsValidation.ComputeHash(txtCurrentPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtCurrentPassword, "Current password is wrong!");
                return;
            }
            else
            {
                errorProvider1.SetError(txtCurrentPassword, null);
            }

        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtNewPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNewPassword, "New Password cannot be blank");
            }
            else
            {
                errorProvider1.SetError(txtNewPassword, null);
            }

        }

        private void txtConfirmingPassword_Validating(object sender, CancelEventArgs e)
        {

            if (txtConfirmingNewPassword.Text.Trim() != txtNewPassword.Text.Trim())
            {
                e.Cancel = true;
                errorProvider1.SetError(txtConfirmingNewPassword, "Password Confirmation does not match New Password!");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtConfirmingNewPassword, null);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (this.ValidateChildren())
            {
                _User.Password = clsValidation.ComputeHash(txtNewPassword.Text);

                if (_User.Save())
                {
                    MessageBox.Show("Data Saved Successfully", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblUserID.Text = _User.UserID.ToString();
                }

            }

        }

        private void ctrlPersonDetails1_OnLinklableClick(int obj)
        {

            if (!ctrlPersonDetails1.isUserControlEmpty())
            {
                frmAddEditNewPerson frmEditPerson = new frmAddEditNewPerson(_User.Person.PersonID);

                frmEditPerson.DataBack += RefreshUserInfo;

                frmEditPerson.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a person first to edit their information.", "No Person Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void RefreshUserInfo()
        {
            _User.Person = clsPerson.Find(_User.Person.PersonID);

            ctrlPersonDetails1.LoadPersonInfo(_User.PersonID);
        }

    }

}
