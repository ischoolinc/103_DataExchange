using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition
{
    public class Utility
    {
        public static readonly string _ContactChar = ":";

        /// <summary>
        /// 把文字轉成DateTime
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime? ConvertStringToDateTime(string strDate)
        {
            DateTime dt;

            if (string.IsNullOrEmpty(strDate))
            {
                return null;
            }
            else
            {
                strDate = strDate.Trim();
            }

            if (DateTime.TryParse(strDate, out dt))
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 看tagIdList1有沒有一個id在tagIdList2裡面
        /// </summary>
        /// <param name="tagIdList1"></param>
        /// <param name="tagIdList2"></param>
        /// <returns></returns>
        public static bool IsContansTagId(List<string> tagIdList1, List<string> tagIdList2)
        {
            foreach(string str in tagIdList1)
            {
                foreach(string str2 in tagIdList2)
                    if(str == str2)
                        return true;
            }

            return false;
        }

        /// <summary>
        /// 處理學習歷程
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static Dictionary<int, ValueObj.SchoolYearSemester> ProcessSemesterHistory(JHSchool.Data.JHSemesterHistoryRecord record)
        {
            Dictionary<int, ValueObj.SchoolYearSemester> result = new Dictionary<int,ValueObj.SchoolYearSemester>();

            /*
             * 1: 一上; 2: 一下; 3: 二上; 4: 二下; 5: 三上; 6: 三下
             * */
            result.Add(1, null);
            result.Add(2, null);
            result.Add(3, null);
            result.Add(4, null);
            result.Add(5, null);
            result.Add(6, null);

            foreach (var item in record.SemesterHistoryItems)
            {
                int gradeYear = item.GradeYear;
                if (gradeYear > 6) gradeYear -= 6;

                // 由[(年級-1)*2+學期]取得Key
                int key = (gradeYear - 1) * 2 + item.Semester;
                if (result.ContainsKey(key))
                {
                    result[key] = new ValueObj.SchoolYearSemester(item.SchoolYear, item.Semester);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得需要的學年度學期
        /// </summary>
        /// <param name="is3Semester"></param>
        /// <param name="gradMap"></param>
        /// <returns></returns>
        public static Dictionary<int, ValueObj.SchoolYearSemester> GetNeedSchoolSemesterIndex(int needSemester, Dictionary<int, ValueObj.SchoolYearSemester> gradMap)
        {
            Dictionary<int, ValueObj.SchoolYearSemester> result = new Dictionary<int,ValueObj.SchoolYearSemester>();
            int startIndex = 1;
            int endIndex = 6;

            switch (needSemester)
            {
                case 3:
                    startIndex = 3;
                    endIndex = 5;
                    break;
                case 5:
                    startIndex = 1;
                    endIndex = 5;
                    break;
                case 6:
                    startIndex = 1;
                    endIndex = 6;
                    break;
            }

            for(int intI=startIndex; intI<=endIndex; intI++)
            {
                result.Add(intI, gradMap[intI]);
            }

            return result;
        }

        /// <summary>
        /// 取得需要的學年度學期
        /// </summary>
        /// <param name="needSemester"></param>
        /// <param name="gradMap"></param>
        /// <returns></returns>
        public static List<ValueObj.SchoolYearSemester> GetNeedSchoolSemester(int needSemester, Dictionary<int, ValueObj.SchoolYearSemester> gradMap)
        {
            List<ValueObj.SchoolYearSemester> result = new List<ValueObj.SchoolYearSemester>();
            int startIndex = 1;
            int endIndex = 6;

            switch (needSemester)
            {
                case 3:
                    startIndex = 3;
                    endIndex = 5;
                    break;
                case 5:
                    startIndex = 1;
                    endIndex = 5;
                    break;
                case 6:
                    startIndex = 1;
                    endIndex = 6;
                    break;
            }

            for (int intI = startIndex; intI <= endIndex; intI++)
            {
                result.Add(gradMap[intI]);
            }

            return result;
        }

        /// <summary>
        /// 取得學生類別的完整名稱
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetTagName(string prefix, string name)
        {
            string result = "";

            if (string.IsNullOrEmpty(prefix))
            {
                result = name;
            }
            else
            {
                result = prefix.Trim() + _ContactChar + name.Trim();
            }

            return result;
        }
        
        /// <summary>
        /// 年級的Index轉成文字
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ConvertGradeIndexToString(int index)
        {
            string grade = "";

            switch (index)
            {
                case 1:
                    grade = "一上";
                    break;
                case 2:
                    grade = "一下";
                    break;
                case 3:
                    grade = "二上";
                    break;
                case 4:
                    grade = "二下";
                    break;
                case 5:
                    grade = "三上";
                    break;
                case 6:
                    grade = "三下";
                    break;
                default:
                    break;
            }

            return grade;
        }
        
        public static string ComposeHistoryLackMsg(List<int> gradYears)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int grade in gradYears)
                sb.Append(ConvertGradeIndexToString(grade)).Append("、");
            
            sb.Length = sb.Length - 1;
            sb.Append("的學習歷程!");

            return sb.ToString();
        }

        public static string ComposeDomainLackMsg(List<int> gradYears)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int grade in gradYears)
                sb.Append(ConvertGradeIndexToString(grade)).Append("、");

            sb.Length = sb.Length - 1;
            sb.Append("的領域成績!");

            return sb.ToString();
        }
    }
}
