using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    class DetailItemConditionVO
    {

        public Dictionary<string, decimal> DetailItemListDic = new Dictionary<string,decimal>();

        public DetailItemConditionVO(int index)
        {
            for (int intI = 0; intI < Global.DetailItemNameList[index].Length; intI++)
            {
                // 子項目名稱
                string detailItemName = Global.DetailItemNameList[index][intI];
                // 子項目的積分
                decimal detailItemPoints = Global.DetailItemPointsList[index][intI];
                
                DetailItemListDic.Add(detailItemName, detailItemPoints);
            }

        }
    }
}
