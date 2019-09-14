using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CreepRateApp.Form
{
    public partial class HistoryHandAnalysis : DevExpress.XtraEditors.XtraForm
    { 
        public HistoryHandAnalysis()
        {
            InitializeComponent();

            List<entity.AnalysisModel> analysisModelList = Core.DataBaseTools.GetAll<entity.AnalysisModel>("AnalysisResult");
            this.gridControl1.DataSource = analysisModelList;
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            int selectIndex = this.gridView1.FocusedRowHandle;
            var queryAllList = Core.DevexpressTools.GetGridViewFilteredAndSortedData(this.gridView1);
            entity.AnalysisModel am = (entity.AnalysisModel)queryAllList[selectIndex];
            List<entity.FileInfo> fileInfoList = am.FileInfoList;
            if (fileInfoList.Count > 0)
            {
                Form.FileList fl = new FileList(fileInfoList);
                fl.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("对不起，此次分析无附件可以查阅", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}