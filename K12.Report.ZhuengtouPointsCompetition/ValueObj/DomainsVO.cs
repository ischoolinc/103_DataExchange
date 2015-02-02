using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class DomainsVO
    {
        private Dictionary<SchoolYearSemester, Dictionary<string, List<DomainVO>>> DomainsBySchoolYear = new Dictionary<SchoolYearSemester,Dictionary<string,List<DomainVO>>>();
        //private Dictionary<string, List<DomainVO>> DomainsByDomainName = new Dictionary<string,List<DomainVO>>();

        public void AddDomain(SchoolYearSemester SchoolYearSemester, string StudentId, string DomainName, decimal? DomainScore)
        {
            DomainVO domainVO = new DomainVO();
            domainVO.DomainName = DomainName;
            if(DomainScore.HasValue)
                domainVO.DomainScore = DomainScore.Value;
            else
                domainVO.DomainScore = 0;
            domainVO.SchoolYearSemester = SchoolYearSemester;
            domainVO.StudentId = StudentId;

            if(!DomainsBySchoolYear.ContainsKey(SchoolYearSemester))
                DomainsBySchoolYear.Add(SchoolYearSemester, new Dictionary<string,List<DomainVO>>());
            if (!DomainsBySchoolYear[SchoolYearSemester].ContainsKey(DomainName))
                DomainsBySchoolYear[SchoolYearSemester].Add(DomainName, new List<DomainVO>());
            DomainsBySchoolYear[SchoolYearSemester][DomainName].Add(domainVO);

            //if(!DomainsByDomainName.ContainsKey(domainName))
            //    DomainsByDomainName.Add(domainName, new List<DomainVO>());
            //DomainsByDomainName[domainName].Add(domainVO);
        }

        public List<DomainVO> GetDomainsBySechoolYear(SchoolYearSemester SchoolYearSemester, string domainName)
        {
            if(DomainsBySchoolYear.ContainsKey(SchoolYearSemester))
            {
                if(DomainsBySchoolYear[SchoolYearSemester].ContainsKey(domainName))
                {
                    return DomainsBySchoolYear[SchoolYearSemester][domainName];
                }
            }
            return new List<DomainVO>();
        }

        //public List<DomainVO> GetDomainByDomainName(string domainName)
        //{
        //    return DomainsByDomainName[domainName];
        //}

    }


    public class DomainVO
    {
        public SchoolYearSemester SchoolYearSemester { get; set; }
        public string StudentId { get; set; }
        public string DomainName { get; set; }
        public decimal DomainScore { get; set; }
    }
}
