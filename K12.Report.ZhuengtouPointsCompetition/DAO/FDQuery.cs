using FISCA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.DAO
{
    /// <summary>
    /// 使用 FISCA.Data Query
    /// </summary>
    public class FDQuery
    {
        /// <summary>
        /// 檢查UDT是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsUDTExists(string tableName)
        {
            bool result = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("select count(1) from _udt_table where name = '" + tableName + "'");

            if (Global.IsDebug) Console.WriteLine("[IsUDTExists] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            int count = int.Parse("" + dt.Rows[0]["count"]);

            if (count > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// 取得學生曾經參與過的社團的學年度學期
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.ClubsVO> GetClubRecordByStudentIdList(List<string> StudentIdList)
        {
            Dictionary<string, ValueObj.ClubsVO> result = new Dictionary<string,ValueObj.ClubsVO>();

            // 社團參與紀錄的取得, 由k12.scjoin.universal改成k12.resultscore.universal, 2013/12/16
            //string tableName1 = "k12.clubrecord.universal";
            //string tableName2 = "k12.scjoin.universal";

            //if (IsUDTExists(tableName1) == false || IsUDTExists(tableName2) == false)
            //{
            //    if (Global.IsDebug) Console.WriteLine("[GetClubRecordByStudentIdList] UDT for Club not found!!");
            //    return result;
            //}

            //StringBuilder sb = new StringBuilder();
            //sb.Append("select t1.school_year, t1.semester, t2.ref_student_id");
            //sb.Append(" from $" + tableName1 + " t1, $" + tableName2 + " t2");
            //sb.Append(" where t2.ref_club_id::int = t1.uid");
            //sb.Append(" and t2.ref_student_id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");
            //sb.Append(" and t1.school_year is not NULL");
            //sb.Append(" and t1.semester is not NULL");

            string tableName = "k12.resultscore.universal";
            if (IsUDTExists(tableName) == false)
            {
                if (Global.IsDebug) Console.WriteLine("[GetClubRecordByStudentIdList] UDT(" + tableName + ") for Club not found!!");
                return result;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("select t.school_year, t.semester, t.ref_student_id");
            sb.Append(" from $" + tableName + " t");
            sb.Append(" where t.ref_student_id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");
            sb.Append(" and t.school_year is not NULL");
            sb.Append(" and t.semester is not NULL");

            if (Global.IsDebug) Console.WriteLine("[GetClubRecordByStudentIdList] sql: [" + sb.ToString() + "]");
            
            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                string studentId = ("" + row["ref_student_id"]).Trim();
                if(!result.ContainsKey(studentId))
                    result.Add(studentId, new ValueObj.ClubsVO());

                result[studentId].AddClub(row);
            }

            return result;
        }

        /// <summary>
        /// 取得學生各學期的服務學習時數
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.ServicesVO> GetLearningServiceByStudentIdList(List<string> StudentIdList)
        {
            Dictionary<string, ValueObj.ServicesVO> result = new Dictionary<string,ValueObj.ServicesVO>();

            string tableName = "k12.service.learning.record";

            if (IsUDTExists(tableName) == false)
            {
                if (Global.IsDebug) Console.WriteLine("[GetLearningServiceByStudentIdList] UDT for Services not found!!");
                return result;
            }
            
            StringBuilder sb = new StringBuilder();
            sb.Append("select ref_student_id, school_year, semester, hours, occur_date");
            sb.Append(" from $" + tableName + "");
            sb.Append(" where ref_student_id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");
            sb.Append(" and school_year is not NULL");
            sb.Append(" and semester is not NULL");

            if (Global.IsDebug) Console.WriteLine("[GetLearningServiceByStudentIdList] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                string studentId = ("" + row["ref_student_id"]).Trim();

                if(!result.ContainsKey(studentId))
                    result.Add(studentId, new ValueObj.ServicesVO());
                
                result[studentId].AddService(row);
            }

            return result;
        }

        /// <summary>
        /// 取得學生資料
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.StudentVO> GetStudentInfo(List<string> StudentIdList)
        {
            Dictionary<string, ValueObj.StudentVO> result = new Dictionary<string,ValueObj.StudentVO>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id as student_id,");
            sb.Append("student.name as student_name,");
            sb.Append("student.birthdate as student_birthday,");
            sb.Append("student.id_number as student_idnumber,");
            sb.Append("class.grade_year as class_grade_year,");
            sb.Append("class.display_order as class_display_order,");
            sb.Append("class.class_name,");
            sb.Append("student.seat_no as student_seat_no,");
            sb.Append("student.student_number as student_number,");
            sb.Append("tag_student.ref_tag_id");
            sb.Append(" from student");
            sb.Append(" inner join class");
            sb.Append(" on student.ref_class_id = class.id");
            sb.Append(" left join tag_student");
            sb.Append(" on tag_student.ref_student_id = student.id");
            sb.Append(" where student.id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");

            if (Global.IsDebug) Console.WriteLine("[GetStudentInfo] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                string studentId = ("" + row["student_id"]).Trim();
                if(!result.ContainsKey(studentId))
                {
                    ValueObj.StudentVO studentVO = new ValueObj.StudentVO(row);
                    result.Add(studentId, studentVO);
                }
                else
                {
                    result[studentId].AddTagId(row);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得學生領域成績
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.DomainsVO> GetDomainScore(List<string> StudentIdList)
        {
            // 學生每個學年度學期的領域分數
            Dictionary<string, ValueObj.DomainsVO> result = new Dictionary<string,ValueObj.DomainsVO>();
            List<JHSchool.Data.JHSemesterScoreRecord> SemesterScoreList = JHSchool.Data.JHSemesterScore.SelectByStudentIDs(StudentIdList);

            foreach (JHSchool.Data.JHSemesterScoreRecord rec in SemesterScoreList)
            {
                string studentId = rec.RefStudentID;
                if(!result.ContainsKey(studentId))
                    result.Add(studentId, new ValueObj.DomainsVO());
                
                foreach(Data.DomainScore domainScore in rec.Domains.Values)
                {
                    ValueObj.SchoolYearSemester SchoolYearSemester = new ValueObj.SchoolYearSemester(domainScore.SchoolYear, domainScore.Semester);
                    ValueObj.DomainsVO domainsVo = result[studentId];
                    domainsVo.AddDomain(SchoolYearSemester, studentId, domainScore.Domain, domainScore.Score);
                }
            }

            return result;
        }   // end of GetDomainScore

        /// <summary>
        /// 取的系統內所有的類別
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetSysTag()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            StringBuilder sb = new StringBuilder();
            sb.Append("select id, prefix, name from tag where category='Student' order by prefix,name");

            if (Global.IsDebug) Console.WriteLine("[GetAllTag] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                String id = ("" + row["id"]).Trim();
                String prefix = ("" + row["prefix"]).Trim();
                String name = ("" + row["name"]).Trim();
                if (!result.ContainsKey(id))
                {
                    result.Add(Utility.GetTagName(prefix, name), id);
                }
            }

            return result;
        }
    }
}
