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
using System.Xml.Linq;
namespace K12.Behavior.AttendanceInput
{
    public partial class RequestDetail : BaseForm
    {
        public RequestDetail(AttendanceRequestRecord arr)
        {
            InitializeComponent();
            this.Text = "請假明細";
            StudentRecord sr = K12.Data.Student.SelectByID(arr.RefStudentId);
            if (sr != null)
            {
                if (sr.Class != null)
                    textBoxX1.Text = sr.Class.Name;
                textBoxX3.Text = "" + sr.SeatNo;
                textBoxX5.Text = sr.Name;
                textBoxX7.Text = sr.StudentNumber;
            }
            else
                textBoxX5.Text = arr.RefStudentId;
            if ( arr.MapStudentHasError )
                errorProvider1.SetError(textBoxX5, "無對應學生");
            textBoxX2.Text = arr.AttOccurDate;
            if ( arr.OccurDateHasError )
                errorProvider1.SetError(textBoxX2, "日期格式錯誤");
            textBoxX4.Text = arr.AttPeriod;
            if (arr.PeriodHasError)
                errorProvider1.SetError(textBoxX4, "節次格式錯誤");
            textBoxX6.Text = arr.AttAbsenceType;
            if (arr.AbsenceTypeHasError)
                errorProvider1.SetError(textBoxX6, "假別格式錯誤");
            textBoxX8.Text = arr.AttReason;

            //SignFlow
            XElement SignFlow;
            try
            {
                SignFlow = XElement.Parse(arr.SignFlow);
                foreach (XElement xe in SignFlow.Element("Stages").Elements("Stage"))
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridViewX1);
                    row.Cells[0].Value = xe.Element("ApprovedBy").Value;
                    row.Cells[1].Value = (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(double.Parse(xe.Element("ApprovedTimestamp").Value) / 1000d).ToLocalTime();
                    row.Cells[2].Value = xe.Element("Message").Value;
                    dataGridViewX1.Rows.Add(row);
                }
            }
            catch (Exception)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells[0].Value = arr.SignFlow;
                dataGridViewX1.Rows.Add(row);
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
