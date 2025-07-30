using Driving_License_Management.PeopleFs;
using DVLDBuiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Driving_License_Management.UserFs
{
    public partial class frmUsers : Form
    {
        
        private DataTable _Usersdt;
        StringBuilder filterString;// = new StringBuilder();
        string[] strings = { "All", "Yes", "No" };


        public enum FilterChoice
        {
            None = 0,
            byUserID = 1,
            byPersonID = 2,
            byFullName = 3,
            byUserName = 4,
            byActiveUsers = 5,
        }
        private FilterChoice _filterChoice;

        public enum FilterCBChoice
        {
            All = 0,
            Yes = 1,
            No = 2
        }
        private FilterCBChoice _filterCBChoice = new FilterCBChoice();

        public frmUsers()
        {
            InitializeComponent();

            RefreshForm();
        }

        private void SetlblRecordCount()
        {
            lblRecords.Text = _Usersdt.DefaultView.Count.ToString();
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Feature is Not implemented yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Feature is Not implemented yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _Usersdt.Dispose();     //If you want to release large memory, you could call _Usersdt.Dispose() when you're done with it.

            this.Close();
        }

        private void ctrlFilter1_OnFilterInputByCB()
        {
            _filterCBChoice = (FilterCBChoice)ctrlFilter1.SelectedCBFilterIndex;

            
            PerformFilter();
        }

        private void ctrlFilter1_OnFilterInput()
        {
            PerformFilter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (frmAddUpdateUser addNewUser = new frmAddUpdateUser())
            {
                addNewUser.RefreshData += RefreshForm;

                addNewUser.ShowDialog();
            }

        }

        private void RefreshForm()
        {
            _Usersdt = clsUser.GetAllUsers();

            dataGridView1.DataSource = _Usersdt.DefaultView;

            SetlblRecordCount();

            ctrlFilter1.FillcbWithData(ref _Usersdt, "None");
        }

        private void ctrlFilter1_OncbChosen()
        {

            _filterChoice = (FilterChoice)ctrlFilter1.SelectedcbIndex;

            if (_filterChoice == FilterChoice.None)
            {
                ctrlFilter1.SetFilters = false;
            }

            if (_filterChoice == FilterChoice.byActiveUsers)
            {
                _Usersdt.DefaultView.RowFilter = null;
                ctrlFilter1.isFilterByCB = true;
                ctrlFilter1.txtFilterValue = "";
                ctrlFilter1.TurntxtFilterInputIntoCB(strings);
            }
            else
            {
                ctrlFilter1.isFilterByCB = false;
                ctrlFilter1.SetCBFilter = false;
            }

            if (_filterChoice == FilterChoice.byPersonID || _filterChoice == FilterChoice.byUserID)
            {
                ctrlFilter1.isFilterByIntegerType = true;
                ctrlFilter1.txtFilterValue = "";
            }
            else
            {
                ctrlFilter1.isFilterByIntegerType = false;
            }

            ctrlFilter1.SettxtFilter = (_filterChoice != FilterChoice.None && _filterChoice != FilterChoice.byActiveUsers);


            PerformFilter();
        }

        private void PerformFilter()
        {

            if (!ctrlFilter1.isFilterByCB)
            {
                if (string.IsNullOrWhiteSpace(ctrlFilter1.txtFilterValue) || _filterChoice == FilterChoice.None)
                {
                    _Usersdt.DefaultView.RowFilter = null;
                    SetlblRecordCount();
                    return;
                }
            }


            if (ctrlFilter1.isFilterByCB)
            {
                switch (_filterCBChoice)
                {
                    case FilterCBChoice.All:
                        {
                            filterString = new StringBuilder(null);
                            //_Usersdt.DefaultView.RowFilter = null;
                            break;
                        }
                    case FilterCBChoice.Yes:
                        {
                            filterString = new StringBuilder("[isActive] = 'True'");
                            break;
                        }
                    case FilterCBChoice.No:
                        {
                            filterString = new StringBuilder("[isActive] = 'False'");
                            break;
                        }
                    default:
                        {
                            return;
                        }

                }

            }
            else
            {
                filterString = new StringBuilder(ctrlFilter1.StringToExecuteFilter());
            }

            // Apply the filter string for other conditions.

            try
            {
                _Usersdt.DefaultView.RowFilter = filterString.ToString();
            }
            catch (EvaluateException ex)
            {
                MessageBox.Show($"Invalid search pattern. Please revise your input. {ex.Message}",
                                "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetlblRecordCount();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmShowUserDetails showDetails = new frmShowUserDetails((int)dataGridView1.CurrentRow.Cells[1].Value))
            {
                showDetails.ShowDialog();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAddUpdateUser addNewUser = new frmAddUpdateUser((short)dataGridView1.CurrentRow.Cells[0].Value))
            {
                addNewUser.RefreshData += RefreshForm;

                addNewUser.ShowDialog();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to delete User [" + (int)dataGridView1.CurrentRow.Cells[0].Value + "]",
                "Confirm Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                //Perform Deleletion and refresh.
                if (clsUser.DeleteUser((int)dataGridView1.CurrentRow.Cells[0].Value))
                {          
                    MessageBox.Show("User has been Deleted Successfully.");
                    RefreshForm();
                }
                else
                    MessageBox.Show("User is NOT deleted.");

            }

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmChangeUserPassword changeUserPassword = new frmChangeUserPassword((int)dataGridView1.CurrentRow.Cells[1].Value);

            changeUserPassword.Yes += RefreshForm;

            changeUserPassword.ShowDialog();
        }

    }

}
