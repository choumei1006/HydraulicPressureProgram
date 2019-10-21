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
    public partial class EquipmentIdConfigForm : DevExpress.XtraEditors.XtraForm
    {
        private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。


        public EquipmentIdConfigForm(SerialPort paramPortDev)
        {
            InitializeComponent();
            mSerialPort = paramPortDev;
            mSerialPort.ReceivedBytesThreshold = 1; 
        }
        /// <summary>
        /// 保存，并发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        { 
            String device_id = textBox1.Text;

            //设置设备ID静态变量
            MainForm.EquipmentId = byte.Parse(device_id, System.Globalization.NumberStyles.Integer);

            try
            {
                //生成配置信息 byte数组 对应的 16进制字符串数组
                byte[] cmd = new byte[10];

                //Header
                cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
                cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
                //Device_id
                cmd[2] = MainForm.EquipmentId;
                //Reserve
                cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //--Category
                cmd[4] = byte.Parse("06", System.Globalization.NumberStyles.HexNumber);

                //Len(2 byte)
                cmd[5] = 0;
                cmd[6] = 2;

                //--data
                cmd[7] = MainForm.EquipmentId;
                cmd[8] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);    //预留
                  

                //Verify
                byte verifyByte = 0;
                for (int i = 0; i < cmd.Length; i++)
                {
                    verifyByte ^= cmd[i];
                }
                cmd[9] = verifyByte;

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

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                MainForm.thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[设备ID设置]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
         
    }
}