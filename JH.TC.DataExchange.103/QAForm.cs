﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JH.TC.DataExchange._103
{
    public partial class QAForm : BaseForm
    {
        public QAForm()
        {
            InitializeComponent();

            // InitDataGridView

            DataGridViewRow dgvrow1 = new DataGridViewRow();
            dgvrow1.CreateCells(dataGridViewX1);
            dgvrow1.Cells[0].Value = "均衡學習";
            //dgvrow1.Cells[1].Value = "系統根據學生的學期歷程資料讀取學生就學期間的學年度、學期，進一步的去比對學生的學期科目成績，將屬於健體、藝文、綜合這三個領域的學期科目成績做五學期的平均計算。";
            //dgvrow1.Cells[1].Value = "系統根據學生的學期歷程資料讀取學生就學期間的學年度、學期，進一步的去比對學生的學期科目成績，將屬於健體、藝術、綜合、科技這四個領域的學期科目成績做五學期的平均計算。";
            dgvrow1.Cells[1].Value = "系統根據學生的學期歷程資料讀取學生就學期間的學年度、學期，進一步的去比對學生的學期科目成績，將屬於  健體、藝術、綜 合、科技四領域五學期平均成績「各」達 60 分(含)以上者，方可「各」獲 3 分，四領域皆符合者即可累積獲 12 分。 ";
            dgvrow1.Cells[2].Value = "12分";
            dataGridViewX1.Rows.Add(dgvrow1);

            DataGridViewRow dgvrow2 = new DataGridViewRow();
            dgvrow2.CreateCells(dataGridViewX1);
            dgvrow2.Cells[0].Value = "德行表現";
            //dgvrow2.Cells[1].Value = "系統根據學生的學期歷程資料讀取學生就學期間的學年度、學期，進一步的去讀取學生的社團資料與服務學習紀錄。而需特別注意的部分是比序積分採計的社團資料為社團學期結算後資料，而服務學習紀錄根據採計截止日期的設定去計算。";
            dgvrow2.Cells[1].Value = "系統根據學生的學期歷程資料，讀取學生就學期間的學年度、學期，進一步的去讀取學生的社團資料與服務學習紀錄。需特別注意的部分是比序積分採計的社團資料為社團學期結算後資料，而服務學習紀錄根據採計截止日期的設定去計算。任一學期參加一項校內社團者給1 分。 任一學期累積服務滿6小時者給1 分，未滿者不予計分。 ";
            dgvrow2.Cells[2].Value = "社團(2分)、服務學習(3分)";
            dataGridViewX1.Rows.Add(dgvrow2);

            DataGridViewRow dgvrow3 = new DataGridViewRow();
            dgvrow3.CreateCells(dataGridViewX1);
            dgvrow3.Cells[0].Value = "無記過紀錄";
            //dgvrow3.Cells[1].Value = "根據採計截止日期的設定去計算懲戒明細紀錄、懲戒非明細資料與銷過紀錄。";
            dgvrow3.Cells[1].Value = "根據採計截止日期的設定去計算懲戒明細紀錄、懲戒非明細資料與銷過紀錄。無處分紀錄者6分；銷過後無懲處紀錄者6分；銷過後無小過(含)以上紀錄者3分。依銷過後計算(不得功過相抵)。3次警告折算1次小過。";
            dgvrow3.Cells[2].Value = "6分";
            dataGridViewX1.Rows.Add(dgvrow3);

            DataGridViewRow dgvrow4 = new DataGridViewRow();
            dgvrow4.CreateCells(dataGridViewX1);
            dgvrow4.Cells[0].Value = "獎勵紀錄";
            //dgvrow4.Cells[1].Value = "根據採計截止日期的設定去計算獎勵明細紀錄與獎勵非明細紀錄。";
            dgvrow4.Cells[1].Value = "根據採計截止日期的設定去計算獎勵明細紀錄與獎勵非明細紀錄。大功每次3分；小功每次1分； 嘉獎每次0.5分。";
            dgvrow4.Cells[2].Value = "4分";
            dataGridViewX1.Rows.Add(dgvrow4);

        }
    }
}
