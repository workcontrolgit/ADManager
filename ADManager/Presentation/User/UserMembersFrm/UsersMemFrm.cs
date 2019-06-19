﻿using System;
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
    public partial class UsersMemFrm : Form
    {
        public UsersMemFrm()
        {
            InitializeComponent();
        }


        public void PrintGroups(IEnumerable<string> userGroupLists)
        {
            IEnumerable<string> _userGroupList = userGroupLists;

            foreach (var grp in _userGroupList)
            {
               GroupList.Items.Add(grp);
            }


        }

     
    }
}
