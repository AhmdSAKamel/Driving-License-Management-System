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
    public partial class frmShowUserDetails : Form
    {

        clsUser User;

        public frmShowUserDetails(int PersonID)
        {
            InitializeComponent();

            User = clsUser.FindByPersonID(PersonID);
            //User.Person = clsPerson.Find(PersonID);
        }

        public frmShowUserDetails()
        {
            InitializeComponent();

            User = clsGlobal.CurrentUser;
        }

        private void frmShowUserDetails_Load(object sender, EventArgs e)
        {

            if (User.Person == null || User == null)
            {
                MessageBox.Show("No person found with the specified PersonID.", "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
            else
            {
                ctrlPersonDetails1.LoadPersonInfo(User.PersonID);
                lblUserID.Text = User.UserID.ToString();
                lblUserName.Text = User.UserName;

                if (User.isActive)
                {
                    lblActiveStatus.Text = "Yes";
                }
                else
                {
                    lblActiveStatus.Text = "No";
                }

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ctrlPersonDetails1_OnLinklableClick(int obj)
        {

            if (!ctrlPersonDetails1.isUserControlEmpty())
            {
                frmAddEditNewPerson frmEditPerson = new frmAddEditNewPerson(User.Person.PersonID);

                frmEditPerson.DataBack += RefreshUserInfo;

                frmEditPerson.ShowDialog();
            }
            else
            {
                MessageBox.Show("There is no a person selected.", "No Person Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void RefreshUserInfo()
        {
            //User.Person = clsPerson.Find(User.Person.PersonID);

            ctrlPersonDetails1.LoadPersonInfo(User.PersonID);
        }
        
    }

}
