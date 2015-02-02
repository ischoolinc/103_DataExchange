using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace K12.Report.ZhuengtouPointsCompetition
{
    public class ExcelHelper
    {
        private static readonly int _MAX_ROW_COUNT = 65535;
        private int _RowIndex, _DetailRowIndex;
        private Style _Style_Normal, _Style_Error;
        private Worksheet _OutSheet;
        private Worksheet _DetailSheet;
        private Cells _OutCells;
        private Cells _DetailCells;

        public string _FileName;
        public Workbook _Report;
        public bool _IsOverRow;

        public ExcelHelper(string fileName)
        {
            _FileName = fileName;
            
            _Report = new Workbook();
            _Report.Open(new MemoryStream(Properties.Resources.中投區積分比序_Template));
            _OutSheet = _Report.Worksheets[Global.Title];

            // 先儲存Style
            GetExcelStyle();

            _OutCells = _OutSheet.Cells;
            _DetailCells = null;

            if (Global.IsDebug == true)
            {
                _DetailSheet = _Report.Worksheets[_Report.Worksheets.Add()];
                _DetailSheet.Name = "Detail Sheet";

                _DetailCells = _DetailSheet.Cells;
                _DetailRowIndex = 0;
            }
        }
        
        #region Excel輸出方法
        // 準備Excel的Style
        private void GetExcelStyle()
        {
            Cells cells = _Report.Worksheets[Global.Title].Cells;

            // 取得正常的Style
            _Style_Normal = cells["A2"].Style;

            // 警告的Style
            _Style_Error = cells["B2"].Style;
            _Style_Error.Borders[BorderType.TopBorder].LineStyle = CellBorderType.None;
            _Style_Error.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
            _Style_Error.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.None;
            _Style_Error.Borders[BorderType.RightBorder].LineStyle = CellBorderType.None;
        }

        private void SetColumnValue(Cells cells, int col, string value, Style style)
        {
            cells[_RowIndex, col].PutValue(value);
            cells[_RowIndex, col].Style = style;
        }

        /// <summary>
        /// 讓index移到下一列
        /// </summary>
        public void OutEmptyRowData()
        {
            _RowIndex++;
        }

        /// <summary>
        /// 輸出一列資料
        /// </summary>
        /// <param name="StudentObj"></param>
        public void OutRowData(ValueObj.StudentVO StudentObj)
        {
            int columnIndex = 0;
            _RowIndex++;

            if (_RowIndex > _MAX_ROW_COUNT)
            {
                _IsOverRow = true;
                return;
            }

            // 學生姓名
            SetColumnValue(_OutCells, columnIndex++, StudentObj.StudentName, _Style_Normal);
            // 身分證統一編號
            SetColumnValue(_OutCells, columnIndex++, StudentObj.StudentIdNumber, _Style_Normal);
            // 生日
            if (StudentObj.StudentBirthday.HasValue)
            {
                // 出生年
                string tmp = ((StudentObj.StudentBirthday.Value.Year) - 1911).ToString().PadLeft(3, '0');
                SetColumnValue(_OutCells, columnIndex++, tmp, _Style_Normal);
                // 出生月
                tmp = StudentObj.StudentBirthday.Value.Month.ToString().PadLeft(2, '0');
                SetColumnValue(_OutCells, columnIndex++, tmp, _Style_Normal);
                // 出生日
                tmp = StudentObj.StudentBirthday.Value.Day.ToString().PadLeft(2, '0');
                SetColumnValue(_OutCells, columnIndex++, tmp, _Style_Normal);
            }
            else
            {
                // 出生年
                SetColumnValue(_OutCells, columnIndex++, "000", _Style_Normal);
                // 出生月
                SetColumnValue(_OutCells, columnIndex++, "00", _Style_Normal);
                // 出生日
                SetColumnValue(_OutCells, columnIndex++, "00", _Style_Normal);
            }
            // 就近入學
            SetColumnValue(_OutCells, columnIndex++, "00", _Style_Normal);
            // 扶助弱勢	均衡學習	德行表現	無記過紀錄	獎勵紀錄
            for (int intI = 0; intI < StudentObj.ItemList.Count; intI++)
            {
                decimal point = StudentObj.ItemList.Values.ToArray()[intI];

                // 針對某些欄位格式化
                switch (intI)
                {
                    case Global.index_BalanceLearning:
                        SetColumnValue(_OutCells, columnIndex++, point.ToString().PadLeft(2, '0'), _Style_Normal);
                        break;
                    case Global.index_Merit:
                        SetColumnValue(_OutCells, columnIndex++, String.Format("{0:0.0}", point), _Style_Normal);
                        break;
                    default:
                        SetColumnValue(_OutCells, columnIndex++, point.ToString(), _Style_Normal);
                        break;
                }
            }
        }

        /// <summary>
        /// 輸出學習歷程或領域成績有問題學生
        /// </summary>
        /// <param name="StudentObj"></param>
        /// <param name="gradeList"></param>
        public void OutWarningData(ValueObj.StudentVO StudentObj, ValueObj.LackMsg msgList)
        {
            int columnIndex = 0;
            _RowIndex++;

            if (_RowIndex > _MAX_ROW_COUNT)
            {
                _IsOverRow = true;
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(StudentObj.StudentName).Append(" 缺少 ");

            if (msgList.HistoryLack.Count > 0)
            {
                sb.Append(Utility.ComposeHistoryLackMsg(msgList.HistoryLack));
                sb.Append(" ");
            }

            if (msgList.DomainLack.Count >0 )
                sb.Append(Utility.ComposeDomainLackMsg(msgList.DomainLack));

            SetColumnValue(_OutCells, columnIndex++, sb.ToString(), _Style_Error);
        }

        // for debug
        private void OutDetailTitle(Cells cells)
        {
            int columnIndex = 0;
            // 年級/班級序號/班級名稱/座號/學號/姓名
            cells[_DetailRowIndex, columnIndex++].PutValue("班級年級");
            cells[_DetailRowIndex, columnIndex++].PutValue("班級序號");
            cells[_DetailRowIndex, columnIndex++].PutValue("班級名稱");
            cells[_DetailRowIndex, columnIndex++].PutValue("座號");
            cells[_DetailRowIndex, columnIndex++].PutValue("學號");

            cells[_DetailRowIndex, columnIndex++].PutValue("學生姓名");
            cells[_DetailRowIndex, columnIndex++].PutValue("身分證統一編號");
            cells[_DetailRowIndex, columnIndex++].PutValue("生日");

            for (int intI = 0; intI < Global.ItemNameList.Length; intI++)
            {
                string itemName = Global.ItemNameList[intI];
                cells[_DetailRowIndex, columnIndex++].PutValue(itemName + ":積分");

                foreach (string detailItemName in Global.DetailItemNameList[intI])
                {
                    cells[_DetailRowIndex, columnIndex++].PutValue(itemName + ":" + detailItemName + ": value");
                    cells[_DetailRowIndex, columnIndex++].PutValue(itemName + ":" + detailItemName + ": point");
                }
            }

        }

        /// <summary>
        /// for debug
        /// </summary>
        /// <param name="StudentObj"></param>
        public void OutDetailData(ValueObj.StudentVO StudentObj)
        {
            
            if (Global.IsDebug == false) return;
            
            int columnIndex = 0;
            if (_DetailRowIndex == 0) OutDetailTitle(_DetailCells);
            _DetailRowIndex++;

            if (_DetailRowIndex > _MAX_ROW_COUNT)
                return;
            // 年級/班級序號/班級名稱/座號/學號/姓名
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.ClassGradeYear);
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.ClassDisplayOrder);
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.ClassName);
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.StudentSeatNo);
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.StudentNumber);

            // 學生姓名
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.StudentName);
            // 身分證統一編號
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue(StudentObj.StudentIdNumber);
            // 生日
            _DetailCells[_DetailRowIndex, columnIndex++].PutValue("" + StudentObj.StudentBirthday);

            // 詳細資料
            foreach (string itemName in StudentObj.ItemList.Keys)
            {
                _DetailCells[_DetailRowIndex, columnIndex++].PutValue("" + StudentObj.ItemList[itemName]);

                foreach (ValueObj.DetailItemVO rec in StudentObj.DetailItemList[itemName].Values)
                {
                    _DetailCells[_DetailRowIndex, columnIndex++].PutValue("" + rec.Value);
                    _DetailCells[_DetailRowIndex, columnIndex++].PutValue("" + rec.Points);
                }
            }
        }

        #endregion

    }


}
