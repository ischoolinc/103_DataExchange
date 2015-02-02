using FISCA.DSAUtil;
using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.ZhuengtouPointsCompetition.DAO
{
    /// <summary>
    /// 處理 UDT 資料
    /// </summary>
    public class TagMapping
    {
        /// <summary>
        /// 建立使用到的 UDT Table：主要檢查資料庫有沒有建立UDT，沒有建自動建立。
        /// </summary>
        public static void CreateConfigureUDTTable()
        {
            FISCA.UDT.SchemaManager Manager = new SchemaManager(new DSConnection(FISCA.Authentication.DSAServices.DefaultDataSource));

            // 設定
            Manager.SyncSchema(new TagMappingRecord());
        }

        /// <summary>
        /// 更新設定
        /// </summary>
        /// <param name="rec"></param>
        public static void SaveByRecordList(List<TagMappingRecord> recList)
        {
            List<TagMappingRecord> dataList = new List<TagMappingRecord>();

            AccessHelper accessHelper = new AccessHelper();

            // 先刪除所有資料
            dataList = accessHelper.Select<TagMappingRecord>();
            accessHelper.DeletedValues(dataList);

            if (recList.Count > 0)
            {
                // 新增此次所有資料
                accessHelper.InsertValues(recList);
            }
        }

        /// <summary>
        /// 取得設定
        /// </summary>
        /// <returns></returns>
        public static List<TagMappingRecord> SelectTagMappingAll()
        {
            List<TagMappingRecord> dataList = new List<TagMappingRecord>();

            AccessHelper accessHelper = new AccessHelper();
            // 當有 Where 條件寫法
            dataList = accessHelper.Select<TagMappingRecord>();

            if (dataList == null)
                return new List<TagMappingRecord>();

            return dataList;
        }

    }
}
