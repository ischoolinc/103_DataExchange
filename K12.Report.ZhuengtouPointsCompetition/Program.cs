using FISCA.Permission;
using FISCA.Presentation;
using K12.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void main()
        {
            CheckUDTExist();

            RibbonBarItem rbRptItem1 = MotherForm.RibbonBarItems["學生", "資料統計"];
            rbRptItem1["報表"]["成績相關報表"]["中投區積分比序"].Enable = Permissions.IsEnablePointsReport;
            rbRptItem1["報表"]["成績相關報表"]["中投區積分比序"].Click += delegate
            {
                if (NLDPanels.Student.SelectedSource.Count > 0)
                {
                    Forms.MainForm frm = new Forms.MainForm(NLDPanels.Student.SelectedSource);
                    frm.ShowDialog();
                }
            };

            // 在權限畫面出現"評量成績未達標準名單"權限
            Catalog catalog1 = RoleAclSource.Instance["學生"]["報表"];
            catalog1.Add(new RibbonFeature(Permissions.KeyPointsReport, "中投區積分比序"));
        }

        private static void CheckUDTExist()
        {
            // 檢查UDT
            BackgroundWorker bkWork;

            bkWork = new BackgroundWorker();
            bkWork.DoWork += new DoWorkEventHandler(_bkWork_DoWork);
            bkWork.RunWorkerAsync();
        }

        static void _bkWork_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // 檢查並建立UDT Table
                DAO.TagMapping.CreateConfigureUDTTable();
            }
            catch
            {

            }
        }
    }
}
