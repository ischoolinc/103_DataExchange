using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JH.TC.DataExchange._103
{
    public partial class AbsenceMap : BaseForm
    {
        private FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();
        List<K12.Data.AbsenceMappingInfo> amil;
        List<K12.Data.PeriodMappingInfo> pmil ;
        private List<AbsenceMapRecord> _MapRecords = new List<AbsenceMapRecord>();
        List<string> period_types;
        public AbsenceMap()
        {
            InitializeComponent();
            amil = K12.Data.AbsenceMapping.SelectAll();
            pmil = K12.Data.PeriodMapping.SelectAll();
            _MapRecords = _AccessHelper.Select<AbsenceMapRecord>();
            DataGridViewColumn dgvc ;
            foreach (K12.Data.AbsenceMappingInfo each in amil)
            {
                dgvc  = new DataGridViewCheckBoxColumn();
                dgvc.Width = 40;
                dgvc.Name = each.Name;
                dataGridView1.Columns.Add(dgvc); 
            }
            DataGridViewRow row;
            period_types = new List<string>();
            foreach (K12.Data.PeriodMappingInfo each in pmil)
            {
                if (!period_types.Contains(each.Type))
                    period_types.Add(each.Type);
            }
            foreach (string each in period_types)
            {
                row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = each;
                for (int i = 0; i < amil.Count; i++)
                {
                    row.Cells[i + 1].Value = false;
                    foreach (AbsenceMapRecord item in _MapRecords)
                    {
                        if (item.absence == amil[i].Name && item.period_type == each)
                            row.Cells[i + 1].Value = true;
                    }
                }
                dataGridView1.Rows.Add(row);
            }
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            _AccessHelper.DeletedValues(_MapRecords);
            _MapRecords.Clear();
            AbsenceMapRecord amr;
            for (int i = 0; i < period_types.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                for (int j = 0; j < amil.Count; j++)
                {
                    if ((bool)row.Cells[j + 1].Value == true)
                    {
                        amr = new AbsenceMapRecord();
                        amr.period_type = period_types[i];
                        amr.absence = amil[j].Name;
                        _MapRecords.Add(amr);
                    }
                }
            }
            _MapRecords.SaveAll();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
