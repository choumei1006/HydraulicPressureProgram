using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.IO;

namespace CreepRateApp.Core
{
    public static class ReadTxt
    {
        public static string[] ReadTxtFile(string filePath)
        {
            try
            {
                string tempContent = File.ReadAllText(filePath);
                string txtContent = tempContent.Replace("\r\n", "").Replace("\n\r", "").Trim();
                if (string.IsNullOrWhiteSpace(txtContent))
                    return null;
                else
                {
                    string[] groupTxt = txtContent.Split(' ');
                    string[] resultTxt = new string[groupTxt.Length - 2];
                    for (int i = 0; i < resultTxt.Length; i++)
                    {
                        if (Core.CheckData.IsNumeric(groupTxt[i+2]))
                        {
                            //resultTxt[i] = ((double.Parse(groupTxt[i + 2]) - 32) * 5 / 9).ToString("0");
                            resultTxt[i] = groupTxt[i + 2];
                        }
                    }
                    return resultTxt;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "软件发生了错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
