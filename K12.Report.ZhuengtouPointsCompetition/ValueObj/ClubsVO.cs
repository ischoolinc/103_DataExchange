using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class ClubsVO
    {
        private Dictionary<SchoolYearSemester, List<ClubVO>> ClubsBySchoolYear = new Dictionary<SchoolYearSemester,List<ClubVO>>();

        public void AddClub(DataRow row)
        {
            ClubVO clubVo = new ClubVO(row);

            if(!ClubsBySchoolYear.ContainsKey(clubVo.SchoolYearSemester))
                ClubsBySchoolYear.Add(clubVo.SchoolYearSemester, new List<ClubVO>());
            
            ClubsBySchoolYear[clubVo.SchoolYearSemester].Add(clubVo);
        }

        public List<ClubVO> GetClubsBySchoolYear(SchoolYearSemester schoolYearSemester)
        {
            if (ClubsBySchoolYear.ContainsKey(schoolYearSemester))
                return ClubsBySchoolYear[schoolYearSemester];
            else
                return new List<ClubVO>();
        }
    }
    
    public class ClubVO
    {
        public string StudentId { get; set; }
        public SchoolYearSemester SchoolYearSemester { get; set; }

        public ClubVO(DataRow row)
        {
            StudentId = ("" + row["ref_student_id"]).Trim();

            SchoolYearSemester = new ValueObj.SchoolYearSemester(("" + row["school_year"]).Trim(), ("" + row["semester"]).Trim());
        }
    }
}
