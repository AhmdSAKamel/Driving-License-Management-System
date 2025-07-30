using Driving_License_Management.PeopleFs;
using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;



namespace Driving_License_Management.UserControls
{

    public partial class ctrlPersonCardWithFilter : UserControl
    {
        public Action PersonDataUpdated;

        public enum FilterBy {ByPersonID = 0, ByNationalNo = 1}

        private FilterBy _filterby;

        public event Action<int> OnPersonSelected;
        protected virtual void PersonSelected(int PersonID)
        {
            Action<int> handler = OnPersonSelected;
            if (handler != null)
            {
                handler(PersonID);
            }
        }


        //private bool _ShowAddPerson = true;
        public bool ShowAddPerson
        {
            get
            {
                return btnAdd.Enabled;
            }
            set
            {
                btnAdd.Enabled = value;
            }
        }

        public bool FilterEnabled
        {
            set
            {
                ctrlFilter1.Enabled = value;
                btnAdd.Enabled = value;
                btnFind.Enabled = value;
            }
            get
            {
                return ctrlFilter1.Enabled;
            }


        }

        public int PersonID
        {
            get { return ctrlPersonDetails1.PersonID; }
        }

        public clsPerson SelectedPersonInfo
        {
            get { return ctrlPersonDetails1.SelectedPersonInfo; }
        }

        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();

            ctrlFilter1.SettxtFilter = true;

            string[] strings = {"PersonID", "NationalNo"};

            ctrlFilter1.FillcbWithData(ref strings, 1);
        }

        public void FilterFocus()
        {
            ctrlFilter1.txtFilterFocus();
        }

        public void LoadPersonInfo(int PersonID, byte SelectedIndex = 0)
        {
            ctrlFilter1.SelectedcbIndex = SelectedIndex;

            ctrlFilter1.txtFilterValue = PersonID.ToString();


            btnAdd.Enabled = false;
            btnFind.Enabled = false;

            FindNow();
        }

        public void LoadPersonInfo(string NationalNo, byte SelectedIndex = 0)
        {
            ctrlFilter1.SelectedcbIndex = SelectedIndex;

            ctrlFilter1.txtFilterValue = NationalNo;

            btnAdd.Enabled = false;
            btnFind.Enabled = false;

            FindNow();
        }

        public void LoadPersonInfo(clsPerson person)
        {
            ctrlFilter1.SelectedcbIndex = 0;

            ctrlFilter1.txtFilterValue = person.PersonID.ToString();

            if (person != null)
            {
                btnAdd.Enabled = false;
                btnFind.Enabled = false;

                ctrlFilter1.Enabled = false;

                ctrlPersonDetails1.LoadPersonInfo(person);
            }
            else
            {
                MessageBox.Show(
                    $"No person found with the specified PersonID: {person.PersonID}. Please verify the ID and try again.",
                    "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }    

        }

        private void btnFind_Click_1(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                // Here we dont continue becuase the form is not valid.
                // MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(ctrlFilter1.txtFilterValue))
            {
                MessageBox.Show("Filter Value is Empty!");
                return;
            }

            FindNow();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (frmAddEditNewPerson addNewPerson = new frmAddEditNewPerson())
            {
                addNewPerson.DataBackWithID += DataBackEvent;

                addNewPerson.ShowDialog();
            }
        }

        private void FindNow()
        {
            _filterby = (FilterBy)ctrlFilter1.SelectedcbIndex;

            switch (_filterby)
            {

                case FilterBy.ByPersonID:
                    {
                        if (int.TryParse(ctrlFilter1.txtFilterValue, out int m))
                        {
                            ctrlPersonDetails1.LoadPersonInfo(m);
                        }

                        break;
                    }

                case FilterBy.ByNationalNo:
                    {
                        ctrlPersonDetails1.LoadPersonInfo(ctrlFilter1.txtFilterValue);

                        break;
                    }

                default:
                    break;

            }

            if (OnPersonSelected != null && FilterEnabled)
                // Raise the event with a parameter
                OnPersonSelected(ctrlPersonDetails1.PersonID);

        }

        private void DataBackEvent(int PersonID)
        {
            ctrlFilter1.txtFilterValue = PersonID.ToString();
            
            ctrlPersonDetails1.LoadPersonInfo(PersonID);

            ctrlFilter1.Enabled = false;
            btnAdd.Enabled = false;
            btnFind.Enabled = false;

            PersonDataUpdated?.Invoke();
        }

        private void ctrlPersonDetails1_OnLinklableClick(int obj)
        {
            if (ctrlPersonDetails1.isUserControlEmpty())
            {
                MessageBox.Show(
                    "No user data is available for adjustment. Please ensure valid user data is provided before attempting to modify it.",
                    "User Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return;
            }
            
            using (frmAddEditNewPerson EditPerson = new frmAddEditNewPerson(ctrlPersonDetails1.PersonID))
            {
                EditPerson.DataBackWithID += DataBackEvent;
                EditPerson.ShowDialog();
            }

        }

        private void ctrlFilter1_OncbChosen()
        {
            _filterby = (FilterBy)ctrlFilter1.SelectedcbIndex;

            if (_filterby == FilterBy.ByPersonID)
            {
                ctrlFilter1.isFilterByIntegerType = true;
                ctrlFilter1.txtFilterValue = "";
            }
            else
            {
                ctrlFilter1.isFilterByIntegerType = false;
            }

        }




    }


}