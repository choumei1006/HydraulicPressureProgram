﻿using System;
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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using System.Threading;
using System.Reflection;

namespace CreepRateApp
{
    public partial class NetCtrlForm : DevExpress.XtraEditors.XtraForm
    {
        static object Locker = new object();
        //1.声明自适应类实例
        AutoSizeFormClass asc = new AutoSizeFormClass();
        Properties.Settings settings = Properties.Settings.Default;
        private SerialPort mSerialPort;
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int received_count = 0;//接收计数
        private long send_count = 0;//发送计数
        private int recv_count = 0;//单次串口收数据计数器
        private byte[] buffer = new byte[8]; //串口缓存
        private byte[] bufferFeedEnough = new byte[6]; //串口缓存



        public NetCtrlForm(SerialPort paramPortDev)
        {

            InitializeComponent();
            mSerialPort = paramPortDev;
            mSerialPort.ReceivedBytesThreshold = 1;
        }

        //2. 为窗体添加Load事件，并在其方法Form1_Load中，调用类的初始化方法，记录窗体和其控件的初始位置和大小
        private void NetCtrlForm_Load(object sender, EventArgs e)
        {
            asc.controllInitializeSize(this);
        }

        //3.为窗体添加SizeChanged事件，并在其方法Form1_SizeChanged中，调用类的自适应方法，完成自适应
        private void NetCtrlForm_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        //=======================================================================================================
        //*******************************************************************************************************

        ///
        /// 用于UDP发送的网络服务类
        ///
        private UdpClient udpcSend;
        ///
        /// 用于UDP接收的网络服务类
        ///
        private UdpClient udpcRecv;
        private static string localIpAddress = GetIpAddress();
        private IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(localIpAddress), 10101); // 本机IP和监听端口号
        //private IPEndPoint localIpep2 = new IPEndPoint(IPAddress.Parse(localIpAddress), 10101); // 本机IP和监听端口号


        //发送按钮点击函数
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("请先输入待发送内容");
                return;
            }


            // 匿名发送
            //udpcSend = new UdpClient(0);             // 自动分配本地IPv4地址
            // 实名发送
            udpcSend = new UdpClient(localIpep);
            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(richTextBox1.Text);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 10105); // 发送到的IP地址和端口号
            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();
            //ResetTextBox(richTextBox1,message);
        }
        
        //回复发送函数
        /*private void callSendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 10105); // 发送到的IP地址和端口号
            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();
        }*/
        /// <summary>
        /// 开关：在监听UDP报文阶段为true，否则为false
        /// </summary>
        bool IsUdpcRecvStart = false;
        /// <summary>
        /// 线程：不断监听UDP报文
        /// </summary>
        Thread thrRecv;



        //接收按钮点击函数
        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsUdpcRecvStart) // 未监听的情况，开始监听
            {
                //IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(localIpAddress), 10101); // 本机IP和监听端口号
                udpcRecv = new UdpClient(localIpep);
                thrRecv = new Thread(ReceiveMessage);
                thrRecv.Start();
                IsUdpcRecvStart = true;
                ShowMessage(richTextBox2, "UDP监听器已成功启动");
            }
            else // 正在监听的情况，终止监听
            {
                
                thrRecv.Abort(); // 必须先关闭这个线程，否则会异常
                //thrRecv.DisableComObjectEagerCleanup();//直接释放掉当前线程 
                //thrRecv.Join();
                //Console.WriteLine(string.Format("thrRecv的状态："+thrRecv.ThreadState));
                udpcRecv.Close();
                IsUdpcRecvStart = false;
                //thrRecv.Abort(); // 必须先关闭这个线程，否则会异常
                ShowMessage(richTextBox2, "UDP监听器已成功关闭"); 

                //开始关闭线程
                /*
                udpcRecv.Close();//关闭UDP连接
                //udpcRecv.Dispose();//释放协议
                udpcRecv = null;//为协议至空值
                //thrRecv.DisableComObjectEagerCleanup();//直接释放掉当前线程 


                IsUdpcRecvStart = false;
                ShowMessage(richTextBox2, "UDP监听器已成功关闭");*/
                 
            }

        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 10105);//
            while (true)
            {
                //try
                //{
                //lock (Locker) { 
                    //Console.WriteLine(udpcRecv.Client);
                    //Thread.Sleep(5000);
                    //if (udpcRecv.Client == null) {
                     //   udpcRecv = new UdpClient(localIpep);
                   // }
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);   
                    string message = Encoding.Unicode.GetString(bytRecv, 0, bytRecv.Length);
                    ShowMessage(richTextBox2, string.Format("{0}[{1}]", remoteIpep, message));

                /*
                    //thrRecv.Abort(); // 必须先关闭这个线程，否则会异常  
                    udpcRecv.Close();
                    //udpcRecv = new UdpClient(localIpep);
                    IsUdpcRecvStart = false; 
                    ShowMessage(richTextBox2, "UDP监听器已成功关闭");
                    udpcSend = new UdpClient(localIpep);
                    Thread thrSend = new Thread(SendMessage);
                    thrSend.Start("2");
                 */
                //}
                   /*
                    //开始关闭线程
                    udpcRecv.Close();//关闭协议
                    //udpcRecv.Dispose();//释放协议
                    udpcRecv = null;//为协议至空值
                    //thrRecv.DisableComObjectEagerCleanup();//直接释放掉当前线程 


                    IsUdpcRecvStart = false;
                    ShowMessage(richTextBox2, "UDP监听器已成功关闭");


                    udpcSend = new UdpClient(localIpep);
                    Thread thrSend = new Thread(callSendMessage);
                    thrSend.Start("2"); 
                    * */

                }

                /*catch (Exception ex)
                {
                    ShowMessage(richTextBox2, ex.Message);
                    break;
                }*/
            //}
        }
        // 向RichTextBox中添加文本
        delegate void ShowMessageDelegate(RichTextBox txtbox, string message);
        private void ShowMessage(RichTextBox txtbox, string message)
        {
            if (txtbox.InvokeRequired)
            {
                ShowMessageDelegate showMessageDelegate = ShowMessage;
                txtbox.Invoke(showMessageDelegate, new object[] { txtbox, message });
            }
            else
            {
                txtbox.Text += message + "\r\n";
            }
        }

        // 清空指定RichTextBox中的文本
        delegate void ResetTextBoxDelegate(RichTextBox txtbox,string message);
        private void ResetTextBox(RichTextBox txtbox,string message)
        {
            if (txtbox.InvokeRequired)
            {
                ResetTextBoxDelegate resetTextBoxDelegate = ResetTextBox;
                txtbox.Invoke(resetTextBoxDelegate, new object[] { txtbox });
            }
            else
            {
                txtbox.Text = "send>>" + message;
            }
        }

        /// <summary>
        /// 关闭程序，强制退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }


        /// <summary>
        ///获取本机IP地址 
        /// </summary>
        /// <returns></returns>
        private static string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];

            return localaddr.ToString();
        }

        /// <summary>
        /// 判断某端口是否正在使用中
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveUdpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

    }

}



































