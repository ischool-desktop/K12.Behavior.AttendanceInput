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
using FISCA.LogAgent;
namespace K12.Behavior.AttendanceInput
{
    public partial class SaveRequests : BaseForm
    {
        private List<AttendanceRequestRecord> arrl;
        public SaveRequests()
        {
            InitializeComponent();
            this.Text = "線上請假登錄";
            arrl = tool._A.Select<AttendanceRequestRecord>("current_status = 'completed' and desktop_processed = '0'");
            Dictionary<string, StudentRecord> dsr = K12.Data.Student.SelectByIDs(arrl.Select(x => x.RefStudentId).Distinct()).Distinct().ToDictionary(x => x.ID, x => x);
            List<string> AbsenceNames = K12.Data.AbsenceMapping.SelectAll().Select(x => x.Name).ToList();
            List<string> PeriodNames = K12.Data.PeriodMapping.SelectAll().Select(x => x.Name).ToList();
            foreach (AttendanceRequestRecord arr in arrl)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                if (dsr.ContainsKey(arr.RefStudentId))
                {
                    if (dsr[arr.RefStudentId].Class != null)
                        row.Cells[0].Value = dsr[arr.RefStudentId].Class.Name;
                    row.Cells[1].Value = dsr[arr.RefStudentId].SeatNo;
                    row.Cells[2].Value = dsr[arr.RefStudentId].StudentNumber;
                    row.Cells[3].Value = dsr[arr.RefStudentId].Name;
                }
                else
                {
                    row.Cells[3].Value = arr.RefStudentId;
                    row.Cells[3].ErrorText = "無對應學生";
                    arr.MapStudentHasError = true;
                }
                row.Cells[4].Value = arr.AttOccurDate;
                DateTime tmp;
                bool tmpBool = true;
                if (!DateTime.TryParse(arr.AttOccurDate.Replace('-','/'), out tmp))
                {
                    row.Cells[4].ErrorText = "日期格式錯誤";
                    arr.OccurDateHasError = true;
                }
                arr.OccurDate = tmp;
                row.Cells[5].Value = arr.AttPeriod;
                tmpBool = false;
                foreach (string item in arr.AttPeriod.Split(','))
                {
                    if (!PeriodNames.Contains(item))
                    {
                        tmpBool = true;
                        break;
                    }
                }
                if (tmpBool)
                {
                    row.Cells[5].ErrorText = "節次格式錯誤";
                    arr.PeriodHasError = true;
                }
                row.Cells[6].Value = arr.AttAbsenceType;
                if (!AbsenceNames.Contains(arr.AttAbsenceType))
                {
                    row.Cells[6].ErrorText = "假別格式錯誤";
                    arr.AbsenceTypeHasError = true;
                }
                row.Cells[7].Value = arr.AttReason;
                row.Tag = arr;
                dataGridViewX1.Rows.Add(row);
            }
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (arrl.Count == 0)
                return;
            List<string> arrDesktopProcessed = new List<string>();
            List<AttendanceRequestRecord> arrUpdate = new List<AttendanceRequestRecord>();
            DataTable dt = tool._Q.Select("select uid from $attendance_request where uid in ('" + string.Join("','", arrl.Select(x => x.UID)) + "') and desktop_processed = '1'");
            Dictionary<string, AttendanceRecord> arExist = K12.Data.Attendance.Select(arrl.Select(x => x.RefStudentId), null, null, arrl.Select(x => x.OccurDate), null, null, null).ToDictionary(x => x.RefStudentID + "@" + x.OccurDate, x => x);
            foreach (DataRow row in dt.Rows)
                arrDesktopProcessed.Add("" + row["uid"]);

            List<AttendanceRecord> arInsert = new List<AttendanceRecord>();
            List<AttendanceRecord> arUpdate = new List<AttendanceRecord>();

            List<string> log1 = new List<string>();
            log1.Add("申請人,申請人類型,申請日期,學生編號,請假別,請假日期,請假節次,請假事由,此請假單要簽核的人,此請假單目前的關卡,此請假單目前狀態,最後回覆訊息,簽核流程XML,Desktop已處理");
            List<string> log2 = new List<string>();
            log2.Add("學生編號,學年度,學期,請假日期,請假別,請假節次");
            List<string> log3 = new List<string>();
            log3.Add("學生編號,學年度,學期,請假日期,請假別,請假節次");

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                AttendanceRequestRecord arr = (AttendanceRequestRecord)row.Tag;
                if (arrDesktopProcessed.Contains(arr.UID) || arr.HasError)
                    continue;
                AttendanceRecord ar =
                    arExist.ContainsKey(arr.RefStudentId + "@" + arr.OccurDate) ?
                    arExist[arr.RefStudentId + "@" + arr.OccurDate] :
                    new AttendanceRecord(arr.RefStudentId, int.Parse(School.DefaultSchoolYear), int.Parse(School.DefaultSemester), arr.OccurDate) { 
                        PeriodDetail = new List<AttendancePeriod>()
                    };
                foreach (string period in arr.AttPeriod.Split(','))
                {
                    bool exist = false;
                    foreach (AttendancePeriod item in ar.PeriodDetail)
                    {
                        if (period == item.Period)
                        {
                            item.AbsenceType = arr.AttAbsenceType;
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        ar.PeriodDetail.Add(new AttendancePeriod()
                        {
                            AbsenceType = arr.AttAbsenceType,
                            Period = period
                        });
                    }
                }
                arr.DesktopProcessed = "1";
                log1.Add(arr.Applicant + "," + arr.ApplicantType + "," + arr.ApplyDate + "," + arr.RefStudentId + "," + arr.AttAbsenceType + "," + arr.AttOccurDate + "," + arr.AttPeriod + "," + arr.AttReason + "," + arr.CurrentResponder + "," + arr.CurrentStageNo + "," + arr.CurrentStatus + "," + arr.LastMessage + "," + arr.SignFlow + "," + arr.DesktopProcessed);
                if (!string.IsNullOrWhiteSpace(ar.ID))
                {
                    arUpdate.Add(ar);
                    log3.Add(ar.RefStudentID + "," + ar.SchoolYear + "," + ar.Semester + "," + ar.OccurDate + "," + arr.AttAbsenceType + "," + arr.AttPeriod);
                }
                else
                {
                    arInsert.Add(ar);
                    log2.Add(ar.RefStudentID + "," + ar.SchoolYear + "," + ar.Semester + "," + ar.OccurDate + "," + arr.AttAbsenceType + "," + arr.AttPeriod);
                }
                arrUpdate.Add(arr);
            }
            tool._A.UpdateValues(arrUpdate);
            ApplicationLog.Log("線上請假", "更新", "線上請假記錄共更新" + arrUpdate.Count + "筆資料\n以下為明細\n" + string.Join("\n", log1));

            if (arInsert.Count > 0)
            {
                K12.Data.Attendance.Insert(arInsert);
                ApplicationLog.Log("線上請假", "新增", "缺曠記錄共新增" + arInsert.Count + "筆資料\n以下為明細\n" + string.Join("\n", log2));
            }
            if (arUpdate.Count > 0)
            {
                K12.Data.Attendance.Update(arUpdate);
                ApplicationLog.Log("線上請假", "更新", "缺曠記錄共更新" + arUpdate.Count + "筆資料\n以下為明細\n" + string.Join("\n", log3));
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            new RequestDetail((AttendanceRequestRecord)dataGridViewX1.Rows[e.RowIndex].Tag).ShowDialog();
        }
    }
}
