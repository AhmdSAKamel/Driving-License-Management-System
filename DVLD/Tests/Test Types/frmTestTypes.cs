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

namespace Driving_License_Management.TestsFs
{
    public partial class frmTestTypes : Form
    {

        DataView dtView;

        public frmTestTypes()
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
            dtView = clsTestType.GetAllTestTypes().DefaultView;

            dataGridView1.DataSource = dtView;

            lblRecords.Text = dtView.Count.ToString();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int testtypeid = (int)(dataGridView1.CurrentRow.Cells[0].Value);

            using (frmEditTestType frmEdit = new frmEditTestType((byte)testtypeid))
            {
                frmEdit.DataUpdated += RefreshForm;

                frmEdit.ShowDialog();
            }

        }

    }

}
