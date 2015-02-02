using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class StudentVO
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentIdNumber { get; set; }
        public DateTime? StudentBirthday { get; set; }

        public List<string> StudentTagId = new List<string>();

        #region 排序
        private string _classGradeYear;
        public string ClassGradeYear {
            get
            {
                return _classGradeYear;
            }
            set
            {
                _classGradeYear = "";

                if (!string.IsNullOrEmpty(value))
                {
                    if (value[0] > '6')
                        _classGradeYear = Convert.ToString(value[0] - '6');
                    else
                        _classGradeYear = value;
                }
            }
        }
        public string ClassDisplayOrder { get; set; }
        public string ClassName { get; set; }
        public string StudentSeatNo { get; set; }
        public string StudentNumber { get; set; }
        #endregion

        /// <summary>
        /// Key: 大項目名稱, Value: 大項目分數
        /// </summary>
        public Dictionary<string, decimal> ItemList { get;set; }

        /// <summary>
        /// Key: 大項目名稱, Value: 子項目的詳細資料(Key: 子項目名稱, Value: 子項目資料)
        /// </summary>
        public Dictionary<string, Dictionary<string, ValueObj.DetailItemVO>> DetailItemList { get; set; }

        public decimal TotalPoints { get; set; }

        public StudentVO(DataRow row)
        {
            StudentId = ("" + row["student_id"]).Trim();
            StudentName = ("" + row["student_name"]).Trim();
            StudentIdNumber = ("" + row["student_idnumber"]).Trim();
            StudentBirthday = Utility.ConvertStringToDateTime(("" + row["student_birthday"]).Trim());
            StudentTagId.Add(("" + row["ref_tag_id"]).Trim());

            ClassGradeYear = ("" + row["class_grade_year"]).Trim();
            ClassDisplayOrder = ("" + row["class_display_order"]).Trim();
            ClassName = ("" + row["class_name"]).Trim();
            StudentSeatNo = ("" + row["student_seat_no"]).Trim();
            StudentNumber = ("" + row["student_number"]).Trim();

            ItemList = new Dictionary<string,decimal>();
            DetailItemList = new Dictionary<string, Dictionary<string, DetailItemVO>>();

            for (int intI=0; intI<Global.ItemNameList.Length; intI++)
            {
                string itemName = Global.ItemNameList[intI];
                ItemList.Add(itemName, 0);

                if (!DetailItemList.ContainsKey(itemName))
                    DetailItemList.Add(itemName, new Dictionary<string, DetailItemVO>());

                Dictionary<string, DetailItemVO> objList = DetailItemList[itemName];
                for (int intJ = 0; intJ < Global.DetailItemNameList[intI].Length; intJ++)
                {
                    DetailItemVO detailItemVO = new DetailItemVO();
                    detailItemVO.Name = Global.DetailItemNameList[intI][intJ];
                    detailItemVO.Points = 0;
                    objList.Add(detailItemVO.Name, detailItemVO);
                }
            }
        }

        public void AddTagId(DataRow row)
        {
            StudentTagId.Add(("" + row["ref_tag_id"]).Trim());
        }

        public override bool Equals(object obj)
        {
            if (obj is StudentVO)
            {
                StudentVO other = obj as StudentVO;
                if (this.StudentId == other.StudentId)
                    return true;
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.StudentId.GetHashCode() ^ this.StudentName.GetHashCode() ^ this.StudentIdNumber.GetHashCode();
        }
    }

    public class DetailItemVO
    {
        /// <summary>
        /// 子項目名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 子項目的內容
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 子項目的積分
        /// </summary>
        public decimal Points { get; set; }

    }
}
