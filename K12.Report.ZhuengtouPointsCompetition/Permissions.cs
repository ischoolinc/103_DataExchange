using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition
{
    class Permissions
    {
        public const string KeyPointsReport = "K12.Report.ZhuengtouPointsCompetition.cs";

        public static bool IsEnablePointsReport
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[KeyPointsReport].Executable;
            }
        }
    }
}
