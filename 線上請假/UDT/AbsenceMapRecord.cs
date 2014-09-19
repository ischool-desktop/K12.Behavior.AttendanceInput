using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace K12.Behavior.AttendanceInput
{
    [FISCA.UDT.TableName("K12.Behavior.AttendanceInput.AbsenceMapRecord")]
    public class AbsenceMapRecord : FISCA.UDT.ActiveRecord
    {
        public AbsenceMapRecord()
        {
        }
        [FISCA.UDT.Field]
        public string absence { get; set; } //假別
        [FISCA.UDT.Field]
        public string allow1 { get; set; } //允許學生家長請假
        [FISCA.UDT.Field]
        public string allow2 { get; set; } //允許學生請假
    }
}
