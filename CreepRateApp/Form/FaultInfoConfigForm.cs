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
    public partial class FaultInfoConfigForm : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraBars.Ribbon.RibbonForm _form;
        //下拉框选值
        public static Byte ValueSate = 0;
        public static Byte Value1 = 0;
        public static Byte Value2 = 0;


        private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int[] fctext = new int[50];//故障配置
        
        //public FaultInfoConfigForm(SerialPort paramPortDev)
        public FaultInfoConfigForm(DevExpress.XtraBars.Ribbon.RibbonForm form)
        {
            InitializeComponent();
            //mSerialPort = paramPortDev;
            //mSerialPort.ReceivedBytesThreshold = 1; 
            this._form = form;

            //初始化配置值
            //获取静态配置类中的配置值
            List<string> configList = FaultInfoConfigValue.getConfigList();
            if (null == configList || configList.Count != 25)
            {
                 
            }
            else
            {
                //1-23循环显示在相应输入框
                for (int i = 1; i <= 23; i++)
                {
                    string configVal = configList[i - 1];
                    Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                    control.GetType().GetProperty("Text").SetValue(control, configVal, null);
                    //String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                }

                //combox设置值
                for (int i = 1; i <= 2; i++)
                { 
                    Control control = Controls.Find("comboBox" + Convert.ToString(i), true)[0]; 
                    control.GetType().GetProperty("SelectedIndex").SetValue(control, configList[22 + i] == "0" ? 1 : 0, null);
                }
            }

        }

        /// <summary>
        /// 保存配置，并发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> faultConfigValues = new List<string>();  //存放故障配置值

            int isVerified = 0;

            //try
            //{
                //检查故障配置合理性
                for (int i = 1; i <= 23; i++)
                {
                    Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                    String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                    if (!string.IsNullOrWhiteSpace(value))  //&& value != "0"
                    {
                        faultConfigValues.Add(value);
                    }
                    else
                    {
                        XtraMessageBox.Show("存在未配置通道，请检查更改！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    
                }
                isVerified = 23;
           
                //combox取值
                
                for (int i = 1; i <= 2; i++)
                {
                    Control control = Controls.Find("comboBox" + Convert.ToString(i), true)[0];
                    Type tp = control.GetType();
                    PropertyInfo pt = control.GetType().GetProperty("SelectedText");
                    object obj = control.GetType().GetProperty("SelectedText").GetValue(control, null);
                    String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                    if (value == "高电平")
                    {
                        faultConfigValues.Add(Convert.ToString(1));
                        isVerified ++;
                    }
                    else if (value == "低电平")
                    {
                        faultConfigValues.Add(Convert.ToString(0));
                        isVerified ++ ;
                    }
                    else { 
                        
                    } 
                }


                //判断是否完成所有故障配置合理性检查
                if (isVerified == 25)
                {
                    //初始化故障配置信息类 
                    FaultInfoConfigValue.setFaultConfigValue(faultConfigValues);

                    //生成配置信息 byte数组 对应的 16进制字符串数组
                    string sendCmdStr = FaultInfoConfigValue.getSendCmd();

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
                    MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[故障配置信息下发]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                    XtraMessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    XtraMessageBox.Show("存在未配置项，请检查更改！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            //}
            //catch (Exception exception)
            //{
                //XtraMessageBox.Show(exception.Message, "配置异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        /// <summary>
        /// 获取故障配置值,并显示在相应输入框中【已废弃】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_used(object sender, EventArgs e)
        {
            //获取静态配置类中的配置值
            List<string> configList = FaultInfoConfigValue.getConfigList();
            if (null == configList || configList.Count != 25) {
                XtraMessageBox.Show("读取失败，请完成配置再读取哦！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //1-23循环显示在相应输入框
            for (int i = 1; i <= 23; i++)
            {
                string configVal = configList[i-1]; 
                Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                control.GetType().GetProperty("Text").SetValue(control, configVal, null);
                //String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
            }

            //combox设置值
            for (int i = 1; i <= 2; i++)
            { 
                Control control = Controls.Find("comboBox" + Convert.ToString(i), true)[0]; 

                control.GetType().GetProperty("SelectedIndex").SetValue(control, configList[22 + i] == "0" ? 1:0, null);
            }

            XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        } 

        /// <summary>
        /// 获取故障配置值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //生成配置信息 byte数组 对应的 16进制字符串数组
                byte[] cmd = new byte[9];

                //Header
                cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
                cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
                //Device_id
                cmd[2] = MainForm.EquipmentId;
                //Reserve
                cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //--Category
                cmd[4] = byte.Parse("05", System.Globalization.NumberStyles.HexNumber);

                //Len(2 byte)
                cmd[5] = 1;
                cmd[6] = 0;


                //data  
                cmd[7] = byte.Parse("01", System.Globalization.NumberStyles.HexNumber);


                //Verify
                byte verifyByte = 0;
                for (int i = 0; i < cmd.Length; i++)
                {
                    verifyByte ^= cmd[i];
                }
                cmd[8] = verifyByte;

                //转换为十六进制字符串
                String sendCmdStr = "";
                for (int i = 0; i < cmd.Length; i++)
                {
                    StringBuilder hexStr = new StringBuilder(cmd[i].ToString("X2"));
                    sendCmdStr += "0x" + hexStr + " ";
                }

                //===============================================


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

                //获取当前FaultInfoConfigValue的最后一次修改时间
                long lastUpdateTime = FaultInfoConfigValue.updateTime;

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                MainForm.thrSend.Start(sendCmdStr); 

                //6、在主界面显示发送内容 
                MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[获取故障配置信息]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));

                int index = 0;
                while (true)
                {
                    index++;
                    if (index >= 20)
                    {
                        XtraMessageBox.Show("读取超时，请稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       break;
                    }
                    if (FaultInfoConfigValue.updateTime < 0)
                    {
                        XtraMessageBox.Show("读取失败，故障信息未配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    if (FaultInfoConfigValue.updateTime > lastUpdateTime)
                    { 
                        //将配置值显示在配置框中
                        //获取静态配置类中的配置值
                        List<string> configList = FaultInfoConfigValue.getConfigList();
                        if (null == configList || configList.Count != 25)
                        {
                            XtraMessageBox.Show("读取失败，配置信息不完整！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        //1-23循环显示在相应输入框
                        for (int i = 1; i <= 23; i++)
                        {
                            string configVal = configList[i - 1];
                            Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                            control.GetType().GetProperty("Text").SetValue(control, configVal, null);
                            //String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                        }

                        //combox设置值
                        for (int i = 1; i <= 2; i++)
                        {
                            Control control = Controls.Find("comboBox" + Convert.ToString(i), true)[0];

                            control.GetType().GetProperty("SelectedIndex").SetValue(control, configList[22 + i] == "0" ? 1 : 0, null);
                        } 

                        XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                }

                //XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
          
    }
}