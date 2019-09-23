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
    public partial class FaultInfoConfigForm : DevExpress.XtraEditors.XtraForm
    {
        private SerialPort mSerialPort; 
        private ModbusCRC crc = new ModbusCRC();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private int[] fctext = new int[50];//故障配置

        public FaultInfoConfigForm(SerialPort paramPortDev)
        {
            InitializeComponent();
            mSerialPort = paramPortDev;
            mSerialPort.ReceivedBytesThreshold = 1; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getint3();
        }

       //
        private int[] getint3()
        {
            fctext[0] = (int)this.numericUpDown1.Value;
            fctext[1] = (int)this.numericUpDown2.Value;
            fctext[2] = (int)this.numericUpDown3.Value;
            fctext[3] = (int)this.numericUpDown4.Value;
            fctext[4] = (int)this.numericUpDown5.Value;
            fctext[5] = (int)this.numericUpDown6.Value;
            fctext[6] = (int)this.numericUpDown7.Value;
            fctext[7] = (int)this.numericUpDown8.Value;
            fctext[8] = (int)this.numericUpDown9.Value;
            fctext[9] = (int)this.numericUpDown10.Value;
            fctext[10] = (int)this.numericUpDown11.Value;
            fctext[11] = (int)this.numericUpDown12.Value;
            fctext[12] = (int)this.numericUpDown13.Value;
            fctext[13] = (int)this.numericUpDown14.Value;
            fctext[14] = (int)this.numericUpDown15.Value;
            fctext[15] = (int)this.numericUpDown16.Value;
            fctext[16] = (int)this.numericUpDown17.Value;
            fctext[17] = (int)this.numericUpDown18.Value;
            fctext[18] = (int)this.numericUpDown19.Value;

            return fctext;
        }
        
        
    }
}