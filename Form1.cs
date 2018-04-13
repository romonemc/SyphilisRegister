using System;
using System.Data;
using System.Windows.Forms;

namespace SyphilisRegister
{
    public partial class Form1 : Form
    {
        int RID;
        int DNO;
        string Parish;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Login lgnfrm = new Login();
            lgnfrm.isLoggedIn = false;
            lgnfrm.ShowDialog();

            lblParish.Text = lgnfrm.Parish;

            if (!lgnfrm.isLoggedIn)
            {
                this.Close();
                Application.Exit();
                return;
            }

            this.patientTableAdapter.Fill(this.syphilisDBDataSet.Patient);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveGridData("Patient");
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

        private void dgvPatients_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != dgvPatients.NewRowIndex)
            {
                RID = Convert.ToInt32(dgvPatients.SelectedRows[0].Cells["RecordID"].Value);
                DNO = Convert.ToInt32(dgvPatients.SelectedRows[0].Cells["DocketNo"].Value);

                this.testTableAdapter.FillWithTestData(this.syphilisDBDataSet.Test, RID);
                this.treatment_DateTableAdapter.FillWithTreatmentDates(syphilisDBDataSet.Treatment_Date, RID);
                this.post_Treatment_TrustTableAdapter.FillWithPOSTData(syphilisDBDataSet.Post_Treatment_Trust, RID);
            }
        }

        // ----------------------------------------------------------------------------------

        private DataRow LastPatientRow = null;
        private DataRow LastTestRow = null;
        private DataRow LastTreatmentRow = null;
        private DataRow LastPOSTRow = null;

        private void SaveGridData(string Grid)
        {
            try
            {
                switch (Grid)
                {
                    case "Patient":
                        patientTableAdapter.Update(LastPatientRow);
                        break;
                    case "Tests":
                        testTableAdapter.Update(LastTestRow);
                        break;
                    case "Treatment":
                        treatment_DateTableAdapter.Update(LastTreatmentRow);
                        break;
                    case "POST Treatment":
                        post_Treatment_TrustTableAdapter.Update(LastPOSTRow);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot save record, please try again or contact your system administrator." + Environment.NewLine + "Details: " + ex.Message, "Error saving record", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void patientBindingSource_PositionChanged(object sender, EventArgs e)
        {
            BindingSource pBindingSource = (BindingSource)sender;
            DataRow thisRow = ((DataRowView)pBindingSource.Current).Row;

            if (thisRow == LastPatientRow)
            {
                return;
            }

            if (LastPatientRow != null)
            {
                if (LastPatientRow.RowState != DataRowState.Unchanged)
                {
                    SaveGridData("Patient");
                }
            }

            LastPatientRow = thisRow;

            dgvTests.Enabled = true;
            dgvTreatmentDates.Enabled = true;
            dgvPostTreatmentTrust.Enabled = true;
        }

        private void testBindingSource_PositionChanged(object sender, EventArgs e)
        {
            BindingSource tBindingSource = (BindingSource)sender;
            DataRow thisRow = ((DataRowView)tBindingSource.Current).Row;

            if (thisRow == LastTestRow)
            {
                return;
            }

            if (LastTestRow != null)
            {
                if (LastTestRow.RowState != DataRowState.Unchanged)
                {
                    SaveGridData("Tests");
                }
            }

            LastTestRow = thisRow;
        }

        private void treatment_DateBindingSource_PositionChanged(object sender, EventArgs e)
        {
            BindingSource treatBindingSource = (BindingSource)sender;
            DataRow thisRow = ((DataRowView)treatBindingSource.Current).Row;

            if (thisRow == LastTreatmentRow)
            {
                return;
            }

            if (LastTreatmentRow != null)
            {
                if (LastTreatmentRow.RowState != DataRowState.Unchanged)
                {
                    SaveGridData("Treatment");
                }
            }

            LastTreatmentRow = thisRow;
        }

        private void post_Treatment_TrustBindingSource_PositionChanged(object sender, EventArgs e)
        {
            BindingSource postBindingSource = (BindingSource)sender;
            DataRow thisRow = ((DataRowView)postBindingSource.Current).Row;

            if (thisRow == LastPOSTRow)
            {
                return;
            }

            if (LastPOSTRow != null)
            {
                if (LastPOSTRow.RowState != DataRowState.Unchanged)
                {
                    SaveGridData("POST Treatment");
                }
            }

            LastPOSTRow = thisRow;
        }

        //--------------------------------------------------------------------------------------------

        // Populate default values of RecordID and Docket No for each datagridviews

        private void dgvTests_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            DataGridView dgvs = (DataGridView)sender;

            if (dgvs.Name == "dgvTests")
            {
                e.Row.Cells["TestRecordID"].Value = RID;
                e.Row.Cells["TestDocketNo"].Value = DNO;

                if (e.Row.IsNewRow)
                {
                    e.Row.Cells["DateTested"].Value = DateTime.Today.ToShortDateString();
                }
            }
            else if (dgvs.Name == "dgvTreatmentDates")
            {
                e.Row.Cells["TreatmentRecordNum"].Value = RID;
                e.Row.Cells["TreatmentDocketNo"].Value = DNO;

                if (e.Row.IsNewRow)
                {
                    e.Row.Cells["TreatmentDate"].Value = DateTime.Today.ToShortDateString();
                }
            }
            else if (dgvs.Name == "dgvPostTreatmentTrust")
            {
                e.Row.Cells["POSTRecordID"].Value = RID;
                e.Row.Cells["POSTDocketNo"].Value = DNO;

                if (e.Row.IsNewRow)
                {
                    e.Row.Cells["POSTDate"].Value = DateTime.Today.ToShortDateString();
                }
            }
        }

        private void dgvPatients_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView grids = (DataGridView)sender;

            switch (grids.Name)
            {
                case "dgvPatients":
                    dgvTests.Enabled = false;
                    dgvTreatmentDates.Enabled = false;
                    dgvPostTreatmentTrust.Enabled = false;
                    break;
                case "dgvTests":
                    dgvPatients.Enabled = false;
                    dgvTreatmentDates.Enabled = false;
                    dgvPostTreatmentTrust.Enabled = false;
                    break;
                case "dgvTreatmentDates":
                    dgvTests.Enabled = false;
                    dgvPatients.Enabled = false;
                    dgvPostTreatmentTrust.Enabled = false;
                    break;
                case "dgvPostTreatmentTrust":
                    dgvTests.Enabled = false;
                    dgvTreatmentDates.Enabled = false;
                    dgvPatients.Enabled = false;
                    break;
            }
        }

        private void dgvPatients_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvPatients.Enabled = true;
            dgvTests.Enabled = true;
            dgvTreatmentDates.Enabled = true;
            dgvPostTreatmentTrust.Enabled = true;
        }

        private void dgvTests_Enter(object sender, EventArgs e)
        {
            if (dgvPatients.SelectedRows[0].IsNewRow || dgvPatients.SelectedRows[0].Cells[0].Value is DBNull)
            {
                MessageBox.Show("Please select an existing patient first.", "Select Patient First", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}