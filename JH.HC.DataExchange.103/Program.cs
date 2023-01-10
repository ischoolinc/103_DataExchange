﻿using System;
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

namespace JH.HS.DataExchange._103
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void Main()
        {
            string ReportName = "(竹苗區免試)學生匯入資料";
            string UUID = "0B19567E-AAD5-4E0E-9AB0-1C9AE21612AC";

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
                    //sids.Sort();
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
                        if (ar.RefStudentID == "-1") continue;
                        int arGrade = 0;
                        try
                        {
                            #region match GradeYear
                            if (dSShr != null && ar.Student !=null && ar.Student.ID !=null)
                                if (dSShr.ContainsKey(ar.Student.ID))
                                    foreach (SemesterHistoryItem item in dSShr[ar.Student.ID].SemesterHistoryItems)//match schoolYear
                                    {
                                        if (item.SchoolYear == ar.SchoolYear && item.Semester == ar.Semester)
                                            arGrade = item.GradeYear;
                                    }

                        }
                        catch (Exception exx)
                        { 
                        
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
                                        case "11":
                                        case "71":
                                        case "12":
                                        case "72":
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
                    #region 在Sql中處理的:健康與體育,藝術,綜合活動,大功支數,小功支數,嘉獎支數,大過支數,小過支數,警告支數,服務學習時數_八上,服務學習時數_八下,服務學習時數_九上

                    // 分批處理，因為一次會認成德高中國中部爆
                    List<string> colNameList = new List<string>();
                    colNameList.Add("id");
                    colNameList.Add("健康與體育");
                    colNameList.Add("藝術");
                    colNameList.Add("綜合活動");
                    colNameList.Add("大功支數");
                    colNameList.Add("小功支數");
                    colNameList.Add("嘉獎支數");
                    colNameList.Add("大過支數");
                    colNameList.Add("小過支數");
                    colNameList.Add("警告支數");
                    colNameList.Add("服務學習時數_七上");
                    colNameList.Add("服務學習時數_七下");
                    colNameList.Add("服務學習時數_八上");
                    colNameList.Add("服務學習時數_八下");
                    colNameList.Add("服務學習時數_九上");

                    DataTable dtTmp = new DataTable();
                    foreach (string name in colNameList)
                        dtTmp.Columns.Add(name);

                    try
                    {
                        foreach (string sid in sids)
                        {
                            int index = sids.IndexOf(sid);
                            string strqq = SqlString.Query1 + " and student.id="+sid;
                            try
                            {
                                DataTable dtqq = _Q.Select(strqq);
                                if (dtqq.Rows.Count > 0)
                                    dtTmp.ImportRow(dtqq.Rows[0]);
                            }
                            catch (Exception ex)
                            {
                                MsgBox.Show("學生系統編號："+sid+"，"+ex.Message);
                            }

                        }
                  
                        
                    }
                    catch (Exception ex)
                    { 
                    
                    }                   
            
                    Dictionary<string, DataRow> dSGrade = new Dictionary<string, DataRow>();
                    
                    foreach (DataRow row in dtTmp.Rows)
                    {
                        dSGrade.Add("" + row[0], row);
                    }
                    #endregion
                    #region 處理類別對應的
                    List<StudentTagRecord> strl = K12.Data.StudentTag.SelectByStudentIDs(sids);
                    List<MapRecord> mrl = _A.Select<MapRecord>();
                    Dictionary<string, List<string>> dlMaps = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> studNameTag = new Dictionary<string, List<string>>();
                    List<TagConfigRecord> strRec = K12.Data.TagConfig.SelectByCategory(TagCategory.Student);
                    List<string> studTagPNameList = new List<string>();

                    foreach (var item in strRec)
                    {
                        if (item.Prefix == "")
                        {
                            if (!studNameTag.ContainsKey(item.Name))
                                studNameTag.Add(item.Name, new List<string>());

                            studNameTag[item.Name].Add(item.Name);                     
                        }
                        else
                        {
                            string pNmae = "[" + item.Prefix + "]";
                            string kName = item.Prefix + ":" + item.Name;

                            if (!studNameTag.ContainsKey(pNmae))
                                studNameTag.Add(pNmae, new List<string>());

                            studNameTag[pNmae].Add(kName);
                            studTagPNameList.Add(kName);
                        }
                    }

                    foreach (MapRecord mr in mrl)
                    {
                        if (Map.StatusList.Contains(mr.key) || Map.SignUpStatusList.Contains(mr.key) || Map.HandicappedList.Contains(mr.key) || Map.OtherList.Contains(mr.key))
                        {
                            // 解析類別對照
                            foreach (string sKey in studNameTag.Keys)
                            {
                                if (sKey == mr.value)
                                {
                                    foreach (string sName in studNameTag[sKey])
                                    {
                                        if (!dlMaps.ContainsKey(sName))
                                            dlMaps.Add(sName, new List<string>());
                                        dlMaps[sName].Add(mr.key);
                                    }
                                }
                            }

                            // 支援原結構
                            if (studTagPNameList.Contains(mr.value))
                            {
                                if (!dlMaps.ContainsKey(mr.value))
                                    dlMaps.Add(mr.value, new List<string>());
                                dlMaps[mr.value].Add(mr.key);
                            }                            
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
                    dt.Columns.Add("非中華民國身分證號");
                    dt.Columns.Add("性別");
                    dt.Columns.Add("出生年(民國年)");
                    dt.Columns.Add("出生月");
                    dt.Columns.Add("出生日");
                    dt.Columns.Add("畢業學校代碼");
                    dt.Columns.Add("畢業年(民國年)");
                    dt.Columns.Add("畢肄業");
                    dt.Columns.Add("學生報名身分");
                    dt.Columns.Add("身心障礙");
                    dt.Columns.Add("就學區");
                    dt.Columns.Add("低收入戶");
                    dt.Columns.Add("中低收入戶");
                    dt.Columns.Add("失業勞工子女");
                    dt.Columns.Add("資料授權");
                    dt.Columns.Add("家長姓名");
                    dt.Columns.Add("市內電話");
                    dt.Columns.Add("市內電話分機");
                    dt.Columns.Add("行動電話");
                    dt.Columns.Add("郵遞區號");
                    dt.Columns.Add("通訊地址");
                    dt.Columns.Add("偏遠鄉鎮國中生");
                    dt.Columns.Add("扶助弱勢");
                    dt.Columns.Add("就近入學");
                    dt.Columns.Add("健康與體育");
                    dt.Columns.Add("藝術");
                    dt.Columns.Add("綜合活動");
                    dt.Columns.Add("國一上曠課紀錄");
                    dt.Columns.Add("國一下曠課紀錄");
                    dt.Columns.Add("國二上曠課紀錄");
                    dt.Columns.Add("國二下曠課紀錄");
                    dt.Columns.Add("國三上曠課紀錄");
                    dt.Columns.Add("國三下曠課紀錄");
                    dt.Columns.Add("大功支數");
                    dt.Columns.Add("小功支數");
                    dt.Columns.Add("嘉獎支數");
                    dt.Columns.Add("大過支數");
                    dt.Columns.Add("小過支數");
                    dt.Columns.Add("警告支數");
                    dt.Columns.Add("服務學習時數_七上");
                    dt.Columns.Add("服務學習時數_七下");
                    dt.Columns.Add("服務學習時數_八上");
                    dt.Columns.Add("服務學習時數_八下");
                    dt.Columns.Add("服務學習時數_九上");
                    dt.Columns.Add("本土語言認證");
                    dt.Columns.Add("本土語言認證證書");
                    dt.Columns.Add("學生電子郵件(E-mail) ");
                    #endregion
                    int seq = 1;
                    foreach (custStudentRecord csr in csrl)
                    {
                        DataRow row = dt.NewRow();
                        #region 填入資料
                        row["考區代碼"] = "06";//1
                        row["集報單位代碼"] = School.Code;//2
                        row["序號"] = seq;//3
                        row["學號"] = csr.StudentNumber;//4
                        row["班級"] = csr.ClassName;//5
                        row["座號"] = csr.SeatNo.HasValue?string.Format("{0:00}",csr.SeatNo.Value):"";//6
                        row["學生姓名"] = csr.Name;//7
                        row["身分證統一編號"] = csr.IDNumber;//8
                        row["非中華民國身分證號"] = "";//9
                        row["性別"] = csr.Gender;//10
                        row["出生年(民國年)"] = csr.Birthday.HasValue ? "" + (csr.Birthday.Value.Year - 1911) : "";//11
                        row["出生月"] = csr.Birthday.HasValue ? "" + (csr.Birthday.Value.Month) : "";//12
                        row["出生日"] = csr.Birthday.HasValue ? "" + (csr.Birthday.Value.Day) : "";//13
                        row["畢業學校代碼"] = School.Code;//14
                        row["畢業年(民國年)"] = "";//15
                        row["畢肄業"] = "";//16
                        string strtmp = "";
                        foreach (KeyValuePair<string, string> item in new Dictionary<string, string>(){
                                                                //{"一般生",0},
                                                                {"一般生","0"},
{"身障生","1"},
{"原住民(有認證)","2"},
{"原住民(無認證)","3"},
{"蒙藏生","4"},
{"外派子女25%","5"},
{"外派子女15%","6"},
{"外派子女10%","7"},
{"退伍軍人25%","8"},
{"退伍軍人20%","9"},
{"退伍軍人15%","A"},
{"退伍軍人10%","B"},
{"退伍軍人5%","C"},
{"退伍軍人3%","D"},
{"優秀子女25%","E"},
{"優秀子女15%","F"},
{"優秀子女10%","G"},
{"僑生","H"}})
                        {
                            if (ddSMaps.ContainsKey(csr.ID + delimiter + item.Key))
                            {    //row["學生身分"] = ("" + row["身心障礙"]) + item.Value;//16
                                strtmp += item.Value;
                                break;
                            }
                        }
                        row["學生報名身分"] = (strtmp == "" ? "0" : strtmp);//17
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
{"情緒行為障礙","8"},
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
                        row["身心障礙"] = string.IsNullOrEmpty(strtmp) ? "0" : strtmp;//18

                        row["就學區"] = "";//19
                        row["低收入戶"] = ddSMaps.ContainsKey(csr.ID + delimiter + "低收入戶") ? "1" : "0";//20
                        row["中低收入戶"] = ddSMaps.ContainsKey(csr.ID + delimiter + "中低收入戶") ? "1" : "0";//21
                        row["失業勞工子女"] = ddSMaps.ContainsKey(csr.ID + delimiter + "失業勞工子女") ? "1" : "0";//22

                        row["資料授權"] = "";//23
                        row["家長姓名"] = csr.CustodianName;//24
                        row["市內電話"] = csr.ContactPhone != null ? csr.ContactPhone.Replace("(", "").Replace(")", "").Replace("-", "") : "";//25
                        row["市內電話分機"] = ""; //26
                        row["行動電話"] = csr.SMSPhone != null ? csr.SMSPhone.Replace("(", "").Replace(")", "").Replace("-", "") : "";//27
                        row["郵遞區號"] = csr.MallingAddressZipCode;//28

                        string address = "";
                        if (!string.IsNullOrWhiteSpace(csr.MallingAddress))
                            address = csr.MallingAddress.Replace("[]", "");
                        else if (!string.IsNullOrWhiteSpace(csr.PermanentAddress))
                            address = csr.PermanentAddress.Replace("[]", "");

                        row["通訊地址"] = address;//29
                        //row["通訊地址"] = csr.MallingAddress != null ? csr.MallingAddress.Replace("[]", "") : "";//27
                        //row["原住民是否含母語認證"] = (ddSMaps.ContainsKey(csr.ID + delimiter + "原住民")) ? (ddSMaps.ContainsKey(csr.ID + delimiter + "原住民是否含母語認證") ? "1" : "0") : null;//28

                        row["偏遠鄉鎮國中生"] = ddSMaps.ContainsKey(csr.ID + delimiter + "偏遠鄉鎮國中生") ? 1 : 0; ; //30
                        //row["特殊生加分百分比"] = "";//29
                        row["扶助弱勢"] = ddSMaps.ContainsKey(csr.ID + delimiter + "扶助弱勢") ? 5 : 0;// null;//31
                        row["就近入學"] = ddSMaps.ContainsKey(csr.ID + delimiter + "就近入學") ? 5 : 0;//32
                        
                        //row["國一上曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "11") || dSGsA.ContainsKey(csr.ID + delimiter + "71") ? "有紀錄" : "無紀錄";//35
                        //row["國一下曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "12") || dSGsA.ContainsKey(csr.ID + delimiter + "72") ? "有紀錄" : "無紀錄";//36
                        //row["國二上曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "21") || dSGsA.ContainsKey(csr.ID + delimiter + "81") ? "有紀錄" : "無紀錄";//35
                        //row["國二下曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "22") || dSGsA.ContainsKey(csr.ID + delimiter + "82") ? "有紀錄" : "無紀錄";//36
                        //row["國三上曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "31") || dSGsA.ContainsKey(csr.ID + delimiter + "91") ? "有紀錄" : "無紀錄";//37
                        //row["國三下曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "32") || dSGsA.ContainsKey(csr.ID + delimiter + "92") ? "有紀錄" : "無紀錄";//38

                        row["國一上曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "11") || dSGsA.ContainsKey(csr.ID + delimiter + "71") ? 0 : 1;//35
                        row["國一下曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "12") || dSGsA.ContainsKey(csr.ID + delimiter + "72") ? 0 : 1;//36
                        row["國二上曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "21") || dSGsA.ContainsKey(csr.ID + delimiter + "81") ? 0 : 1;//35
                        row["國二下曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "22") || dSGsA.ContainsKey(csr.ID + delimiter + "82") ? 0 : 1;//36
                        row["國三上曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "31") || dSGsA.ContainsKey(csr.ID + delimiter + "91") ? 0 : 1;//37
                        row["國三下曠課紀錄"] = dSGsA.ContainsKey(csr.ID + delimiter + "32") || dSGsA.ContainsKey(csr.ID + delimiter + "92") ? 0 : 1;//38


                        // 預設0
                        row["大功支數"] = 0;
                        row["小功支數"] = 0;
                        row["嘉獎支數"] = 0;
                        row["大過支數"] = 0;
                        row["小過支數"] = 0;
                        row["警告支數"] = 0;

                        if (dSGrade.ContainsKey(csr.ID))
                        {
                            row["健康與體育"] = dSGrade[csr.ID][1];//32
                            row["藝術"] = dSGrade[csr.ID][2];//33
                            row["綜合活動"] = dSGrade[csr.ID][3];//34

                            if (dSGrade[csr.ID][4] != null && dSGrade[csr.ID][4].ToString()!="")
                                row["大功支數"] = dSGrade[csr.ID][4];//39

                            if (dSGrade[csr.ID][5] != null && dSGrade[csr.ID][5].ToString() != "")
                                row["小功支數"] = dSGrade[csr.ID][5];//40

                            if (dSGrade[csr.ID][6] != null && dSGrade[csr.ID][6].ToString() != "")
                                row["嘉獎支數"] = dSGrade[csr.ID][6];//41

                            if (dSGrade[csr.ID][7] != null && dSGrade[csr.ID][7].ToString() != "")
                                row["大過支數"] = dSGrade[csr.ID][7];//42

                            if (dSGrade[csr.ID][8] != null && dSGrade[csr.ID][8].ToString() != "")
                                row["小過支數"] = dSGrade[csr.ID][8];//43

                            if (dSGrade[csr.ID][9] != null && dSGrade[csr.ID][9].ToString() != "")
                                row["警告支數"] = dSGrade[csr.ID][9];//44

                            row["服務學習時數_七上"] = dSGrade[csr.ID][10];//45
                            row["服務學習時數_七下"] = dSGrade[csr.ID][11];//46
                            row["服務學習時數_八上"] = dSGrade[csr.ID][12];//45
                            row["服務學習時數_八下"] = dSGrade[csr.ID][13];//46
                            row["服務學習時數_九上"] = dSGrade[csr.ID][14];//47
                            row["本土語言認證"] = ddSMaps.ContainsKey(csr.ID + delimiter + "本土語言認證") ? 2 : 0;//48
                            //row["本土語言認證證書"] = ddSMaps.ContainsKey(csr.ID + delimiter + "本土語言認證證書") ? 3 : 0;//49
                            strtmp = "";
                            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>(){
{"本土語言認證證書:原住民族語","1"},
{"本土語言認證證書:客語","2"},
{"本土語言認證證書:閩南語","3"}
                            })
                            {
                                if (ddSMaps.ContainsKey(csr.ID + delimiter + item.Key))
                                {
                                    strtmp += item.Value;
                                    break;
                                }
                            }
                            row["本土語言認證證書"] = (strtmp == "" ? "0" : strtmp);//49
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

            string reportName = inputReportName;


            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xls");

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
                wb.Save(path, Aspose.Cells.FileFormatType.Excel2003);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xls";
                sd.Filter = "XLS檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, Aspose.Cells.FileFormatType.Excel2003);

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
