using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace KH.DataExchange._103
{
    public partial class frmSelectGradeYear : BaseForm
    {

        List<string> GradeYearList;

        string SelectedGradeYear = "";

        public frmSelectGradeYear()
        {
            InitializeComponent();
            GradeYearList = new List<string>();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            SelectedGradeYear = cboGradeYear.Text;
            this.DialogResult = DialogResult.OK;
        }

        // 取得選擇的年級
        public string GetSelectedGradeYear()
        {
            return SelectedGradeYear;
        }

        private void frmSelectGradeYear_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;
            btnRun.Enabled = false;

            // 取得年級資料
            GradeYearList = SqlString.GetClassGradeYear();
            foreach (string gr in GradeYearList)
            {
                cboGradeYear.Items.Add(gr);
            }

            if (cboGradeYear.Items.Count > 0)
                cboGradeYear.SelectedIndex = 0;

            cboGradeYear.DropDownStyle = ComboBoxStyle.DropDownList;

            btnRun.Enabled = true;
        }
    }
}
