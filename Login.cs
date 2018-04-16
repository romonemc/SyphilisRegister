using System;
using System.Windows.Forms;

namespace SyphilisRegister
{
    public partial class Login : Form
    {
        private bool adminMode = false;
        internal bool isLoggedIn { get; set; }
        internal string Parish { get; set; }

        private int tries = 0;

        public Login()
        {
            InitializeComponent();
        }

        private class Settings
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (adminMode)
            {
                //Settings.ShowDialog();
                return;
            }

            if (tries < 5)
            {
                //if (!System.Text.RegularExpressions.Regex.IsMatch(cbxParish.Text, "^[A-Za-z0-9]+$") || !System.Text.RegularExpressions.Regex.IsMatch(txtPassword.Text, "^[A-Za-z0-9]+$"))
                //{
                //    MessageBox.Show("Username or password is invalid. Please try again.", "Invalid username or password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                string encuser = AesCryp.Encrypt(cbxParish.Text);
                string enpass = AesCryp.Encrypt(txtPassword.Text);

                if (UsersTableAdapter.ValidateUser(encuser, enpass) == 1)
                {
                    this.Parish = cbxParish.Text;
                    isLoggedIn = true;
                    this.Close();
                }
                else
                {
                    txtPassword.Clear();
                    MessageBox.Show("Authentication failed, please check your credentials and try again.", "Login Failed - " + (5 - tries) + " tries remaining", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            cbxParish.SelectedIndex = 0;
        }
    }
}
