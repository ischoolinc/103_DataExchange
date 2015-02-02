using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    
    public class ServicesVO
    {
        private Dictionary<SchoolYearSemester, List<ServiceVO>> ServicesBySchoolYear = new Dictionary<SchoolYearSemester,List<ServiceVO>>();

        public void AddService(DataRow row)
        {
            ServiceVO serviceVo = new ServiceVO(row);

            SchoolYearSemester SchoolYearSemester = serviceVo.SchoolYearSemester;

            if(!ServicesBySchoolYear.ContainsKey(SchoolYearSemester))
                ServicesBySchoolYear.Add(SchoolYearSemester, new List<ServiceVO>());

            ServicesBySchoolYear[SchoolYearSemester].Add(serviceVo);
        }

        public List<ServiceVO> GetServicesBySchoolYear(SchoolYearSemester schoolYearSemester)
        {
            if (ServicesBySchoolYear.ContainsKey(schoolYearSemester))
                return ServicesBySchoolYear[schoolYearSemester];
            else
                return new List<ServiceVO>();
        }
    }

    public class ServiceVO
    {
        public SchoolYearSemester SchoolYearSemester { get; set; }
        public string StudentId { get; set; }
        public DateTime? OccurTime;
        public decimal Hours { get; set; }

        public ServiceVO(DataRow row)
        {
            SchoolYearSemester = new ValueObj.SchoolYearSemester(("" + row["school_year"]).Trim(), ("" + row["semester"]).Trim());
            
            StudentId = ("" + row["ref_student_id"]).Trim();
            OccurTime = Utility.ConvertStringToDateTime(("" + row["occur_date"]).Trim());
            Hours = decimal.Parse(("" + row["hours"]).Trim());
        }
    }
}
