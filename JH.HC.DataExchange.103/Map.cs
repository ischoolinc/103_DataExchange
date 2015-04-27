using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JH.HS.DataExchange._103
{
    public partial class Map : BaseForm
    {
        private List<TagConfigRecord> _TagConfigRecords = new List<TagConfigRecord>();

        private FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();
        private List<MapRecord> _MapRecords = new List<MapRecord>();
        public static List<string> SpecialList = new List<string>() { "原住民", "派外人員子女", "蒙藏生", "回國僑生", "港澳生", "退伍軍人", "境外優秀科學技術人才子女", "智能障礙", "視覺障礙", "聽覺障礙", "語言障礙", "肢體障礙", "身體病弱", "情緒行為障礙", "學習障礙", "多重障礙", "自閉症", "其他障礙", "低收入戶", "中低收入戶", "失業勞工子女", "原住民是否含母語認證", "就近入學" };
        public string AbsenceType = "";
        public Map()
        {
            InitializeComponent();
            _TagConfigRecords = K12.Data.TagConfig.SelectByCategory(TagCategory.Student);
            List<string> prefix = new List<string>();
            List<string> tag = new List<string>();
            List<string> StudTagNameList = new List<string>();            

            StudentTag.Items.Clear();
            StudentTag.Items.Add("");
            foreach (var item in _TagConfigRecords)
            {
                //if (item.Prefix != "")
                //    StudentTag.Items.Add(item.Prefix + ":" + item.Name);
                //else
                //    StudentTag.Items.Add(item.Name);
                
                if (item.Prefix=="")
                {
                    if(!StudTagNameList.Contains(item.Name))
                        StudTagNameList.Add(item.Name);
                }
                else
                {
                    string pNmae = "[" + item.Prefix + "]";
                    string kName = item.Prefix + ":" + item.Name;
                    if (!StudTagNameList.Contains(pNmae))
                        StudTagNameList.Add(pNmae);
                    // 原本子項目也加入
                    StudTagNameList.Add(kName);
                }              
            }

            // 排序並加入
            StudTagNameList.Sort();
            foreach (string name in StudTagNameList)
                StudentTag.Items.Add(name);

            _MapRecords = _AccessHelper.Select<MapRecord>();
            DataGridViewRow row;
            int index;
            foreach (string key in SpecialList)
            {
                index = 1;
                row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = key;
                foreach (MapRecord item in _MapRecords)
                {
                    if (key == item.key)
                    {
                        if (index == 1)
                        {
                            if (StudentTag.Items.Contains(item.value))
                                row.Cells[1].Value = item.value;
                            else
                                row.Cells[1].Value = "";                          
                         
                            row.Cells[2].Value = item.note;                         
                        }
                        index++;
                    }
                }
                dataGridView1.Rows.Add(row);
            }
            List<K12.Data.AbsenceMappingInfo> infoList = K12.Data.AbsenceMapping.SelectAll();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            _AccessHelper.DeletedValues(_MapRecords);
            _MapRecords.Clear();
            MapRecord mr;
            for (int i = 0; i < SpecialList.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                if ("" + row.Cells[0].Value == SpecialList[i])
                {
                    mr = new MapRecord();
                    mr.key = SpecialList[i];
                    mr.value = "" + row.Cells["StudentTag"].Value;
                    mr.note = "" + row.Cells["note"].Value;
                    _MapRecords.Add(mr);
                }
            }
            _MapRecords.SaveAll();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new AbsenceMap().ShowDialog();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void Map_Load(object sender, EventArgs e)
        {

        }
    }
}
