using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace CreepRateApp.Form
{
    public partial class FileList : DevExpress.XtraEditors.XtraForm
    {
        public FileList(List<entity.FileInfo> fileInfoList)
        {
            InitializeComponent();
            this.gridControl1.DataSource = fileInfoList;
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            int selectIndex = this.gridView1.FocusedRowHandle;
            var queryAllList = Core.DevexpressTools.GetGridViewFilteredAndSortedData(this.gridView1);
            entity.FileInfo fi = (entity.FileInfo)queryAllList[selectIndex];
            this.downloadFile(fi);
        }

        private void downloadFile(entity.FileInfo fi)
        {
            try
            {
                var readFile = Core.DataBaseTools.ReadFile(fi.Id);
                var fileStream = readFile.OpenRead();
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, (int)fileStream.Length);
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(this.folderBrowserDialog1.SelectedPath);
                    System.IO.FileStream tempFileStream = new System.IO.FileStream(this.folderBrowserDialog1.SelectedPath + "\\" + fi.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    tempFileStream.Write(fileBytes, 0, (int)fileStream.Length);
                    tempFileStream.Flush();
                    tempFileStream.Close();
                    XtraMessageBox.Show("从云端保存至本地计算机成功。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }
    }
}