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
    public partial class SensorChannelConfigForm : DevExpress.XtraEditors.XtraForm
    {
        private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int[] sctext = new int[50]; //传感器数据


        public SensorChannelConfigForm(SerialPort paramPortDev)
        {
            InitializeComponent();
            mSerialPort = paramPortDev;
            mSerialPort.ReceivedBytesThreshold = 1; 
        }

        private void config_current_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int[] t1 = getint1();
            
        }

        private int[] getint1()
        {
            sctext[0] = (int)this.numericUpDown1.Value;
            sctext[1] = (int)this.numericUpDown2.Value;
            sctext[2] = (int)this.numericUpDown3.Value;
            sctext[3] = (int)this.numericUpDown4.Value;
            sctext[4] = (int)this.numericUpDown5.Value;
            sctext[5] = (int)this.numericUpDown6.Value;
            sctext[6] = (int)this.numericUpDown7.Value;
            sctext[7] = (int)this.numericUpDown8.Value;


            return sctext;
        }


        private int[] getint2()
        {
            sctext[8] = (int)this.numericUpDown13.Value;
            sctext[9] = (int)this.numericUpDown14.Value;
            sctext[10] = (int)this.numericUpDown15.Value;
            sctext[11] = (int)this.numericUpDown16.Value;

            return sctext;
        }


        private int[] getint3()
        {
            sctext[12] = (int)this.numericUpDown17.Value;
            sctext[13] = (int)this.numericUpDown18.Value;
            sctext[14] = (int)this.numericUpDown19.Value;
            sctext[15] = (int)this.numericUpDown20.Value;
            sctext[16] = (int)this.numericUpDown21.Value;
            sctext[17] = (int)this.numericUpDown22.Value;
            sctext[18] = (int)this.numericUpDown23.Value;
            sctext[19] = (int)this.numericUpDown24.Value;
            sctext[20] = (int)this.numericUpDown25.Value;
            sctext[21] = (int)this.numericUpDown26.Value;
            sctext[22] = (int)this.numericUpDown27.Value;
            sctext[23] = (int)this.numericUpDown28.Value;
            sctext[24] = (int)this.numericUpDown29.Value;
            sctext[25] = (int)this.numericUpDown30.Value;
            sctext[26] = (int)this.numericUpDown31.Value;

            return sctext;
        }


        private int[] getint4()
        {
            sctext[27] = (int)this.numericUpDown32.Value;
            sctext[28] = (int)this.numericUpDown33.Value;
            sctext[29] = (int)this.numericUpDown34.Value;

            return sctext;
        }




        private void button4_Click(object sender, EventArgs e)
        {
            getint2();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getint3();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getint4();
        }
        
    }
}