using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.AttendanceInput
{
    class Permissions
    {
        public static string 學生可請假別設定 { get { return "K12.Behavior.AttendanceInput.Config.cs"; } }
        public static bool 學生可請假別設定權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學生可請假別設定].Executable;
            }
        }
        public static string 線上請假登錄 { get { return "K12.Behavior.AttendanceInput.Edit.cs"; } }
        public static bool 線上請假登錄權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[線上請假登錄].Executable;
            }
        }
    }
}
