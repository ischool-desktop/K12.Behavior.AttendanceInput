using FISCA.Presentation.Controls;
using K12.Data;
using K12.Data.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace K12.Behavior.AttendanceInput
{
    public partial class LeaveRule : BaseForm
    {
        ConfigData cd = School.Configuration["線上請假規則設定"];
        public LeaveRule()
        {
            InitializeComponent();
            this.Text = "線上請假規則設定";
            
            textBoxX1.Text = cd["content"];
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            cd["content"] = textBoxX1.Text;
            cd.Save();
            this.Close();
        }
        
    }
}
