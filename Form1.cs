using System;
using System.Data;
using System.Windows.Forms;

namespace SyphilisRegister
{
    public partial class Form1 : Form
    {
        int RecordID;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'syphilisDBDataSet.Test' table. You can move, or remove it, as needed.
            this.patientTableAdapter.Fill(this.syphilisDBDataSet.Patient);
        }

        private DataRow LastDataRow = null;

        private void dgvPatients_SelectionChanged(object sender, EventArgs e)
        {

            BindingSource pBindingSource = (BindingSource)sender;
            DataRow thisRow = ((DataRowView)pBindingSource.Current).Row;
            
            this.testTableAdapter.GetTestData()
        }

        private void SaveData()
        {
            if (LastDataRow != null)
            {
                if (LastDataRow.RowState != DataRowState.Unchanged)
                {
                    patientTableAdapter.Update(LastDataRow);
                }
            }
        }

        private void patientBindingSource_PositionChanged(object sender, EventArgs e)
        {
            BindingSource pBindingSource = (BindingSource)sender;
            DataRow thisRow = ((DataRowView)pBindingSource.Current).Row;

            if (thisRow == LastDataRow)
            {
                throw new ApplicationException("It seems the" +
                " PositionChanged event was fired twice for" +
                " the same row");
            }

            SaveData();
            LastDataRow = thisRow;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveData();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtFilter.Text, out int query))
            {
                patientBindingSource.Filter = "[Docket No] = " + query; 
            }
            else
            {
                string q = "'%" + txtFilter.Text + "%'";
                patientBindingSource.Filter = "[First Name] like " + q + " OR [Last Name] like " + q;
            }
        }
    }
}
