using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO.Ports;

namespace CreepRateApp.Form
{
    public partial class SerialPortConfig : DevExpress.XtraEditors.XtraForm
    {
        //定义端口类
        private SerialPort ComDevice = new SerialPort();

        Properties.Settings settings = Properties.Settings.Default;

        public SerialPortConfig()
        {
            try
            {
                ComDevice.Close();
            }
            catch { }

            InitializeComponent();

            Init();
        }

        /// <summary>
        /// 配置初始化
        /// </summary>
        private void Init()
        {
            string[] portsList = SerialPort.GetPortNames();
            if (portsList.Length > 0)
            {
                this.comboBoxEdit1.Properties.Items.Clear();
                this.comboBoxEdit1.Properties.Items.AddRange(portsList);
                this.comboBoxEdit1.EditValue = portsList[0];
                this.comboBoxEdit1.Text = settings.PortName;

                this.comboBoxEdit2.Text = settings.BaudRate;
                this.comboBoxEdit3.Text = settings.DataBits;
                this.comboBoxEdit4.Text = settings.StopBits;
                this.comboBoxEdit5.Text = settings.Parity;
                this.textEditor1.Text = settings.IntervalTime;
            }
            else
            {
                XtraMessageBox.Show("未检测到串口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //端口
            if (!string.IsNullOrWhiteSpace(comboBoxEdit1.Text))
            {
                settings.PortName = comboBoxEdit1.Text;
                GlobalValue.PortName = comboBoxEdit1.Text;
            }
            //波特率
            if (!string.IsNullOrWhiteSpace(comboBoxEdit2.Text))
            {
                settings.BaudRate = comboBoxEdit2.Text;
                GlobalValue.BaudRate = comboBoxEdit2.Text;
            }
            //数据位
            if (!string.IsNullOrWhiteSpace(comboBoxEdit3.Text))
            {
                settings.DataBits = comboBoxEdit3.Text;
                GlobalValue.DataBits = comboBoxEdit3.Text;
            }
            //停止位
            if (!string.IsNullOrWhiteSpace(comboBoxEdit4.Text))
            {
                settings.StopBits = comboBoxEdit4.Text;
                GlobalValue.StopBits = comboBoxEdit4.Text;
            }
            //校验方式
            if (!string.IsNullOrWhiteSpace(comboBoxEdit5.Text))
            {
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
            }
            //间隔时间
            if(!string.IsNullOrWhiteSpace(textEditor1.Text))
            {
                settings.IntervalTime = textEditor1.Text;
                GlobalValue.IntalvasTime= textEditor1.Text;
            }

            settings.Save();

            XtraMessageBox.Show("串口配置成功,软件需要重新启动配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
    }
}