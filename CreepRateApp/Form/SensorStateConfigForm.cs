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
using SerialportSample;
using com.sun.xml.@internal.bind.v2; 

using System.Reflection;

using System.Threading;
using System.Net;
using System.Net.Sockets; 

namespace CreepRateApp
{
    public partial class SensorStateConfigForm : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraBars.Ribbon.RibbonForm _form;
        private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。

        //public SensorStateConfigForm(SerialPort paramPortDev)
        public SensorStateConfigForm(DevExpress.XtraBars.Ribbon.RibbonForm form)
        {
            InitializeComponent();
            //mSerialPort = paramPortDev;
            //mSerialPort.ReceivedBytesThreshold = 1; 
            this._form = form;

            //获取静态配置类中的配置值
            List<string> configList = SensorStateConfigValue.getSensorStateConfigList();

            if (null == configList || configList.Count != 5)
            {

            }
            else
            {
                //1-5循环显示在相应输入框
                for (int i = 1; i <= 5; i++)
                {
                    Boolean configVal = configList[i - 1] == "True" ? true : false;
                    Control control = Controls.Find("toggleSwitch" + Convert.ToString(i), true)[0];
                    control.GetType().GetProperty("IsOn").SetValue(control, configVal, null);
                }
            }
  
        }
        /// <summary>
        /// 保存，并发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> stateConfigValues = new List<string>();  //存放故障配置值

            //int isVerified = 0;

            try
            {
                for (int i = 1; i <= 5; i++) {
                    Control control = Controls.Find("toggleSwitch"+Convert.ToString(i),true)[0];
                    string value = control.GetType().GetProperty("IsOn").GetValue(control,null).ToString();
                    stateConfigValues.Add(value);
                }
                SensorStateConfigValue.setSensorStateConfigValue(stateConfigValues);

                //检查故障配置合理性并初始化保存
                //if (toggleSwitch1.IsOn) {
                //    SensorStateConfigValue.Valve1 = 1;
                //}
                //if (toggleSwitch2.IsOn)
                //{
                //    SensorStateConfigValue.Valve2 = 1;
                //}
                //if (toggleSwitch3.IsOn)
                //{
                //    SensorStateConfigValue.Valve3 = 1;
                //}
                //if (toggleSwitch4.IsOn)
                //{
                //    SensorStateConfigValue.Valve4 = 1;
                //}
                //if (toggleSwitch5.IsOn)
                //{
                //    SensorStateConfigValue.Valve5 = 1;
                //}
                 

                //生成配置信息 byte数组 对应的 16进制字符串数组
                string sendCmdStr = SensorStateConfigValue.getSendCmd();

                //将上述16进制字符串数组 拼接为 0x_ _ 格式 的字符串
                /*string sendCmdStr = "";
                for (int i = 0; i < sendCmd.Length; i++)
                {
                    sendCmdStr += "0x" + sendCmd[i] + " ";
                }
                 * */

                //下发通道配置信息
                //1、关闭线程 
                //MainForm.thrRecv.Abort();    //所谓的关闭线程
                //MainForm.thrRecv.Join();    //挂起
                //2、关闭udpcRecv
                //MainForm.udpcRecv.Close();
                //MainForm.udpcRecv = null;
                //3、创建udpcSend

                //4、创建thrSend
                MainForm.thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                MainForm.thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[传感器状态配置信息下发]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));



                XtraMessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();

            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "配置异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// 读取配置值，并显示在相应的框中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //获取静态配置类中的配置值
            List<string> configList = SensorStateConfigValue.getSensorStateConfigList();
            
            if (null == configList || configList.Count != 5)
            {
                XtraMessageBox.Show("读取失败，请完成配置再读取哦！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //1-5循环显示在相应输入框
            for (int i = 1; i <= 5; i++)
            {
                Boolean configVal = configList[i - 1] == "True" ? true : false;
                Control control = Controls.Find("toggleSwitch" + Convert.ToString(i), true)[0];
                control.GetType().GetProperty("IsOn").SetValue(control, configVal, null); 
            }
            XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

       
        
        
    }
}