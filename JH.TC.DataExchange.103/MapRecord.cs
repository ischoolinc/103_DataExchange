using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JH.TC.DataExchange._103
{
    [FISCA.UDT.TableName("ischool.JH.TC.DataExchange.103")]
    public class MapRecord : FISCA.UDT.ActiveRecord
    {
        public MapRecord()
        {
        }
        [FISCA.UDT.Field]
        public string key { get; set; }
        [FISCA.UDT.Field]
        public string value { get; set; }
        [FISCA.UDT.Field]
        public string note { get; set; }  
    }
}
