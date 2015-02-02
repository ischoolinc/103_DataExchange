using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.ZhuengtouPointsCompetition.DAO
{
    [FISCA.UDT.TableName("K12.Report.ZhuengtouPointsCompetition.TagMapping")]
    public class TagMappingRecord : FISCA.UDT.ActiveRecord
    {
        /// <summary>
        /// 自訂的類別名稱
        /// </summary>
        [FISCA.UDT.Field(Field = "my_tag_name")]
        public string MyTagName { get; set; }

        /// <summary>
        /// 系統類別的名稱
        /// </summary>
        [FISCA.UDT.Field(Field = "sys_tag_name")]
        public string SysTagName { get; set; }

        public TagMappingRecord() {}

        public TagMappingRecord(string myTagName, string sysTagName)
        {
            this.MyTagName = myTagName;
            this.SysTagName = sysTagName;
        }
    }
}
