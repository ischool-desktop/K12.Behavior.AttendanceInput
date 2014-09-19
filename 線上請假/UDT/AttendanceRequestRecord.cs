using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.Xml.Linq;

namespace K12.Behavior.AttendanceInput
{
    [TableName("attendance_request")]
    public class AttendanceRequestRecord : ActiveRecord
    {
        [Field(Field = "applicant", Indexed = false)]
        public string Applicant { get; set; }

        [Field(Field = "applicant_type", Indexed = false)]
        public string ApplicantType { get; set; }

        [Field(Field = "apply_date", Indexed = false)]
        public DateTime ApplyDate { get; set; }

        [Field(Field = "ref_student_id", Indexed = false)]
        public string RefStudentId { get; set; }

        [Field(Field = "att_absence_type", Indexed = false)]
        public string AttAbsenceType { get; set; }

        [Field(Field = "att_occur_date", Indexed = false)]
        public string AttOccurDate { get; set; }

        [Field(Field = "att_period", Indexed = false)]
        public string AttPeriod { get; set; }

        [Field(Field = "att_reason", Indexed = false)]
        public string AttReason { get; set; }

        [Field(Field = "current_responder", Indexed = false)]
        public string CurrentResponder { get; set; }

        [Field(Field = "current_stage_no", Indexed = false)]
        public string CurrentStageNo { get; set; }

        [Field(Field = "current_status", Indexed = false)]
        //notStarted：在某關合責人尚未看過。
        //pending：在某關負責人已經看過，但尚未簽合。
        //rejected：此請假單已被退回
        //completed：此請假單已經完成
        //error : 發生錯誤
        public string CurrentStatus { get; set; }

        [Field(Field = "last_message", Indexed = false)]
        public string LastMessage { get; set; }

        [Field(Field = "sign_flow", Indexed = false)]
        //<ApprovedBy>huangwc@chhs.hcc.edu.tw</ApprovedBy>
        //<ApprovedTimestamp>1411014304009</ApprovedTimestamp>
        //<Message>薩達薩大聲的</Message>
        public string SignFlow { get; set; }

        [Field(Field = "desktop_processed", Indexed = false)]
        public string DesktopProcessed { get; set; }

        //only on desktop
        public bool HasError
        {
            get
            {
                if (this.AbsenceTypeHasError || this.MapStudentHasError || this.OccurDateHasError || this.PeriodHasError)
                    return true;
                else
                    return false;

            }
        }
        public bool MapStudentHasError { get; set; }
        public bool OccurDateHasError { get; set; }
        public bool PeriodHasError { get; set; }
        public bool AbsenceTypeHasError { get; set; }
        public DateTime OccurDate { get; set; }
        //public bool DesktopProcessed { get; set; }

        //private bool isParsed = false;
        //public int MeritA;
        //public int MeritB;
        //public int MeritC;
        //public int DemeritA;
        //public int DemeritB;
        //public int DemeritC;
        //private void parse()
        //{

        //    if (!string.IsNullOrWhiteSpace(Detail))
        //    {
        //        XElement xe = XElement.Parse(Detail);
        //        if (xe.Element("Merit") != null)
        //        {
        //            int.TryParse(xe.Element("Merit").Attribute("A").Value, out this.MeritA);
        //            int.TryParse(xe.Element("Merit").Attribute("B").Value, out this.MeritB);
        //            int.TryParse(xe.Element("Merit").Attribute("C").Value, out this.MeritC);
        //        }
        //        if (xe.Element("Demerit") != null)
        //        {
        //            int.TryParse(xe.Element("Demerit").Attribute("A").Value, out this.DemeritA);
        //            int.TryParse(xe.Element("Demerit").Attribute("B").Value, out this.DemeritB);
        //            int.TryParse(xe.Element("Demerit").Attribute("C").Value, out this.DemeritC);
        //        }
        //    }
        //}
        //public string DisciplineString
        //{
        //    get
        //    {
        //        if (!this.isParsed)
        //            this.parse();
        //        string result = "";
        //        if (this.MeritFlag == 1)
        //        {
        //            if (this.MeritA > 0)
        //                result += string.Format("大功:{0}", this.MeritA);
        //            if (this.MeritB > 0)
        //                result += string.Format("小功:{0}", this.MeritB);
        //            if (this.MeritC > 0)
        //                result += string.Format("嘉獎:{0}", this.MeritC);
        //        }
        //        else if (this.MeritFlag == 0)
        //        {
        //            if (this.DemeritA > 0)
        //                result += string.Format("大過:{0}", this.DemeritA);
        //            if (this.DemeritB > 0)
        //                result += string.Format("小過:{0}", this.DemeritB);
        //            if (this.DemeritC > 0)
        //                result += string.Format("警告:{0}", this.DemeritC);
        //        }
        //        return result;
        //    }
        //}
    }
}
