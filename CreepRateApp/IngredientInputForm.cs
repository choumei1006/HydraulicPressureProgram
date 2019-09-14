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
    public partial class IngredientInputForm : DevExpress.XtraEditors.XtraForm
    {

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

        public IngredientInputForm(SerialPort paramPortDev)
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

        //2. 为窗体添加Load事件，并在其方法Form1_Load中，调用类的初始化方法，记录窗体和其控件的初始位置和大小
        private void IngredientInputForm_Load(object sender, EventArgs e)
        {
            asc.controllInitializeSize(this);
        }

        //3.为窗体添加SizeChanged事件，并在其方法Form1_SizeChanged中，调用类的自适应方法，完成自适应
        private void IngredientInputForm_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }


        /// <summary>
        /// 添加配料_完成(按钮)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try {
                //铁水重量IronWaterWeight
                if (!string.IsNullOrWhiteSpace(textEdit9.Text))
                {
                    settings.IronWaterWeight = textEdit9.Text;
                    GlobalValue.IronWaterWeight = textEdit9.Text;
                }
                //硫含量SulfurPercent
                if (!string.IsNullOrWhiteSpace(textEdit1.Text))
                {
                    settings.SulfurPercent = textEdit1.Text;
                    GlobalValue.SulfurPercent = textEdit1.Text;
                }
                //铁水温度IronWaterTemperature
                if (!string.IsNullOrWhiteSpace(textEdit10.Text))
                {
                    settings.IronWaterTemperature = textEdit10.Text;
                    GlobalValue.IronWaterTemperature = textEdit10.Text;
                }
                //稀土TombarthiteWeight
                if (!string.IsNullOrWhiteSpace(textEdit11.Text))
                {
                    settings.TombarthiteWeight = textEdit11.Text;
                    GlobalValue.TombarthiteWeight = textEdit11.Text;
                }
                //蠕化剂RuhuajiWeight
                if (!string.IsNullOrWhiteSpace(textEdit14.Text))
                {
                    settings.RuhuajiWeight = textEdit14.Text;
                    GlobalValue.RuhuajiWeight = textEdit14.Text;
                }
                //一次孕育剂FirstPregnant
                if (!string.IsNullOrWhiteSpace(textEdit27.Text))
                {
                    settings.FirstPregnant = textEdit27.Text;
                    GlobalValue.FirstPregnant = textEdit27.Text;
                }
                //二次孕育剂SecendPregnant
                if (!string.IsNullOrWhiteSpace(textEdit28.Text))
                {
                    settings.SecendPregnant = textEdit28.Text;
                    GlobalValue.SecendPregnant = textEdit28.Text;
                }

                settings.Save();
                XtraMessageBox.Show("添加配料完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();

            }catch{
                XtraMessageBox.Show("添加配料失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            

            
        }


        /// <summary>
        /// 铁水重量_变化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit9_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textEdit9.Text))
                {
                    if (CreepRateApp.Core.CheckData.IsNumeric(textEdit9.Text))
                    {
                        textEdit8.Text = textEdit9.Text;
                        textEdit12.Text = textEdit9.Text;
                        textEdit22.Text = textEdit9.Text;
                        textEdit26.Text = textEdit9.Text;
                        textEdit24.Text = textEdit9.Text;
                    }
                    else {
                        XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }
            }
            catch {
                XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            

        }

        /// <summary>
        /// 硫含量_变化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textEdit1.Text) )
                {
                    if (CreepRateApp.Core.CheckData.IsNumeric(textEdit1.Text))
                    {
                        textEdit16.Text = textEdit1.Text;
                        textEdit18.Text = textEdit1.Text;
                        textEdit21.Text = textEdit1.Text;
                    }
                    else {
                        XtraMessageBox.Show("硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch {
                XtraMessageBox.Show("硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            }
            
        }

        /// <summary>
        /// 硫含量_计算公式展开折叠
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void button1_Click(object sender, EventArgs e)
        {
            bool flag = panel1.Visible;
            panel1.Visible = !flag;

        }*/

        /// <summary>
        /// [生铁含量] 变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit2.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("生铁含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// [生铁中硫含量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit3_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit3.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("生铁中硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        /// <summary>
        /// [废钢重量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit4_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit4.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("废钢重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// [废钢中硫含量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit5_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit5.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("废钢中硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// [其他材料重量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit6_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit6.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("其他材料重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// [其他材料中硫含量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit7_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit7.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("其他材料中硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// [铁水重量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit8_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit8.Text))
            {
                try
                {
                    double text2Value = (!string.IsNullOrWhiteSpace(textEdit2.Text)) ? double.Parse(textEdit2.Text) : 0.00;
                    double text3Value = (!string.IsNullOrWhiteSpace(textEdit3.Text)) ? double.Parse(textEdit3.Text) : 0.00;
                    double text4Value = (!string.IsNullOrWhiteSpace(textEdit4.Text)) ? double.Parse(textEdit4.Text) : 0.00;
                    double text5Value = (!string.IsNullOrWhiteSpace(textEdit5.Text)) ? double.Parse(textEdit5.Text) : 0.00;
                    double text6Value = (!string.IsNullOrWhiteSpace(textEdit6.Text)) ? double.Parse(textEdit6.Text) : 0.00;
                    double text7Value = (!string.IsNullOrWhiteSpace(textEdit7.Text)) ? double.Parse(textEdit7.Text) : 0.00;
                    double text8Value = (!string.IsNullOrWhiteSpace(textEdit8.Text)) ? double.Parse(textEdit8.Text) : 0.00;

                    double s_weight = (text2Value * text3Value + text4Value * text5Value + text6Value * text7Value) / text8Value;
                    textEdit1.Text = s_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        /// <summary>
        /// 稀土：[铁水中稀土含量%]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit13_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit13.Text))
            {
                try
                {
                    double text12Value = (!string.IsNullOrWhiteSpace(textEdit12.Text)) ? double.Parse(textEdit12.Text) : 0.00;   //铁水重量
                    double text13Value = (!string.IsNullOrWhiteSpace(textEdit13.Text)) ? double.Parse(textEdit13.Text) : 0.00;   //铁水中硫含量%


                    double xt_weight = (text12Value * text13Value);
                    textEdit11.Text = xt_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("铁水中稀土含量%输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 稀土：[铁水重量]_变化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit12_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit12.Text))
            {
                try
                {
                    double text12Value = (!string.IsNullOrWhiteSpace(textEdit12.Text)) ? double.Parse(textEdit12.Text) : 0.00;   //铁水重量
                    double text13Value = (!string.IsNullOrWhiteSpace(textEdit13.Text)) ? double.Parse(textEdit13.Text) : 0.00;   //铁水中硫含量%


                    double xt_weight = (text12Value * text13Value);
                    textEdit11.Text = xt_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 蠕化剂：[吸收率]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit15_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit15.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("吸收率输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 蠕化剂：[硫含量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit16_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit16.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 蠕化剂：[修正系数α]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit17_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit17.Text))
            {
                try
                {
                    textEdit20.Text = textEdit17.Text;

                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 蠕化剂：[硫含量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit18_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit18.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch {
                    XtraMessageBox.Show("硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }


        /// <summary>
        /// 蠕化剂：[Mg%]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit19_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit19.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("Mg%输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 蠕化剂：[修正系数α]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit20_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit20.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("修正系数α输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 蠕化剂：[硫含量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit21_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit21.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("硫含量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        /// <summary>
        /// 蠕化剂：[铁水重量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit22_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit22.Text))
            {
                try
                {
                    double text15Value = (!string.IsNullOrWhiteSpace(textEdit15.Text)) ? double.Parse(textEdit15.Text) : 0.00;   //吸收率
                    double text16Value = (!string.IsNullOrWhiteSpace(textEdit16.Text)) ? double.Parse(textEdit16.Text) : 0.00;   //硫含量
                    double text17Value = (!string.IsNullOrWhiteSpace(textEdit17.Text)) ? double.Parse(textEdit17.Text) : 0.00;   //修正系数α
                    double text19Value = (!string.IsNullOrWhiteSpace(textEdit19.Text)) ? double.Parse(textEdit19.Text) : 0.00;   //Mg%
                    double text22Value = (!string.IsNullOrWhiteSpace(textEdit22.Text)) ? double.Parse(textEdit22.Text) : 0.00;   //铁水重量


                    double xt_weight = ((0.644 / text15Value) * text16Value + 0.008 + (text17Value * text16Value) / 2.5) / (text19Value + (text17Value * text16Value) / 2.5) * text22Value;
                    textEdit14.Text = xt_weight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 一次孕育剂：[铁水重量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit26_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit26.Text))
            {
                try
                {
                    double text25Value = (!string.IsNullOrWhiteSpace(textEdit25.Text)) ? double.Parse(textEdit25.Text) : 0.00;   //铁水中一次孕育剂百分比
                    double text26Value = (!string.IsNullOrWhiteSpace(textEdit26.Text)) ? double.Parse(textEdit26.Text) : 0.00;   //铁水重量


                    double firstYyWeight = text25Value * text26Value;
                    textEdit27.Text = firstYyWeight.ToString();
                }
                catch {
                    XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 一次孕育剂：[%]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit25_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit25.Text))
            {
                try
                {
                    double text25Value = (!string.IsNullOrWhiteSpace(textEdit25.Text)) ? double.Parse(textEdit25.Text) : 0.00;   //铁水中一次孕育剂百分比
                    double text26Value = (!string.IsNullOrWhiteSpace(textEdit26.Text)) ? double.Parse(textEdit26.Text) : 0.00;   //铁水重量


                    double firstYyWeight = text25Value * text26Value;
                    textEdit27.Text = firstYyWeight.ToString();
                }
                catch
                {
                    XtraMessageBox.Show("%输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 二次孕育剂：[铁水重量]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit24_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit24.Text))
            {
                try
                {
                    double text23Value = (!string.IsNullOrWhiteSpace(textEdit23.Text)) ? double.Parse(textEdit23.Text) : 0.00;   //铁水中一次孕育剂百分比
                    double text24Value = (!string.IsNullOrWhiteSpace(textEdit24.Text)) ? double.Parse(textEdit24.Text) : 0.00;   //铁水重量


                    double secYyWeight = text23Value * text24Value;
                    textEdit28.Text = secYyWeight.ToString();
                }
                catch {
                    XtraMessageBox.Show("铁水重量输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        /// <summary>
        /// 二次孕育剂：[%]_变更函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit23_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textEdit23.Text))
            {
                try
                {
                    double text23Value = (!string.IsNullOrWhiteSpace(textEdit23.Text)) ? double.Parse(textEdit23.Text) : 0.00;   //铁水中一次孕育剂百分比
                    double text24Value = (!string.IsNullOrWhiteSpace(textEdit24.Text)) ? double.Parse(textEdit24.Text) : 0.00;   //铁水重量


                    double secYyWeight = text23Value * text24Value;
                    textEdit28.Text = secYyWeight.ToString();
                }
                catch {
                    XtraMessageBox.Show("%输入错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        

     

        

       

        

       
       
        

       


       
       
       
        


        

       



     
    }
}