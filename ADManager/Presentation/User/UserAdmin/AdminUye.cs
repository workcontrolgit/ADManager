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
    public partial class AdminUye : Form
    {

        private string samAccountName { get; set; }
      

        public  AdminUye(string samAccountName)
        {
            this.samAccountName = samAccountName;
            InitializeComponent();
        }

        private void TekSecBtn_Click(object sender, EventArgs e)
        {
            TekAktar();
        }

        public void TekAktar()
        {
            listBox3.Items.Add(listBox2.SelectedItem.ToString());


        }

        public void HepsiniAktar()
        {
            foreach(var group in listBox2.Items)

                listBox3.Items.Add(group);
        }

        private void HepsiSecBtn_Click(object sender, EventArgs e)
        {
            HepsiniAktar();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
        }

        private void KydtBtn_Click(object sender, EventArgs e)
        {
            AddUserToAdminGroup();
        }

        public void AddUserToAdminGroup()
        {
            if (listBox3.Items.Count != 0)
            {
                string groupName = listBox3.Items[0].ToString();
                BLUser blUser = new BLUser();
                MessageBox.Show(blUser.AddUserToAdminGroup(samAccountName, groupName));

            }

            else
                MessageBox.Show("Grup Seçilmedi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); 

                


        }

        private void AdminUye_Load(object sender, EventArgs e)
        {

        }
    }
}
