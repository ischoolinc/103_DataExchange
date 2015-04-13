using Aspose.Cells;
using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K12.Report.ZhuengtouPointsCompetition.Forms
{
    public partial class MainForm : BaseForm
    {
        private List<string> _StudentIdList;
        private Dictionary<ValueObj.StudentVO, ValueObj.LackMsg> _WarningStudentDic = new Dictionary<ValueObj.StudentVO, ValueObj.LackMsg>();

        // Key: 自訂的類別名稱, Value: 系統類別ID
        private Dictionary<string, List<string>> _TagMap = new Dictionary<string,List<string>>();
        // Key: 類別名稱(prefix:name), Value: 類別系統ID
        private Dictionary<string, string> _SysTagDic = new Dictionary<string,string>();

        private BackgroundWorker _BGW = new BackgroundWorker();

        #region Form元件事件
        public MainForm(List<string> StudentIdList)
        {
            InitializeComponent();

            _StudentIdList = StudentIdList;

            // 準備TagMap的Key, 之後也不能新增
            foreach(string str in Global.DetailItemNameList[Global.index_VulnerableGroups])
            {
                _TagMap.Add(str, new List<string>());
            }

            this.Text = Global.Title;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            _BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            _BGW.ProgressChanged += new ProgressChangedEventHandler(BGW_ProgressChanged);
            _BGW.WorkerReportsProgress = true;

            SetDataGridViewValue();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            FormComponeEnable(false);
            
            ClearData();
            
            // 儲存DataGridView的資料
            SaveDataGridViewValue();

            // 取得畫面上的類別對照
            GetDataGridViewData();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "另存新檔";
            saveFileDialog1.FileName = Global.Title;
            saveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // 新增背景執行緒來處理資料的輸出, 把檔案儲存的路徑當作參數傳入
                _BGW.RunWorkerAsync(new object[] { saveFileDialog1.FileName });
            }
            else
                FormComponeEnable(true);
        }
        #endregion

        #region 背景處理
        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 取得需要的資料
            string fileName = (string)((object[])e.Argument)[0];

            // 取得學生資料
            Dictionary<string, ValueObj.StudentVO> studentListDic = DAO.FDQuery.GetStudentInfo(_StudentIdList);

            // 取得學生學習歷程
            List<JHSchool.Data.JHSemesterHistoryRecord> semesterHistoryList = JHSchool.Data.JHSemesterHistory.SelectByStudentIDs(_StudentIdList);
            Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord> semesterHistoryListDic = new Dictionary<string, JHSchool.Data.JHSemesterHistoryRecord>();
            foreach (var record in semesterHistoryList)
            {
                if (!semesterHistoryListDic.ContainsKey(record.RefStudentID))
                    semesterHistoryListDic.Add(record.RefStudentID, record);
            }

            _BGW.ReportProgress(10);

            // 取得領域成績
            Dictionary<string, ValueObj.DomainsVO> domainScoreListDic = DAO.FDQuery.GetDomainScore(_StudentIdList);

            // 取得社團
            Dictionary<string, ValueObj.ClubsVO> clubListDic = DAO.FDQuery.GetClubRecordByStudentIdList(_StudentIdList);

            _BGW.ReportProgress(20);

            // 取得服務學習
            Dictionary<string, ValueObj.ServicesVO> serviceListDic = DAO.FDQuery.GetLearningServiceByStudentIdList(_StudentIdList);

            // 取得懲戒紀錄
            List<Data.DemeritRecord> demeritList = K12.Data.Demerit.SelectByStudentIDs(_StudentIdList);
            Dictionary<string, ValueObj.DemeritsVO> demeritListDic = new Dictionary<string,ValueObj.DemeritsVO>();
            // 把資料依照學生跟學年度學期分好
            foreach (Data.DemeritRecord rec in demeritList)
            {
                if (!demeritListDic.ContainsKey(rec.RefStudentID))
                    demeritListDic.Add(rec.RefStudentID, new ValueObj.DemeritsVO());

                demeritListDic[rec.RefStudentID].AddDemerit(rec);
            }

            _BGW.ReportProgress(30);

            // 取得獎歷紀錄
            List<Data.MeritRecord> meritList = K12.Data.Merit.SelectByStudentIDs(_StudentIdList);
            Dictionary<string, ValueObj.MeritsVO> meritListDic = new Dictionary<string,ValueObj.MeritsVO>();
            // 把資料依照學生跟學年度學期分好
            foreach (Data.MeritRecord rec in meritList)
            {

                if (!meritListDic.ContainsKey(rec.RefStudentID))
                    meritListDic.Add(rec.RefStudentID, new ValueObj.MeritsVO());

                meritListDic[rec.RefStudentID].AddMerit(rec);
                
            }
            #endregion

            _BGW.ReportProgress(50);

            #region 計算分數
            foreach (ValueObj.StudentVO studentObj in studentListDic.Values)
            {
                // 由學習歷程來取得年級跟學年度學期的關係
                Dictionary<int, ValueObj.SchoolYearSemester> gradeMap = Utility.ProcessSemesterHistory(semesterHistoryListDic[studentObj.StudentId]);

                decimal totalPoints = 0;

                // 計算扶助弱勢
                totalPoints += Cal_VulnerableGroups(studentObj, _TagMap);

                // 計算均衡學習
                totalPoints +=  Cal_BalanceLearning(studentObj, gradeMap,
                                (domainScoreListDic.ContainsKey(studentObj.StudentId) ? domainScoreListDic[studentObj.StudentId] : null));

                // 計算德行表現
                totalPoints += Cal_VirtuousConduct(studentObj, gradeMap,
                                (clubListDic.ContainsKey(studentObj.StudentId) ? clubListDic[studentObj.StudentId] : null),
                                (serviceListDic.ContainsKey(studentObj.StudentId) ? serviceListDic[studentObj.StudentId] : null));

                // 計算無記過紀錄
                totalPoints += Cal_Demerit(studentObj, gradeMap,
                                (demeritListDic.ContainsKey(studentObj.StudentId) ? demeritListDic[studentObj.StudentId] : null));

                // 計算獎勵紀錄
                totalPoints += Cal_Merit(studentObj, gradeMap,
                                (meritListDic.ContainsKey(studentObj.StudentId) ? meritListDic[studentObj.StudentId] : null));
                
                // 回存學生的總分數
                studentObj.TotalPoints = totalPoints;
            }
            #endregion

            _BGW.ReportProgress(70);

            #region 輸出到Excel
            
            // 輸出到Excel
            ExcelHelper excelHelper = new ExcelHelper(fileName);

            List<ValueObj.StudentVO> StudentList = studentListDic.Values.ToList();

            // 排序
            StudentList.Sort(SortStudent);

            // 輸出正常資料
            foreach (ValueObj.StudentVO studentObj in StudentList)
            {
                excelHelper.OutRowData(studentObj);
                excelHelper.OutDetailData(studentObj);
            }

            // 輸出有問題的學生
            excelHelper.OutEmptyRowData();
            foreach (var rec in _WarningStudentDic)
            {
                excelHelper.OutWarningData(rec.Key, rec.Value);
            }
            #endregion

            _BGW.ReportProgress(90);

            // 儲存結果
            e.Result = new object[] { excelHelper };
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FormComponeEnable(true);

            if (e.Error == null)
            {
                ExcelHelper excelHelper = (ExcelHelper)((object[])e.Result)[0];

                #region 儲存 Excel
                string path = excelHelper._FileName;

                if (File.Exists(path))
                {
                    bool needCount = true;
                    try
                    {
                        File.Delete(path);
                        needCount = false;
                    }
                    catch { }
                    int i = 1;
                    while (needCount)
                    {
                        string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                        if (!File.Exists(newPath))
                        {
                            path = newPath;
                            break;
                        }
                        else
                        {
                            try
                            {
                                File.Delete(newPath);
                                path = newPath;
                                break;
                            }
                            catch { }
                        }
                    }
                }
                try
                {
                    File.Create(path).Close();
                }
                catch
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = Path.GetFileNameWithoutExtension(path) + ".xls";
                    sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.Create(sd.FileName);
                            path = sd.FileName;
                        }
                        catch
                        {
                            FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                excelHelper._Report.Save(path, FileFormatType.Excel2003);
                #endregion
                if (excelHelper._IsOverRow)
                    MsgBox.Show("匯出資料已經超過Excel的極限(65536筆)。\n超出的資料無法被匯出。\n\n請減少選取學生人數。");

                FISCA.Presentation.MotherForm.SetStatusBarMessage(Global.Title + "產生完成。", 100);

                System.Diagnostics.Process.Start(path);
            }
            else
            {
                MsgBox.Show(Global.Title + "發生未預期錯誤。\n" + e.Error.Message);
                // 將訊息儲存在本機 Exception 資料夾並傳回 ischool
                SmartSchool.ErrorReporting.ErrorMessgae errMsg = new SmartSchool.ErrorReporting.ErrorMessgae(e.Error); 
            }
        }

        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage(Global.Title + "產生中", e.ProgressPercentage);
        }
        #endregion

        #region 每個項目的處理

        /// <summary>
        /// 處理扶助弱勢
        /// </summary>
        /// <param name="studentObj"></param>
        /// <param name="tagIdList"></param>
        private decimal Cal_VulnerableGroups(ValueObj.StudentVO studentObj, Dictionary<string, List<string>> tagIdList)
        {
            int ItemIndex = Global.index_VulnerableGroups;
            // 取得大項目的條件
            ValueObj.ItemConditionVO itemCondition = new ValueObj.ItemConditionVO(ItemIndex);
            // 取得子項目的條件
            ValueObj.DetailItemConditionVO detailCondition = new ValueObj.DetailItemConditionVO(ItemIndex);
            // 大項目名稱
            string itemName = itemCondition.ItemName;
            // 項目的積分
            decimal ItemTotalPoints = 0;

            // loop 每個子項目
            for (int intI = 0; intI < Global.DetailItemNameList[ItemIndex].Length; intI++)
            {
                // 子項目名稱
                string detailItemName = Global.DetailItemNameList[ItemIndex][intI];
                // 子項目積分
                decimal points = detailCondition.DetailItemListDic[detailItemName];

                // 看學生的類別有沒有在TagIdList
                bool isFound = Utility.IsContansTagId(studentObj.StudentTagId, tagIdList[detailItemName]);

                if (isFound == false)
                {
                    points = 0;
                }

                // 取得detail item in student
                ValueObj.DetailItemVO detailItem = studentObj.DetailItemList[itemName][detailItemName];

                // 回存此項目顯示的內容
                detailItem.Value = "TagID: " + string.Join(",", tagIdList[detailItemName].ToArray());
                // 回存此項目的積分
                detailItem.Points = points;

                // 加總
                ItemTotalPoints += points;
            }

            // 假如超過上限, 就以上限為主
            ItemTotalPoints = (ItemTotalPoints > itemCondition.MaxItemPoints) ? itemCondition.MaxItemPoints : ItemTotalPoints;

            // 此學生在這個項目的積分
            studentObj.ItemList[itemName] = ItemTotalPoints;

            return ItemTotalPoints;
        }

        /// <summary>
        /// 處理均衡學習
        /// </summary>
        /// <param name="studentObj"></param>
        /// <param name="gradeMap"></param>
        /// <param name="domains"></param>
        private decimal Cal_BalanceLearning(ValueObj.StudentVO studentObj, Dictionary<int, ValueObj.SchoolYearSemester> gradeMap, ValueObj.DomainsVO domains)
        {
            int ItemIndex = Global.index_BalanceLearning;
            // 取得大項目的條件
            ValueObj.ItemConditionVO itemCondition = new ValueObj.ItemConditionVO(ItemIndex);
            // 取得子項目的條件
            ValueObj.DetailItemConditionVO detailCondition = new ValueObj.DetailItemConditionVO(ItemIndex);
            // 取得需要看的學年度學期
            Dictionary<int, ValueObj.SchoolYearSemester> needSchoolYearList = Utility.GetNeedSchoolSemesterIndex(itemCondition.NeedSemester, gradeMap);
            // 大項目名稱
            string itemName = itemCondition.ItemName;
            // 及格分數
            decimal PassScore = 60;
            // 項目的積分
            decimal ItemTotalPoints = 0;

            if (domains != null)
            {
                // loop 每個需要的領域
                for (int intI = 0; intI < Global.DetailItemNameList[ItemIndex].Length; intI++)
                {
                    // 子項目名稱
                    string detailItemName = Global.DetailItemNameList[ItemIndex][intI];
                    // 子項目積分
                    decimal points = detailCondition.DetailItemListDic[detailItemName];

                    decimal totalScore = 0;
                    decimal avgScore = 0;
                    decimal scoreCount = 0;
                    // loop 領域的每個學期分數
                    foreach (KeyValuePair<int, ValueObj.SchoolYearSemester> pair in needSchoolYearList)
                    {
                        ValueObj.SchoolYearSemester schoolYear = pair.Value;
                        if (schoolYear == null)
                        {
                            // 少了需要的學期
                            if (!_WarningStudentDic.ContainsKey(studentObj))
                                _WarningStudentDic.Add(studentObj, new ValueObj.LackMsg());
                            _WarningStudentDic[studentObj].AddHistoryLack(pair.Key);
                            continue;
                        }
                        List<ValueObj.DomainVO> domainList = domains.GetDomainsBySechoolYear(schoolYear, detailItemName);

                        if (domainList.Count == 0)
                        {
                            // 少了需要的領域
                            if (!_WarningStudentDic.ContainsKey(studentObj))
                                _WarningStudentDic.Add(studentObj, new ValueObj.LackMsg());
                            _WarningStudentDic[studentObj].AddDomainLack(pair.Key);
                            continue;
                        }

                        foreach (ValueObj.DomainVO domain in domainList)
                        {
                            totalScore += domain.DomainScore;
                            scoreCount++;
                        }
                    }

                    // 取得detail item in student
                    ValueObj.DetailItemVO detailItem = studentObj.DetailItemList[itemName][detailItemName];

                    // 領域的平均
                    // 原本是固定除以需要的學期, 這樣有少一個學期分數的話, 會很難看, 所以改成有幾個分數就除以幾
                    // avgScore = Math.Round((totalScore / needSchoolYearList.Count), 2, MidpointRounding.AwayFromZero);
                    if (scoreCount > 0)
                        avgScore = Math.Round((totalScore / scoreCount), 2, MidpointRounding.AwayFromZero);

                    // 回存此項目顯示的內容
                    detailItem.Value = "總分: " + totalScore + ", 學期數: " + scoreCount + ", 平均: " + avgScore.ToString();

                    // 看學生有沒有得到此積分
                    if (avgScore >= PassScore)
                    {
                        ItemTotalPoints += points;
                        // 回存此項目的積分
                        detailItem.Points = points;
                    }

                }
            }
            else
            {
                // 完全沒有領域成績
                if (!_WarningStudentDic.ContainsKey(studentObj))
                    _WarningStudentDic.Add(studentObj, new ValueObj.LackMsg());

                foreach(int rec in needSchoolYearList.Keys)
                    _WarningStudentDic[studentObj].AddDomainLack(rec);
            }

            // 假如超過上限, 就以上限為主
            ItemTotalPoints = (ItemTotalPoints > itemCondition.MaxItemPoints) ? itemCondition.MaxItemPoints : ItemTotalPoints;

            // 此學生在這個項目的積分
            studentObj.ItemList[itemName] = ItemTotalPoints;

            return ItemTotalPoints;
        }

        /// <summary>
        /// 處理德行表現
        /// </summary>
        /// <param name="studentObj"></param>
        /// <param name="gradeMap"></param>
        /// <param name="clubsObj"></param>
        /// <param name="servicesObj"></param>
        private decimal Cal_VirtuousConduct(ValueObj.StudentVO studentObj, Dictionary<int, ValueObj.SchoolYearSemester> gradeMap, ValueObj.ClubsVO clubsObj, ValueObj.ServicesVO servicesObj)
        {
            int ItemIndex = Global.index_VirtuousConduct;
            // 取得大項目的條件
            ValueObj.ItemConditionVO itemCondition = new ValueObj.ItemConditionVO(ItemIndex);
            // 取得子項目的條件
            ValueObj.DetailItemConditionVO detailCondition = new ValueObj.DetailItemConditionVO(ItemIndex);
            // 取得需要看的學年度學期
            List<ValueObj.SchoolYearSemester> needSchoolYearList = Utility.GetNeedSchoolSemester(itemCondition.NeedSemester, gradeMap);

            // 大項目名稱
            string itemName = itemCondition.ItemName;
            decimal ItemTotalPoints = 0;


            #region 處理社團
            // 社團名稱
            string detailItemName = Global.DetailItemNameList[ItemIndex][0];
            // 社團積分
            decimal clubPoints = detailCondition.DetailItemListDic[detailItemName];
            decimal clubTotalPoints = 0;
            if (clubsObj != null)
            {
                // loop 每個學期
                foreach (ValueObj.SchoolYearSemester schoolYear in needSchoolYearList)
                {
                    if (schoolYear == null) continue;

                    List<ValueObj.ClubVO> clubList = clubsObj.GetClubsBySchoolYear(schoolYear);
                    // 假如有參加過社團, +1分
                    if (clubList.Count > 0)
                        clubTotalPoints += 1;
                }
            }
            // 取得detail item in student
            ValueObj.DetailItemVO detailItem = studentObj.DetailItemList[itemName][detailItemName];

            // 回存此項目顯示的內容
            detailItem.Value = "Count:" + clubTotalPoints.ToString();

            // 假如超過上限, 就以上限為主
            clubPoints = (clubTotalPoints > clubPoints) ? clubPoints : clubTotalPoints;

            // 回存此項目的積分
            detailItem.Points = clubPoints;

            #endregion

            #region 處理服務學習
            decimal ServiceHours = 6;
            // 服務學習名稱
            detailItemName = Global.DetailItemNameList[ItemIndex][1];
            // 服務學習積分
            decimal servicePoints = detailCondition.DetailItemListDic[detailItemName];
            decimal serviceTotalPoints = 0;

            // 特殊條件
            // 取得三下的學年度學期
            ValueObj.SchoolYearSemester specilSchoolYear = gradeMap[6];
            // 截止時間 103/5/9
            DateTime deadTime = new DateTime(2015, 5, 9);

            if(servicesObj != null)
            {
                // loop 每個學期
                foreach (ValueObj.SchoolYearSemester schoolYear in needSchoolYearList)
                {
                    if (schoolYear == null) continue;

                    decimal hours = 0;
                    List<ValueObj.ServiceVO> serviceList = servicesObj.GetServicesBySchoolYear(schoolYear);
                    // 加總此學期的時數
                    foreach(var serviceObj in serviceList)
                    {
                        if (schoolYear == specilSchoolYear)
                        {
                            if (serviceObj.OccurTime.HasValue && (serviceObj.OccurTime.Value <= deadTime))
                                hours += serviceObj.Hours;
                        }
                        else
                            hours += serviceObj.Hours;
                    }

                    // 假如超過6小時, +1分
                    if (hours >= ServiceHours)
                        serviceTotalPoints += 1;
                }
             }
            
            // 取得detail item in student
            detailItem = studentObj.DetailItemList[itemName][detailItemName];

            // 回存此項目顯示的內容
            detailItem.Value = "Count:" + serviceTotalPoints.ToString();

            // 假如超過上限, 就以上限為主
            servicePoints = (serviceTotalPoints > servicePoints) ? servicePoints : serviceTotalPoints;

            // 回存此項目的積分
            detailItem.Points = servicePoints;

            #endregion

            ItemTotalPoints = clubPoints + servicePoints;

            // 此學生在這個項目的積分
            studentObj.ItemList[itemName] = ItemTotalPoints;

            return ItemTotalPoints;

        }

        /// <summary>
        /// 處理無記過紀錄
        /// </summary>
        /// <param name="studentObj"></param>
        /// <param name="gradeMap"></param>
        /// <param name="demeritObj"></param>
        private decimal Cal_Demerit(ValueObj.StudentVO studentObj, Dictionary<int, ValueObj.SchoolYearSemester> gradeMap, ValueObj.DemeritsVO demeritObj)
        {
            int ItemIndex = Global.index_Demerit;
            // 取得大項目的條件
            ValueObj.ItemConditionVO itemCondition = new ValueObj.ItemConditionVO(ItemIndex);
            // 取得子項目的條件
            ValueObj.DetailItemConditionVO detailCondition = new ValueObj.DetailItemConditionVO(ItemIndex);
            // 取得需要看的學年度學期
            List<ValueObj.SchoolYearSemester> needSchoolYearList = Utility.GetNeedSchoolSemester(itemCondition.NeedSemester, gradeMap);

            // 大項目名稱
            string itemName = itemCondition.ItemName;
            decimal ItemTotalPoints = 0;

            #region 處理無記過紀錄
            // 無記過紀錄名稱
            string detailItemName = Global.DetailItemNameList[ItemIndex][0];
            // 無記過紀錄積分
            decimal noDemeritPoints = detailCondition.DetailItemListDic[detailItemName];

            if (demeritObj != null)
            {
                foreach (ValueObj.SchoolYearSemester schoolYear in needSchoolYearList)
                {
                    if (schoolYear == null) continue;
                    int demeritCnt = demeritObj.GetDemeritsBySchoolYear(schoolYear).Count;
                    // 其中有一個學期被記過或警告, 就為0分
                    if (demeritCnt > 0)
                    {
                        noDemeritPoints = 0;
                        break;
                    }
                }
            }

            // 取得detail item in student
            ValueObj.DetailItemVO detailItem = studentObj.DetailItemList[itemName][detailItemName];

            // 回存此項目顯示的內容
            detailItem.Value = "--";
            // 回存此項目的積分
            detailItem.Points = noDemeritPoints;

            #endregion

            if (noDemeritPoints > 0 )
            {
                // 回存此項目顯示的內容
                detailItem.Value = "無記過紀錄!";

                studentObj.ItemList[itemName] = noDemeritPoints;
                
                // 不處理無小過以上記錄
                return noDemeritPoints;
            }

            #region 處理無小過以上記錄

            int MaxDemeritCount = 3;

            // 無小過以上記錄名稱
            detailItemName = Global.DetailItemNameList[ItemIndex][1];
            // 無小過以上記錄積分
            decimal demeritPoints = detailCondition.DetailItemListDic[detailItemName];

            // loop 每個學期, 取得所有的大過, 小過, 警告
            decimal[] Demerit = new decimal[3];
            foreach (ValueObj.SchoolYearSemester schoolYear in needSchoolYearList)
            {
                if (schoolYear == null) continue;

                List<Data.DemeritRecord> demeritList = demeritObj.GetDemeritsBySchoolYear(schoolYear);

                foreach(Data.DemeritRecord rec in demeritList)
                {
                    if(rec.DemeritA.HasValue)
                        Demerit[0] = rec.DemeritA.Value;
                    if(rec.DemeritB.HasValue)
                        Demerit[1] = rec.DemeritB.Value;
                    if(rec.DemeritC.HasValue)
                        Demerit[2] += rec.DemeritC.Value;
                }
            }

            if (Demerit[0] > 0 || Demerit[1] >0 )
            {
                // 此學生在這個項目的積分
                ItemTotalPoints = 0;
            }
            else
            {
                // 看警告有沒有超過三次
                if (Demerit[2] >= MaxDemeritCount)
                    ItemTotalPoints = 0;
                else
                    ItemTotalPoints = demeritPoints;
            }

            // 取得detail item in student
            detailItem = studentObj.DetailItemList[itemName][detailItemName];

            // 回存此項目顯示的內容
            detailItem.Value = "大過:" + Demerit[0] + ", 小過:" + Demerit[1] + ", 警告:" + Demerit[2];
            // 回存此項目的積分
            detailItem.Points = ItemTotalPoints;
            #endregion

            // 此學生在這個項目的積分
            studentObj.ItemList[itemName] = ItemTotalPoints;
            return ItemTotalPoints;
        }

        /// <summary>
        /// 處理獎勵紀錄
        /// </summary>
        /// <param name="studentObj"></param>
        /// <param name="gradeMap"></param>
        /// <param name="meritsObj"></param>
        private decimal Cal_Merit(ValueObj.StudentVO studentObj, Dictionary<int, ValueObj.SchoolYearSemester> gradeMap, ValueObj.MeritsVO meritsObj)
        {
            int ItemIndex = Global.index_Merit;
            // 取得大項目的條件
            ValueObj.ItemConditionVO itemCondition = new ValueObj.ItemConditionVO(ItemIndex);
            // 取得子項目的條件
            ValueObj.DetailItemConditionVO detailCondition = new ValueObj.DetailItemConditionVO(ItemIndex);
            // 取得需要看的學年度學期
            List<ValueObj.SchoolYearSemester> needSchoolYearList = Utility.GetNeedSchoolSemester(itemCondition.NeedSemester, gradeMap);
            // 大項目名稱
            string itemName = itemCondition.ItemName;
            // 項目的積分
            decimal ItemTotalPoints = 0;

            decimal[] meritCount = new decimal[Global.DetailItemNameList[ItemIndex].Length];

            if (meritsObj != null)
            {
                // loop 每個學期分數的大功, 小功, 獎勵
                foreach (ValueObj.SchoolYearSemester schoolYear in needSchoolYearList)
                {
                    if (schoolYear == null) continue;
                    List<Data.MeritRecord> meritList = meritsObj.GetMeritsBySchoolYear(schoolYear);
                    foreach (Data.MeritRecord rec in meritList)
                    {
                        if(rec.MeritA.HasValue)
                            meritCount[0] += rec.MeritA.Value;
                        if(rec.MeritB.HasValue)
                            meritCount[1] += rec.MeritB.Value;
                        if(rec.MeritC.HasValue)
                            meritCount[2] += rec.MeritC.Value;
                    }
                }
            }

            // loop 每個子項目
            for (int intI = 0; intI < Global.DetailItemNameList[ItemIndex].Length; intI++)
            {
                // 子項目名稱
                string meritName = Global.DetailItemNameList[ItemIndex][intI];
                // 子項目積分
                decimal points = detailCondition.DetailItemListDic[meritName];

                // 取得detail item in student
                ValueObj.DetailItemVO detailItem = studentObj.DetailItemList[itemName][meritName];

                // 計算積分
                decimal mixPoints = meritCount[intI] * points;

                // 回存此項目顯示的內容
                detailItem.Value = "Points:" + points + "  Counts:" + meritCount[intI].ToString();

                // 回存此項目的積分
                detailItem.Points = mixPoints;

                // 加總
                ItemTotalPoints += mixPoints;
            }

            // 假如超過上限, 就以上限為主
            ItemTotalPoints = (ItemTotalPoints > itemCondition.MaxItemPoints) ? itemCondition.MaxItemPoints : ItemTotalPoints;

            // 此學生在這個項目的積分
            studentObj.ItemList[itemName] = ItemTotalPoints;

            return ItemTotalPoints;
        }
        
        #endregion

        #region 排序
        /// <summary>
        /// 排序:年級/班級序號/班級名稱/座號/學號/姓名
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        private int SortStudent(ValueObj.StudentVO obj1, ValueObj.StudentVO obj2)
        {
            string seatno1 = obj1.ClassGradeYear.PadLeft(1, '0');   // 年級
            seatno1 += obj1.ClassDisplayOrder.PadLeft(3, '0');      // 班級序號
            seatno1 += obj1.ClassName.PadLeft(20, '0');             // 班級名稱
            seatno1 += obj1.StudentSeatNo.PadLeft(3, '0');          // 座號
            seatno1 += obj1.StudentNumber.PadLeft(20, '0');         // 學號
            seatno1 += obj1.StudentName.PadLeft(10, '0');           // 姓名

            string seatno2 = obj2.ClassGradeYear.PadLeft(1, '0');   // 年級
            seatno2 += obj2.ClassDisplayOrder.PadLeft(3, '0');      // 班級序號
            seatno2 += obj2.ClassName.PadLeft(20, '0');             // 班級名稱
            seatno2 += obj2.StudentSeatNo.PadLeft(3, '0');          // 座號
            seatno2 += obj2.StudentNumber.PadLeft(20, '0');         // 學號
            seatno2 += obj2.StudentName.PadLeft(10, '0');           // 姓名

            return seatno1.CompareTo(seatno2);
        }

        #endregion

        #region 自訂方法

        /// <summary>
        /// 準備DataGridView的欄位內容
        /// </summary>
        private void PrepareDataGridView()
        {
            // 設定DataGridView第一個欄位
            this.ColMyTag.Items.AddRange(_TagMap.Keys.ToArray());

            // 取得學校自訂的類別
            this._SysTagDic = DAO.FDQuery.GetSysTag();

            // 設定DataGridView第二個欄位
            this.ColSysTag.Items.AddRange(_SysTagDic.Keys.ToArray());

        }

        /// <summary>
        /// 設定DataGridView的內容
        /// </summary>
        private void SetDataGridViewValue()
        {
            PrepareDataGridView();

            // 取得資料庫資料
            List<DAO.TagMappingRecord> recList = DAO.TagMapping.SelectTagMappingAll();
            if(recList.Count == 0)
            {
                SetDefaultDataGridViewValue();
            }
            else
            {
                DataGridViewRow row;

                for (int intI = 0; intI < recList.Count; intI++)
                {
                    row = new DataGridViewRow();
                    row.CreateCells(dataGridViewX1);
                    row.Cells[0].Value = recList[intI].MyTagName;
                    if (string.IsNullOrEmpty(recList[intI].SysTagName))
                    {
                        row.Cells[1].Value = null;
                    }
                    else
                    {
                        row.Cells[1].Value = recList[intI].SysTagName;
                    }
                    dataGridViewX1.Rows.Add(row);
                }
            }

        }

        /// <summary>
        /// 設定DataGridView的預設值
        /// </summary>
        private void SetDefaultDataGridViewValue()
        {
            DataGridViewRow row;
            for (int intI = 0; intI < this.ColMyTag.Items.Count; intI++)
            {
                row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = this.ColMyTag.Items[intI];
                dataGridViewX1.Rows.Add(row);
            }
        }

        /// <summary>
        /// 儲存DataGridView的資料到DB
        /// </summary>
        private void SaveDataGridViewValue()
        {
            List<DAO.TagMappingRecord> recList = new List<DAO.TagMappingRecord>();

            //取得DataDataGridViewRow資料
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                //遇到空白的MyTagName即跳到下個loop
                if (row.Cells[0].Value == null)
                {
                    continue;
                }

                String MyTagName = row.Cells[0].Value.ToString().Trim();
                String SysTagName = "";
                if (row.Cells[1].Value != null)
                {
                    SysTagName = row.Cells[1].Value.ToString().Trim();
                }

                DAO.TagMappingRecord rec = new DAO.TagMappingRecord(MyTagName, SysTagName);
                
                recList.Add(rec);
            }

            // 存入DB
            DAO.TagMapping.SaveByRecordList(recList);
        }

        /// <summary>
        /// 取得DataGridView的內容
        /// </summary>
        private void GetDataGridViewData()
        {
            
            //取得DataDataGridViewRow資料
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                //遇到有空白的欄位, continue
                if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                {
                    continue;
                }

                String MyTagName = row.Cells[0].Value.ToString().Trim();
                String SysTagName = row.Cells[1].Value.ToString().Trim();

                if (_TagMap.ContainsKey(MyTagName))
                {
                    // 假如找到類別系統ID, 加到物件裡
                    if (_SysTagDic.ContainsKey(SysTagName))
                        _TagMap[MyTagName].Add(_SysTagDic[SysTagName]);
                }
            }
        }

        /// <summary>
        /// 清除暫存的資料
        /// </summary>
        private void ClearData()
        {
            foreach(List<string> obj in _TagMap.Values)
            {
                obj.Clear();
            }

            this._WarningStudentDic.Clear();
        }

        /// <summary>
        /// 開啟/關閉畫面上的元件
        /// </summary>
        /// <param name="enabled"></param>
        private void FormComponeEnable(bool enabled)
        {
            this.btnClose.Enabled = enabled;
            this.btnPrint.Enabled = enabled;
            this.dataGridViewX1.Enabled = enabled;
        }
        #endregion
    }
}
