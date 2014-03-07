using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JH.HS.DataExchange._103
{
    [FISCA.UDT.TableName("ischool.JH.HS.DataExchange.103.Absence")]
    public class AbsenceMapRecord : FISCA.UDT.ActiveRecord
    {
        public AbsenceMapRecord()
        {
        }
        [FISCA.UDT.Field]
        public string period_type { get; set; }
        [FISCA.UDT.Field]
        public string absence { get; set; }
    }
}
