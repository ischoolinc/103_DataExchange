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
        //public static List<string> SpecialList = new List<string>() { "原住民", "派外人員子女", "蒙藏生", "回國僑生", "港澳生", "退伍軍人", "境外優秀科學技術人才子女", "智能障礙", "視覺障礙", "聽覺障礙", "語言障礙", "肢體障礙", "身體病弱", "情緒行為障礙", "學習障礙", "多重障礙", "自閉症", "其他障礙", "低收入戶", "中低收入戶", "失業勞工子女", "原住民是否含母語認證", "就近入學" };
        public static List<string> SpecialList = new List<string>() { "身障生", "原住民(有認證)", "原住民(無認證)", "蒙藏生", "外派子女25%", "外派子女15%", "外派子女10%", "退伍軍人25%", "退伍軍人20%", "退伍軍人15%", "退伍軍人10%", "退伍軍人5%", "退伍軍人3%", "優秀子女25%", "優秀子女15%", "優秀子女10%", "僑生", "智能障礙", "視覺障礙", "聽覺障礙", "語言障礙", "肢體障礙", "腦性麻痺", "身體病弱", "情緒行為障礙", "學習障礙", "多重障礙", "自閉症", "發展遲緩", "其他障礙", "低收入戶", "中低收入戶", "失業勞工子女", "就近入學", "扶助弱勢", "偏遠鄉鎮國中生", "本土語言認證", "本土語言認證證書:原住民族語", "本土語言認證證書:客語", "本土語言認證證書:閩南語" };

        //本土語言
        public static List<string> StatusList = new List<string>() { "本土語言認證", "本土語言認證證書:原住民族語", "本土語言認證證書:客語", "本土語言認證證書:閩南語" };

        //學生報名身分
        public static List<string> SignUpStatusList = new List<string>() { "身障生", "原住民(有認證)", "原住民(無認證)", "蒙藏生", "外派子女25%", "外派子女15%", "外派子女10%", "退伍軍人25%", "退伍軍人20%", "退伍軍人15%", "退伍軍人10%", "退伍軍人5%", "退伍軍人3%", "優秀子女25%", "優秀子女15%", "優秀子女10%", "僑生" };

        //身心障礙
        public static List<string> HandicappedList = new List<string>() { "智能障礙", "視覺障礙", "聽覺障礙", "語言障礙", "肢體障礙", "腦性麻痺", "身體病弱", "情緒行為障礙", "學習障礙", "多重障礙", "自閉症", "發展遲緩", "其他障礙", };

        //其他
        public static List<string> OtherList = new List<string>() { "低收入戶", "中低收入戶", "失業勞工子女", "就近入學", "扶助弱勢", "偏遠鄉鎮國中生" };




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
            StudentTag2.Items.Clear();
            StudentTag2.Items.Add("");
            StudentTag3.Items.Clear();
            StudentTag3.Items.Add("");
            StudentTag4.Items.Clear();
            StudentTag4.Items.Add("");

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
            {
                StudentTag.Items.Add(name);
                StudentTag2.Items.Add(name);
                StudentTag3.Items.Add(name);
                StudentTag4.Items.Add(name);
            }

            _MapRecords = _AccessHelper.Select<MapRecord>();
            DataGridViewRow row;
            int index;
            foreach (string key in StatusList)
            {
                index = 1;
                row = new DataGridViewRow();
                row.CreateCells(dgvStatus);
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
                dgvStatus.Rows.Add(row);
            }
            foreach (string key in SignUpStatusList)
            {
                index = 1;
                row = new DataGridViewRow();
                row.CreateCells(dgvSignUpStatus);
                row.Cells[0].Value = key;
                foreach (MapRecord item in _MapRecords)
                {
                    if (key == item.key)
                    {
                        if (index == 1)
                        {
                            if (StudentTag2.Items.Contains(item.value))
                                row.Cells[1].Value = item.value;
                            else
                                row.Cells[1].Value = "";

                            if (StudentTag2.Items.Contains(item.note))
                                row.Cells[2].Value = item.note;
                            else
                                row.Cells[2].Value = "";
                        }
                        index++;
                    }
                }
                dgvSignUpStatus.Rows.Add(row);
            }
            foreach (string key in HandicappedList)
            {
                index = 1;
                row = new DataGridViewRow();
                row.CreateCells(dgvHandicapped);
                row.Cells[0].Value = key;
                foreach (MapRecord item in _MapRecords)
                {
                    if (key == item.key)
                    {
                        if (index == 1)
                        {
                            if (StudentTag3.Items.Contains(item.value))
                                row.Cells[1].Value = item.value;
                            else
                                row.Cells[1].Value = "";

                            if (StudentTag3.Items.Contains(item.note))
                                row.Cells[2].Value = item.note;
                            else
                                row.Cells[2].Value = "";
                        }
                        index++;
                    }
                }
                dgvHandicapped.Rows.Add(row);
            }
            foreach (string key in OtherList)
            {
                index = 1;
                row = new DataGridViewRow();
                row.CreateCells(dgvOther);
                row.Cells[0].Value = key;
                foreach (MapRecord item in _MapRecords)
                {
                    if (key == item.key)
                    {
                        if (index == 1)
                        {
                            if (StudentTag4.Items.Contains(item.value))
                                row.Cells[1].Value = item.value;
                            else
                                row.Cells[1].Value = "";

                            if (StudentTag4.Items.Contains(item.note))
                                row.Cells[2].Value = item.note;
                            else
                                row.Cells[2].Value = "";
                        }
                        index++;
                    }
                }
                dgvOther.Rows.Add(row);
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
            for (int i = 0; i < StatusList.Count; i++)
            {
                DataGridViewRow row = dgvStatus.Rows[i];
                if ("" + row.Cells[0].Value == StatusList[i])
                {
                    mr = new MapRecord();
                    mr.key = StatusList[i];
                    mr.value = "" + row.Cells["StudentTag"].Value;
                    mr.note = "" + row.Cells["note"].Value;
                    _MapRecords.Add(mr);
                }
            }
            for (int i = 0; i < SignUpStatusList.Count; i++)
            {
                DataGridViewRow row = dgvSignUpStatus.Rows[i];
                if ("" + row.Cells[0].Value == SignUpStatusList[i])
                {
                    mr = new MapRecord();
                    mr.key = SignUpStatusList[i];
                    mr.value = "" + row.Cells["StudentTag2"].Value;
                    mr.note = "" + row.Cells["note2"].Value;
                    _MapRecords.Add(mr);
                }
            }
            for (int i = 0; i < HandicappedList.Count; i++)
            {
                DataGridViewRow row = dgvHandicapped.Rows[i];
                if ("" + row.Cells[0].Value == HandicappedList[i])
                {
                    mr = new MapRecord();
                    mr.key = HandicappedList[i];
                    mr.value = "" + row.Cells["StudentTag3"].Value;
                    mr.note = "" + row.Cells["note3"].Value;
                    _MapRecords.Add(mr);
                }
            }
            for (int i = 0; i < OtherList.Count; i++)
            {
                DataGridViewRow row = dgvOther.Rows[i];
                if ("" + row.Cells[0].Value == OtherList[i])
                {
                    mr = new MapRecord();
                    mr.key = OtherList[i];
                    mr.value = "" + row.Cells["StudentTag4"].Value;
                    mr.note = "" + row.Cells["note4"].Value;
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
