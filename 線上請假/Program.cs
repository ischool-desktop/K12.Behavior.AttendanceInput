using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.Behavior.AttendanceInput
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {
            FISCA.ServerModule.AutoManaged("http://module.ischool.com.tw/module/193005/K12.Behavior.AttendanceInput/udm.xml");

            RibbonBarItem item = MotherForm.RibbonBarItems["學務作業", "線上作業"];
            item["設定"].Size = RibbonBarButton.MenuButtonSize.Large;
            item["設定"].Image = Properties.Resources.設定;
            item["設定"]["學生可請假別設定"].Enable = Permissions.學生可請假別設定權限;
            item["設定"]["學生可請假別設定"].Click += delegate
            {
                new AbsenceMap().ShowDialog();
            };
            Catalog detail1 = RoleAclSource.Instance["學務作業"];
            detail1.Add(new RibbonFeature(Permissions.學生可請假別設定, "學生可請假別設定"));


            item = MotherForm.RibbonBarItems["學務作業", "線上作業"];
            item["線上請假登錄"].Size = RibbonBarButton.MenuButtonSize.Medium;
            item["線上請假登錄"].Image = Properties.Resources.線上請假登錄;
            item["線上請假登錄"].Enable = Permissions.線上請假登錄權限;
            item["線上請假登錄"].Click += delegate
            {
                new SaveRequests().ShowDialog();
            };
            detail1 = RoleAclSource.Instance["學務作業"];
            detail1.Add(new RibbonFeature(Permissions.線上請假登錄, "線上請假登錄"));
        }
    }
}
