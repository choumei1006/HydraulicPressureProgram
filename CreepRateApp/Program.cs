using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.IO;
using System.IO.Ports;

namespace CreepRateApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //全局异常处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ExceptionDealFunction);

            //软件启动时，首先清楚temp临时文件内的临时文件
            string tempFolder = Application.StartupPath + "\\temp\\";
            if (Directory.Exists(tempFolder))
            {
                DeleteDirFile(tempFolder);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            //读取配置文件串口参数
            Properties.Settings settings = Properties.Settings.Default;
            GlobalValue.PortName = settings.PortName;
            GlobalValue.BaudRate = settings.BaudRate;
            GlobalValue.DataBits = settings.DataBits;
            GlobalValue.StopBits = settings.StopBits;
            //GlobalValue.CodeType = settings.CodeType;

            if (settings.Parity == "None")
                GlobalValue.Parity = Parity.None;
            else if (settings.Parity == "Odd")
                GlobalValue.Parity = Parity.Odd;
            else if (settings.Parity == "Even")
                GlobalValue.Parity = Parity.Even;
            else if (settings.Parity == "Mark")
                GlobalValue.Parity = Parity.Mark;
            else if (settings.Parity == "Space")
                GlobalValue.Parity = Parity.Space;
            else
                GlobalValue.Parity = Parity.None;

            GlobalValue.IntalvasTime = settings.IntervalTime;

            Application.Run(new MainForm());
        }

        static void ExceptionDealFunction(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
            }
            catch { }

            string str = "";
            string strDateInfo = "Error: " + DateTime.Now.ToString() + " ";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + " Error Type:{0}\r\n Error Message:{1}\r\n Error Infomation:{2}\r\n", error.GetType(), error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Program thread error:{0}", e);
            }

            //开发人员看
            MessageBox.Show(str, "当您看到这个窗口时，请将此窗口截图及复制窗体内容，反馈给售后工程师。", MessageBoxButtons.OK, MessageBoxIcon.Error);

            string userMessage = "Something Error: \r\n 1.Please check your input or output. \r\n 2.Please check program configuration(e.g.FTP configuration, Database Configuration). \r\n 3.The Map or Layer Date is Empty or mistake may cause errors in the operation of some software plugins or modules. \r\n 4.Maybe the plugin operate exception.\r\n 5.If the above method can not solve the problem, please try to contact the technical service staff.";
            //用户看
            //se = new SystemError(userMessage);
            //se.ShowDialog();
        }

        static void DeleteDirFile(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);
                    }
                    else
                    {
                        File.Delete(i.FullName);
                    }
                }
            }
            catch { }
        }

    }
}
