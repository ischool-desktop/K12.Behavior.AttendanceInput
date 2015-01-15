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
    public partial class ApprovedResponderConigForm : BaseForm
    {
        bool useUpdate;
        string flowName = "LeaveRequestFlow_001";
        XElement SignFlow;
        public ApprovedResponderConigForm()
        {
            InitializeComponent();
            this.Text = "線上請假核可角色設定";

            DataTable dt = tool._Q.Select("select content from list where name = '" + flowName + "'");
            useUpdate = dt.Rows.Count > 0;
            colRole.Items.Add("班導師");
            colRole.Items.Add("指定老師");

            List<string> tmpTeachers = K12.Data.Teacher.SelectAll().Select(x => x.Name + (string.IsNullOrWhiteSpace(x.Nickname) ? "" : "(" + x.Nickname + ")")).ToList();
            tmpTeachers.Sort();
            colResponder.Items.AddRange(tmpTeachers.ToArray());
            if (useUpdate)
            {
                try
                {
                    SignFlow = XElement.Parse(dt.Rows.Count > 0?"" + dt.Rows[0]["content"]:"");
                    int stage = 0;
                    foreach (XElement xe in SignFlow.Element("Stages").Elements("Stage"))
                    {
                        stage++;
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dataGridViewX1);
                        row.Cells[0].Value = stage;
                        if (xe.Element("ResponderRole") != null)
                        {
                            switch (xe.Element("ResponderRole").Value)
                            {
                                case "HomeTeacher":
                                    row.Cells[1].Value = "班導師";
                                    break;
                                default:
                                    row.Cells[1].Value = "指定老師";
                                    break;
                            }
                        }
                        else
                            row.Cells[1].Value = "指定老師";
                        if (xe.Element("Responder") != null)
                            row.Cells[2].Value = xe.Element("Responder").Value;
                        dataGridViewX1.Rows.Add(row);

                    }
                }
                catch (Exception)
                {
                    MsgBox.Show("設定資料有誤，請重新設定");
                }
            }
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (!useUpdate)
            {
                SignFlow = new XElement("Workflow", new XAttribute("name", flowName),
                         new XElement("Description", "請假流程"), new XElement("Stages")
                    );
            }
            else
            {
                SignFlow.Element("Stages").RemoveNodes();
            }
            XElement xe = SignFlow.Element("Stages");
            Dictionary<string,TeacherRecord> dtr = K12.Data.Teacher.SelectAll().ToDictionary(x=>x.Name+(string.IsNullOrWhiteSpace(x.Nickname)?"":"("+x.Nickname+")"),x=>x );
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                switch ("" + row.Cells[1].Value)
                {
                    case "班導師":
                        xe.Add(new XElement("Stage", new XAttribute("no", row.Cells[0].Value), new XElement("ResponderRole", "HomeTeacher")));
                        break;
                    case "指定老師":
                        if (dtr.ContainsKey("" + row.Cells[2].Value))
                            xe.Add(new XElement("Stage", new XAttribute("no", row.Cells[0].Value), new XElement("Responder", dtr["" + row.Cells[2].Value].TALoginName)));
                        else
                            xe.Add(new XElement("Stage", new XAttribute("no", row.Cells[0].Value), new XElement("Responder", "" + row.Cells[2].Value)));
                        break;
                }
            }
            if (useUpdate)
                tool._U.Execute("update list set content = '" + SignFlow.ToString(SaveOptions.DisableFormatting) + "' where name = '"+flowName+"'");
            else
                tool._U.Execute("insert into list (name,content) values ('"+flowName+"','" + SignFlow.ToString(SaveOptions.DisableFormatting) + "')");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            resetStage();
        }
        private void dataGridViewX1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            resetStage();
        }
        private void resetStage()
        {
            int i = 1;
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
                row.Cells[0].Value = i++;
        }
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == colRole.Index)
            { 
                switch (""+dataGridViewX1.Rows[e.RowIndex].Cells[1].Value )
	            {
                    case "班導師":
                        dataGridViewX1.Rows[e.RowIndex].Cells[2].Value = "";
                        dataGridViewX1.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                        break;
                    case "指定老師":
                        dataGridViewX1.Rows[e.RowIndex].Cells[2].Value = "";
                        dataGridViewX1.Rows[e.RowIndex].Cells[2].ReadOnly = false;
                        break;
	            }
            }
        }
    }
}
