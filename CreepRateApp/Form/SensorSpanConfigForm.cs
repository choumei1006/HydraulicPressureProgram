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
        private static string localIpAddress = GetLocalIpAddress();
        //private IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(localIpAddress), 10101); 

       
        //public SensorChannelConfigForm(SerialPort paramPortDev)
        public SensorSpanConfigForm(DevExpress.XtraBars.Ribbon.RibbonForm form)
        {
            InitializeComponent();
            //mSerialPort = paramPortDev;
            //mSerialPort.ReceivedBytesThreshold = 1;
            this._form = form;
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
            //int[] t1 = getint1();
            
            List<string> channelConfigValues = new List<string>();  //存放通道配置值

            int isVerified = 0;

            try
            {
                //检查通道配置合理性
                for (int i = 1; i <= 24; i++)
                {
                    //Control control2 = Controls.Find("numericUpDown1", true)[0];
                    Control control = Controls.Find("numericUpDown" + Convert.ToString(i), true)[0];
                    String value = control.GetType().GetProperty("Text").GetValue(control, null).ToString();
                    
                        
                    channelConfigValues.Add(value);
                                                            
                                           
                    isVerified = i;
                }
                //判断是否完成所有传感器通道的配置合理性检查
                if (isVerified == 24)
                {
                    //初始化传感器通道配置信息类
                    SensorSpanConfigValue.setChannelConfigValue(channelConfigValues);

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
                    MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[传感器通道配置信息下发]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));

                    

                    XtraMessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    XtraMessageBox.Show("存在未配置通道，请检查更改！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception exception) {
                XtraMessageBox.Show(exception.Message, "配置异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            
        }

        
       


        



        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            return localaddr.ToString();
        }

        
        
        
    }
}