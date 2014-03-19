using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FISCA.DSAUtil;
using FISCA.Data;
using FISCA.UDT;
using System.Data;
using Aspose.Cells;
using FISCA.Presentation.Controls;
using System.Windows.Forms;
using K12.Data;
using System.ComponentModel;

namespace JH.TC.DataExchange._103
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            string ReportName = "103(中投區會考)學生匯入資料";
            string UUID = "138B7160-058D-40CF-9494-6DF0E87357EB";

            FISCA.Permission.Catalog cat = FISCA.Permission.RoleAclSource.Instance["教務作業"]["十二年國教"];
            cat.Add(new FISCA.Permission.RibbonFeature(UUID, ReportName));

            var button = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "十二年國教"][ReportName];
            Exception error = null;
            System.ComponentModel.BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker();
            button.Enable = FISCA.Permission.UserAcl.Current[UUID].Executable;
            bkw.WorkerReportsProgress = true;
            bkw.ProgressChanged += delegate(object sender, System.ComponentModel.ProgressChangedEventArgs e)
            {
                string message = e.ProgressPercentage == 100 ? "計算完成" : "計算中...";
                FISCA.Presentation.MotherForm.SetStatusBarMessage(ReportName + message, e.ProgressPercentage);
            };
            bkw.RunWorkerCompleted += delegate
            {
                button.Enable = FISCA.Permission.UserAcl.Current[UUID].Executable;
                if (error != null) throw new Exception(ReportName, error);
            };
            bkw.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                try
                {
                    bkw.ReportProgress(1);
                    QueryHelper _Q = new QueryHelper();
                    AccessHelper _A = new AccessHelper();
                    DataTable dt = new DataTable(), tmp;
                    #region 取得及整理資料
                    tmp = _Q.Select("select student.id from student left outer join class on student.ref_class_id=class.id where student.status = 1 and class.grade_year in (3, 9)");
                    List<string> sids = new List<string>();
                    sids.Add("-1");
                    foreach (DataRow row in tmp.Rows)
                    {
                        sids.Add("" + row[0]);
                    }
                    bkw.ReportProgress(20);
                    //List<AttendanceRecord> arl = K12.Data.Attendance.SelectByStudentIDs(sids);
                    var arl2 = K12.BusinessLogic.AutoSummary.Select(sids, null);
                    List<custStudentRecord> csrl = new List<custStudentRecord>();
                    tmp = _Q.Select("SELECT student.*," +
                                        "class.class_name ,class.grade_year AS class_grade_year,class.ref_dept_id AS class_ref_dept_id," +
                                        "dept.name AS dept_name " +
                                    "FROM student " +
                                    "LEFT JOIN class ON student.ref_class_id = class.id " +
                                    "LEFT JOIN dept ON dept.id = class.ref_dept_id " +
                                    "WHERE student.id IN (" + string.Join(",", sids) + ")" +
                                    "ORDER BY class.display_order, class.class_name, seat_no");
                    foreach (DataRow row in tmp.Rows)
                    {
                        csrl.Add(new custStudentRecord(row));
                    }
                    Dictionary<string, SemesterHistoryRecord> dSShr = K12.Data.SemesterHistory.SelectByStudentIDs(sids).ToDictionary(x => x.RefStudentID, x => x);
                    List<AbsenceMapRecord> amrl = _A.Select<AbsenceMapRecord>();
                    Dictionary<string, int> dSGsA = new Dictionary<string, int>();
                    string delimiter = "^^^";
                    #region 檢查有無曠課記錄//36~39
                    Dictionary<string, PeriodMappingInfo> dPmi = K12.Data.PeriodMapping.SelectAll().ToDictionary(x => x.Name, x => x);
                    foreach (var ar in arl2)
                    {
                        int arGrade = 0;
                        #region match GradeYear
                       
                            if (dSShr != null && ar.Student !=null && ar.Student.ID != null)
                                if (dSShr.ContainsKey(ar.Student.ID))
                                    foreach (SemesterHistoryItem item in dSShr[ar.Student.ID].SemesterHistoryItems)//match schoolYear
                                    {
                                        if (item.SchoolYear == ar.SchoolYear && item.Semester == ar.Semester)
                                            arGrade = item.GradeYear;
                                    }                   

                        #endregion
                        foreach (var ap in ar.AbsenceCounts)
                        {//ap.Name
                            foreach (AbsenceMapRecord amr in amrl)
                            {
                                if (amr.absence == ap.Name && amr.period_type == ap.PeriodType)
                                {
                                    switch (arGrade + "" + ar.Semester)
                                    {
                                        case "21":
                                        case "81":
                                        case "22":
                                        case "82":
                                        case "31":
                                        case "91":
                                        case "32":
                                        case "92":
                                            if (!dSGsA.ContainsKey(ar.RefStudentID + delimiter + arGrade + ar.Semester))
                                                dSGsA.Add(ar.RefStudentID + delimiter + arGrade + ar.Semester, 1);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    //foreach (AttendanceRecord ar in arl)
                    //{
                    //    int arGrade = 0;
                    //    #region match GradeYear
                    //    foreach (SemesterHistoryItem item in dSShr[ar.Student.ID].SemesterHistoryItems)//match schoolYear
                    //    {
                    //        if (item.SchoolYear == ar.SchoolYear && item.Semester == ar.Semester)
                    //            arGrade = item.GradeYear;
                    //    }

                    //    #endregion
                    //    foreach (AttendancePeriod ap in ar.PeriodDetail)
                    //    {
                    //        foreach (AbsenceMapRecord amr in amrl)
                    //        {
                    //            if (amr.absence == ap.AbsenceType && dPmi.ContainsKey(ap.Period) && amr.period_type == dPmi[ap.Period].Type)
                    //            {
                    //                switch (arGrade + "" + ar.Semester)
                    //                {
                    //                    case "21":
                    //                    case "81":
                    //                    case "22":
                    //                    case "82":
                    //                    case "31":
                    //                    case "91":
                    //                    case "32":
                    //                    case "92":
                    //                        if (!dSGsA.ContainsKey(ar.RefStudentID + delimiter + arGrade + ar.Semester))
                    //                            dSGsA.Add(ar.RefStudentID + delimiter + arGrade + ar.Semester, 1);
                    //                        break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region 在Sql中處理的:健康與體育,藝術與人文,綜合活動,大功支數,小功支數,嘉獎支數,大過支數,小過支數,警告支數,服務學習時數_八上,服務學習時數_八下,服務學習時數_九上
                    tmp = _Q.Select(SqlString.Query1);
                    Dictionary<string, DataRow> dSGrade = new Dictionary<string, DataRow>();
                    foreach (DataRow row in tmp.Rows)
                    {
                        dSGrade.Add("" + row[0], row);
                    }
                    #endregion
                    #region 處理類別對應的
                    List<StudentTagRecord> strl = K12.Data.StudentTag.SelectByStudentIDs(sids);
                    List<MapRecord> mrl = _A.Select<MapRecord>();
                    Dictionary<string, List<string>> dlMaps = new Dictionary<string, List<string>>();
                    foreach (MapRecord mr in mrl)
                    {
                        if (Map.SpecialList.Contains(mr.key))
                        {
                            if (!dlMaps.ContainsKey(mr.value))
                                dlMaps.Add(mr.value, new List<string>());
                            dlMaps[mr.value].Add(mr.key);
                        }
                    }
                    Dictionary<string, int> ddSMaps = new Dictionary<string, int>();
                    foreach (StudentTagRecord str in strl)
                    {
                        if (dlMaps.ContainsKey(str.FullName))
                        {
                            foreach (string item in dlMaps[str.FullName])
                            {
                                if (!ddSMaps.ContainsKey(str.RefStudentID + delimiter + item))
                                    ddSMaps.Add(str.RefStudentID + delimiter + item, 1);
                            }
                        }
                    }
                    #endregion
                    #endregion
                    #region 報表header初始化
                    dt.Columns.Add("考區代碼");
                    dt.Columns.Add("集報單位代碼");
                    dt.Columns.Add("序號");
                    dt.Columns.Add("學號");
                    dt.Columns.Add("班級");
                    dt.Columns.Add("座號");
                    dt.Columns.Add("學生姓名");
                    dt.Columns.Add("身分證統一編號");
                    dt.Columns.Add("性別");
                    dt.Columns.Add("出生年(民國年)");
                    dt.Columns.Add("出生月");
                    dt.Columns.Add("出生日");
                    dt.Columns.Add("畢業學校代碼");
                    dt.Columns.Add("畢業年(民國年)");
                    dt.Columns.Add("畢肄業");
                    dt.Columns.Add("學生身分");
                    dt.Columns.Add("身心障礙");
                    dt.Columns.Add("就學區");
                    dt.Columns.Add("低收入戶");
                    dt.Columns.Add("中低收入戶");
                    dt.Columns.Add("失業勞工子女");
                    dt.Columns.Add("資料授權");
                    dt.Columns.Add("家長姓名");
                    dt.Columns.Add("市內電話");
                    dt.Columns.Add("行動電話");
                    dt.Columns.Add("郵遞區號");
                    dt.Columns.Add("通訊地址");
                    dt.Columns.Add("原住民是否含母語認證");
                    dt.Columns.Add("非中華民國身分證號");
                    dt.Columns.Add("就近入學", typeof(int));
                    dt.Columns.Add("偏遠地區", typeof(int));
                    dt.Columns.Add("健康與體育", typeof(int));
                    dt.Columns.Add("藝術與人文", typeof(int));
                    dt.Columns.Add("綜合活動", typeof(int));
                    dt.Columns.Add("記過紀錄", typeof(int));
                    dt.Columns.Add("大功支數", typeof(int));
                    dt.Columns.Add("小功支數", typeof(int));
                    dt.Columns.Add("嘉獎支數", typeof(int));
                    dt.Columns.Add("服務學習得分", typeof(int));
                    dt.Columns.Add("社團得分", typeof(int));
                    #endregion
                    int seq = 1;
                    foreach (custStudentRecord csr in csrl)
                    {
                        DataRow row = dt.NewRow();
                        #region 填入資料
                        row["考區代碼"] = "07";//1
                        row["集報單位代碼"] = "";//2
                        row["序號"] = seq;//3
                        row["學號"] = csr.StudentNumber;//4
                        row["班級"] = csr.ClassName;//5
                        row["座號"] = csr.SeatNo;//6
                        row["學生姓名"] = csr.Name;//7
                        row["身分證統一編號"] = csr.IDNumber;//8
                        row["性別"] = csr.Gender;//9
                        row["出生年(民國年)"] = csr.Birthday.HasValue ? "" + (csr.Birthday.Value.Year - 1911) : "";//10
                        row["出生月"] = csr.Birthday.HasValue ? "" + (csr.Birthday.Value.Month) : "";//11
                        row["出生日"] = csr.Birthday.HasValue ? "" + (csr.Birthday.Value.Day) : "";//12
                        row["畢業學校代碼"] = School.Code;//13
                        row["畢業年(民國年)"] = "";//14
                        row["畢肄業"] = "";//15
                        string strtmp = "";
                        foreach (KeyValuePair<string, int> item in new Dictionary<string, int>(){
                                                                //{"一般生",0},
                                                                {"原住民",1},
                                                                {"派外人員子女",2},
                                                                {"蒙藏生",3},
                                                                {"回國僑生",4},
                                                                {"港澳生",5},
                                                                {"退伍軍人",6},
                                                                {"境外優秀科學技術人才子女",7}})
                        {
                            if (ddSMaps.ContainsKey(csr.ID + delimiter + item.Key))
                            {
                                //row["學生身分"] = ("" + row["身心障礙"]) + item.Value;//16
                                strtmp += item.Value;
                                break;
                            }
                        }
                        row["學生身分"] = (strtmp == "" ? "0" : strtmp);
                        strtmp = "";
                        foreach (KeyValuePair<string, string> item in new Dictionary<string, string>(){
                                                                //{"非身心障礙考生",0},
                                                                {"智能障礙","1"},
                                                                {"視覺障礙","2"},
                                                                {"聽覺障礙","3"},
                                                                {"語言障礙","4"},
                                                                {"肢體障礙","5"},
                                                                {"腦性麻痺","6"},
                                                                {"身體病弱","7"},
                                                                {"情緖行為障礙","8"},
                                                                {"學習障礙","9"},
                                                                {"多重障礙","A"},
                                                                {"自閉症","B"},
                                                                {"發展遲緩","C"},
                                                                {"其他障礙","D"}})
                        {
                            if (ddSMaps.ContainsKey(csr.ID + delimiter + item.Key))
                            {
                                //row["身心障礙"] = ("" + row["身心障礙"]) + item.Value;//17
                                strtmp += item.Value;
                                break;
                            }
                        }
                        row["身心障礙"] = string.IsNullOrEmpty(strtmp) ? "0" : strtmp;

                        row["就學區"] = "";//18
                        row["低收入戶"] = ddSMaps.ContainsKey(csr.ID + delimiter + "低收入戶") ? "1" : "0";//19
                        row["中低收入戶"] = ddSMaps.ContainsKey(csr.ID + delimiter + "中低收入戶") ? "1" : "0";//20
                        row["失業勞工子女"] = ddSMaps.ContainsKey(csr.ID + delimiter + "失業勞工子女") ? "1" : "0";//21

                        row["資料授權"] = "";//22
                        row["家長姓名"] = csr.CustodianName;//23
                        row["市內電話"] = csr.ContactPhone != null ? csr.ContactPhone.Replace("(", "").Replace(")", "").Replace("-", "") : "";//24
                        row["行動電話"] = csr.SMSPhone != null ? csr.SMSPhone.Replace("(", "").Replace(")", "").Replace("-", "") : "";//25
                        row["郵遞區號"] = csr.MallingAddressZipCode;//26
                        row["通訊地址"] = csr.MallingAddress != null ? csr.MallingAddress.Replace("[]", "") : "";//27
                        row["原住民是否含母語認證"] = (ddSMaps.ContainsKey(csr.ID + delimiter + "原住民")) ? (ddSMaps.ContainsKey(csr.ID + delimiter + "原住民是否含母語認證") ? "1" : "0") : null;//28
                        row["非中華民國身分證號"] = ddSMaps.ContainsKey(csr.ID + delimiter + "非中華民國身分證號") ? "V" : null;//29
                        row["就近入學"] = ddSMaps.ContainsKey(csr.ID + delimiter + "就近入學") ? 10 : 0;//30
                        row["偏遠地區"] = ddSMaps.ContainsKey(csr.ID + delimiter + "偏遠地區") ? 1 : 0;//31

                        if (dSGrade.ContainsKey(csr.ID))
                        {
                            row["健康與體育"] = dSGrade[csr.ID][1];//32
                            row["藝術與人文"] = dSGrade[csr.ID][2];//33
                            row["綜合活動"] = dSGrade[csr.ID][3];//34
                            row["記過紀錄"] = dSGrade[csr.ID][4];//35
                            row["大功支數"] = dSGrade[csr.ID][5];//36
                            row["小功支數"] = dSGrade[csr.ID][6];//37
                            row["嘉獎支數"] = dSGrade[csr.ID][7];//38
                            row["服務學習得分"] = dSGrade[csr.ID][8];// dSGrade[csr.ID][10];//39
                            row["社團得分"] = dSGrade[csr.ID][9];// dSGrade[csr.ID][11];//40
                        }
                        #endregion
                        dt.Rows.Add(row);
                        seq++;
                    }
                    bkw.ReportProgress(80);
                    CompletedXls(ReportName, dt, new Workbook());
                    bkw.ReportProgress(100);
                }
                catch (Exception exc)
                {
                    error = exc;
                }
            };
            button.Click += delegate
            {
                if (new Map().ShowDialog() == DialogResult.OK && !bkw.IsBusy)
                {
                    button.Enable = false;
                    bkw.RunWorkerAsync();
                }
            };
        }
        public static void CompletedXls(string inputReportName, DataTable dt, Workbook inputXls)
        {

            string reportName = "中投區免試報名上傳資料";


            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xlsx");

            Workbook wb = inputXls;

            wb.Worksheets[0].Cells.ImportDataTable(dt, true, "A1");
            wb.Worksheets[0].Name = "Student";
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
                wb.Save(path, SaveFormat.Xlsx);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xlsx";
                sd.Filter = "XLSX檔案 (*.xlsx)|*.xlsx|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, SaveFormat.Xlsx);

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
