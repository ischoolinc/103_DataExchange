using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition
{
    class Global
    {
        public static bool IsDebug = false;

        public static readonly string Title = "中投區積分比序";

        // 大項目的Index
        public const int index_VulnerableGroups = 0;
        public const int index_BalanceLearning = 1;
        public const int index_VirtuousConduct = 2;
        public const int index_Demerit = 3;
        public const int index_Merit = 4;
        
        // 大項目的名稱
        public static readonly string[] ItemNameList = { "扶助弱勢", "均衡學習", "德行表現", "無記過紀錄", "獎勵紀錄" };
        
        // 大項目的最多分數
        public static decimal[] ItemMaxPointsList = { 3, 12, 5, 6, 4 };
        
        // 大項目需要算幾個學期, 6: 都算, 5:一上~三上, 3:二上~三上
        public static int[] ItemNeedSemester = { 6, 5, 6, 5, 5 }; // 104 學年度
        //public static int[] ItemNeedSemester = { 6, 3, 6, 3, 5 };  103學年度


        // 大項目下的子項目
        public static string[][] DetailItemNameList = {
                                            new string[] { "偏遠地區", "中低收入戶", "低收入戶" },
                                            new string[] { "健康與體育", "藝術與人文", "綜合活動" },
                                            new string[] { "社團", "服務學習" },
                                            new string[] { "無記過紀錄", "無小過以上記錄" },
                                            new string[] { "大功", "小功", "嘉獎" } };
        // 子項目的分數
        public static decimal[][] DetailItemPointsList = {
                                            new decimal[] { 1, 1, 2 },
                                            new decimal[] { 4, 4, 4 },
                                            new decimal[] { 2, 3},
                                            new decimal[] { 6, 3},
                                            new decimal[] { 3, 1, (decimal)0.5 } };
    }
}
