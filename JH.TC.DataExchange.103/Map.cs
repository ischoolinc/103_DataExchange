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

namespace JH.TC.DataExchange._103
{
    public partial class Map : BaseForm
    {
        public string date { get; set; }
        private List<TagConfigRecord> _TagConfigRecords = new List<TagConfigRecord>();

        private FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();
        private List<MapRecord> _MapRecords = new List<MapRecord>();
        //public static List<string> SpecialList = new List<string>() { "原住民", "派外人員子女", "蒙藏生", "回國僑生", "港澳生", "退伍軍人", "境外優秀科學技術人才子女", "智能障礙", "視覺障礙", "聽覺障礙", "語言障礙", "肢體障礙", "腦性麻痺", "身體病弱", "情緖行為障礙", "學習障礙", "多重障礙", "自閉症", "發展遲緩", "其他障礙", "低收入戶", "中低收入戶", "失業勞工子女", "原住民是否含母語認證", "非中華民國身分證號", "就近入學", "偏遠地區" };
        public static List<string> SpecialList = new List<string>() { "原住民", "派外人員子女", "蒙藏生", "回國僑生", "港澳生", "退伍軍人", "境外優秀科學技術人才子女", "智能障礙", "視覺障礙", "聽覺障礙", "語言障礙", "肢體障礙", "腦性麻痺", "身體病弱", "情緖行為障礙", "學習障礙", "多重障礙", "自閉症", "發展遲緩", "其他障礙", "低收入戶", "中低收入戶", "失業勞工子女", "非中華民國身分證號", "就近入學", "偏遠地區", "身障生", "原住民(有認證)", "原住民(無認證)", "蒙藏生", "外派子女25%", "外派子女15%", "外派子女10%", "退伍軍人25%", "退伍軍人20%", "退伍軍人15%", "退伍軍人10%", "退伍軍人5%", "退伍軍人3%", "優秀子女25%", "優秀子女15%", "優秀子女10%", "僑生" };
        public string AbsenceType = "";
        public Map()
        {
            InitializeComponent();
            _TagConfigRecords = K12.Data.TagConfig.SelectByCategory(TagCategory.Student);
            List<string> prefix = new List<string>();
            List<string> tag = new List<string>();

            StudentTag.Items.Clear();
            StudentTag.Items.Add("");
            foreach (var item in _TagConfigRecords)
            {
                if (item.Prefix != "")
                    StudentTag.Items.Add(item.Prefix + ":" + item.Name);
                else
                    StudentTag.Items.Add(item.Name);
            }
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

                            if (StudentTag.Items.Contains(item.note))
                                row.Cells[2].Value = item.note;
                            else
                                row.Cells[2].Value = "";
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
            date = dateTimeInput1.Value.ToString("yyyy/MM/dd");
            //string sql = SqlString.Query(dateTimeInput1.Value.ToString("yyyy/MM/dd"));
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

        private void qaLb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            (new QAForm()).ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            (new LeagleForm()).ShowDialog();
        }
    }
}
