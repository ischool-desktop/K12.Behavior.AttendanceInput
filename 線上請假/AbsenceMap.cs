using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K12.Data;
namespace K12.Behavior.AttendanceInput
{
    public partial class AbsenceMap : BaseForm
    {
        private FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();
        List<K12.Data.AbsenceMappingInfo> amil;
        private List<AbsenceMapRecord> _MapRecords = new List<AbsenceMapRecord>();
        public AbsenceMap()
        {
            InitializeComponent();
            this.Text = "";
            amil = K12.Data.AbsenceMapping.SelectAll();
            _MapRecords = _AccessHelper.Select<AbsenceMapRecord>();
            Dictionary<string,AbsenceMapRecord> Damr = new Dictionary<string,AbsenceMapRecord>();
            try
            {
                Damr = _MapRecords.ToDictionary(x => x.absence, x => x);
            }
            catch (Exception)
            {
                throw;
            }
            foreach (K12.Data.AbsenceMappingInfo item in amil)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = item.Name;
                row.Cells[1].Value = Damr.ContainsKey(item.Name) ? Damr[item.Name].allow1 == "1" : false;
                row.Cells[2].Value = Damr.ContainsKey(item.Name) ? Damr[item.Name].allow2 == "1" : false;
                dataGridViewX1.Rows.Add(row);
            }
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            _AccessHelper.DeletedValues(_MapRecords);
            _MapRecords.Clear();
            AbsenceMapRecord amr;
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                amr = new AbsenceMapRecord();
                amr.absence = "" + row.Cells[0].Value;
                amr.allow1 = (bool)row.Cells[1].Value == true ? "1" : "0";
                amr.allow2 = (bool)row.Cells[2].Value == true ? "1" : "0";
                _MapRecords.Add(amr);
            }
            _MapRecords.SaveAll();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
