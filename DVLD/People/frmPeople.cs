using System;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Driving_License_Management.PeopleFs;
using DVLDBuiness;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Driving_License_Management
{
    public partial class frmPeople : Form
    {

        // A way to select the columns that we want to show in the grid.
        //private DataTable _dtPeople = _dtAllPeople.DefaultView.ToTable(false, "PersonID", "NationalNo",
        //                                               "FirstName", "SecondName", "ThirdName", "LastName",
        //                                               "GendorCaption", "DateOfBirth", "CountryName",
        //                                               "Phone", "Email");

        //dgvPeople.Columns[0].HeaderText = "Person ID";     To customize the datagridview
        //dgvPeople.Columns[0].Width = 110;


        //private string _txtFilterOption = null;

        private DataTable _peopledt;

        public enum FilterChoice
        {
            None = 0,
            byPersonID,
            byNationalNo,
            byFirstname,
            bySecondName,
            byThirdName,
            byLastName,
            byDateOfBirth,
            byNationality,
            byPhone,
            byEmail,


            //byGender = 7,
            //byAddress = 9,
            //byImagePath = 13
        }
        private FilterChoice _filterChoice; // Use an instance variable

        public frmPeople()
        {
            InitializeComponent();
        }

        private void frmPeople_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _peopledt.Dispose();
            this.Close();
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Feature is Not implemented yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Feature is Not implemented yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (frmShowDetails frmDetails = new frmShowDetails((int)dataGridView1.CurrentRow.Cells[0].Value))
            {
                frmDetails.DataBack += RefreshForm;

                frmDetails.ShowDialog();
            }

        }
        
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAddEditNewPerson frmAddNew = new frmAddEditNewPerson((int)dataGridView1.CurrentRow.Cells[0].Value))
            {
                frmAddNew.DataBack += RefreshForm;

                frmAddNew.ShowDialog();
            }

        }

        private void ctrlFilter1_OncbChosen()
        {
            _filterChoice = (FilterChoice)ctrlFilter1.SelectedcbIndex;

            if (_filterChoice == FilterChoice.None)
            {
                ctrlFilter1.SetFilters = false;
            }
            else
            {
                ctrlFilter1.SettxtFilter = true;
            }

            if (_filterChoice == FilterChoice.byPersonID)
            {
                ctrlFilter1.SettxtFilter = true;
                ctrlFilter1.isFilterByIntegerType = true;
                ctrlFilter1.txtFilterValue = "";
            }
            else
            {
                ctrlFilter1.isFilterByIntegerType = false;
            }

            PerformFilter();
        }

        private void ctrlFilter1_OnFilterInput()
        {
            PerformFilter();
        }

        private void RefreshForm()
        { 
            //clsCurrent.Countries = clsCountries.GetAllCountries();
            _peopledt = clsPerson.GetAllPeople();

            dataGridView1.DataSource = _peopledt.DefaultView;

            lblTotalRecords.Text = _peopledt.Rows.Count.ToString();

            ctrlFilter1.FillcbWithData(ref _peopledt, "None");
        }
        
        private void btnAddNewPeople_Click(object sender, EventArgs e)
        {
            //StandardDT.DefaultView.RowFilter = null;
            ctrlFilter1.txtFilterValue = "";

            frmAddEditNewPerson frmAddNew = new frmAddEditNewPerson();

            frmAddNew.DataBack += RefreshForm;    // When happen, DataGrid view will update.

            frmAddNew.ShowDialog();

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to delete Person [" + (int)dataGridView1.CurrentRow.Cells[0].Value + "]",
                "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK) 
            {
                //Perform Deleletion and refresh.
                if (clsPerson.DeletePerson((int)dataGridView1.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show("User Deleted Successfully.");
                    RefreshForm();
                    //ctrlFilter1.ClearFilterString();
                }
                else
                    MessageBox.Show("Person is NOT deleted.");

            }
        }
         
        private void PerformFilter()
        {
            // If no filter option is provided, clear the filter and return.
            if (string.IsNullOrWhiteSpace(ctrlFilter1.txtFilterValue) || _filterChoice == FilterChoice.None)
            {
                _peopledt.DefaultView.RowFilter = null;
                return;
            }

            StringBuilder filterString = new StringBuilder("");

            // Specific handling for filtering by Date of Birth.
            if (_filterChoice == FilterChoice.byDateOfBirth)
            {

                if (DateTime.TryParse(ctrlFilter1.txtFilterValue, out DateTime dob))
                {
                    // Format date to "yyyy-MM-dd" and build the RowFilter string.
                    filterString = new StringBuilder($"[Date Of Birth] = '{dob:yyyy-MM-dd}'");
                }
                else
                {
                    //filterString = null;
                }

            }
            else
            {
                // For non-date filters, rely on the helper method for constructing the filter string.
                filterString = new StringBuilder(ctrlFilter1.StringToExecuteFilter());
            }


            try
            {
                _peopledt.DefaultView.RowFilter = filterString.ToString();
            }
            catch (EvaluateException ex)
            {
                MessageBox.Show($"Invalid search pattern. Please revise your input. {ex.Message}", 
                    "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }


    }

}
