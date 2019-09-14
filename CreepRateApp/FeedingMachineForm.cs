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

namespace CreepRateApp
{
    public partial class FeedingMachineForm : DevExpress.XtraEditors.XtraForm
    {
        private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int received_count = 0;//接收计数
        private long send_count = 0;//发送计数
        private int recv_count = 0;//单次串口收数据计数器
        private byte[] buffer = new byte[8]; //串口缓存
        private byte[] bufferFeedEnough = new byte[6]; //串口缓存

        private entity.ResultFormat AStep01Result = new entity.ResultFormat();
        private entity.ResultFormat AStep02Result = new entity.ResultFormat();
        private entity.ResultFormat AStep03Result = new entity.ResultFormat();

        private entity.ResultFormat BStep01Result = new entity.ResultFormat();
        private entity.ResultFormat BStep02Result = new entity.ResultFormat();
        private entity.ResultFormat BStep03Result = new entity.ResultFormat();

        private entity.ResultFormat AStartStep01Result = new entity.ResultFormat();
        private entity.ResultFormat AStartStep02Result = new entity.ResultFormat();
        private entity.ResultFormat AStartStep03Result = new entity.ResultFormat();
        private entity.ResultFormat AStartStep04Result = new entity.ResultFormat();

        private entity.ResultFormat BStartStep01Result = new entity.ResultFormat();
        private entity.ResultFormat BStartStep02Result = new entity.ResultFormat();
        private entity.ResultFormat BStartStep03Result = new entity.ResultFormat();
        private entity.ResultFormat BStartStep04Result = new entity.ResultFormat();

        private entity.ResultFormat AFeedEnoughResult = new entity.ResultFormat();
        private entity.ResultFormat BFeedEnoughResult = new entity.ResultFormat(); 

        public FeedingMachineForm(SerialPort paramPortDev)
        {
            InitializeComponent();
            mSerialPort = paramPortDev;
            mSerialPort.ReceivedBytesThreshold = 1;

            AStep01Result.IsSuccess = false;
            AStep02Result.IsSuccess = false;
            AStep03Result.IsSuccess = false;

            BStep01Result.IsSuccess = false;
            BStep02Result.IsSuccess = false;
            BStep03Result.IsSuccess = false;

            AFeedEnoughResult.IsSuccess = false;
            BFeedEnoughResult.IsSuccess = false;


        }

        /// <summary>
        /// 自动模式
        /// 20190102版本暂不实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoModeSwitch_Toggled(object sender, EventArgs e)
        {
            XtraMessageBox.Show("敬请期待！");
        }

        /// <summary>
        /// A线写入（按钮单击操作）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aParamInput_Click(object sender, EventArgs e)
        {
            if (!mSerialPort.IsOpen)
            {
                mSerialPort.Open();
            }
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AS01_DataReceived);
            }
            catch { }
            //if (!mSerialPort.IsOpen)
            //{
            //    mSerialPort.Open();
            //}
            byte[] buf01 = crc.FillCRC16("6F 05 00 11 FF 00");//发送”6F 05 00 11 FF 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf01, 0, buf01.Length);//发送数据

            //sxm
            //try
            //{
            //    mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AS01_DataReceived);
            //}
            //catch { }

            int aS01Count = 0;
            while (!AStep01Result.IsSuccess)//当AS01步骤执行成功时，跳出这个步骤
            {
                if (AStep01Result.Msg == "指令返回异常")//AS01返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStep01Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                aS01Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (aS01Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AS01步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //MessageBox.Show(AStep01Result.Msg, "提示", MessageBoxButtons.OK);
            
            //当程序运行到这里的时候，证明已经跳出AS01的while循环，AS01步骤已经执行成功
            //此处开始AS02步骤
            //首先需要注销AS01系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AS01_DataReceived);
            }
            catch { }
            //注册AS02步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AS02_DataReceived);
            }
            catch { }
            //下面的代码暂时没有做检验，待流程调通后完善
            int paramSpreed = int.Parse(te_speed.Text);
            if (paramSpreed > 60)
            {
                XtraMessageBox.Show("速度不得大于60", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;//此流程终止执行
            }
            //速度参数通过校验
            string speedMsg = "6F 06 00 0A " + paramSpreed.ToString("x4");//x4,将速度转换为4为16进制，不足4为补足4位
            byte[] buf02 = crc.FillCRC16(speedMsg);//发送速度数据,自动计算校验位并合并发送
            mSerialPort.Write(buf02, 0, buf02.Length);//发送数据

            int aS02Count = 0;
            while (!AStep02Result.IsSuccess)//当AS02步骤执行成功时，跳出这个步骤
            {
                if (AStep02Result.Msg == "指令返回异常")//AS02返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStep02Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                aS02Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (aS02Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AS02步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //当程序运行到这里的时候，证明已经跳出AS02的while循环，AS02步骤已经执行成功
            //此处开始AS03步骤
            //首先需要注销AS02系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AS02_DataReceived);
            }
            catch { }
            //注册AS03步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AS03_DataReceived);
            }
            catch { }
            //下面的代码暂时没有做检验，待流程调通后完善
            int paramLength = int.Parse((double.Parse(te_FeedLength.Text) * 10).ToString());//米转分米，分米取整数
            string LengthMsg = "6F 10 00 0C 00 02 04 " + paramSpreed.ToString("x8");//x8,将速度转换为4为16进制，不足8为补足8位

            byte[] buf03 = crc.FillCRC16(LengthMsg);//发送长度数据,自动计算校验位并合并发送
            mSerialPort.Write(buf03, 0, buf03.Length);//发送数据

            int aS03Count = 0;
            while (!AStep03Result.IsSuccess)//当AS03步骤执行成功时，跳出这个步骤
            {
                if (AStep03Result.Msg == "指令返回异常")//AS03返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStep03Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                aS03Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (aS03Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AS03步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }

            //当程序运行到这里的时候，证明已经跳出AS03的while循环，AS03步骤已经执行成功
            //此按钮所有流程步骤完成
            //首先需要注销AS03系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AS03_DataReceived);
            }
            catch { }

            XtraMessageBox.Show(AStep03Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

        }

        /// <summary>
        /// AS01系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AS01_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
           // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for(int k = 0; k < n; k ++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 17)
                {
                    AStep01Result.IsSuccess = true;
                    AStep01Result.Msg = "SUCCESS";
                    //Console.WriteLine("SUCCESS");
                } else
                {
                    AStep01Result.IsSuccess = false;
                    AStep01Result.Msg = "指令返回异常"; 
                }
            }
            //if (n >= 4)
            //{
            //    byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            //    received_count += n;//增加接收计数
            //    mSerialPort.Read(buf, 0, n);//读取缓冲数据
            //    Console.Write(buf);
            //    builder.Clear();//清除字符串构造器的内容

            //    String message = this.JieMa(buf);
            //    String[] messages = message.Split(' ');


            //    //此处可能会出现问题，就像周日早上和你一起调试的时候，由于设备之间的延迟造成的数据丢失问题，你参考我给你那天调的代码改一下，改之前先备份代码一份

            //    String t1 = Convert.ToInt32(messages[0], 16) + "";
            //    String t2 = Convert.ToInt32(messages[1], 16) + "";
            //    String t3 = Convert.ToInt32(messages[2], 16) + "";
            //    String t4 = Convert.ToInt32(messages[3], 16) + "";



            //    Boolean isSame = t1.Equals("111") && t2.Equals("5") && t3.Equals("0") && t4.Equals("17");

            //    if (isSame)
            //    {
            //        AStep01Result.IsSuccess = true;
            //        AStep01Result.Msg = "SUCCESS";
            //    }
            //    else
            //    {
            //        AStep01Result.IsSuccess = false;
            //        AStep01Result.Msg = "指令返回异常";
            //    }
            //}
            //else
            //{
            //    AStep01Result.IsSuccess = false;
            //    AStep01Result.Msg = "指令返回字节数不够！";
            //}
        }

        /// <summary>
        /// AS02系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AS02_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
           // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 6 && buffer[2] == 0 && buffer[3] == 10)
                {
                    AStep02Result.IsSuccess = true;
                    AStep02Result.Msg = "SUCCESS"; 
                }
                else
                {
                    AStep02Result.IsSuccess = false;
                    AStep02Result.Msg = "指令返回异常"; 
                }
            }



            // if (n >= 4)
            //{
            //    byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            //    received_count += n;//增加接收计数
            //    mSerialPort.Read(buf, 0, n);//读取缓冲数据 
            //    builder.Clear();//清除字符串构造器的内容

            //    String message = this.JieMa(buf);
            //    String[] messages = message.Split(' ');

            //    //此处可能会出现问题，就像周日早上和你一起调试的时候，由于设备之间的延迟造成的数据丢失问题，你参考我给你那天调的代码改一下，改之前先备份代码一份

            //    String t1 = Convert.ToInt32(messages[0], 16) + "";
            //    String t2 = Convert.ToInt32(messages[1], 16) + "";
            //    String t3 = Convert.ToInt32(messages[2], 16) + "";
            //    String t4 = Convert.ToInt32(messages[3], 16) + "";

            //    Boolean isSame = t1.Equals("111") && t2.Equals("6") && t3.Equals("0") && t4.Equals("10");

            //    if (isSame)
            //    {
            //        AStep02Result.IsSuccess = true;
            //        AStep02Result.Msg = "SUCCESS";
            //    }
            //    else
            //    {
            //        AStep02Result.IsSuccess = false;
            //        AStep02Result.Msg = "指令返回异常";
            //    } 
            //}
            //else
            //{
            //    AStep02Result.IsSuccess = false;
            //    AStep02Result.Msg = "指令返回字节数不够！";
            //}
        }

        /// <summary>
        /// AS03系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AS03_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
            //Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 16 && buffer[2] == 0 && buffer[3] == 12 && buffer[4] == 0 && buffer[5] ==  2)
                {
                    AStep03Result.IsSuccess = true;
                    AStep03Result.Msg = "A线参数写入成功！";
                }
                else
                {
                    AStep03Result.IsSuccess = false;
                    AStep03Result.Msg = "指令返回异常";
                }
            }


            // if (n >= 4)
            //{
            //    byte[] buf = new byte[n];               //声明一个临时数组存储当前来的串口数据
            //    received_count += n;                    //增加接收计数
            //    mSerialPort.Read(buf, 0, n);            //读取缓冲数据
            //    builder.Clear();                        //清除字符串构造器的内容

            //    String message = this.JieMa(buf);
            //    String[] messages = message.Split(' ');

            //    //此处可能会出现问题，就像周日早上和你一起调试的时候，由于设备之间的延迟造成的数据丢失问题，你参考我给你那天调的代码改一下，改之前先备份代码一份

            //    String t1 = Convert.ToInt32(messages[0], 16) + "";
            //    String t2 = Convert.ToInt32(messages[1], 16) + "";
            //    String t3 = Convert.ToInt32(messages[2], 16) + "";
            //    String t4 = Convert.ToInt32(messages[3], 16) + "";
            //    String t5 = Convert.ToInt32(messages[4], 16) + "";
            //    String t6 = Convert.ToInt32(messages[5], 16) + "";

            //    Boolean isSame = t1.Equals("111") && t2.Equals("16") && t3.Equals("0") && t4.Equals("12") && t5.Equals("0") && t6.Equals("2");

            //    if (isSame)
            //    {
            //        AStep03Result.IsSuccess = true;
            //        AStep03Result.Msg = "A线参数写入成功！";
            //    }
            //    else
            //    {
            //        AStep03Result.IsSuccess = false;
            //        AStep03Result.Msg = "指令返回异常";
            //    } 
            //}
            //else
            //{
            //    AStep03Result.IsSuccess = false;
            //    AStep03Result.Msg = "指令返回字节数不够！";
            //}
        }


        /// <summary>
        /// 串口接收数据解码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string JieMa(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", data[i]);
            }
            return sb.ToString().ToUpper();
        }

        private void FeedingMachineForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void FeedingMachineForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        
        /// <summary>
        /// B线写入（按钮单击操作）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bParamInput_Click(object sender, EventArgs e)
        {
            if (!mSerialPort.IsOpen)
            {
                mSerialPort.Open();
            }
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BS01_DataReceived);
            }
            catch { }
            //if (!mSerialPort.IsOpen)
            //{
            //    mSerialPort.Open();
            //}
            byte[] buf01 = crc.FillCRC16("6F 05 00 11 FF 00");//发送”6F 05 00 11 FF 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf01, 0, buf01.Length);//发送数据

            //sxm
            //try
            //{
            //    mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AS01_DataReceived);
            //}
            //catch { }

            int bS01Count = 0;
            while (!BStep01Result.IsSuccess)//当BS01步骤执行成功时，跳出这个步骤
            {
                if (BStep01Result.Msg == "指令返回异常")//BS01返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(BStep01Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                bS01Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (bS01Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("BS01步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //MessageBox.Show(BStep01Result.Msg, "提示", MessageBoxButtons.OK);

            //当程序运行到这里的时候，证明已经跳出AS01的while循环，AS01步骤已经执行成功
            //此处开始BS02步骤
            //首先需要注销AS01系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BS01_DataReceived);
            }
            catch { }
            //注册BS02步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BS02_DataReceived);
            }
            catch { }
            //下面的代码暂时没有做检验，待流程调通后完善
            int paramSpreed = int.Parse(te_speed.Text);
            if (paramSpreed > 60)
            {
                XtraMessageBox.Show("速度不得大于60", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;//此流程终止执行
            }
            //速度参数通过校验
            string speedMsg = "6F 06 00 0B " + paramSpreed.ToString("x4");//x4,将速度转换为4为16进制，不足4为补足4位
            byte[] buf02 = crc.FillCRC16(speedMsg);//发送速度数据,自动计算校验位并合并发送
            mSerialPort.Write(buf02, 0, buf02.Length);//发送数据

            int bS02Count = 0;
            while (!BStep02Result.IsSuccess)//当AS02步骤执行成功时，跳出这个步骤
            {
                if (BStep02Result.Msg == "指令返回异常")//AS02返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(BStep02Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                bS02Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (bS02Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("BS02步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //当程序运行到这里的时候，证明已经跳出AS02的while循环，AS02步骤已经执行成功
            //此处开始BS03步骤
            //首先需要注销BS02系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BS02_DataReceived);
            }
            catch { }
            //注册BS03步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BS03_DataReceived);
            }
            catch { }
            //下面的代码暂时没有做检验，待流程调通后完善
            int paramLength = int.Parse((double.Parse(te_FeedLength.Text) * 10).ToString());//米转分米，分米取整数
            string LengthMsg = "6F 10 00 0E 00 02 04 " + paramSpreed.ToString("x8");//x8,将速度转换为4为16进制，不足8为补足8位

            byte[] buf03 = crc.FillCRC16(LengthMsg);//发送长度数据,自动计算校验位并合并发送
            mSerialPort.Write(buf03, 0, buf03.Length);//发送数据

            int bS03Count = 0;
            while (!BStep03Result.IsSuccess)//当AS03步骤执行成功时，跳出这个步骤
            {
                if (BStep03Result.Msg == "指令返回异常")//AS03返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(BStep03Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                bS03Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (bS03Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("BS03步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }

            //当程序运行到这里的时候，证明已经跳出AS03的while循环，AS03步骤已经执行成功
            //此按钮所有流程步骤完成
            //首先需要注销AS03系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BS03_DataReceived);
            }
            catch { }

            XtraMessageBox.Show(BStep03Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

        }

        /// <summary>
        /// AS01系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BS01_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 17)
                {
                    BStep01Result.IsSuccess = true;
                    BStep01Result.Msg = "SUCCESS";
                    //Console.WriteLine("SUCCESS");
                }
                else
                {
                    BStep01Result.IsSuccess = false;
                    BStep01Result.Msg = "指令返回异常";
                }
            }
            //if (n >= 4)
            //{
            //    byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            //    received_count += n;//增加接收计数
            //    mSerialPort.Read(buf, 0, n);//读取缓冲数据
            //    Console.Write(buf);
            //    builder.Clear();//清除字符串构造器的内容

            //    String message = this.JieMa(buf);
            //    String[] messages = message.Split(' ');


            //    //此处可能会出现问题，就像周日早上和你一起调试的时候，由于设备之间的延迟造成的数据丢失问题，你参考我给你那天调的代码改一下，改之前先备份代码一份

            //    String t1 = Convert.ToInt32(messages[0], 16) + "";
            //    String t2 = Convert.ToInt32(messages[1], 16) + "";
            //    String t3 = Convert.ToInt32(messages[2], 16) + "";
            //    String t4 = Convert.ToInt32(messages[3], 16) + "";



            //    Boolean isSame = t1.Equals("111") && t2.Equals("5") && t3.Equals("0") && t4.Equals("17");

            //    if (isSame)
            //    {
            //        BStep01Result.IsSuccess = true;
            //        BStep01Result.Msg = "SUCCESS";
            //    }
            //    else
            //    {
            //        BStep01Result.IsSuccess = false;
            //        BStep01Result.Msg = "指令返回异常";
            //    }
            //}
            //else
            //{
            //    BStep01Result.IsSuccess = false;
            //    BStep01Result.Msg = "指令返回字节数不够！";
            //}
        }

        /// <summary>
        /// BS02系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BS02_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 6 && buffer[2] == 0 && buffer[3] == 11)
                {
                    BStep02Result.IsSuccess = true;
                    BStep02Result.Msg = "SUCCESS";
                }
                else
                {
                    BStep02Result.IsSuccess = false;
                    BStep02Result.Msg = "指令返回异常";
                }
            }



            // if (n >= 4)
            //{
            //    byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            //    received_count += n;//增加接收计数
            //    mSerialPort.Read(buf, 0, n);//读取缓冲数据 
            //    builder.Clear();//清除字符串构造器的内容

            //    String message = this.JieMa(buf);
            //    String[] messages = message.Split(' ');

            //    //此处可能会出现问题，就像周日早上和你一起调试的时候，由于设备之间的延迟造成的数据丢失问题，你参考我给你那天调的代码改一下，改之前先备份代码一份

            //    String t1 = Convert.ToInt32(messages[0], 16) + "";
            //    String t2 = Convert.ToInt32(messages[1], 16) + "";
            //    String t3 = Convert.ToInt32(messages[2], 16) + "";
            //    String t4 = Convert.ToInt32(messages[3], 16) + "";

            //    Boolean isSame = t1.Equals("111") && t2.Equals("6") && t3.Equals("0") && t4.Equals("10");

            //    if (isSame)
            //    {
            //        BStep02Result.IsSuccess = true;
            //        BStep02Result.Msg = "SUCCESS";
            //    }
            //    else
            //    {
            //        BStep02Result.IsSuccess = false;
            //        BStep02Result.Msg = "指令返回异常";
            //    } 
            //}
            //else
            //{
            //    BStep02Result.IsSuccess = false;
            //    BStep02Result.Msg = "指令返回字节数不够！";
            //}
        }

        /// <summary>
        /// BS03系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BS03_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
            //Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 16 && buffer[2] == 0 && buffer[3] == 14 && buffer[4] == 0 && buffer[5] == 2)
                {
                    BStep03Result.IsSuccess = true;
                    BStep03Result.Msg = "B线参数写入成功！";
                }
                else
                {
                    BStep03Result.IsSuccess = false;
                    BStep03Result.Msg = "指令返回异常";
                }
            }


            // if (n >= 4)
            //{
            //    byte[] buf = new byte[n];               //声明一个临时数组存储当前来的串口数据
            //    received_count += n;                    //增加接收计数
            //    mSerialPort.Read(buf, 0, n);            //读取缓冲数据
            //    builder.Clear();                        //清除字符串构造器的内容

            //    String message = this.JieMa(buf);
            //    String[] messages = message.Split(' ');

            //    //此处可能会出现问题，就像周日早上和你一起调试的时候，由于设备之间的延迟造成的数据丢失问题，你参考我给你那天调的代码改一下，改之前先备份代码一份

            //    String t1 = Convert.ToInt32(messages[0], 16) + "";
            //    String t2 = Convert.ToInt32(messages[1], 16) + "";
            //    String t3 = Convert.ToInt32(messages[2], 16) + "";
            //    String t4 = Convert.ToInt32(messages[3], 16) + "";
            //    String t5 = Convert.ToInt32(messages[4], 16) + "";
            //    String t6 = Convert.ToInt32(messages[5], 16) + "";

            //    Boolean isSame = t1.Equals("111") && t2.Equals("16") && t3.Equals("0") && t4.Equals("12") && t5.Equals("0") && t6.Equals("2");

            //    if (isSame)
            //    {
            //        BStep03Result.IsSuccess = true;
            //        BStep03Result.Msg = "A线参数写入成功！";
            //    }
            //    else
            //    {
            //        BStep03Result.IsSuccess = false;
            //        BStep03Result.Msg = "指令返回异常";
            //    } 
            //}
            //else
            //{
            //    BStep03Result.IsSuccess = false;
            //    BStep03Result.Msg = "指令返回字节数不够！";
            //}
        }

        /// <summary>
        /// A线启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aSwitch_Toggled(object sender, EventArgs e)
        {

            if (!mSerialPort.IsOpen)
            {
                mSerialPort.Open();
            }
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AStart01_DataReceived);
            }
            catch { }
            //if (!mSerialPort.IsOpen)
            //{
            //    mSerialPort.Open();
            //}
            byte[] buf01 = crc.FillCRC16("6F 05 00 13 FF 00");//发送”6F 05 00 13 FF 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf01, 0, buf01.Length);//发送数据
             
            int AStart01Count = 0;
            while (!AStartStep01Result.IsSuccess)//当AStart01步骤执行成功时，跳出这个步骤
            {
                if (AStartStep01Result.Msg == "指令返回异常")//AStart01返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStartStep01Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                AStart01Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (AStart01Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AStart01步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //MessageBox.Show(AStartStep01Result.Msg, "提示", MessageBoxButtons.OK);

            //当程序运行到这里的时候，证明已经跳出AStart01的while循环，AStart01步骤已经执行成功
            //此处开始AStart02步骤
            //首先需要注销AStart01系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AStart01_DataReceived);
            }
            catch { }
            //注册AStart02步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AStart02_DataReceived);
            }
            catch { }

            byte[] buf02 = crc.FillCRC16("6F 05 00 13 00 00");//发送”6F 05 00 13 00 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf02, 0, buf02.Length);//发送数据

            int AStart02Count = 0;
            while (!AStartStep02Result.IsSuccess)//当AStep02步骤执行成功时，跳出这个步骤
            {
                if (AStartStep02Result.Msg == "指令返回异常")//AStep02返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStartStep02Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                AStart02Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (AStart02Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AStart02步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //当程序运行到这里的时候，证明已经跳出AStart02的while循环，AStart02步骤已经执行成功
            //此处开始AStart03步骤
            //首先需要注销AStart03系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AStart02_DataReceived);
            }
            catch { }
            //注册AStart03步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AStart03_DataReceived);
            }
            catch { }
             

            byte[] buf03 = crc.FillCRC16("6F 05 00 12 FF 00");//发送”6F 05 00 12 FF 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf03, 0, buf03.Length);//发送数据

            int AStart03Count = 0;
            while (!AStartStep03Result.IsSuccess)//当AStep03步骤执行成功时，跳出这个步骤
            {
                if (AStartStep03Result.Msg == "指令返回异常")//AStep03返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStartStep03Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                AStart03Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (AStart03Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AStart03步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }

            //当程序运行到这里的时候，证明已经跳出AStart03的while循环，AStart03步骤已经执行成功
            //此处开始AStart04步骤
            //首先需要注销AStart04系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AStart03_DataReceived);
            }
            catch { }
            //注册AStart04步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AStart04_DataReceived);
            }
            catch { }


            byte[] buf04 = crc.FillCRC16("6F 05 00 12 00 00");//发送”6F 05 00 12 00 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf04, 0, buf04.Length);//发送数据

            int AStart04Count = 0;
            while (!AStartStep04Result.IsSuccess)//当AStep03步骤执行成功时，跳出这个步骤
            {
                if (AStartStep04Result.Msg == "指令返回异常")//AStep03返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStartStep04Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                AStart04Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (AStart04Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AStart04步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
 
            //当程序运行到这里的时候，证明已经跳出AS04的while循环，AS04步骤已经执行成功
            //此按钮所有流程步骤完成
            //首先需要注销AS04系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(AStart04_DataReceived);
            }
            catch { }
            XtraMessageBox.Show(AStartStep04Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            //开始循环检测A线喂够标志

            //注册AFeedEnough步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(AFeedEnough_DataReceived);
            }
            catch { }

            byte[] buf05 = crc.FillCRC16("6F 01 00 16 00 01");//发送”6F 01 00 16 00 01 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf05, 0, buf05.Length);//发送数据

            while (!AFeedEnoughResult.IsSuccess)
            {
                System.Threading.Thread.Sleep(2000);  // while休眠一秒
                mSerialPort.Write(buf05, 0, buf05.Length);//发送数据
            }
            
            XtraMessageBox.Show(AFeedEnoughResult.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

        }

        /// <summary>
        /// AStart01系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AStart01_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 19 )
                {
                    AStartStep01Result.IsSuccess = true;
                    AStartStep01Result.Msg = "SUCCESS";
                    //Console.WriteLine("SUCCESS");
                }
                else
                {
                    AStartStep01Result.IsSuccess = false;
                    AStartStep01Result.Msg = "指令返回异常";
                }
            }
            
        }

        /// <summary>
        /// AStart02系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AStart02_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 19)
                {
                    AStartStep02Result.IsSuccess = true;
                    AStartStep02Result.Msg = "SUCCESS";
                }
                else
                {
                    AStartStep02Result.IsSuccess = false;
                    AStartStep02Result.Msg = "指令返回异常";
                }
            }
        }
        /// <summary>
        /// AStart03系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AStart03_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
            //Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 18 )
                {
                    AStartStep03Result.IsSuccess = true;
                    AStartStep03Result.Msg = "SUCCESS";
                }
                else
                {
                    AStartStep03Result.IsSuccess = false;
                    AStartStep03Result.Msg = "指令返回异常";
                }
            }

             
        }

        /// <summary>
        /// AStart04系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AStart04_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
            //Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 18)
                {
                    AStartStep04Result.IsSuccess = true;
                    AStartStep04Result.Msg = "A线启动成功！";
                }
                else
                {
                    AStartStep04Result.IsSuccess = false;
                    AStartStep04Result.Msg = "指令返回异常";
                }
            }


        }

        /// <summary>
        /// AFeedEnough系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AFeedEnough_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                bufferFeedEnough[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 6)
            {
                recv_count = 0;
                if (bufferFeedEnough[0] == 111 && bufferFeedEnough[1] == 1 && bufferFeedEnough[2] == 1 && bufferFeedEnough[3] == 1)
                {
                    AFeedEnoughResult.IsSuccess = true;
                    AFeedEnoughResult.Msg = "A线喂够！";
                    //Console.WriteLine("SUCCESS");
                }
                else
                {
                    AFeedEnoughResult.IsSuccess = false;
                    AFeedEnoughResult.Msg = "指令返回异常";
                }
            }

        }
        /// <summary>
        /// b线启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bSwitch_Toggled(object sender, EventArgs e)
        {
             
            if (!mSerialPort.IsOpen)
            {
                mSerialPort.Open();
            }
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BStart01_DataReceived);
            }
            catch { }
            //if (!mSerialPort.IsOpen)
            //{
            //    mSerialPort.Open();
            //}
            byte[] buf01 = crc.FillCRC16("6F 05 00 15 FF 00");//发送”6F 05 00 15 FF 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf01, 0, buf01.Length);//发送数据

            int BStart01Count = 0;
            while (!BStartStep01Result.IsSuccess)//当BStart01步骤执行成功时，跳出这个步骤
            {
                if (BStartStep01Result.Msg == "指令返回异常")//BStart01返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(BStartStep01Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                BStart01Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (BStart01Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("BStart01步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //MessageBox.Show(BStartStep01Result.Msg, "提示", MessageBoxButtons.OK);

            //当程序运行到这里的时候，证明已经跳出BStart01的while循环，BStart01步骤已经执行成功
            //此处开始BStart02步骤
            //首先需要注销BStart01系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BStart01_DataReceived);
            }
            catch { }
            //注册BStart02步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BStart02_DataReceived);
            }
            catch { }

            byte[] buf02 = crc.FillCRC16("6F 05 00 15 00 00");//发送”6F 05 00 15 00 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf02, 0, buf02.Length);//发送数据

            int BStart02Count = 0;
            while (!BStartStep02Result.IsSuccess)//当BStep02步骤执行成功时，跳出这个步骤
            {
                if (BStartStep02Result.Msg == "指令返回异常")//AStep02返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(AStartStep02Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                BStart02Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (BStart02Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("BStart02步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }
            //当程序运行到这里的时候，证明已经跳出BStart02的while循环，BStart02步骤已经执行成功
            //此处开始BStart03步骤
            //首先需要注销BStart03系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BStart02_DataReceived);
            }
            catch { }
            //注册BStart03步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BStart03_DataReceived);
            }
            catch { }


            byte[] buf03 = crc.FillCRC16("6F 05 00 14 FF 00");//发送”6F 05 00 14 FF 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf03, 0, buf03.Length);//发送数据

            int BStart03Count = 0;
            while (!BStartStep03Result.IsSuccess)//当BStep03步骤执行成功时，跳出这个步骤
            {
                if (BStartStep03Result.Msg == "指令返回异常")//BStep03返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(BStartStep03Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                BStart03Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (BStart03Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("AStart03步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }

            //当程序运行到这里的时候，证明已经跳出BStart03的while循环，BStart03步骤已经执行成功
            //此处开始BStart04步骤
            //首先需要注销BStart04系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BStart03_DataReceived);
            }
            catch { }
            //注册BStart04步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BStart04_DataReceived);
            }
            catch { }


            byte[] buf04 = crc.FillCRC16("6F 05 00 14 00 00");//发送”6F 05 00 14 00 00 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf04, 0, buf04.Length);//发送数据

            int BStart04Count = 0;
            while (!BStartStep04Result.IsSuccess)//当AStep03步骤执行成功时，跳出这个步骤
            {
                if (BStartStep04Result.Msg == "指令返回异常")//AStep03返回指令不一致，执行失败
                {
                    XtraMessageBox.Show(BStartStep04Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }

                BStart04Count += 1;
                System.Threading.Thread.Sleep(1000);  // while休眠一秒
                if (BStart04Count > 10)//超过10次重试，则认为失败
                {
                    XtraMessageBox.Show("BStart04步骤执行失败。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;//此流程终止执行
                }
            }

            //当程序运行到这里的时候，证明已经跳出BStart04的while循环，AS04步骤已经执行成功
            //此按钮所有流程步骤完成
            //首先需要注销BStart04系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived -= new SerialDataReceivedEventHandler(BStart04_DataReceived);
            }
            catch { }

            XtraMessageBox.Show(BStartStep04Result.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            //开始循环检测A线喂够标志

            //注册BFeedEnough步骤系统接收数据托管监听事件
            try
            {
                mSerialPort.DataReceived += new SerialDataReceivedEventHandler(BFeedEnough_DataReceived);
            }
            catch { }

            byte[] buf05 = crc.FillCRC16("6F 01 00 17 00 01");//发送”6F 01 00 16 00 01 xx xx”，其中xx xx为校验位，有CRC算法计算
            mSerialPort.Write(buf05, 0, buf05.Length);//发送数据

            while (!BFeedEnoughResult.IsSuccess)
            {
                System.Threading.Thread.Sleep(2000);  // while休眠一秒
                mSerialPort.Write(buf05, 0, buf05.Length);//发送数据
            }

            XtraMessageBox.Show(BFeedEnoughResult.Msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

        }

        /// <summary>
        /// BStart01系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BStart01_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 21)
                {
                    BStartStep01Result.IsSuccess = true;
                    BStartStep01Result.Msg = "SUCCESS";
                    //Console.WriteLine("SUCCESS");
                }
                else
                {
                    BStartStep01Result.IsSuccess = false;
                    BStartStep01Result.Msg = "指令返回异常";
                }
            }

        }

        /// <summary>
        /// BStart02系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BStart02_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 21)
                {
                    BStartStep02Result.IsSuccess = true;
                    BStartStep02Result.Msg = "SUCCESS";
                }
                else
                {
                    BStartStep02Result.IsSuccess = false;
                    BStartStep02Result.Msg = "指令返回异常";
                }
            }
        }
        /// <summary>
        /// BStart03系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BStart03_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
            //Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 20)
                {
                    BStartStep03Result.IsSuccess = true;
                    BStartStep03Result.Msg = "SUCCESS";
                }
                else
                {
                    BStartStep03Result.IsSuccess = false;
                    BStartStep03Result.Msg = "指令返回异常";
                }
            }


        }

        /// <summary>
        /// BStart04系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BStart04_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            //Console.WriteLine(n);
            //Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                buffer[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 8)
            {
                recv_count = 0;
                if (buffer[0] == 111 && buffer[1] == 5 && buffer[2] == 0 && buffer[3] == 20)
                {
                    BStartStep04Result.IsSuccess = true;
                    BStartStep04Result.Msg = "B线启动成功！";
                }
                else
                {
                    BStartStep04Result.IsSuccess = false;
                    BStartStep04Result.Msg = "指令返回异常";
                }
            }
        }
        /// <summary>
        /// BFeedEnough系统托管收取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BFeedEnough_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = mSerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                                            //Console.WriteLine(n);
                                            // Console.WriteLine(' ');
            byte[] buf = new byte[n];
            mSerialPort.Read(buf, 0, n);
            for (int k = 0; k < n; k++)
            {
                bufferFeedEnough[k + recv_count] = buf[k];
                //Console.WriteLine(buf[k]);
            }
            recv_count += n;
            //Console.WriteLine(' ');
            //for (int k = 0; k < 8; k++)
            //{
            //    Console.WriteLine(buffer[k]);
            //}
            //Console.WriteLine(recv_count);
            //Console.WriteLine(' ');
            if (recv_count == 6)
            {
                recv_count = 0;
                if (bufferFeedEnough[0] == 111 && bufferFeedEnough[1] == 1 && bufferFeedEnough[2] == 1 && bufferFeedEnough[3] == 1)
                {
                    BFeedEnoughResult.IsSuccess = true;
                    BFeedEnoughResult.Msg = "B线喂够！";
                    //Console.WriteLine("SUCCESS");
                }
                else
                {
                    BFeedEnoughResult.IsSuccess = false;
                    BFeedEnoughResult.Msg = "指令返回异常";
                }
            } 
        }
    }
}