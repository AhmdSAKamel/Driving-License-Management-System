using System;
using System.Windows.Forms;







namespace Driving_License_Management.Licenses
{
    public partial class frmShowPersonLicenseHistory : Form
    {
        int _PersonID = -1;
        string _NationalNo = "";

        public Action InformationUpdated;

        private enum toFindBy { PersonID, NationalNo }

        private toFindBy _FindBy;

        public frmShowPersonLicenseHistory()
        {
            InitializeComponent();
        }

        public frmShowPersonLicenseHistory(int PersonID)
        {
            InitializeComponent();

            _PersonID = PersonID;
            _FindBy = toFindBy.PersonID;
        }

        public frmShowPersonLicenseHistory(string nationalNo)
        {
            InitializeComponent();

            _NationalNo = nationalNo;
            _FindBy = toFindBy.NationalNo;
        }

        private void frmShowPersonLicenseHistory_Load(object sender, EventArgs e)
        {

            switch (_FindBy)
            {
                case toFindBy.PersonID:
                    {
                        if (_PersonID != -1)
                        {
                            ctrlPersonCardWithFilter1.LoadPersonInfo(_PersonID);
                            ctrlPersonCardWithFilter1.FilterEnabled = false;


                            ctrlDriverLicenses1.LoadInfoByPersonID(_PersonID);
                        }
                        else
                        {
                            ctrlPersonCardWithFilter1.Enabled = true;
                            ctrlPersonCardWithFilter1.FilterFocus();
                        }
                        break;
                    }
                case toFindBy.NationalNo:
                    {
                        if (!string.IsNullOrEmpty(_NationalNo)) 
                        {
                            ctrlPersonCardWithFilter1.LoadPersonInfo(_NationalNo, 1);
                            ctrlPersonCardWithFilter1.FilterEnabled = false;


                            ctrlDriverLicenses1.LoadInfoByPersonID(ctrlPersonCardWithFilter1.PersonID);
                        }
                        else
                        {
                            ctrlPersonCardWithFilter1.Enabled = true;
                            ctrlPersonCardWithFilter1.FilterFocus();
                        }


                        break;

                    }

            }

            ctrlPersonCardWithFilter1.PersonDataUpdated += Refreshform;
        }

        private void Refreshform()
        {
            frmShowPersonLicenseHistory_Load(null, null);

            InformationUpdated?.Invoke(); ;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ctrlPersonCardWithFilter1_OnPersonSelected(int obj)
        {
            _PersonID = obj;

            if (_PersonID == -1)
            {
                ctrlDriverLicenses1.Clear();
            }
            else
                ctrlDriverLicenses1.LoadInfoByPersonID(_PersonID);
        }


    }


}
