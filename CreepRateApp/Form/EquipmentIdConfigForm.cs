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
        private bool readFlag = false;


        public EquipmentIdConfigForm(SerialPort paramPortDev)
        {
            InitializeComponent();
            mSerialPort = paramPortDev;
            mSerialPort.ReceivedBytesThreshold = 1; 

            //初始化设备ID
            byte ID = MainForm.EquipmentId;
            textBox1.Text = ID.ToString(); 
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
                //cmd[2] = MainForm.EquipmentId;
                cmd[2] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //Reserve
                cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //--Category
                cmd[4] = byte.Parse("06", System.Globalization.NumberStyles.HexNumber);

                //Len(2 byte)
                cmd[5] = 2;
                cmd[6] = 0;

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
                Action action = () =>
                {
                    MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[设备ID设置]_" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "：", sendCmdStr));
                };
                action.Invoke();

                XtraMessageBox.Show("指令已下发！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MainForm.isCollecting = true;
                //this.Close();
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }

        /// <summary>
        /// 读取设备ID,显示已配置ID【已废弃】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_used(object sender, EventArgs e)
        {
            byte ID = MainForm.EquipmentId;
            textBox1.Text = ID.ToString();
            XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
       
        }

        /// <summary>
        /// 获取设备ID
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
                cmd[7] = byte.Parse("06", System.Globalization.NumberStyles.HexNumber);


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

                long lastUpdateTime = MainForm.EquipmentIdUpdateTime;

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                MainForm.thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                Action action = () =>
                {
                    MainForm.showMessage(MainForm.richTextBox1, string.Format("{0}{1}", "上位机(" + MainForm.localIpep + ")[获取设备ID]_" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "：", sendCmdStr));
                };
                action.Invoke();
                

                //监听EquipmentID变化
                Thread thrListenID = new Thread(new ParameterizedThreadStart(listenEquipmentID));
                thrListenID.Start(lastUpdateTime);




                /*
                lock (MainForm.EquipmentIdUpdateTime) {
                    while ((long)MainForm.EquipmentIdUpdateTime == (long)lastUpdateTime) {
                        Monitor.Wait(MainForm.EquipmentIdUpdateTime);
                    }
                    if ((long)MainForm.EquipmentIdUpdateTime > (long)lastUpdateTime) {
                        //将配置值显示在配置框中
                        byte ID = MainForm.EquipmentId;
                        textBox1.Text = ID.ToString();

                        XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       
                    }
                    if ((long)MainForm.EquipmentIdUpdateTime < 0) {
                        XtraMessageBox.Show("设备ID未配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); 

                    }
                }*/

                /*
                int index = 0;
                while (true)
                { 
                    if (index >= 5)
                    {
                        XtraMessageBox.Show("读取超时，请稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    if (MainForm.EquipmentIdUpdateTime < 0) {
                        XtraMessageBox.Show("设备ID未配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    if (MainForm.EquipmentIdUpdateTime > lastUpdateTime)
                    {
                        //将配置值显示在配置框中
                        byte ID = MainForm.EquipmentId;
                        textBox1.Text = ID.ToString();

                        XtraMessageBox.Show("读取成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    index++;
                    Thread.Sleep(1000);
                }
                */

                //XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
             
        }

        /// <summary>
        /// 监听EquipmentID变化
        /// </summary> 
        public void listenEquipmentID(Object  lastUpdateTime) {
            int index = 0;
            while (true)
            {
                if (index >= 10)
                {
                    XtraMessageBox.Show("读取超时，请稍后重试！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                if (MainForm.EquipmentIdUpdateTime < Convert.ToInt64(lastUpdateTime))
                {
                    XtraMessageBox.Show("设备ID未配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                if (MainForm.EquipmentIdUpdateTime > Convert.ToInt64(lastUpdateTime))
                {
                    Action action = () =>
                    {
                        byte ID = MainForm.EquipmentId;
                        textBox1.Text = ID.ToString();
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