using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSchool.Customization.Data;
using System.IO;
using FISCA.DSAUtil;
using FISCA.Data;
using System.Data;
using Aspose.Cells;
using FISCA.Presentation.Controls;
using System.Windows.Forms;
using K12.Data;

namespace KH.DataExchange._103
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            //  使用者選的年級
            string SelectedGradeYear = "";

            FISCA.Permission.Catalog cat = FISCA.Permission.RoleAclSource.Instance["教務作業"]["十二年國教"];
            cat.Add(new FISCA.Permission.RibbonFeature("90B751EE-8ADD-4AE8-A09F-1732D2DD9D8B", "高雄區免試入學資料轉檔"));


            var button = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "十二年國教"]["高雄區免試入學資料轉檔"];
            button.Enable = FISCA.Permission.UserAcl.Current["90B751EE-8ADD-4AE8-A09F-1732D2DD9D8B"].Executable;
            Exception error = null;

            System.ComponentModel.BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker();
            bkw.WorkerReportsProgress = true;
            bkw.ProgressChanged += delegate (object sender, System.ComponentModel.ProgressChangedEventArgs e)
            {
                string message = e.ProgressPercentage == 100 ? "計算完成" : "計算中...";
                FISCA.Presentation.MotherForm.SetStatusBarMessage("103高雄區免試入學資料轉檔" + message, e.ProgressPercentage);
            };
            bkw.RunWorkerCompleted += delegate
            {
                button.Enable = FISCA.Permission.UserAcl.Current["90B751EE-8ADD-4AE8-A09F-1732D2DD9D8B"].Executable;
                if (error != null) throw new Exception("103高雄區免試入學資料轉檔產生失敗", error);
            };
            bkw.DoWork += delegate
            {
                try
                {
                    bkw.ReportProgress(1);
                    QueryHelper _Q = new QueryHelper();
                    DataTable dt_source, student;
                    //     student = _Q.Select("select student.id from student left outer join class on student.ref_class_id=class.id where student.status = 1 and class.grade_year in (" + SelectedGradeYear + ")");

                    string query = string.Format(@"
                    SELECT
                        student.id
                    FROM
                        student
                        INNER JOIN class ON student.ref_class_id = class.id
                    WHERE
                        student.status = 1
                        AND class.grade_year = {0} 
                    ", SelectedGradeYear);

                    student = _Q.Select(query);

                    List<string> sids = new List<string>();
                    sids.Add("-1");
                    foreach (DataRow row in student.Rows)
                    {
                        sids.Add("" + row[0]);
                    }
                    bkw.ReportProgress(20);
                    try
                    {


                        dt_source = _Q.Select(SqlString.MultivariateScore(SelectedGradeYear));
                        List<UpdateRecordRecord> urrl = K12.Data.UpdateRecord.SelectByStudentIDs(sids);
                        Dictionary<string, SemesterHistoryRecord> shrl = K12.Data.SemesterHistory.SelectByStudentIDs(sids).ToDictionary(x => x.RefStudentID, x => x);
                        Dictionary<string, string> dsSchoolyear = new Dictionary<string, string>();
                        Dictionary<string, int> dsHas1year = new Dictionary<string, int>();
                        foreach (UpdateRecordRecord urr in urrl)
                        {
                            if (!dsHas1year.ContainsKey(urr.Student.IDNumber))
                                dsHas1year.Add(urr.Student.IDNumber, 1);//預設為1
                            if (!dsSchoolyear.ContainsKey(urr.Student.IDNumber))
                                dsSchoolyear.Add(urr.Student.IDNumber, "");//預設為""
                            if (urr.UpdateCode == "1")
                            {
                                dsSchoolyear[urr.Student.IDNumber] = "" + urr.SchoolYear;
                            }
                            else if (urr.UpdateCode == "3") //轉入
                            {
                                int urrGrade = 0;
                                foreach (SemesterHistoryItem item in shrl[urr.StudentID].SemesterHistoryItems)//match schoolYear
                                {
                                    if (item.SchoolYear == urr.SchoolYear && item.Semester == urr.Semester)
                                        urrGrade = item.GradeYear;
                                }
                                if (urrGrade == 3 || urrGrade == 9)
                                {
                                    if (urr.Semester == 1)
                                    {
                                        if (DateTime.Parse(urr.UpdateDate) >= DateTime.Parse("" + (1911 + urr.SchoolYear) + "/9/1"))
                                        {
                                            //確認轉入日期為3上開學後
                                            dsHas1year[urr.Student.IDNumber] = 0;
                                        }
                                    }
                                    else
                                        dsHas1year[urr.Student.IDNumber] = 0;
                                }
                                //else if (urrGrade == 0)
                                //    ;
                            }
                        }
                        List<string> verysmart = new List<string>();
                        foreach (var item in K12.Data.StudentTag.SelectByStudentIDs(sids))
                        {
                            if (item.Name == "資賦優異縮短修業年限學生")
                                verysmart.Add(item.Student.IDNumber);
                        }

                        bkw.ReportProgress(60);
                        //List<string> l = new List<string> { "藝術與人文", "健康與體育", "綜合活動", "服務學習", "大功", "小功", "嘉獎", "大過", "小過", "警告", "幹部任期次數", "坐姿體前彎", "立定跳遠", "仰臥起坐", "心肺適能" };
                        List<string> l = new List<string> { "藝術", "健康與體育", "綜合活動", "科技", "服務學習", "大功", "小功", "嘉獎", "大過", "小過", "警告", "幹部任期次數", "坐姿體前彎", "立定跳遠", "仰臥起坐", "心肺適能" };

                        Dictionary<string, Dictionary<string, DataRow>> rowMapping = new Dictionary<string, Dictionary<string, DataRow>>();
                        int index = 0;
                        List<DataRow> deletedRows = new List<DataRow>();
                        DataTable dt_tmp = dt_source.Clone();
                        List<string> needFix = new List<string>();
                        foreach (DataRow row in dt_source.Rows)
                        {
                            row[3] = (dsSchoolyear.ContainsKey("" + row[2])) ? "" + dsSchoolyear["" + row[2]] : "";
                            if ("" + row[3] == "" && !needFix.Contains("" + row[2]))
                            {
                                needFix.Add("" + row[2]);
                            }
                            row[4] = dsHas1year.ContainsKey("" + row[2]) ? "" + dsHas1year["" + row[2]] : "";
                            if (verysmart.Contains("" + row[2]))
                                row[5] = 1;

                            if (!rowMapping.ContainsKey("" + row[2]))
                            {
                                rowMapping.Add("" + row[2], new Dictionary<string, DataRow>());
                            }
                            if (!rowMapping["" + row[2]].ContainsKey("" + row[6]))
                                rowMapping["" + row[2]].Add("" + row[6], row);
                            //if (l[index] != "" + row[6]) //只取最後一筆(最新) , sql的left join已保證至少有一筆
                            //{
                            //    dt_tmp.Rows.RemoveAt(dt_tmp.Rows.Count - 1);
                            //    dt_tmp.ImportRow(row);
                            //    continue;
                            //}
                            //dt_tmp.ImportRow(row);
                            //index++;
                            //if (index >= l.Count)
                            //    index = 0;
                        }
                        foreach (var ssn in needFix)
                        {
                            foreach (var key in l)
                            {
                                var row = rowMapping[ssn][key];
                                dt_tmp.ImportRow(row);
                            }
                        }
                        foreach (var ssn in rowMapping.Keys)
                        {
                            if (!needFix.Contains(ssn))
                            {
                                foreach (var key in l)
                                {
                                    var row = rowMapping[ssn][key];
                                    row[13] = "";
                                    row[14] = "";
                                    row[15] = "";
                                    dt_tmp.ImportRow(row);
                                }
                            }
                        }
                        bkw.ReportProgress(80);

                        CompletedXls("高雄區多元成績交換資料格式", dt_tmp, new Workbook());
                        dt_tmp = _Q.Select(SqlString.IncentiveRecord(SelectedGradeYear));
                        //DataTable newDt_tmp = new DataTable();
                        //foreach (DataRow dr in dt_tmp.Rows)
                        //{
                        //    var newRow;
                        //    newRow[0]=dr["class_name"];

                        //    newDt_tmp.ImportRow(newRow);
                        //}
                        CompletedXls("高雄區多元成績-獎懲記錄交換資料格式", dt_tmp, new Workbook());
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show(ex.Message);
                    }
                }
                catch (Exception exc)
                {
                    error = exc;
                }
                bkw.ReportProgress(100);
            };
            button.Click += delegate
            {
                if (!bkw.IsBusy)
                {
                    button.Enable = false;

                    frmSelectGradeYear frm = new frmSelectGradeYear();
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        SelectedGradeYear = frm.GetSelectedGradeYear();
                        bkw.RunWorkerAsync();
                    }
                    button.Enable = true;
                }
            };
        }
        public static void CompletedXls(string inputReportName, DataTable dt, Workbook inputXls)
        {
            //遮蔽九下成績資料
            if (dt.Columns.Contains("科目") && dt.Columns.Contains("9下成績"))
            {
                //List<string> avoids = new List<string>(new string[] { "藝術與人文", "健康與體育", "綜合活動" });
                List<string> avoids = new List<string>(new string[] { "藝術", "健康與體育", "綜合活動", "科技" });
                foreach (DataRow row in dt.Rows)
                {
                    string subject = row["科目"].ToString();

                    if (avoids.Contains(subject))
                        row["9下成績"] = "";
                }
            }
            if (dt.Columns.Contains("事由"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    string reason = row["事由"].ToString();

                    row["事由"] = reason.Replace("\r", "").Replace("\n", "");
                }
            }

            if (dt.Columns.Contains("事由類別"))
            {
                List<string> unAvoids = new List<string>(new string[] { "幹部", "競賽", "服務學習", "證照", "體適能" });
                foreach (DataRow row in dt.Rows)
                {
                    string reasonType = row["事由類別"].ToString();

                    if (!unAvoids.Contains(reasonType))
                        row["事由類別"] = "";
                }
            }
            string reportName = inputReportName;


            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xlsx");

            Workbook wb = inputXls;

            wb.Worksheets[0].Cells.ImportDataTable(dt, true, "A1");
            wb.Worksheets[0].Name = inputReportName;
            wb.Worksheets[0].AutoFitColumns();
            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                wb.Save(path, FileFormatType.Xlsx);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xlsx";
                sd.Filter = "Excel檔案 (*.xlsx)|*.xlsx|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, FileFormatType.Xlsx);

                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }
    }
}
