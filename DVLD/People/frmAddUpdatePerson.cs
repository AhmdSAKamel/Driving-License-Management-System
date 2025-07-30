using System;
using System.Data;
using DVLDBuiness;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms;
using Driving_License_Management.Global_Classes;
using Driving_License_Management.Properties;
using System.Drawing;
using Driving_License_Management.General_Forms;


namespace Driving_License_Management.PeopleFs
{
    public partial class frmAddEditNewPerson : Form
    { 

        public Action<int> DataBackWithID;

        public Action DataBack;

        public Action<clsPerson> SendPersonInfoBack;


        public enum enMode { AddNew = 0, Update = 1 };

        private enMode _Mode;

        int _PersonID;

        clsPerson _Person = new clsPerson();

        public frmAddEditNewPerson(int PersonID)
        {
            InitializeComponent();

            _PersonID = PersonID;
            _Mode = enMode.Update;
        }

        public frmAddEditNewPerson()
        {
            InitializeComponent();

            _Mode = enMode.AddNew;
        }

        private void frmAddEditNewPerson_Load(object sender, EventArgs e)
        {
            // Done!
            _ResetDefualtValues();

            if (_Mode == enMode.Update)
                _LoadData();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbMale_CheckedChanged(object sender, EventArgs e)
        {
            if (pbPersonImage.ImageLocation == null)
                pbPersonImage.Image = Resources.Male_512;
        }

        private void rbFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (pbPersonImage.ImageLocation == null)
                pbPersonImage.Image = Resources.Female_512;
        }

        private void SetTakenPhoto(string ImageLocation)
        {
            pbPersonImage.ImageLocation = ImageLocation;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (frmTakePicture takePicture = new frmTakePicture())
            {
                takePicture.imageBack += SetTakenPhoto;
                takePicture.ShowDialog();
            }

        }

        private void _FillCountriesInComoboBox()
        {
            // Done!
            DataTable dtCountries = clsCountry.GetAllCountries();

            foreach (DataRow row in dtCountries.Rows)
            {
                cbCountry.Items.Add(row["CountryName"]);
            }

        }

        private void llSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Done!

            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                pbPersonImage.Load(selectedFilePath);
                llRemoveImage.Visible = true;
            }

        }

        private void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Done!

            pbPersonImage.ImageLocation = null;

            if (rbMale.Checked)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            llRemoveImage.Visible = false;
        }

        private void _LoadData()
        {

            _Person = clsPerson.Find(_PersonID);

            if (_Person == null)
            {
                MessageBox.Show("No Person with ID = " + _PersonID, "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            lblPersonID.Text = _PersonID.ToString();
            txtFirstName.Text = _Person.FirstName;
            txtSecondName.Text = _Person.SecondName;
            txtThirdName.Text = _Person.ThirdName;
            txtLastName.Text = _Person.LastName;
            txtNationalNo.Text = _Person.NationalNo;
            dtpDateOfBirth.Value = _Person.DateOfBirth;


            txtAddress.Text = _Person.Address;
            txtPhone.Text = _Person.Phone;
            txtEmail.Text = _Person.Email;

            cbCountry.SelectedIndex = (_Person.CountryInfo.NationalityCountryID - 1);


            rbFemale.Checked = _Person.Gender;

            //string mm = Configu


            //load person image incase it was set.
            if (_Person.ImagePath != "")
            {
                clsUtil.SetPersonImage(ref pbPersonImage, ref _Person);

                llRemoveImage.Visible = pbPersonImage.ImageLocation != null;
            }

        }

        private void _ResetDefualtValues()
        {
            //this will initialize the reset the defaule values
            _FillCountriesInComoboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Person";
                this.Text = "Add New Person";
                _Person = new clsPerson();
            }
            else
            {
                lblTitle.Text = "Update Person";
                this.Text = "Update Person";
            }

            //set default image for the person.
            if (rbMale.Checked)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            //hide/show the remove linke incase there is no image for the person.
            llRemoveImage.Visible = (pbPersonImage.ImageLocation != null);

            //we set the max date to 18 years from today, and set the default value the same.
            dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18);
            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;

            //should not allow adding age more than 100 years
            dtpDateOfBirth.MinDate = DateTime.Now.AddYears(-100);

            //this will set default country to jordan.
            cbCountry.SelectedIndex = cbCountry.FindString("Egypt");

            txtFirstName.Text = "";
            txtSecondName.Text = "";
            txtThirdName.Text = "";
            txtLastName.Text = "";
            txtNationalNo.Text = "";
            rbMale.Checked = true;
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            _Person.FirstName = txtFirstName.Text.Trim();
            _Person.SecondName = txtSecondName.Text.Trim();
            _Person.ThirdName = txtThirdName.Text.Trim();
            _Person.LastName = txtLastName.Text.Trim();
            _Person.NationalNo = txtNationalNo.Text.Trim();
            _Person.Email = txtEmail.Text.Trim();
            _Person.Phone = txtPhone.Text.Trim();
            _Person.Address = txtAddress.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth.Value;

            _Person.Gender = rbFemale.Checked;

            _Person.NationalityCountryID = (cbCountry.SelectedIndex + 1);


            if (clsUtil.HandlePersonImage(ref pbPersonImage, _Person.ImagePath))
            {
                if (pbPersonImage.ImageLocation != null)
                    _Person.ImagePath = clsUtil.GUIDAddress.ToString();
                else
                    _Person.ImagePath = "";
            }



            if (_Person.Save())
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblPersonID.Text = _Person.PersonID.ToString();
                //change form mode to update.
                _Mode = enMode.Update;
                lblTitle.Text = "Update Person";

                // To the caller form.
                DataBack?.Invoke();
                DataBackWithID?.Invoke(_Person.PersonID);
                //SendPersonInfoBack?.Invoke(_Person);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void txtEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Done!

            if (txtEmail.Text.Trim() == "")
                return;

            //validate email format
            if (!clsValidation.ValidateEmail(txtEmail.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "Invalid Email Address Format!");
            }
            else
            {
                errorProvider1.SetError(txtEmail, null);
            }

        }

        private void txtNationalNo_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNationalNo.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNationalNo, "This field is required!");
                return;
            }
            else
            {
                errorProvider1.SetError(txtNationalNo, null);
                e.Cancel = false;
            }

            // Restart the debounce timer without immediate validation
            debounceTimer.Stop();
            debounceTimer.Tag = e; // Optional: keep reference to event args if needed
            debounceTimer.Start();

        }

        private void debounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();

            var eventArgs = (System.ComponentModel.CancelEventArgs)debounceTimer.Tag;
            if ((txtNationalNo.Text.Trim() != _Person.NationalNo) && clsPerson.isPersonExist(txtNationalNo.Text.Trim()))
            {
                eventArgs.Cancel = true;
                errorProvider1.SetError(txtNationalNo, "National Number is used for another person!");
            }
            else
            {

                errorProvider1.SetError(txtNationalNo, null);
                eventArgs.Cancel = false;
            }

        }

        
    }

}
