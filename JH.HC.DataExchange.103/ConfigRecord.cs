using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JH.HS.DataExchange._103
{
    [TableName("ischool.JH.HS.DataExchange.103.config")]
    public class ConfigRecord : ActiveRecord
    {
        ///<summary>
        /// 名稱
        ///</summary>
        [Field(Field = "name", Indexed = false)]
        public string Name { get; set; }

        ///<summary>
        /// 值
        ///</summary>
        [Field(Field = "value", Indexed = false)]
        public string Value { get; set; }

    }
}
