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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;



namespace Driving_License_Management.UserControls
{
    public partial class ctrlFilter : UserControl
    {

        public event Action OncbChosen;
        protected virtual void cbProcess()
        {
            Action handler = OncbChosen;

            if (handler != null)
            {
                OncbChosen();
            }

        }


        public event Action OnFilterInput;
        protected virtual void FilterProcess()
        {
            Action handler = OnFilterInput;

            if (handler != null)
            {
                OnFilterInput();
            }

        }


        public event Action OnFilterInputByCB;
        protected virtual void CBFilterProcess()
        {
            Action handler = OnFilterInputByCB;

            if (handler != null)
            {
                //OnFilterInput.Invoke(FilterString);
                OnFilterInputByCB();
            }

        }


        public ctrlFilter()
        {
            InitializeComponent();
        }

        public int SelectedcbIndex
        {            
            set { cbDataOptions.SelectedIndex = value; }

            get { return cbDataOptions.SelectedIndex; }
        }

        public string cbChosenString
        {
            get { return cbDataOptions.SelectedItem.ToString(); }
        }

        public string txtFilterValue
        {
            get { return txtFilter.Text; }
            set { txtFilter.Text = value; }
        }

        public int SelectedCBFilterIndex
        {
            get { return comboBox1.SelectedIndex; }
        }

        public string CBFilterString
        {
            get { return comboBox1.SelectedItem.ToString(); }
        }

        public bool CBFilter
        {
            get { return comboBox1.Visible; }
            set
            {
                comboBox1.Visible = value;
                comboBox1.Enabled = value;

                txtFilter.Enabled = !value;
                txtFilter.Visible = !value;
            }

        }

        public bool SetFilters
        {
            set
            {
                if (txtFilter.Enabled || comboBox1.Enabled)
                {
                    txtFilter.Enabled = value;
                    txtFilter.Visible = value;

                    comboBox1.Visible = value;
                    comboBox1.Enabled = value;
                    comboBox1.Items.Clear();

                    //comboBox1.Items.Clear();

                    if (!value)
                    {
                        txtFilter.Text = "";
                    }

                }

            }

            get
            {
                return (txtFilter.Enabled || comboBox1.Enabled);
            }
        }

        public bool SettxtFilter
        {
            set { txtFilter.Enabled = value; txtFilter.Visible = value; }

            get { return txtFilter.Enabled; }
        }

        public bool SetCBFilter
        {
            set { comboBox1.Enabled = value; comboBox1.Visible = value; }
        }

        public bool EnabletxtFilter => cbDataOptions.SelectedIndex > 0;

      
        /*The property is defined to return the result of the expression cbDataOptions.SelectedIndex > 0. 
          This syntax was introduced in C# 6 to simplify read-only properties and similar members. */


        public bool isFilterByIntegerType = false;
        public bool isFilterByCB = false;
        public bool DidDataChange = false;


        public void txtFilterFocus()
        {
            txtFilter.Focus();
        }

        public void FillcbWithData(ref DataTable db, string ItemAtStart = "", byte SelectedIndex = 0, string itemToIgnore = "")
        {
            // Check if the DataTable is null.
            if (db == null)
                return;

            // Clear existing items in the ComboBox  
            cbDataOptions.Items.Clear();

            if (!string.IsNullOrWhiteSpace(ItemAtStart))
            {
                cbDataOptions.Items.Add(ItemAtStart);
            }

            // Add each column name to the ComboBox
            foreach (DataColumn col in db.Columns)
            {
                if (!string.Equals(col.ColumnName, itemToIgnore, StringComparison.OrdinalIgnoreCase))
                {
                    cbDataOptions.Items.Add(col.ColumnName);
                }
            }

            cbDataOptions.SelectedIndex = 0;
        }

        public void FillcbWithData(ref string[] arrString, short selectedIndex = 0)
        {
            foreach (string ss in arrString)
            {
                cbDataOptions.Items.Add(ss);
            }

            cbDataOptions.SelectedIndex = selectedIndex;

        }

        public void FillcbWithData(ref DataView dtview)
        {
            if (dtview == null)
                return;

            cbDataOptions.Items.Clear();

            cbDataOptions.Items.Add("None");
            cbDataOptions.SelectedIndex = 0;

            foreach (DataColumn col in dtview.Table.Columns)
            {
                cbDataOptions.Items.Add(col.ColumnName);
            }

        }

        private void cbDataOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            DidDataChange = true;
            cbProcess();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            DidDataChange = true;
            FilterProcess();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DidDataChange = true;
            CBFilterProcess();
        }

        public void TurntxtFilterInputIntoCB(string[] CBOptions, short selectedIndex = 0)
        {
            SetFilters = false;

            comboBox1.Visible = true;
            comboBox1.Enabled = true;

            comboBox1.Items.Clear();

            foreach (string m in CBOptions)
            {
                comboBox1.Items.Add(m);
            }

            comboBox1.SelectedIndex = selectedIndex;
        }

        public void TurntxtFilterInputIntoCB(ref DataTable dt, short selectedIndex = 0, string atbeggining = "")
        {
            if (dt == null)
                return;

            SetFilters = false;

            comboBox1.Visible = true;
            comboBox1.Enabled = true;

            comboBox1.Items.Clear();

            if (!string.IsNullOrEmpty(atbeggining))
                comboBox1.Items.Add(atbeggining);

            foreach (DataRow row in dt.Rows)
            {
                comboBox1.Items.Add(row["ClassName"].ToString());
            }

            comboBox1.SelectedIndex = selectedIndex;
        }

        private void txtFilter_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (isFilterByIntegerType)
            {
                // Allow only digits and control characters (e.g., backspace)  
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // Reject the input  
                }
                else
                    DidDataChange = true;
            }

        }

        public string StringToExecuteFilter()
        {
            // Trim the input value (assumed coming from a TextBox) to avoid leading/trailing spaces.
            StringBuilder filterValue = new StringBuilder(txtFilterValue.Trim());

            // Build the filter condition based on the type of filtering.
            // For integer filtering, no quotes are used; for string filtering, use quotes and a wildcard.
            return isFilterByIntegerType
                ? $"[{cbChosenString}] = {filterValue}"
                : $"[{cbChosenString}] LIKE '{filterValue}%'";
        }
        
    }

}

