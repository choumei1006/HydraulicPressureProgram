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
    public partial class SensorSpanConfigForm : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraBars.Ribbon.RibbonForm _form;
        //private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int[] sctext = new int[50]; //传感器数据

        private UdpClient udpcSend;   //用于UDP发送的网络服务类 
        //private IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(localIpAddress), 10101); 

       
        //public SensorChannelConfigForm(SerialPort paramPortDev)
        public SensorSpanConfigForm(DevExpress.XtraBars.Ribbon.RibbonForm form)
        {
            InitializeComponent();
            //mSerialPort = paramPortDev;
            //mSerialPort.ReceivedBytesThreshold = 1;
            this._form = form;


            //初始化设置值
            //获取静态配置类中的配置值
            List<string> configList = SensorSpanConfigValue.getSensorSpanConfigValue();
            if (null == configList || configList.Count != 24)
            {

            }
            else
            {
                //1-24循环显示在相应输入框
                for (int i = 1; i <= 24; i++)
                {
                    string configVal = configList[i - 1];
                    Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                    control.GetType().GetProperty("Text").SetValue(control, configVal, null);
                    //String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                }
            }

        }

        private void config_current_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>
        /// 保存配置，并发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        { 
            
            List<string> spanConfigValues = new List<string>();  //存放量程配置值

            int isVerified = 0;

            try
            {
                //检查量程配置合理性
                for (int i = 1; i <= 24; i++)
                { 
                    Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                    String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();

                    spanConfigValues.Add(value);  
                                           
                    isVerified = i;
                }
                //判断是否完成所有传感器通道的配置合理性检查
                if (isVerified == 24)
                {
                    //初始化传感器通道配置信息类
                    SensorSpanConfigValue.setSpanConfigValue(spanConfigValues);

                    //生成配置信息 byte数组 对应的 16进制字符串数组
                    string sendCmdStr = SensorSpanConfigValue.getSendCmd();

                    //将上述16进制字符串数组 拼接为 0x_ _ 格式 的字符串
                    /*string sendCmdStr = "";
                    for (int i = 0; i < sendCmd.Length; i++) {
                        sendCmdStr+="0x"+sendCmd[i]+" ";
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
                    MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[传感器量程配置信息下发]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));
                     
                    XtraMessageBox.Show("指令已下发！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MainForm.isCollecting = true;
                    //this.Close();
                }
                else
                {
                    XtraMessageBox.Show("存在未配置量程，请检查更改！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception exception) {
                XtraMessageBox.Show(exception.Message, "配置异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            

            
        }

        /// <summary>
        /// 读取数据【已废弃】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_used(object sender, EventArgs e)
        {
            //获取静态配置类中的配置值
            List<string> configList = SensorSpanConfigValue.getSensorSpanConfigValue();
            if (null == configList || configList.Count != 24)
            {
                XtraMessageBox.Show("读取失败，请完成配置再读取哦！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //1-23循环显示在相应输入框
            for (int i = 1; i <= 24; i++)
            {
                string configVal = configList[i - 1];
                Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                control.GetType().GetProperty("Text").SetValue(control, configVal, null);
                //String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
            } 

            XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        /// <summary>
        /// 获取量程配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            bool flag = false;
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
                cmd[7] = byte.Parse("07", System.Globalization.NumberStyles.HexNumber);


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

                long lastUpdateTime = SensorSpanConfigValue.updateTime;

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                MainForm.thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[获取传感器量程配置信息]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                //监听传感器量程配置值变化
                Thread thrListenSensorSpanConfigValue = new Thread(new ParameterizedThreadStart(listenSensorSpanConfigValue));
                thrListenSensorSpanConfigValue.Start(lastUpdateTime);
                 

                //XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (flag) {
                XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 监听传感器量程配置值变化
        /// </summary>
        /// <param name="lastUpdateTime"></param>
        public void listenSensorSpanConfigValue(Object lastUpdateTime) {
            int index = 0;
            while (true)
            {
                //超时
                if (index >= 10)
                {
                    XtraMessageBox.Show("读取超时，请稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                //未配置
                if (SensorSpanConfigValue.updateTime < Convert.ToInt64(lastUpdateTime))
                {
                    XtraMessageBox.Show("传感器量程未配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                //读取配置成功
                if (SensorSpanConfigValue.updateTime > Convert.ToInt64(lastUpdateTime))
                {
                    Action action = () =>
                    {
                        //获取静态配置类中的配置值
                        List<string> configList = SensorSpanConfigValue.getSensorSpanConfigValue();
                        if (null == configList || configList.Count != 24)
                        {
                            XtraMessageBox.Show("读取失败，配置值不完整！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        //1-23循环显示在相应输入框
                        for (int i = 1; i <= 24; i++)
                        {
                            string configVal = configList[i - 1];
                            Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                            control.GetType().GetProperty("Text").SetValue(control, configVal, null);
                            //String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                        } 

                    };
                    Invoke(action);

                    XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                index++;
                Thread.Sleep(1000);
            }
        }

        
       


        


 
        
        
    }
}