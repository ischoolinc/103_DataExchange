using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class LackMsg
    {
        private List<int> domainLack = new List<int>();
        private List<int> historyLack = new List<int>();

        public List<int> DomainLack { get { return domainLack; } }
        public List<int> HistoryLack { get { return historyLack; } }

        public void AddDomainLack(int grade)
        {
            if (!domainLack.Contains(grade))
                domainLack.Add(grade);
        }

        public void AddHistoryLack(int grade)
        {
            if (!historyLack.Contains(grade))
                historyLack.Add(grade);
        }
    }
}
