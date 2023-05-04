using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JH.HS.DataExchange._103
{
    public partial class QAForm : BaseForm
    {
        public QAForm()
        {
            InitializeComponent();

            // InitDataGridView

            DataGridViewRow dgvrow1 = new DataGridViewRow();
            dgvrow1.CreateCells(dataGridViewX1);
            dgvrow1.Cells[0].Value = "扶助弱勢";
            dgvrow1.Cells[1].Value = "系統類別欄位、有標籤者符合。扶助弱勢標籤+偏遠鄉鎮國中生符合者( 5 分 )，扶助弱勢標籤符合者( 3 分 )。";
            dgvrow1.Cells[2].Value = "扶助弱勢/就近入學/均衡學習,合併上限( 30 分 )";
            dataGridViewX1.Rows.Add(dgvrow1);

            DataGridViewRow dgvrow2 = new DataGridViewRow();
            dgvrow2.CreateCells(dataGridViewX1);
            dgvrow2.Cells[0].Value = "就近入學";
            dgvrow2.Cells[1].Value = "系統類別欄位、有標籤者符合。就近入學標籤符合者( 5 分 )。";
            dgvrow2.Cells[2].Value = "扶助弱勢/就近入學/均衡學習,合併上限( 30 分 )";
            dataGridViewX1.Rows.Add(dgvrow2);

            DataGridViewRow dgvrow3 = new DataGridViewRow();
            dgvrow3.CreateCells(dataGridViewX1);
            dgvrow3.Cells[0].Value = "均衡學習";
            dgvrow3.Cells[1].Value = @"以健體、藝術及綜合三項領域採計之各學期加總平均成績達及格者換算，3 領域皆符合( 15 分)，2 領域皆符合( 10 分)，1 領域皆符合 (5 分)";
            dgvrow3.Cells[2].Value = "扶助弱勢/就近入學/均衡學習,合併上限( 30 分 )";
            dataGridViewX1.Rows.Add(dgvrow3);

            DataGridViewRow dgvrow4 = new DataGridViewRow();
            dgvrow4.CreateCells(dataGridViewX1);
            dgvrow4.Cells[0].Value = "日常生活表現評量";
            dgvrow4.Cells[1].Value = "功過相抵後無懲戒( 10 分)、單一學期無曠課紀錄( 2 分 )、功過相抵後獎勵紀錄大功( 4.5 分 )小功( 1.5 分 )嘉獎( 0.5 分 )";
            dgvrow4.Cells[2].Value = "無懲戒( 10分 )、無曠課紀錄( 12分 )、獎勵紀錄( 20分 )";
            dataGridViewX1.Rows.Add(dgvrow4);

            DataGridViewRow dgvrow5 = new DataGridViewRow();
            dgvrow5.CreateCells(dataGridViewX1);
            dgvrow5.Cells[0].Value = "服務學習";
            dgvrow5.Cells[1].Value = "根據採計截止日期的設定去計算滿 3 小時( 1分 )、每學期最多( 2分 )、五學期最多( 10分 )。";
            dgvrow5.Cells[2].Value = "10分";
            dataGridViewX1.Rows.Add(dgvrow5);

            DataGridViewRow dgvrow6 = new DataGridViewRow();
            dgvrow6.CreateCells(dataGridViewX1);
            dgvrow6.Cells[0].Value = "本土語言認證";
            dgvrow6.Cells[1].Value = "系統的類別欄位、有標籤者符合。。本土語言認證標籤符合者( 2 分 )。";
            dgvrow6.Cells[2].Value = "2分";
            dataGridViewX1.Rows.Add(dgvrow6);

        }
    }
}
