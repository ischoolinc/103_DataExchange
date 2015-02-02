using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class ItemConditionVO
    {
        // 大項目名稱
        public string ItemName { get; set; }
        // 大項目最高的積分
        public decimal MaxItemPoints { get; set; }
        // 大項目是否算3學期
        public int NeedSemester { get; set; }

        public ItemConditionVO(int index)
        {
            // 大項目名稱
            ItemName = Global.ItemNameList[index];

            // 大項目最高的積分
            MaxItemPoints = Global.ItemMaxPointsList[index];

            // 大項目需要幾個學期
            NeedSemester = Global.ItemNeedSemester[index];
        }
    }
}
