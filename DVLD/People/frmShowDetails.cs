using Driving_License_Management.UserControls;
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


namespace Driving_License_Management.PeopleFs
{
    public partial class frmShowDetails : Form
    {
        public Action DataBack;

        private int _PersonID;
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Refreshdgv()
        {
            DataBack?.Invoke();
        }

        public frmShowDetails(int PersonID) //DataView DV)
        {
            InitializeComponent();

            _PersonID = PersonID;

            ctrlPersonDetails1.LoadPersonInfo(_PersonID);
        }

        private void ctrlPersonDetails1_OnLinklableClick(int obj)
        {
            using (frmAddEditNewPerson frmEdit = new frmAddEditNewPerson(_PersonID))
            {
                frmEdit.DataBack += action;

                frmEdit.ShowDialog();
            }

        }

        public void action()
        {
            ctrlPersonDetails1.LoadPersonInfo(_PersonID);

            Refreshdgv();
        }     

    }

}
