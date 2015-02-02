using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class MeritsVO
    {
        private Dictionary<SchoolYearSemester, List<Data.MeritRecord>> MeritsBySchoolYear = new Dictionary<SchoolYearSemester,List<Data.MeritRecord>>();

        public void AddMerit(Data.MeritRecord rec)
        {
            ValueObj.SchoolYearSemester SchoolYearSemester = new ValueObj.SchoolYearSemester(rec.SchoolYear, rec.Semester);
            if (!MeritsBySchoolYear.ContainsKey(SchoolYearSemester))
                MeritsBySchoolYear.Add(SchoolYearSemester, new List<Data.MeritRecord>());
            MeritsBySchoolYear[SchoolYearSemester].Add(rec);
        }

        public List<Data.MeritRecord> GetMeritsBySchoolYear(SchoolYearSemester schoolYearSemester)
        {
            if (MeritsBySchoolYear.ContainsKey(schoolYearSemester))
                return MeritsBySchoolYear[schoolYearSemester];
            else
                return new List<Data.MeritRecord>();
        }
    }
}
