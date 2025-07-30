using Driving_License_Management.Applications;
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

namespace Driving_License_Management.Forms
{
    public partial class frmApplicationTypes : Form
    {

        public frmApplicationTypes()
        {
            InitializeComponent();

            RefreshForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshForm()
        {
            DataTable dtTable = new DataTable();

            dtTable = clsApplicationType.GetAllApplicationsTypes();

            dataGridView1.DataSource = dtTable.DefaultView;

            dtTable.DefaultView.Sort = "[ApplicationTypeID]";

            lblRecords.Text = dtTable.Rows.Count.ToString();
        } 

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdd_EditApplicationType editApplicationType = new frmAdd_EditApplicationType((int)dataGridView1.CurrentRow.Cells[0].Value);

            editApplicationType.DataUpdated += RefreshForm;

            editApplicationType.ShowDialog();
        }

    }

}
