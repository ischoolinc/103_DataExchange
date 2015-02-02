using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class SchoolYearSemester : IComparable<SchoolYearSemester>
    {
        public string SchoolYear { get; private set; }
        public string Semester { get; private set; }

        public SchoolYearSemester(string schoolYear, string semester)
        {
            SchoolYear = schoolYear;
            Semester = semester;
        }

        public SchoolYearSemester(int schoolYear, int semester)
        {
            SchoolYear = schoolYear.ToString();
            Semester = semester.ToString();
            
        }

        public override bool Equals(object obj)
        {
            if (obj is SchoolYearSemester)
            {
                SchoolYearSemester other = obj as SchoolYearSemester;
                if (this.SchoolYear == other.SchoolYear && this.Semester == other.Semester)
                    return true;
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return SchoolYear.GetHashCode() ^ Semester.GetHashCode();
        }

        #region IComparable<SchoolYearSemester> 成員

        public int CompareTo(SchoolYearSemester other)
        {
            return this.SchoolYear == other.SchoolYear ? this.Semester.CompareTo(other.Semester) : this.SchoolYear.CompareTo(other.SchoolYear);
        }

        #endregion
    }
}
