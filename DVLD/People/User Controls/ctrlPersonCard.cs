using Driving_License_Management.PeopleFs;
using Driving_License_Management.Properties;
using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Driving_License_Management.UserControls
{
    public partial class ctrlPersonCard : UserControl
    {

        private clsPerson _Person;

        private int _PersonID = -1;
        
        public event Action<int> OnLinklableClick;  
        protected virtual void ProcessComplete(int Click)
        {
            Action<int> handler = OnLinklableClick;

            if (handler != null)
            {
                OnLinklableClick(1);
            }

        }

        public clsPerson SelectedPersonInfo
        {
            get { return _Person; }
        }

        public int PersonID
        {
            get { return _PersonID; }
        }
       
        public ctrlPersonCard()
        {
            InitializeComponent();
        }

        private void _LoadPersonImage()
        {
            if (string.IsNullOrEmpty(_Person.ImagePath) )
            {
                if (_Person.Gender)
                {
                    picbximage.Image = Resources.Female_512;
                }
                else
                    picbximage.Image = Resources.Male_512;


                return;
            }

            clsUtil.SetPersonImage(ref picbximage, ref _Person);
        }

        public void ResetPersonInfo()
        {
            lblID.Text = "[????]";
            lblNationalNo.Text = "[????]";
            lblName.Text = "[????]";
            lblGender.Text = "[????]";
            lblEmail.Text = "[????]";
            lblPhone.Text = "[????]";
            lblDate.Text = "[????]";
            lblCounry.Text = "[????]";
            lblAddress.Text = "[????]";

            picbximage.Image = Resources.Male_512;
            picbximage.ImageLocation = null;
        }

        public void LoadPersonInfo(int PersonID)
        {

            if (PersonID <= 0)
            {
                MessageBox.Show("Object refrence not set to an object");
                return;
            }

            _Person = clsPerson.Find(PersonID);

            ResetPersonInfo();


            if (_Person == null)
            {
                MessageBox.Show("No Person with PersonID = " + PersonID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                _PersonID = _Person.PersonID;
            }

            _FillPersonInfo();

        }

        public void LoadPersonInfo(string NationalNo)
        {

            _Person = clsPerson.Find(NationalNo);

            if (_Person == null)
            {
                MessageBox.Show("No Person with National No. = " + NationalNo.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetPersonInfo();

                return;
            }
            else
            {
                _PersonID = _Person.PersonID;
            }

            _FillPersonInfo();
        }

        public void LoadPersonInfo(clsPerson person)
        {
            _Person = person;
            _PersonID = _Person.PersonID;

            _FillPersonInfo();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessComplete(1);
        }

        public bool isUserControlEmpty()
        {
            bool m = lblID.Text == "[????]" ? true : false;

            return m;
        }

        private void _FillPersonInfo()
        {
            //llEditPersonInfo.Enabled = true;
            //_PersonID = _Person.PersonID;

            lblID.Text = _Person.PersonID.ToString();
            
            lblNationalNo.Text = _Person.NationalNo;
            lblName.Text = _Person.FullName;
            lblGender.Text = _Person.Gender == false ? "Male" : "Female";
            lblEmail.Text = _Person.Email;
            lblPhone.Text = _Person.Phone;
            lblDate.Text = _Person.DateOfBirth.ToShortDateString();
            lblCounry.Text = _Person.CountryInfo.CountryName ;
            lblAddress.Text = _Person.Address;
            
            _LoadPersonImage();
        
        }
        
    }

}
