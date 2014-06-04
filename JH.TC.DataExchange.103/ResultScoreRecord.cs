using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JH.TC.DataExchange._103
{
    [TableName("K12.ResultScore.Universal")]
    public class ResultScoreRecord : ActiveRecord
    {
        /// <summary>
        /// 學生參考
        /// </summary>
        [Field(Field = "ref_student_id", Indexed = true)]
        public string RefStudentID { get; set; }

        /// <summary>
        /// 社團參考
        /// </summary>
        [Field(Field = "ref_club_id", Indexed = false)]
        public string RefClubID { get; set; }

        /// <summary>
        /// 社團參與記錄參考
        /// </summary>
        [Field(Field = "ref_scjoin_id", Indexed = false)]
        public string RefSCJoinID { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [Field(Field = "school_year", Indexed = false)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [Field(Field = "semester", Indexed = false)]
        public int Semester { get; set; }

        /// <summary>
        /// 社團名稱
        /// </summary>
        [Field(Field = "club_name", Indexed = false)]
        public string ClubName { get; set; }

        /// <summary>
        /// 學期成績
        /// </summary>
        [Field(Field = "result_score", Indexed = false)]
        public decimal? ResultScore { get; set; }

        /// <summary>
        /// 幹部名稱
        /// </summary>
        [Field(Field = "cadre_name", Indexed = false)]
        public string CadreName { get; set; }
    }
}