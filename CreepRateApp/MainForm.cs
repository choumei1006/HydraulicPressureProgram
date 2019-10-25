using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using java.io;
using DevExpress.XtraEditors;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting;
using MongoDB.Driver.GridFS;
using System.Threading;
using System.IO.Ports;
using SerialportSample;
using System.Timers;
using System.Collections;
using System.IO;//引用此命名空间是用于数据的写入与读取

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Reflection;



namespace CreepRateApp
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        delegate void SetChartControlPointsBack(SeriesPoint point);
        delegate void ClearChartControlPointsBack();

        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

        //richTextBox1窗体
        public static System.Windows.Forms.RichTextBox richTextBox1;
        public static byte EquipmentId = 255;
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int received_count = 0;//接收计数
        private long send_count = 0;//发送计数

        private int nextSeriesN = 1;//即将工作的线路
        private int currentSeriesN = 0;//当前正在正在工作的线路，初始化0表示不存在

        private int selectSeriesCount = 3;
        private Boolean isOneSeriesCheck = true;
        private Boolean isTwoSeriesCheck = true;
        private Boolean isThreeSeriesCheck = true;
        private String oneSeriesSendMsg = "";
        private String twoSerisSendMsg = "";
        private String threeSeriesSendMsg = "";
         


        //线路循环切换
        public void seriesChange()
        {
            List<int> roadNumList = getRoadNumList();
            if (this.nextSeriesN == roadNumList[roadNumList.Count - 1])
            {
                this.nextSeriesN = roadNumList[0];
            }
            else
            {
                int index = roadNumList.IndexOf(nextSeriesN);
                this.nextSeriesN = roadNumList[index + 1];
            }
            /*if (this.nextSeriesN == selectSeriesCount)
            {
                this.nextSeriesN = 1;
            }
            else
            {
                this.nextSeriesN += 1;
            }*/
        }

        //获取当前选中的温度线路数组
        private List<int> getRoadNumList()
        {
            List<int> result = new List<int>();
            if (barCheckItem1.Checked)
            {
                result.Add(1);
            }
            if (barCheckItem2.Checked)
            {
                result.Add(2);
            }
            if (barCheckItem3.Checked)
            {
                result.Add(3);
            }

            return result;

        }
        /*private List<int> getRoadNumList()
        {
            List<int> result = new List<int>();
            if (checkBox1.Checked) {
                result.Add(1);
            }
            if (checkBox2.Checked) {
                result.Add(2);
            }
            if (checkBox3.Checked) {
                result.Add(3);
            }

            return result;

        }*/

        //Series series1 = new Series("曲线2", ViewType.Spline);
        //Series series2 = new Series("曲线3", ViewType.Spline);

        public void SetChartControl1Point(SeriesPoint point)
        {
            if (this.chartControl1.InvokeRequired)
            {
                SetChartControlPointsBack setChartControlPointsBack = new SetChartControlPointsBack(SetChartControl1Point);
                this.chartControl1.Invoke(setChartControlPointsBack, point);
            }
            else
            {

                if (this.currentSeriesN == 1)
                {
                    this.chartControl1.Series[0].Points.Add(point);
                }
                else if (this.currentSeriesN == 2)
                {
                    this.chartControl1.Series[1].Points.Add(point);
                }
                else if (this.currentSeriesN == 3)
                {
                    this.chartControl1.Series[2].Points.Add(point);
                }
                else
                {

                }

                //--------------------------------sxm三路温度数据-------------------
                //series1.Points.Add(point);
                //series2.Points.Add(point);

                //this.chartControl1.Series.Add(series1);
                //this.chartControl1.Series.Add(series2);




                //this.chartControl1.Series[1].Points.Add(point);
                //this.chartControl1.Series[2].Points.Add(point);
                //------------------------------------------------------------------

                //MessageBox.Show(this.chartControl1.Series[0].Points.Count+ "", "信息提示message", MessageBoxButtons.OK);
                this.chartControl1.Refresh();
            }
        }

        public void SetChartControl2Point(SeriesPoint point)
        {
            if (this.chartControl2.InvokeRequired)
            {
                SetChartControlPointsBack setChartControlPointsBack = new SetChartControlPointsBack(SetChartControl2Point);
                this.chartControl2.Invoke(setChartControlPointsBack, point);
            }
            else
            {
                if (this.currentSeriesN == 1)
                {
                    this.chartControl2.Series[0].Points.Add(point);
                }
                else if (this.currentSeriesN == 2)
                {
                    this.chartControl2.Series[1].Points.Add(point);
                }
                else if (this.currentSeriesN == 3)
                {
                    this.chartControl2.Series[2].Points.Add(point);
                }
                else
                {

                }

                //series1.Points.Add(point);
                //series2.Points.Add(point);

                //this.chartControl1.Series.Add(series1);
                //this.chartControl1.Series.Add(series2);

                this.chartControl2.Refresh();
            }
        }

        public void ClearChartcontrol1Points()
        {
            if (this.chartControl1.InvokeRequired)
            {
                ClearChartControlPointsBack clearChartControlPointsBack = new ClearChartControlPointsBack(ClearChartcontrol1Points);
                this.chartControl1.Invoke(clearChartControlPointsBack);
            }
            else
            {
                try
                {
                    //this.chartControl1.Series[0].Points.Clear();
                    //series1.Points.Clear();
                    //series2.Points.Clear();
                    XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
                    diagram.AxisX.ConstantLines.Clear();
                    diagram.AxisY.ConstantLines.Clear();
                    this.chartControl1.Refresh();

                }
                catch { }

                AxisRange DIA = (AxisRange)((XYDiagram)chartControl1.Diagram).AxisY.Range;
                DIA.SetMinMaxValues(3300, 3600);
            }
        }

        public void ClearChartcontrol2Points()
        {
            if (this.chartControl2.InvokeRequired)
            {
                ClearChartControlPointsBack clearChartControlPointsBack = new ClearChartControlPointsBack(ClearChartcontrol2Points);
                this.chartControl2.Invoke(clearChartControlPointsBack);
            }
            else
            {
                try
                {
                    /*this.chartControl2.Series[0].Points.Clear();
                    this.chartControl2.Refresh();*/
                    XYDiagram diagram2 = (XYDiagram)chartControl2.Diagram;
                    diagram2.AxisX.ConstantLines.Clear();
                    diagram2.AxisY.ConstantLines.Clear();
                    this.chartControl2.Refresh();
                }
                catch { }
            }
        }

        //定义端口类
        private SerialPort ComDevice = new SerialPort();

        private int num_tal = 0;     //tal特征值的横坐标
        private int index_tsef = 0;  //tsef特征值的横坐标
        private int index_tem = 0;   //tem特征值的横坐标
        private double result_tal = 0.0;
        private List<string> temperatureList = new List<string>();
        private List<String> slopDrawList = new List<string>();


        private List<string> temperatureList1 = new List<string>();
        private List<String> slopDrawList1 = new List<string>();


        private List<string> temperatureList2 = new List<string>();
        private List<String> slopDrawList2 = new List<string>();

        private Boolean isNeedComRecevied = false;

        //网口通信 
        private static string localIPAddress = GetIpAddress();   //获取本地IP地址
        public static IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(localIPAddress), 10101);   //（本地）应用程序与特定主机特定服务的连接点
        
        //public static UdpClient udpcSend;
        public static UdpClient udpClient = new UdpClient(localIpep);

        
        
        //bool isNeedUdpRecv;   //是否监听UDP报文，在UDP监听阶段为true
        public static Thread thrRecv = new Thread(ReceiveMessage);       //线程：监听UDP报文
        public static Thread thrSend;       //线程：监听UDP报文
        public static bool isCollecting = false;
        

        private static readonly object lockHelper = new object();

        public MainForm()
        {

            //Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();

            //设置RichTextBox1的相关位置属性
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            layoutControl1.Controls.Add(richTextBox1);
            richTextBox1.Location = new System.Drawing.Point(24, 571);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(1571, 106);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "";
            this.layoutControlItem3.Control = richTextBox1;
            this.layoutControlItem3.TextVisible = false;



            //设置串口相关属性
            //端口
            ComDevice.PortName = GlobalValue.PortName;
            //波特率
            ComDevice.BaudRate = int.Parse(GlobalValue.BaudRate);
            //校验
            ComDevice.Parity = (Parity)Convert.ToInt32(GlobalValue.Parity);
            //停止位
            ComDevice.StopBits = (StopBits)Convert.ToInt32(GlobalValue.StopBits);
            //数据位
            ComDevice.DataBits = int.Parse(GlobalValue.DataBits);


            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);


            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = double.Parse(GlobalValue.IntalvasTime); //执行间隔时间,单位为毫秒; 这里实际间隔为10分钟  
            //timer.Start();
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick);   //定时发送接收数据的指令

            //开启UDP监听
            //udpClient = new UdpClient(localIpep);
            udpClient.Client.ReceiveTimeout = 5000;
            //thrRecv = new Thread(ReceiveMessage);
            thrRecv.Start(); 
            showMessage(richTextBox1,string.Format("{0}{1}", "上位机(" + localIpep + ")_" + System.DateTime.Now.ToString() + "：", "UDP监听已开启"));
            

        }

        /// <summary>
        /// 手动分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAnalysisFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            bool isHuaTem = false;
            //选择数据类型（华氏温度、摄氏温度）
            if (XtraMessageBox.Show("文件中温度数据是否是华氏温度数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                isHuaTem = true;
            }

            //清空temperatureList全局变量
            try
            {
                this.temperatureList.Clear();
            }
            catch { }


            //清空温度、斜率曲线
            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
            XYDiagram diagram2 = (XYDiagram)chartControl2.Diagram;

            try
            {
                this.chartControl1.Series[0].Points.Clear();
                this.chartControl2.Series[0].Points.Clear();
                this.chartControl1.Series[1].Points.Clear();
                this.chartControl2.Series[1].Points.Clear();
                this.chartControl1.Series[2].Points.Clear();
                this.chartControl2.Series[2].Points.Clear();

                diagram.AxisX.ConstantLines.Clear();
                diagram.AxisY.ConstantLines.Clear();

                diagram2.AxisX.ConstantLines.Clear();
                diagram2.AxisY.ConstantLines.Clear();
            }
            catch { }

            entity.AnalysisModel uploadAnalysisModel = new entity.AnalysisModel();
            if (uploadAnalysisModel.FileInfoList == null)
            {
                uploadAnalysisModel.FileInfoList = new List<entity.FileInfo>();
            }

            /*string timeString = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            List<string> slopList = new List<string>();
            List<string> secDeriList = new List<string>();
            List<double> quxianList = new List<double>();
            List<string> formatTxt = new List<string>();*/



            //最高温度
            //string maxTemperature = this.maxTemperatureInput.Text;
            //最低温度
            //string minTemperature = this.minTemperatureInput.Text;

            this.preAnalysisOpenFileDialog.Multiselect = true;
            if (this.preAnalysisOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                //string filePath = this.preAnalysisOpenFileDialog.FileName;
                string[] fileNames = this.preAnalysisOpenFileDialog.FileNames;
                int fileNum = fileNames.Length;


                if (fileNum > 0 && fileNum <= 3)
                {
                    //if (string.IsNullOrWhiteSpace(maxTemperature) || string.IsNullOrWhiteSpace(minTemperature))
                    //{
                    for (int fileIndex = 0; fileIndex < fileNum; fileIndex++)
                    {
                        //绘制曲线、特征值，计算参数
                        drawLine(fileNames[fileIndex], null, null, isHuaTem, fileIndex, diagram, uploadAnalysisModel);
                    }
                    //}
                    //else
                    {
                        /*double maxTep = double.Parse(maxTemperature) * 1.8 + 32;
                        double minTep = double.Parse(minTemperature) * 1.8 + 32;

                        temperatureList = Core.ChartList.GetTemperature(filePath, maxTep.ToString(), minTep.ToString());
                        slopList = Core.ChartList.GetSlop(temperatureList);

                        if (temperatureList.Count > 0 && slopList.Count > 0)
                        {
                            List<entity.XY> list = new List<entity.XY>();
                            List<entity.XY> list2 = new List<entity.XY>();

                            for (int i = 0; i < temperatureList.Count; i++)
                            {
                                entity.XY xy = new entity.XY();
                                xy.x = i * 0.5;
                                xy.y = Math.Round(double.Parse(temperatureList[i]), 2);
                                list.Add(xy);
                            }

                            for (int i = 0; i < slopList.Count; i++)
                            {
                                entity.XY xy = new entity.XY();
                                xy.x = i * 0.5;
                                xy.y = Math.Round(double.Parse(slopList[i]), 2);
                                list2.Add(xy);
                            }

                            for (int i = 0; i < list.Count; i++)
                            {
                                list[i].y = Math.Round((list[i].y - 32) * 5 / 9, 2);
                                quxianList.Add(list[i].y);
                            }

                            int min1 = (int)list[list.Count - 1].y - 1;
                            int max1 = (int)list[0].y + 1;
                            int min2 = (int)list2[0].y - 1;
                            int max2 = (int)list2[list2.Count - 1].y + 1;

                            this.chartControl1.DataSource = list;
                            AxisRange DIA = (AxisRange)((XYDiagram)chartControl1.Diagram).AxisY.Range;
                            DIA.SetMinMaxValues(min1, max1);

                            this.chartControl2.DataSource = list2;
                            AxisRange DIA2 = (AxisRange)((XYDiagram)chartControl2.Diagram).AxisY.Range;
                            //DIA2.SetMinMaxValues(min2, max2);

                            List<entity.ChartData> temperatureGridList = new List<entity.ChartData>();
                            for (int i = 0; i < list.Count; i++)
                            {
                                entity.ChartData cd = new entity.ChartData();
                                cd.Number = i;
                                cd.DataValue = list[i].y;
                                temperatureGridList.Add(cd);
                            }
                            this.gridControl2.DataSource = temperatureGridList;

                            List<entity.ChartData> slopGridList = new List<entity.ChartData>();
                            for (int i = 0; i < list2.Count; i++)
                            {
                                entity.ChartData cd = new entity.ChartData();
                                cd.Number = i;
                                cd.DataValue = list2[i].y;
                                slopGridList.Add(cd);
                            }
                            this.gridControl3.DataSource = slopGridList;
                        }*/
                    }
                }
                else if (fileNames.Length > 3)
                {
                    MessageBox.Show("最多只能选择3个数据文件！", "操作提示");
                }
                if (XtraMessageBox.Show("计算完毕，是否将计算结果及相关文件保存至云端？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string uploadFileTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                    //待分析的txt文件路径
                    //string uploadTxtPath = filePath;
                    //温度分析曲线图
                    string uploadTempereturePic = Application.StartupPath + "\\temp\\温度分析曲线_" + uploadFileTime + ".Jpeg";
                    this.chartControl1.ExportToImage(uploadTempereturePic, ImageFormat.Jpeg);
                    //温度采集Excel报表
                    string uploadTemperetureXls = Application.StartupPath + "\\temp\\温度采集报表_" + uploadFileTime + ".xls";
                    //this.gridControl2.ExportToXls(uploadTemperetureXls);
                    //斜率分析曲线图
                    string uploadSlopPic = Application.StartupPath + "\\temp\\斜率分析曲线图_" + uploadFileTime + ".Jpeg";
                    this.chartControl2.ExportToImage(uploadSlopPic, ImageFormat.Jpeg);
                    //斜率计算记录Excel报表
                    string uploadSlopXls = Application.StartupPath + "\\temp\\斜率计算记录报表_" + uploadFileTime + ".xls";
                    //this.gridControl3.ExportToXls(uploadSlopXls);

                    try
                    {
                        //uploadAnalysisModel.FileInfoList.Add(UploadFiles(uploadTxtPath, "txt"));
                    }
                    catch { }

                    try
                    {
                        uploadAnalysisModel.FileInfoList.Add(UploadFiles(uploadTempereturePic, "Jpeg"));
                    }
                    catch { }

                    try
                    {
                        uploadAnalysisModel.FileInfoList.Add(UploadFiles(uploadTemperetureXls, "xls"));
                    }
                    catch { }

                    try
                    {
                        uploadAnalysisModel.FileInfoList.Add(UploadFiles(uploadSlopPic, "Jpeg"));
                    }
                    catch { }

                    try
                    {
                        uploadAnalysisModel.FileInfoList.Add(UploadFiles(uploadSlopXls, "xls"));
                    }
                    catch { }

                    try
                    {
                        Core.DataBaseTools.Insert<entity.AnalysisModel>("AnalysisResult", uploadAnalysisModel);
                    }
                    catch { }
                    XtraMessageBox.Show("操作完成。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public entity.FileInfo UploadFiles(string filePath, string fileType)
        {
            MongoGridFSFileInfo uploadFileInfo = Core.DataBaseTools.UpFile(filePath);
            entity.FileInfo fi = new entity.FileInfo();
            fi.Id = uploadFileInfo.Id.ToString();
            fi.Name = uploadFileInfo.Name;
            fi.Type = fileType;
            return fi;
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreepRateApp.Form.HistoryHandAnalysis hha = new Form.HistoryHandAnalysis();
            hha.ShowDialog();
        }

       

        

        /// <summary>
        /// 一旦ComDevice.DataReceived事件发生，就将从串口接收到的数据显示到接收端对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            if (isNeedComRecevied)
            {
                //this.ClearChartcontrol1Points();
                //this.ClearChartcontrol2Points();


                int n = ComDevice.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致

                if (n == 9)
                {

                    byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
                    received_count += n;//增加接收计数
                    ComDevice.Read(buf, 0, n);//读取缓冲数据
                    builder.Clear();//清除字符串构造器的内容

                    //String[] getStrings = new String[buf.Length];

                    //for(int i = 0; i < buf.Length; i++)
                    //{
                    //    getStrings[i] = buf[i].ToString("X2");
                    //}

                    String message = this.JieMa(buf);
                    //MessageBox.Show("" + message, "信息提示message", MessageBoxButtons.OK);
                    String[] messages = message.Split(' ');
                    //MessageBox.Show("" + messages[3], "信息提示3", MessageBoxButtons.OK);
                    //MessageBox.Show(messages[4] + "", "信息提示4", MessageBoxButtons.OK);
                    //MessageBox.Show("" + messages[5], "信息提示5", MessageBoxButtons.OK);
                    //MessageBox.Show("" + messages[6], "信息提示6", MessageBoxButtons.OK);

                    //String t1 = Convert.ToInt32(messages[3], 16)+"";
                    //String t2= Convert.ToInt32(messages[4], 16) + "";
                    //String t3 = Convert.ToInt32(messages[5], 16) + "";
                    //String t4 = Convert.ToInt32(messages[6], 16) + "";



                    //将接收到的16进制串的4、5、6、7字节用对应的16进制数表示
                    try
                    {
                        int hex_t1 = Convert.ToInt32(messages[3], 16);
                        int hex_t2 = Convert.ToInt32(messages[4], 16);
                        int hex_t3 = Convert.ToInt32(messages[5], 16);
                        int hex_t4 = Convert.ToInt32(messages[6], 16);

                        string sss = "";
                        for (int i = 0; i < messages.Length; i++)
                        {
                            sss += messages[i] + " ";
                        }

                        byte[] bytes = new byte[4];

                        BitConverter.ToSingle(bytes, 0);

                        //16进制单精度IEEE754浮点数--->10进制
                        int data = hex_t1 << 24 | hex_t2 << 16 | hex_t3 << 8 | hex_t4;

                        int nSign;
                        if ((data & 0x80000000) > 0)
                        {
                            nSign = -1;
                        }
                        else
                        {
                            nSign = 1;
                        }
                        int nExp = data & (0x7F800000);
                        nExp = nExp >> 23;
                        float nMantissa = data & (0x7FFFFF);

                        if (nMantissa != 0)
                            nMantissa = 1 + nMantissa / 8388608;

                        double ty = Math.Round(nSign * nMantissa * (2 << (nExp - 128)), 2);
                        //MessageBox.Show("" + ty, "信息提示ty", MessageBoxButtons.OK);


                        //接收数据没问题了，你去确认一下16进制具体怎么转换10进制温度的。
                        //String tempture = t1 + t2 + t3 + t4;
                        //double ty = double.Parse(tempture);

                        /// **********************************************************************************************
                        /// **********************************************************************************************
                        /// ********************************************************************************************** 
                        /// 后面请根据实际COM串口获取数据进行数据处理，例如字符串切割操作等等，将其转换为可以使用的数据类型。
                        /// 此处可以使用的数据类型为double[]，即double数组
                        /// **********************************************************************************************
                        /// 处理代码，开始
                        /// 
                        //string[] split = receivedString.Split(new char[] { ' ' });
                        //Double[] ty = new Double[split.Length];
                        //for (int i = 0; i < split.Length; i++)
                        //{
                        //    if (String.IsNullOrWhiteSpace(split[i]))
                        //        continue;
                        //    ty[i] = Convert.ToDouble(split[i]);
                        //}
                        ///double[] ty = { 2370.0, 2369.0, 2366.0, 2360.0, 2350.0, 2343.0, 2334.0, 2328.0, 2322.0, 2313.0, 2307.0, 2300.0, 2294.0, 2289.0, 2283.0, 2278.0, 2272.0, 2268.0, 2264.0, 2259.0, 2256.0, 2251.0, 2249.0, 2246.0, 2244.0, 2241.0, 2239.0, 2238.0, 2237.0, 2235.0, 2235.0, 2234.0, 2233.0, 2233.0, 2233.0, 2232.0, 2232.0, 2232.0, 2232.0, 2232.0, 2232.0, 2231.0, 2231.0, 2231.0, 2231.0, 2231.0, 2231.0, 2231.0, 2230.0, 2230.0, 2229.0, 2229.0, 2228.0, 2227.0, 2226.0, 2225.0, 2223.0, 2222.0, 2221.0, 2219.0, 2218.0, 2216.0, 2214.0, 2213.0, 2211.0, 2210.0, 2208.0, 2206.0, 2205.0, 2203.0, 2202.0, 2200.0, 2198.0, 2197.0, 2195.0, 2194.0, 2192.0, 2190.0, 2189.0, 2187.0, 2186.0, 2184.0, 2183.0, 2181.0, 2179.0, 2178.0, 2176.0, 2175.0, 2174.0, 2172.0, 2171.0, 2169.0, 2168.0, 2166.0, 2165.0, 2164.0, 2162.0, 2161.0, 2159.0, 2158.0, 2156.0, 2155.0, 2154.0, 2153.0, 2151.0, 2150.0, 2148.0, 2147.0, 2146.0, 2145.0, 2144.0, 2142.0, 2141.0, 2140.0, 2138.0, 2138.0, 2136.0, 2135.0, 2134.0, 2133.0, 2132.0, 2131.0, 2130.0, 2129.0, 2128.0, 2127.0, 2125.0, 2125.0, 2123.0, 2122.0, 2121.0, 2120.0, 2120.0, 2119.0, 2117.0, 2116.0, 2115.0, 2115.0, 2114.0, 2113.0, 2112.0, 2111.0, 2110.0, 2109.0, 2108.0, 2108.0, 2107.0, 2106.0, 2105.0, 2104.0, 2104.0, 2103.0, 2102.0, 2102.0, 2101.0, 2100.0, 2100.0, 2099.0, 2098.0, 2098.0, 2098.0, 2097.0, 2096.0, 2096.0, 2096.0, 2095.0, 2095.0, 2094.0, 2094.0, 2093.0, 2093.0, 2092.0, 2091.0, 2091.0, 2090.0, 2089.0, 2088.0, 2088.0, 2087.0, 2086.0, 2086.0, 2085.0, 2084.0, 2084.0, 2083.0, 2083.0, 2082.0, 2082.0, 2081.0, 2080.0, 2080.0, 2080.0, 2079.0, 2079.0, 2079.0, 2078.0, 2078.0, 2078.0, 2078.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2077.0, 2078.0, 2078.0, 2078.0, 2078.0, 2079.0, 2079.0, 2079.0, 2080.0, 2080.0, 2081.0, 2081.0, 2082.0, 2082.0, 2082.0, 2083.0, 2084.0, 2084.0, 2085.0, 2085.0, 2086.0, 2086.0, 2087.0, 2087.0, 2088.0, 2089.0, 2089.0, 2090.0, 2090.0, 2091.0, 2091.0, 2092.0, 2092.0, 2093.0, 2093.0, 2094.0, 2094.0, 2095.0, 2095.0, 2096.0, 2096.0, 2096.0, 2096.0, 2097.0, 2097.0, 2097.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2098.0, 2097.0, 2097.0, 2097.0, 2097.0, 2096.0, 2096.0, 2096.0, 2096.0, 2095.0, 2095.0, 2095.0, 2094.0, 2094.0, 2094.0, 2093.0, 2093.0, 2093.0, 2092.0, 2092.0, 2091.0, 2091.0, 2090.0, 2090.0, 2089.0, 2089.0, 2088.0, 2088.0, 2087.0, 2087.0, 2086.0, 2085.0, 2085.0, 2084.0, 2084.0, 2083.0, 2083.0, 2082.0, 2081.0, 2081.0, 2080.0, 2079.0, 2079.0, 2078.0, 2077.0, 2076.0, 2076.0, 2075.0, 2074.0, 2073.0, 2073.0, 2072.0, 2071.0, 2070.0, 2069.0, 2069.0, 2067.0, 2067.0, 2066.0, 2065.0, 2064.0, 2063.0, 2062.0, 2060.0, 2060.0, 2059.0, 2058.0, 2057.0, 2056.0, 2055.0, 2054.0, 2052.0, 2051.0, 2050.0, 2049.0, 2048.0, 2046.0, 2045.0, 2044.0, 2043.0, 2042.0, 2040.0, 2039.0, 2038.0, 2037.0, 2036.0, 2034.0, 2033.0, 2031.0, 2030.0, 2029.0, 2027.0, 2026.0, 2024.0, 2023.0, 2022.0, 2021.0, 2019.0, 2018.0, 2017.0, 2015.0, 2014.0, 2012.0, 2011.0, 2010.0, 2008.0, 2007.0, 2006.0, 2004.0, 2003.0, 2001.0 };
                        /// 上面ty为数据例子
                        /// 具体应该这样做
                        /// 1.处理receivedString
                        /// 2.处理后获取例如变量ty类型的数据，如用split函数进行切割，获取数组
                        /// @@@@@@@@@@@@@@@@@@@如果实在处理不了，请在570行，鼠标点击行号打断点，然后鼠标悬浮在receivedString上，将receivedString的值粘贴到txt中发给我，我来处理。
                        /// 处理代码，结束
                        /// 

                        if (this.currentSeriesN == 1)//表示操作第1路
                        {
                            int oldListCount = temperatureList.Count;

                            //将旧数据加入数组
                            //for (int i = 0; i < oldListCount; i++)
                            //{
                            //    SeriesPoint sp_old = new SeriesPoint();
                            //    sp_old.Argument = (i * 0.5).ToString();
                            //    double[] ys_old = { Math.Round(double.Parse(temperatureList[i]), 2) };

                            //    sp_old.Values = ys_old;
                            //    this.chartControl1.Series[0].Points.Add(sp_old);
                            //}


                            SeriesPoint sp = new SeriesPoint();
                            sp.Argument = (oldListCount * 1).ToString();//接收数据不可能一次接收完，此处为了实现新增点继续在原有点上绘制。
                            //Double[] tys = new Double[1];
                            double[] tys = { ty };
                            //tys[0] = ty;
                            sp.Values = tys;

                            //this.chartControl1.Series[0].Points.Add(sp); 

                            this.SetChartControl1Point(sp);
                            temperatureList.Add(ty.ToString());
                            //temperatureList[temperatureList.Count] = ty.ToString();
                            //temperatureList.Insert(temperatureList.Count,ty.ToString()) ;

                            if (oldListCount > 0)
                            {
                                //将斜率旧数据添加
                                //int oldSlopCount = slopDrawList.Count;
                                //for (int i = 0; i < oldSlopCount; i++) {

                                //    SeriesPoint sp2_old = new SeriesPoint();
                                //    sp2_old.Argument = (i * 0.5).ToString();
                                //    double[] tyy2_old = { Math.Round(double.Parse(temperatureList[i]), 2) };

                                //    sp2_old.Values = tyy2_old;
                                //    this.chartControl2.Series[0].Points.Add(sp2_old); 
                                //}

                                SeriesPoint sp2 = new SeriesPoint();
                                //sp2.Argument = (temperatureList.Count).ToString();
                                sp2.Argument = oldListCount.ToString();
                                //TODO：时间需要根据实际情况测定，单位秒S，
                                double paramArgument = 1;
                                double xianshi = (double.Parse(temperatureList[temperatureList.Count - 1]) - double.Parse(temperatureList[temperatureList.Count - 2])) / paramArgument; ;
                                //TODO：实际测定后，就删除就行了。
                                double fangda = xianshi * 1000;
                                double[] tyy2 = { fangda };
                                //double[] tyy2 = { (ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5 };
                                sp2.Values = tyy2;
                                this.SetChartControl2Point(sp2);
                                slopDrawList.Add(((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 1).ToString());
                                //slopDrawList.Insert(slopDrawList.Count, ((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5).ToString());
                                //slopDrawList[slopDrawList.Count] = ((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5).ToString();

                            }
                            this.seriesChange();


                            //for (int i = 0; i < ty.Length; i++)
                            //{
                            //    SeriesPoint sp = new SeriesPoint();
                            //    sp.Argument = ((oldListCount + i) * 0.5).ToString();//接收数据不可能一次接收完，此处为了实现新增点继续在原有点上绘制。
                            //    double[] tyy = { ty[i] };
                            //    sp.Values = tyy;
                            //    //this.chartControl1.Series[0].Points.Add(sp);
                            //    this.SetChartControl1Point(sp);
                            //    temperatureList.Add(ty[i].ToString());

                            //    if (i > 0)
                            //    {
                            //        SeriesPoint sp2 = new SeriesPoint();
                            //        sp2.Argument = ((temperatureList.Count + i) * 0.5).ToString();
                            //        double[] tyy2 = { (ty[i] - ty[i - 1]) / 0.5 };
                            //        sp2.Values = tyy2;
                            //        this.SetChartControl2Point(sp2);
                            //    }
                            //}
                        }
                        else if (this.currentSeriesN == 2)//表示操作第2路
                        {
                            int oldListCount = temperatureList1.Count;

                            //将旧数据加入数组
                            //for (int i = 0; i < oldListCount; i++)
                            //{
                            //    SeriesPoint sp_old = new SeriesPoint();
                            //    sp_old.Argument = (i * 0.5).ToString();
                            //    double[] ys_old = { Math.Round(double.Parse(temperatureList[i]), 2) };

                            //    sp_old.Values = ys_old;
                            //    this.chartControl1.Series[0].Points.Add(sp_old);
                            //}


                            SeriesPoint sp = new SeriesPoint();
                            sp.Argument = (oldListCount * 1).ToString();//接收数据不可能一次接收完，此处为了实现新增点继续在原有点上绘制。
                            //Double[] tys = new Double[1];
                            double[] tys = { ty };
                            //tys[0] = ty;
                            sp.Values = tys;

                            //this.chartControl1.Series[0].Points.Add(sp); 

                            this.SetChartControl1Point(sp);
                            temperatureList1.Add(ty.ToString());
                            //temperatureList[temperatureList.Count] = ty.ToString();
                            //temperatureList.Insert(temperatureList.Count,ty.ToString()) ;

                            if (oldListCount > 0)
                            {
                                //将斜率旧数据添加
                                //int oldSlopCount = slopDrawList.Count;
                                //for (int i = 0; i < oldSlopCount; i++) {

                                //    SeriesPoint sp2_old = new SeriesPoint();
                                //    sp2_old.Argument = (i * 0.5).ToString();
                                //    double[] tyy2_old = { Math.Round(double.Parse(temperatureList[i]), 2) };

                                //    sp2_old.Values = tyy2_old;
                                //    this.chartControl2.Series[0].Points.Add(sp2_old); 
                                //}

                                SeriesPoint sp2 = new SeriesPoint();
                                //sp2.Argument = (temperatureList1.Count).ToString();
                                sp2.Argument = oldListCount.ToString();
                                //TODO：时间需要根据实际情况测定，单位秒S，
                                double paramArgument = 1;
                                double xianshi = (double.Parse(temperatureList1[temperatureList1.Count - 1]) - double.Parse(temperatureList1[temperatureList1.Count - 2])) / paramArgument; ;
                                //TODO：实际测定后，就删除就行了。
                                double fangda = xianshi * 1000;
                                double[] tyy2 = { fangda };
                                //double[] tyy2 = { (ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5 };
                                sp2.Values = tyy2;
                                this.SetChartControl2Point(sp2);
                                slopDrawList1.Add(((ty - double.Parse(temperatureList1[temperatureList1.Count - 1])) / 1).ToString());
                                //slopDrawList.Insert(slopDrawList.Count, ((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5).ToString());
                                //slopDrawList[slopDrawList.Count] = ((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5).ToString();

                            }

                            this.seriesChange();

                            //for (int i = 0; i < ty.Length; i++)
                            //{
                            //    SeriesPoint sp = new SeriesPoint();
                            //    sp.Argument = ((oldListCount + i) * 0.5).ToString();//接收数据不可能一次接收完，此处为了实现新增点继续在原有点上绘制。
                            //    double[] tyy = { ty[i] };
                            //    sp.Values = tyy;
                            //    //this.chartControl1.Series[0].Points.Add(sp);
                            //    this.SetChartControl1Point(sp);
                            //    temperatureList.Add(ty[i].ToString());

                            //    if (i > 0)
                            //    {
                            //        SeriesPoint sp2 = new SeriesPoint();
                            //        sp2.Argument = ((temperatureList.Count + i) * 0.5).ToString();
                            //        double[] tyy2 = { (ty[i] - ty[i - 1]) / 0.5 };
                            //        sp2.Values = tyy2;
                            //        this.SetChartControl2Point(sp2);
                            //    }
                            //}
                        }
                        else if (this.currentSeriesN == 3)//表示操作第3路
                        {
                            int oldListCount = temperatureList2.Count;

                            //将旧数据加入数组
                            //for (int i = 0; i < oldListCount; i++)
                            //{
                            //    SeriesPoint sp_old = new SeriesPoint();
                            //    sp_old.Argument = (i * 0.5).ToString();
                            //    double[] ys_old = { Math.Round(double.Parse(temperatureList[i]), 2) };

                            //    sp_old.Values = ys_old;
                            //    this.chartControl1.Series[0].Points.Add(sp_old);
                            //}


                            SeriesPoint sp = new SeriesPoint();
                            sp.Argument = (oldListCount * 1).ToString();//接收数据不可能一次接收完，此处为了实现新增点继续在原有点上绘制。
                            //Double[] tys = new Double[1];
                            double[] tys = { ty };
                            //tys[0] = ty;
                            sp.Values = tys;

                            //this.chartControl1.Series[0].Points.Add(sp); 

                            this.SetChartControl1Point(sp);
                            temperatureList2.Add(ty.ToString());
                            //temperatureList[temperatureList.Count] = ty.ToString();
                            //temperatureList.Insert(temperatureList.Count,ty.ToString()) ;

                            if (oldListCount > 0)
                            {
                                //将斜率旧数据添加
                                //int oldSlopCount = slopDrawList.Count;
                                //for (int i = 0; i < oldSlopCount; i++) {

                                //    SeriesPoint sp2_old = new SeriesPoint();
                                //    sp2_old.Argument = (i * 0.5).ToString();
                                //    double[] tyy2_old = { Math.Round(double.Parse(temperatureList[i]), 2) };

                                //    sp2_old.Values = tyy2_old;
                                //    this.chartControl2.Series[0].Points.Add(sp2_old); 
                                //}

                                SeriesPoint sp2 = new SeriesPoint();
                                //sp2.Argument = (temperatureList2.Count).ToString();
                                sp2.Argument = oldListCount.ToString();
                                //TODO：时间需要根据实际情况测定，单位秒S，
                                double paramArgument = 1;
                                double xianshi = (double.Parse(temperatureList2[temperatureList2.Count - 1]) - double.Parse(temperatureList2[temperatureList2.Count - 2])) / paramArgument; ;
                                //TODO：实际测定后，就删除就行了。
                                double fangda = xianshi * 1000;
                                double[] tyy2 = { fangda };
                                //double[] tyy2 = { (ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5 };
                                sp2.Values = tyy2;
                                this.SetChartControl2Point(sp2);
                                slopDrawList2.Add(((ty - double.Parse(temperatureList2[temperatureList2.Count - 1])) / 1).ToString());
                                //slopDrawList.Insert(slopDrawList.Count, ((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5).ToString());
                                //slopDrawList[slopDrawList.Count] = ((ty - double.Parse(temperatureList[temperatureList.Count - 1])) / 0.5).ToString();

                            }

                            this.seriesChange();

                        }
                        else
                        {

                        }
                    }
                    catch { }
                }
            }
        }

        



        /// <summary>
        /// 串口接收数据解码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string JieMa(byte[] data)
        {
            //if (GlobalValue.CodeType == "1")
            //{
            //    StringBuilder sb = new StringBuilder();
            //    for (int i = 0; i < data.Length; i++)
            //    {
            //        sb.AppendFormat("{0:x2}" + " ", data[i]);
            //    }
            //    return sb.ToString().ToUpper();
            //}
            //else if (GlobalValue.CodeType == "2")
            //{
            //    return new ASCIIEncoding().GetString(data);
            //}
            //else if (GlobalValue.CodeType == "3")
            //{
            //    return new UTF8Encoding().GetString(data);
            //}
            //else if (GlobalValue.CodeType == "4")
            //{
            //    return new UnicodeEncoding().GetString(data);
            //}
            //else
            //{
            //    StringBuilder sb = new StringBuilder();
            //    for (int i = 0; i < data.Length; i++)
            //    {
            //        sb.AppendFormat("{0:x2}" + " ", data[i]);
            //    }
            //    return sb.ToString().ToUpper();
            //}

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", data[i]);
            }
            return sb.ToString().ToUpper();
        }

        
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        private void SendData(string str)
        {
            try
            {
                //if (GlobalValue.CodeType == "1")//16进制
                //    ComDevice.Write(System.Text.Encoding.Default.GetBytes(str), 0, System.Text.Encoding.Default.GetBytes(str).Length);
                //else if (GlobalValue.CodeType == "2")//ASCIIs
                //    ComDevice.Write(System.Text.Encoding.ASCII.GetBytes(str), 0, System.Text.Encoding.ASCII.GetBytes(str).Length);
                //else if (GlobalValue.CodeType == "3")//UTF-8
                //    ComDevice.Write(System.Text.Encoding.UTF8.GetBytes(str), 0, System.Text.Encoding.UTF8.GetBytes(str).Length);
                //else if (GlobalValue.CodeType == "4")//Unicode
                //    ComDevice.Write(System.Text.Encoding.Unicode.GetBytes(str), 0, System.Text.Encoding.Unicode.GetBytes(str).Length);
                //else
                //    ComDevice.Write(System.Text.Encoding.Default.GetBytes(str), 0, System.Text.Encoding.Default.GetBytes(str).Length);
                ComDevice.Write(System.Text.Encoding.Default.GetBytes(str), 0, System.Text.Encoding.Default.GetBytes(str).Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //timer函数，定时发送指令
        private void timer1_Tick(object source, ElapsedEventArgs e)
        {
            //System.Console.WriteLine(ComDevice.IsOpen+"----isopen-----");
            if (isNeedComRecevied)
            {
                List<int> roadNumList = getRoadNumList();


                //List<int> roadList = getRoadNumList();
                if (this.currentSeriesN == 0)
                {
                    //this.nextSeriesN = roadList[0];
                    if (barCheckItem1.Checked)
                    {
                        this.nextSeriesN = 1;
                    }
                    else if (barCheckItem2.Checked)
                    {
                        this.nextSeriesN = 2;
                    }
                    else if (barCheckItem3.Checked)
                    {
                        this.nextSeriesN = 3;
                    }
                    else
                    {

                    }
                }

                int flag = 10;
                while (flag > 0)
                {
                    if (!ComDevice.IsOpen)
                    {
                        try
                        {
                            ComDevice.Open();
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        break;
                    }
                    flag--;
                }


                try
                {
                    if (ComDevice.IsOpen)
                    {
                        if (this.currentSeriesN != this.nextSeriesN && roadNumList.Count > 1)//如果即将工作的线路不等于当前工作的线路，表示已经接收完数据
                        {
                            byte[] buf = crc.FillCRC16("01 03 10 00 00 02");

                            if (this.nextSeriesN == 1)//表示即将发送第1路
                            {
                                buf = crc.FillCRC16("01 03 10 00 00 02");
                                this.currentSeriesN = this.nextSeriesN;//将当前路数变量currentSeriesN置为当前正在发送的路数nextSeriesN
                                //转换列表为数组后发送
                                ComDevice.Write(buf, 0, buf.Length);
                            }
                            else if (this.nextSeriesN == 2)//表示即将发送第2路
                            {
                                buf = crc.FillCRC16("01 03 11 00 00 02");
                                this.currentSeriesN = this.nextSeriesN;
                                //转换列表为数组后发送
                                ComDevice.Write(buf, 0, buf.Length);
                            }
                            else if (this.nextSeriesN == 3)//表示即将发送第2路
                            {
                                buf = crc.FillCRC16("01 03 12 00 00 02");
                                this.currentSeriesN = this.nextSeriesN;
                                //转换列表为数组后发送
                                ComDevice.Write(buf, 0, buf.Length);
                            }
                            else
                            {

                            }
                        }
                        else if (roadNumList.Count == 1)
                        {
                            byte[] buf = crc.FillCRC16("01 03 10 00 00 02");
                            if (this.nextSeriesN == 1)//表示即将发送第1路
                            {
                                buf = crc.FillCRC16("01 03 10 00 00 02");
                                this.currentSeriesN = this.nextSeriesN;//将当前路数变量currentSeriesN置为当前正在发送的路数nextSeriesN
                                //转换列表为数组后发送
                                ComDevice.Write(buf, 0, buf.Length);
                            }
                            else if (this.nextSeriesN == 2)//表示即将发送第2路
                            {
                                buf = crc.FillCRC16("01 03 11 00 00 02");
                                this.currentSeriesN = this.nextSeriesN;
                                //转换列表为数组后发送
                                ComDevice.Write(buf, 0, buf.Length);
                            }
                            else if (this.nextSeriesN == 3)//表示即将发送第2路
                            {
                                buf = crc.FillCRC16("01 03 12 00 00 02");
                                this.currentSeriesN = this.nextSeriesN;
                                //转换列表为数组后发送
                                ComDevice.Write(buf, 0, buf.Length);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("ComDevice.isOpen未打开！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Console.WriteLine("ComDevice.isOpen未打开！");
                    }
                }
                catch
                {
                    //MessageBox.Show("数据接收未开启1。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Console.WriteLine("数据接收未开启1。");
                }
            }
            //}
            else
            {
                //MessageBox.Show("数据接收未开启2。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Console.WriteLine("数据接收未开启2。");
            }
        }

        /*
        private void b_FeedingMachine_ItemClick(object sender, ItemClickEventArgs e)
        {
            //“喂料机”功能也需要发送串口数据，占用信道，故先关闭Main界面已经开启的串口
            try
            {
                if (ComDevice.IsOpen)
                {
                    //打开时点击，则关闭串口
                    ComDevice.Close();
                }
            }
            catch { }
            //Main界面的循环回访数据的接收标志位也需要关闭
            isNeedComRecevied = false;

            ///注销操作系统端口取数据监听事件
            ///此处必须注销现有托管到操作系统的监听事件，因为操作系统一个端口只支持一个事件的监听
            ///为了防止准备关闭的监听事件不存在而报错，故要加入try处理
            try
            {
                ComDevice.DataReceived -= new SerialDataReceivedEventHandler(Com_DataReceived);
            }
            catch { }

            //SensorChannelConfigForm fmf = new SensorChannelConfigForm(ComDevice);
            fmf.ShowDialog();
            if (fmf.DialogResult == DialogResult.OK)//此处通过弹出窗口的DialogResult的值来判断窗口关闭，需要在弹窗关闭事件中设定dialogresult的值
            {
                //窗口关闭了，重新注册主界面的端口数据获取操作系统托管监听事件
                try
                {
                    ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);
                }
                catch { }
            }
        }*/



        


        /// <summary>
        /// 根据文件路径，绘制相应路曲线，分析txt文件中的温度数据特征值
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="maxTemperature">温度最大值</param>
        /// <param name="minTemperature">温度最小值</param>
        /// <param name="isHuaTem">是否进行华氏温度转换摄氏温度</param>
        /// <param name="roadIndex">曲线路数</param>
        /// <param name="diagram">温度数据框图形对象</param>
        /// <param name="uploadAnalysisModel">参数表格对象</param>
        public void drawLine(string filePath, string maxTemperature, string minTemperature, bool isHuaTem, int roadIndex, XYDiagram diagram, entity.AnalysisModel uploadAnalysisModel)
        {
            string timeString = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            List<string> temperatureArrList = new List<string>();
            List<string> slopList = new List<string>();
            List<string> secDeriList = new List<string>();
            List<double> quxianList = new List<double>();
            List<string> formatTxt = new List<string>();

            temperatureArrList = Core.ChartList.GetTemperature(filePath, maxTemperature, minTemperature);
            slopList = Core.ChartList.GetSlop(temperatureArrList);

            //二阶导
            secDeriList = Core.ChartList.GetSlop(slopList);

            if (temperatureArrList.Count > 0 && slopList.Count > 0)
            {
                List<entity.XY> list = new List<entity.XY>();
                List<entity.XY> list2 = new List<entity.XY>();

                for (int i = 0; i < temperatureArrList.Count; i++)
                {
                    entity.XY xy = new entity.XY();
                    xy.x = i;
                    xy.y = Math.Round(double.Parse(temperatureArrList[i]), 2);
                    list.Add(xy);
                }

                for (int i = 0; i < slopList.Count; i++)
                {
                    entity.XY xy = new entity.XY();
                    xy.x = i;
                    xy.y = Math.Round(double.Parse(slopList[i]), 2);
                    list2.Add(xy);
                }

                //华氏温度转摄氏温度
                for (int i = 0; i < list.Count; i++)
                {
                    if (isHuaTem)
                    {
                        list[i].y = Math.Round((list[i].y - 32) * 5 / 9, 2);
                    }
                    //list[i].y = Math.Round((list[i].y - 32) * 5 / 9, 2);
                    quxianList.Add(list[i].y);
                }

                int min1 = (int)list[list.Count - 1].y - 1;
                int max1 = (int)list[0].y + 1;
                int min2 = (int)list2[0].y - 1;
                int max2 = (int)list2[list2.Count - 1].y + 1;

                AxisRange DIA = (AxisRange)((XYDiagram)chartControl1.Diagram).AxisY.Range;
                DIA.SetMinMaxValues(min1, max1);

                for (int i = 0; i < list.Count; i++)
                {
                    SeriesPoint sp = new SeriesPoint();
                    sp.Argument = list[i].x.ToString();
                    double[] ys = { list[i].y };
                    sp.Values = ys;
                    this.chartControl1.Series[roadIndex].Points.Add(sp);
                }

                for (int i = 0; i < list2.Count; i++)
                {
                    SeriesPoint sp = new SeriesPoint();
                    sp.Argument = list2[i].x.ToString();
                    double[] ys = { list2[i].y };
                    sp.Values = ys;
                    this.chartControl2.Series[roadIndex].Points.Add(sp);
                }

                //this.chartControl1.DataSource = list;
                //AxisRange DIA = (AxisRange)((XYDiagram)chartControl1.Diagram).AxisY.Range;
                //DIA.SetMinMaxValues(min1, max1);

                //this.chartControl2.DataSource = list2;
                //AxisRange DIA2 = (AxisRange)((XYDiagram)chartControl2.Diagram).AxisY.Range;
                //DIA2.SetMinMaxValues(min2, max2);

                List<entity.ChartData> temperatureGridList = new List<entity.ChartData>();
                for (int i = 0; i < list.Count; i++)
                {
                    entity.ChartData cd = new entity.ChartData();
                    cd.Number = i;
                    cd.DataValue = list[i].y;
                    temperatureGridList.Add(cd);
                }
                //this.gridControl2.DataSource = temperatureGridList;

                List<entity.ChartData> slopGridList = new List<entity.ChartData>();
                for (int i = 0; i < list2.Count; i++)
                {
                    entity.ChartData cd = new entity.ChartData();
                    cd.Number = i;
                    cd.DataValue = list2[i].y;
                    slopGridList.Add(cd);
                }
                //this.gridControl3.DataSource = slopGridList;
            }

            try
            {
                int maxNum = Core.FindMax.FindMaxData(temperatureArrList);
                int minNum = Core.FindMin.FindMinData(temperatureArrList, maxNum);
                double a = double.Parse(temperatureArrList[maxNum]);
                double b = double.Parse(temperatureArrList[minNum]);
                int num = 0;
                List<string> listTal = new List<string>();
                List<double> listData = Core.DataDeal.BeforeMaxData(Core.ReadTxt.ReadTxtFile(filePath));
                double resultTal = 0;
                for (int i = 10; i < minNum - 1; i++)
                {
                    double temp = listData[i + 1] - listData[i];
                    if (temp == 0.0)
                    {
                        resultTal = listData[i];
                        num = i;
                        break;
                    }
                }
                num_tal = num;
                //TAL
                result_tal = quxianList[num_tal];
                a = (a - 32) * 5 / 9;
                //TEU
                b = (b - 32) * 5 / 9;
                double f = a - b;
                double DeltaT1 = result_tal - b;
                double DeltaT2 = f;
                double v1 = DeltaT1 / (minNum - num_tal);
                double v2 = DeltaT2 / (maxNum - minNum);
                double result1 = 84.42768 - 17.44406 * v1 + 27.01059 * v2;
                double result2 = -155.02346 + 0.20778 * result_tal - 31.7993 * v1 + 60.51782 * v2 - 0.04042 * DeltaT1 - 0.47029 * DeltaT2;
                double result3 = -106.46181 + 0.16536 * result_tal - 31.98602 * v1 + 60.37525 * v2 - 0.48856 * DeltaT2;
                double result = (result1 + result2 + result3) / 3;

                //--------------------------0619 add-----------------------------
                //Tsef
                for (int i = 0; i < secDeriList.Count; i++)
                {
                    if (i > num_tal && i < minNum)
                    {
                        if (System.Math.Abs(double.Parse(secDeriList[i])) < System.Math.Abs(double.Parse(secDeriList[i + 1])))
                        {
                            index_tsef = i;
                            break;
                        }
                    }
                }
                //Tem
                for (int i = 0; i < secDeriList.Count; i++)
                {
                    if (i > minNum && i < maxNum)
                    {
                        if (System.Math.Abs(double.Parse(secDeriList[i])) < System.Math.Abs(double.Parse(secDeriList[i + 1])))
                        {
                            index_tem = i;
                            break;
                        }
                    }
                }

                //向温度贝塞尔曲线中加入X、Y轴TAL垂直线
                /*XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
                diagram.AxisX.ConstantLines.Clear();
                diagram.AxisY.ConstantLines.Clear();*/
                //TAL
                ConstantLine TALLine1 = new ConstantLine("TAL_" + (roadIndex + 1), num_tal);
                TALLine1.LineStyle.DashStyle = DashStyle.Dash;
                TALLine1.ShowInLegend = false;//是否显示到图例中
                TALLine1.LegendText = "TAL:初晶温度";
                TALLine1.Color = Color.Red; //直线颜色
                TALLine1.Title.TextColor = Color.Red;   //直线文本字体颜色    
                TALLine1.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisX.ConstantLines.Add(TALLine1);

                ConstantLine TALLine2 = new ConstantLine("", result_tal);
                TALLine2.LineStyle.DashStyle = DashStyle.Dash;
                TALLine2.ShowInLegend = false;//是否显示到图例中
                TALLine2.Color = Color.Red; //直线颜色
                TALLine2.Title.TextColor = Color.Red;   //直线文本字体颜色    
                TALLine2.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisY.ConstantLines.Add(TALLine2);

                //TEU
                ConstantLine TEULine1 = new ConstantLine("TEU_" + (roadIndex + 1), minNum);
                TEULine1.LineStyle.DashStyle = DashStyle.Dash;
                TEULine1.ShowInLegend = false;//是否显示到图例中
                TEULine1.LegendText = "TEU:共晶最低温度";
                TEULine1.Color = Color.Green; //直线颜色
                TEULine1.Title.TextColor = Color.Green;   //直线文本字体颜色    
                TEULine1.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisX.ConstantLines.Add(TEULine1);

                ConstantLine TEULine2 = new ConstantLine("", quxianList[minNum]);
                TEULine2.LineStyle.DashStyle = DashStyle.Dash;
                TEULine2.ShowInLegend = false;//是否显示到图例中
                TEULine2.LegendText = "TEU:共晶最低温度";
                TEULine2.Color = Color.Green; //直线颜色
                TEULine2.Title.TextColor = Color.Green;   //直线文本字体颜色    
                TEULine2.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisY.ConstantLines.Add(TEULine2);

                //TER
                ConstantLine TERLine1 = new ConstantLine("TER_" + (roadIndex + 1), maxNum);
                TERLine1.LineStyle.DashStyle = DashStyle.Dash;
                TERLine1.ShowInLegend = false;//是否显示到图例中
                TERLine1.LegendText = "TER:共晶最高温度";
                TERLine1.Color = Color.Blue; //直线颜色
                TERLine1.Title.TextColor = Color.Blue;   //直线文本字体颜色    
                TERLine1.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisX.ConstantLines.Add(TERLine1);

                ConstantLine TERLine2 = new ConstantLine("", quxianList[maxNum]);
                TERLine2.LineStyle.DashStyle = DashStyle.Dash;
                TERLine2.ShowInLegend = false;//是否显示到图例中
                TERLine2.LegendText = "TER:共晶最高温度";
                TERLine2.Color = Color.Blue; //直线颜色
                TERLine2.Title.TextColor = Color.Blue;   //直线文本字体颜色    
                TERLine2.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisY.ConstantLines.Add(TERLine2);

                //------------------------------0619 add--------------------------------------------------------

                //Tsef
                ConstantLine TsefLine1 = new ConstantLine("TSEF_" + (roadIndex + 1), index_tsef);
                TsefLine1.LineStyle.DashStyle = DashStyle.Dash;
                TsefLine1.ShowInLegend = false;//是否显示到图例中
                TsefLine1.LegendText = "TSEF:共晶最高温度";                                       //TODO
                TsefLine1.Color = Color.DarkOrange; //直线颜色
                TsefLine1.Title.TextColor = Color.DarkOrange;   //直线文本字体颜色    
                TsefLine1.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisX.ConstantLines.Add(TsefLine1);

                ConstantLine TsefLine2 = new ConstantLine("", quxianList[index_tsef]);
                TsefLine2.LineStyle.DashStyle = DashStyle.Dash;
                TsefLine2.ShowInLegend = false;//是否显示到图例中
                TsefLine2.LegendText = "TSEF:共晶最高温度";                                        //TODO
                TsefLine2.Color = Color.DarkOrange; //直线颜色
                TsefLine2.Title.TextColor = Color.DarkOrange;   //直线文本字体颜色    
                TsefLine2.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisY.ConstantLines.Add(TsefLine2);

                //Tem
                ConstantLine TemLine1 = new ConstantLine("TEM_" + (roadIndex + 1), index_tem);
                TemLine1.LineStyle.DashStyle = DashStyle.Dash;
                TemLine1.ShowInLegend = false;//是否显示到图例中
                TemLine1.LegendText = "TEM:共晶最高温度";                                           //TODO
                TemLine1.Color = Color.MediumPurple; //直线颜色
                TemLine1.Title.TextColor = Color.MediumPurple;   //直线文本字体颜色    
                TemLine1.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisX.ConstantLines.Add(TemLine1);

                ConstantLine TemLine2 = new ConstantLine("", quxianList[index_tem]);
                TemLine2.LineStyle.DashStyle = DashStyle.Dash;
                TemLine2.ShowInLegend = false;//是否显示到图例中
                TemLine2.LegendText = "TEM:共晶最高温度";                                        //TODO
                TemLine2.Color = Color.MediumPurple; //直线颜色
                TemLine2.Title.TextColor = Color.MediumPurple;   //直线文本字体颜色    
                TemLine2.Title.Alignment = ConstantLineTitleAlignment.Far;//字体对其方式
                diagram.AxisY.ConstantLines.Add(TemLine2);


                //------------------------------0619 add---------------end-----------------------------------------

                entity.AnalysisModel am = new entity.AnalysisModel();
                am.TAL = quxianList[num_tal].ToString();
                am.TSEF = quxianList[index_tsef].ToString();
                am.TEU = quxianList[minNum].ToString();
                am.TEM = quxianList[index_tem].ToString();
                am.TER = quxianList[maxNum].ToString();
                am.Ter_Teu = Math.Round((quxianList[maxNum] - quxianList[minNum]), 2).ToString();
                am.Tal_Teu = Math.Round((quxianList[num_tal] - quxianList[minNum]), 2).ToString();
                am.RuHuaLv = Math.Round(result, 2).ToString();
                if (b > 1142.7)
                {
                    am.IsHuiZhuTie = "是";
                }
                else
                {
                    if (result > 95)
                    {
                        am.IsHuiZhuTie = "是";
                    }
                    else
                    {
                        am.IsHuiZhuTie = "否";
                    }
                }
                //this.gridView1.AddNewRow();
                //int focuedRow = this.gridView1.FocusedRowHandle;
                java.io.File file = new java.io.File(filePath);
                if (file.exists())
                {
                    /*this.gridView1.SetRowCellValue(focuedRow, "Time", timeString);
                    uploadAnalysisModel.Time = timeString;
                    this.gridView1.SetRowCellValue(focuedRow, "FileName", file.getName());
                    uploadAnalysisModel.FileName = file.getName();
                    this.gridView1.SetRowCellValue(focuedRow, "TAL", am.TAL);
                    uploadAnalysisModel.TAL = am.TAL;
                    this.gridView1.SetRowCellValue(focuedRow, "TSEF", am.TSEF);
                    uploadAnalysisModel.TSEF = am.TSEF;
                    this.gridView1.SetRowCellValue(focuedRow, "TEU", am.TEU);
                    uploadAnalysisModel.TEU = am.TEU;
                    this.gridView1.SetRowCellValue(focuedRow, "TEM", am.TEM);
                    uploadAnalysisModel.TEM = am.TEM;
                    this.gridView1.SetRowCellValue(focuedRow, "TER", am.TER);
                    uploadAnalysisModel.TER = am.TER;
                    this.gridView1.SetRowCellValue(focuedRow, "Ter_Teu", am.Ter_Teu);
                    uploadAnalysisModel.Ter_Teu = am.Ter_Teu;
                    this.gridView1.SetRowCellValue(focuedRow, "Tal_Teu", am.Tal_Teu);
                    uploadAnalysisModel.Tal_Teu = am.Tal_Teu;
                    this.gridView1.SetRowCellValue(focuedRow, "RuHuaLv", am.RuHuaLv);
                    uploadAnalysisModel.RuHuaLv = am.RuHuaLv;
                    this.gridView1.SetRowCellValue(focuedRow, "IsHuiZhuTie", am.IsHuiZhuTie);
                    uploadAnalysisModel.IsHuiZhuTie = am.IsHuiZhuTie;
                     * */
                }
                else
                {
                    /*
                    this.gridView1.SetRowCellValue(focuedRow, "Time", timeString);
                    uploadAnalysisModel.Time = timeString;
                    this.gridView1.SetRowCellValue(focuedRow, "FileName", "-");
                    uploadAnalysisModel.FileName = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "TAL", "-");
                    uploadAnalysisModel.TAL = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "TSEF", "-");
                    uploadAnalysisModel.TSEF = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "TEU", "-");
                    uploadAnalysisModel.TEU = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "TEM", "-");
                    uploadAnalysisModel.TEM = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "TER", "-");
                    uploadAnalysisModel.TER = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "Ter_Teu", "-");
                    uploadAnalysisModel.Ter_Teu = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "Tal_Teu", "-");
                    uploadAnalysisModel.Tal_Teu = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "RuHuaLv", "-");
                    uploadAnalysisModel.RuHuaLv = "-";
                    this.gridView1.SetRowCellValue(focuedRow, "IsHuiZhuTie", "-");
                    uploadAnalysisModel.IsHuiZhuTie = "-";
                     * */
                }
            }
            catch
            {
                /*
                this.gridView1.AddNewRow();
                java.io.File file = new java.io.File(filePath);
                int focuedRow = this.gridView1.FocusedRowHandle;
                this.gridView1.SetRowCellValue(focuedRow, "Time", timeString);
                uploadAnalysisModel.Time = timeString;
                if (file.exists())
                {
                    this.gridView1.SetRowCellValue(focuedRow, "FileName", file.getName());
                    uploadAnalysisModel.FileName = file.getName();
                }
                else
                {
                    this.gridView1.SetRowCellValue(focuedRow, "FileName", "-");
                    uploadAnalysisModel.FileName = "-";
                }
                this.gridView1.SetRowCellValue(focuedRow, "TAL", "-");
                uploadAnalysisModel.TAL = "-";
                this.gridView1.SetRowCellValue(focuedRow, "TSEF", "-");
                uploadAnalysisModel.TSEF = "-";
                this.gridView1.SetRowCellValue(focuedRow, "TEU", "-");
                uploadAnalysisModel.TEU = "-";
                this.gridView1.SetRowCellValue(focuedRow, "TEM", "-");
                uploadAnalysisModel.TEM = "-";
                this.gridView1.SetRowCellValue(focuedRow, "TER", "-");
                uploadAnalysisModel.TER = "-";
                this.gridView1.SetRowCellValue(focuedRow, "Ter_Teu", "-");
                uploadAnalysisModel.Ter_Teu = "-";
                this.gridView1.SetRowCellValue(focuedRow, "Tal_Teu", "-");
                uploadAnalysisModel.Tal_Teu = "-";
                this.gridView1.SetRowCellValue(focuedRow, "RuHuaLv", "-");
                uploadAnalysisModel.RuHuaLv = "-";
                this.gridView1.SetRowCellValue(focuedRow, "IsHuiZhuTie", "-");
                uploadAnalysisModel.IsHuiZhuTie = "-";
                 * */
            }
        }



        //==========================================================================================================================
        //*******************************************这是一条明显的分割线***********************************************************
        //==========================================================================================================================


        //**************************************************UDP通信基本功能部分******************************************************
        
        /// <summary>
        /// 开始/停止监听
        /// 开启UDP接收，按钮caption切换
        /// 注：该按钮【已弃用】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            //实现按钮caption切换
            if (barButtonItem12.Caption == "开始监听")
            {

                barButtonItem12.Caption = "停止监听";
                udpClient = new UdpClient(localIpep);
                thrRecv = new Thread(ReceiveMessage);
                thrRecv.Start();
                showMessage(richTextBox1, "上位机：UDP监听已开启");
            }
            else
            {
                barButtonItem12.Caption = "开始监听";
                thrRecv.Abort();    //所谓的关闭线程
                udpClient.Close();
                showMessage(richTextBox1, "上位机：UDP监听已关闭");

            }

        }

        /// <summary>
        /// 网口通信窗口
        /// 注：该按钮【已弃用】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            NetCtrlForm spc = new NetCtrlForm(ComDevice);
            try//此处用try做异常处理，是为了防止COM不存在释放Dialog后，ShowDialog无法找到窗体资源而报错。
            {
                spc.ShowDialog();
            }
            catch { } 
        }

        /// <summary>
        /// UDP通信 发送数据
        /// </summary>
        /// <param name="obj"></param>
        public static void SendMessage(object obj)
        {
            try
            {
                Monitor.Enter(lockHelper);  //锁定对象

                string message = (string)obj;
                //byte[] sendbytes = Encoding.Unicode.GetBytes(message);

                //将16进制数的字符串形式转换为byte数组
                String[] cmdStrs = message.Split(new string[]{"0x"},StringSplitOptions.RemoveEmptyEntries);
                byte[] cmdBytes = new byte[cmdStrs.Length];
                for (int i = 0; i < cmdStrs.Length; i++) {
                    cmdBytes[i] = Byte.Parse(cmdStrs[i],System.Globalization.NumberStyles.HexNumber); 
                }


                //System.Text.ASCIIEncoding ascEncoding = new ASCIIEncoding();
                //byte[] sendbytes = ascEncoding.GetBytes(message);
                IPEndPoint remoteIpep2 = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 10105); // 发送到的IP地址和端口号
                udpClient.Send(cmdBytes, cmdBytes.Length, remoteIpep2);
                udpClient.Close();

                Monitor.Pulse(lockHelper);  //通知其他线程，我忙完了，等我Monitor.Exit(obj)了，你们就继续吧
                Monitor.Exit(lockHelper);  //释放锁
            }
            catch(Exception exception) {
                XtraMessageBox.Show(exception.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        

        /// <summary>
        /// UDP通信 接收数据
        /// </summary>
        /// <param name="obj"></param>
        public static  void ReceiveMessage(Object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 10105);   //（下位机）应用程序与特定主机特定端口之间的连接

            
            while (true)
            {
                Monitor.Enter(lockHelper);  //锁定lockHelper
                
                try
                {
                    byte[] byteRecv = udpClient.Receive(ref remoteIpep);  //ref高级参数目的：使引用地址一致  
                    StringBuilder message = new StringBuilder(0);   //存储16进制字节数拼接成的字符串 
                    List<StringBuilder> hexStrs = new List<StringBuilder>();
                    for (int i = 0; i < byteRecv.Length;i++ )
                    {
                        StringBuilder hexStr = new StringBuilder(byteRecv[i].ToString("X2"));
                        hexStrs.Add(hexStr);
                        message.Append("0x" + hexStr + " ");
                    }
                     

                    if (hexStrs.Count >= 8)   //数据包至少包含Header（2byte）、DEVICE_ID、Reserve、Category、Len（2byte）、Verify，共计8字节
                    {
                        //分离Len，并判断数据包长度信息是否正确 
                        //获取数据段长度(根据协议规定，data_len存在数据包的第6 、7个字节，下标为5、6)
                        byte b1 = byteRecv[5];
                        byte b2 = byteRecv[6]; 
                        int dataLen = (b2 << 8) ^ b1;
                        if ( (hexStrs[4]+"" != "83") &&(dataLen != hexStrs.Count-8) )                 //没有数据或数据段长度无效
                        {
                            showMessage(richTextBox1, string.Format("{0}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "：无效数据包"));
                        }
                        else {
                            //读取并设置设备号device_id
                            byte device_num = byteRecv[2];
                            EquipmentId = device_num;    

                            StringBuilder cmdStr = new StringBuilder();
                            for (int i = 7; i <= 7+dataLen-1; i++)
                            {
                                cmdStr.Append("0x" + hexStrs[i] + " ");
                            }
                            switch (hexStrs[4]+"")
                            {
                                //故障配置应答
                                case "81":
                                    showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_故障配置应答：", cmdStr));
                                    break;
                                //传感器通道配置应答
                                case "82":
                                    showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_传感器通道配置应答：", cmdStr));
                                    break;
                                //数据应答
                                case "83":
                                    showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_数据应答：", cmdStr));
                                    //TODO   曲线显示
                                    break;
                                //传感器量程配置应答
                                case "84":
                                    showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_传感器量程配置应答：", cmdStr));
                                    break;
                                //交互应答
                                case "85":
                                    if (dataLen == 1)    //有效“交互应答”命令，数据段占1个字节，status
                                    {
                                        StringBuilder status = hexStrs[7];   //获取status值
                                        switch (status + "")
                                        {
                                            //故障已配置信息   TODO:接收随后的故障信息应答包
                                            case "01":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "故障已配置信息"));
                                                //TODO存储状态变量
                                                break;
                                            //传感器通道已配置信息   TODO:接收随后的传感器通道信息应答包
                                            case "02":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "传感器通道已配置信息"));
                                                //TODO存储状态变量
                                                break;
                                            //已暂停采集
                                            case "03":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "已暂停采集"));
                                                break;
                                            //已开始采集
                                            case "04":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "已开始采集"));
                                                break;
                                            //擦除结束可重启采集
                                            case "05":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "擦除结束可重启采集"));
                                                break;
                                            //已设置设备ID      TODO:接收随后的设备ID应答包
                                            case "06":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "已设置设备ID"));
                                                break;
                                            //获取量程配置信息      TODO:接收随后的设备ID应答包
                                            case "07":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "获取量程配置信息"));
                                                break;
                                            //传感器通道未配置信息
                                            case "81":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "传感器通道未配置信息"));
                                                //TODO存储状态变量
                                                break;
                                            //故障未配置信息
                                            case "82":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "故障未配置信息"));
                                                //TODO存储状态变量
                                                break;
                                            //未设置设备ID
                                            case "86":
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "未设置设备ID"));
                                                break; 
                                            //default
                                            default:
                                                showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", "无效命令"));
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        cmdStr = new StringBuilder("无效命令");
                                        showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_交互应答：", cmdStr));
                                    }

                                    break;
                                //设备ID设置
                                case "86":
                                    if (dataLen == 2)//data =2字节 Device_ID+reserve
                                    {
                                        StringBuilder device_id = hexStrs[7];   //获取device_id值
                                        byte device_byte = byte.Parse(device_id + "", System.Globalization.NumberStyles.HexNumber);
                                        MainForm.EquipmentId = device_byte;
                                        showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_设备ID设置：", device_byte));
                                    }
                                    else {
                                        cmdStr = new StringBuilder("无效命令");
                                        showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_设备ID设置：", cmdStr));
                                    }
                                    break; 
                                default:
                                    showMessage(richTextBox1, string.Format("{0}{1}", "下位机(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "_无法识别命令：", cmdStr));
                                    break;
                            }
                        } 
                    }
                    else {
                        showMessage(richTextBox1, string.Format("{0}{1}", "未知命令(" + remoteIpep + ")_" + System.DateTime.Now.ToString() + "：", message));
                    }
                    //Monitor.Wait(lockHelper);  //将该线程暂停，并释放锁允许其他线程访问

                    //等到了Monitor.Pulse(obj)和Monitor.Exit(obj)的信号，就继续往下执行
                    Monitor.Exit(lockHelper);

                    
                }
                catch{
                    //Object locker = new object();
                    //lock (locker) {
                        //if (udpClient != null)
                        //{
                            udpClient.Close();
                            udpClient = null;
                            showMessage(richTextBox1, string.Format("系统消息_"+System.DateTime.Now.ToString()+"：下位机5s无回应"));
                            //thrRecv.Abort();    //所谓的关闭线程
                            udpClient = new UdpClient(localIpep);
                            udpClient.Client.ReceiveTimeout = 4000;

                            Thread.Sleep(1000);

                            //Monitor.Wait(lockHelper);  //将该线程暂停，并释放锁允许其他线程访问

                            //等到了Monitor.Pulse(obj)和Monitor.Exit(obj)的信号，就继续往下执行
                            Monitor.Exit(lockHelper);   
                            continue; 
                    //}
                    
                } 
            }
        }

        /// <summary>
        /// 通信窗口的监听消息显示
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="message"></param>
        delegate void showMessageDelegate(RichTextBox textBox, string message);
        public static void showMessage(RichTextBox textBox, string message) {
            if (textBox.InvokeRequired)
            {
                showMessageDelegate showMessageDelegate = showMessage;
                textBox.Invoke(showMessageDelegate,new object[] {textBox,message});
            }
            else {
                //让文本框获取焦点  
                textBox.Focus();
                //设置光标的位置到文本尾  
                textBox.Select(textBox.TextLength, 0);
                //滚动到控件光标处  
                textBox.ScrollToCaret();
                //添加文本内容
                textBox.AppendText(message + "\r\n");
                //textBox.Text += message + "\r\n";
            }
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

        //********************************************Button点击：初始化窗口*******************************************

        /// <summary>
        /// 传感器通道设置 button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            SensorChannelConfigForm spc = new SensorChannelConfigForm(this);
            try//此处用try做异常处理，是为了防止COM不存在释放Dialog后，ShowDialog无法找到窗体资源而报错。
            {
                spc.ShowDialog();
            }
            catch { }
        }

        /// <summary>
        /// 故障信息配置  button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            FaultInfoConfigForm spc = new FaultInfoConfigForm(ComDevice);
            try//此处用try做异常处理，是为了防止COM不存在释放Dialog后，ShowDialog无法找到窗体资源而报错。
            {
                spc.ShowDialog();
            }
            catch { }
        }

        /// <summary>
        /// 传感器状态配置   button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            SensorStateConfigForm spc = new SensorStateConfigForm(ComDevice);
            try//此处用try做异常处理，是为了防止COM不存在释放Dialog后，ShowDialog无法找到窗体资源而报错。
            {
                spc.ShowDialog();
            }
            catch { }
        }


        //*************************************************主界面的命令发送事件函数*******************************************************

        /// <summary>
        /// 开始采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (!isCollecting)
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
                    cmd[7] = byte.Parse("04", System.Globalization.NumberStyles.HexNumber);


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
                    thrSend = new Thread(MainForm.SendMessage);

                    //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                    thrSend.Start(sendCmdStr);

                    //6、在主界面显示发送内容 
                    showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[开始采集]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                    XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    isCollecting = true;
                }
                else {
                    XtraMessageBox.Show("下位机已经开始采集数据！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }  
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }

        /// <summary>
        /// 暂停采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if(isCollecting){
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
                    cmd[5] = 0;
                    cmd[6] = 1;


                    //data  
                    cmd[7] = byte.Parse("03", System.Globalization.NumberStyles.HexNumber);


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
                    thrSend = new Thread(MainForm.SendMessage);

                    //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                    thrSend.Start(sendCmdStr);

                    //6、在主界面显示发送内容 
                    showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[暂停采集]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                    XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    isCollecting = false;

                }else{
                    XtraMessageBox.Show("下位机已经暂停采集数据！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                } 
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        /// <summary>
        /// 开始/暂停回传数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            bool isOpenTrans = true;
            //bool isTransAllData = true;    //是否回传全部数据


            //实现按钮caption切换
            if (barButtonItem5.Caption == "开始实时回传")
            {
                barButtonItem5.Caption = "暂停实时回传";
                //isOpenTrans = true;
                /*if (XtraMessageBox.Show("是否实时回传全部数据？", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    isTransAllData = true;
                }
                else
                {
                    XtraMessageBox.Show("该功能正在开发，请您耐心等候！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                    //TODO部分数据实时回传
                }*/
            }
            else
            {
                barButtonItem5.Caption = "开始实时回传";
                isOpenTrans = false;
            }

            //开始发送
            try
            {
                //生成配置信息 byte数组 对应的 16进制字符串数组
                byte[] cmd = new byte[12];

                //Header
                cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
                cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
                //Device_id
                cmd[2] = MainForm.EquipmentId;
                //Reserve
                cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //--Category
                cmd[4] = byte.Parse("03", System.Globalization.NumberStyles.HexNumber);

                //Len(2 byte)
                cmd[5] = 4;
                cmd[6] = 0;


                //data 
                //开始/停止实时回传
                if (isOpenTrans)
                {
                    cmd[7] = byte.Parse("00", System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    cmd[7] = byte.Parse("01", System.Globalization.NumberStyles.HexNumber);
                }
                //是否回传全部数据
                //if (isTransAllData)
                //{
                    cmd[8] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                    cmd[9] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                    cmd[10] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //}
                //else
                //{
                    //TODO回传相应组号数据
                //}


                //Verify
                byte verifyByte = 0;
                for (int i = 0; i < cmd.Length; i++)
                {
                    verifyByte ^= cmd[i];
                }
                cmd[11] = verifyByte;

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
                thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                if (isOpenTrans)
                {
                    showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[开始实时回传数据]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));

                }
                else
                {
                    showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[暂停实时回传数据]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));

                }

                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        /// <summary>
        /// 获取传感器通道配置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
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
                cmd[7] = byte.Parse("02", System.Globalization.NumberStyles.HexNumber);


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
                thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[获取传感器通道配置信息]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        /// <summary>
        /// 获取故障配置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
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
                thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[获取故障配置信息]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            EquipmentIdConfigForm spc = new EquipmentIdConfigForm(ComDevice);
            try//此处用try做异常处理，是为了防止COM不存在释放Dialog后，ShowDialog无法找到窗体资源而报错。
            {
                spc.ShowDialog();
            }
            catch { }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
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
                cmd[7] = byte.Parse("05", System.Globalization.NumberStyles.HexNumber);


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
                thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[清除数据]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 传感器量程配置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            SensorSpanConfigForm spc = new SensorSpanConfigForm(this);
            try//此处用try做异常处理，是为了防止COM不存在释放Dialog后，ShowDialog无法找到窗体资源而报错。
            {
                spc.ShowDialog();
            }
            catch { }
        }


        /// <summary>
        /// 获取设备ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
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
                thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[获取设备ID]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));


                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 开始/暂停离线回传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {
            bool isOpenTrans = true;
            //bool isTransAllData = true;    //是否回传全部数据


            //实现按钮caption切换
            if (barButtonItem19.Caption == "开始离线回传")
            {
                barButtonItem19.Caption = "暂停离线回传";
                isOpenTrans = true;
                /*if (XtraMessageBox.Show("是否离线回传全部数据？", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    isTransAllData = true;
                }
                else
                {
                    XtraMessageBox.Show("该功能正在开发，请您耐心等候！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                    //TODO部分数据实时回传
                }*/
            }
            else
            {
                barButtonItem19.Caption = "开始离线回传";
                isOpenTrans = false;
            }

            //开始发送
            try
            {
                //生成配置信息 byte数组 对应的 16进制字符串数组
                byte[] cmd = new byte[12];

                //Header
                cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
                cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
                //Device_id
                cmd[2] = MainForm.EquipmentId;
                //Reserve
                cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //--Category
                cmd[4] = byte.Parse("03", System.Globalization.NumberStyles.HexNumber);

                //Len(2 byte)
                cmd[5] = 4;
                cmd[6] = 0;


                //data 
                //开始/停止实时回传
                if (isOpenTrans)
                {
                    cmd[7] = byte.Parse("02", System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    cmd[7] = byte.Parse("03", System.Globalization.NumberStyles.HexNumber);
                }
                //是否回传全部数据
                //if (isTransAllData)
                //{
                    cmd[8] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                    cmd[9] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                    cmd[10] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
                //}
                //else

                //{
                    //TODO回传相应组号数据
                //}


                //Verify
                byte verifyByte = 0;
                for (int i = 0; i < cmd.Length; i++)
                {
                    verifyByte ^= cmd[i];
                }
                cmd[11] = verifyByte;

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
                thrSend = new Thread(MainForm.SendMessage);

                //5、开启thrSend（thrSend执行结束后自动关闭udpcSend，销毁thrSend） 
                thrSend.Start(sendCmdStr);

                //6、在主界面显示发送内容 
                if (isOpenTrans)
                {
                    showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[开始离线回传数据]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));

                }
                else
                {
                    showMessage(richTextBox1, string.Format("{0}{1}", "上位机(" + localIpep + ")[暂停离线回传数据]_" + System.DateTime.Now.ToString() + "：", sendCmdStr));

                }

                XtraMessageBox.Show("指令下发成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //**************************************************绘制曲线功能部分******************************************************



    }
}