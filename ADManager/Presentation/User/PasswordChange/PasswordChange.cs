using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADManager
{
    public partial class PasswordChange : Form
    {

        // Kullanıcı Parola değiştirme işlemini yapan form
        private string samAccountName;
        private BLUser blUser;
        public PasswordChange(string samAccountName )
        {
            this.samAccountName = samAccountName;
            blUser = new BLUser();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
         MessageBox.Show(  blUser.ResetUserPassword(samAccountName, newPassTxt.Text));
        }
    }
}
