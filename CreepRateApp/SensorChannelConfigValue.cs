using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace CreepRateApp
{
    /// <summary>
    /// 传感器通道配置信息类
    /// </summary>
    public static class SensorChannelConfigValue
    {
        //配置信息数组
        public static List<string> configList = 
            new List<string>{"1","2","3","4","5","6","7","8","9","10","11","12","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","1","2","3"};

        /// <summary>
        /// 压力测点1-8通道号
        /// </summary>
        public static int ADC1_CHx1 = 1;

        public static int ADC1_CHx2 = 2;

        public static int ADC1_CHx3 = 3;

        public static int ADC1_CHx4 = 4;

        public static int ADC1_CHx5 = 5;

        public static int ADC1_CHx6 = 6;

        public static int ADC1_CHx7 = 7;

        public static int ADC1_CHx8 = 8;

       
        public static int ADC1_CHx9 = 9;

        public static int ADC1_CHx10 = 10;

        
        public static int ADC1_CHx11 = 11;

        public static int ADC1_CHx12 = 12;

        /// <summary>
        /// 24V测点1-15通道号
        /// </summary>
        public static int ADC2_CHx1 = 1;

        public static int ADC2_CHx2 = 2;

        public static int ADC2_CHx3 = 3;

        public static int ADC2_CHx4 = 4;

        public static int ADC2_CHx5 = 5;

        public static int ADC2_CHx6 = 6;

        public static int ADC2_CHx7 = 7;

        public static int ADC2_CHx8 = 8;

        public static int ADC2_CHx9 = 9;

        public static int ADC2_CHx10 = 10;

        public static int ADC2_CHx11 = 11;

        public static int ADC2_CHx12 = 12;

        public static int ADC2_CHx13 = 13;

        public static int ADC2_CHx14 = 14;

        public static int ADC2_CHx15 = 15;

        /// <summary>
        /// 开关量测点1-3
        /// </summary>
        public static int DIN_CHx1 = 1;

        public static int DIN_CHx2 = 2;

        public static int DIN_CHx3 = 3;

        /// <summary>
        /// 通过集合数据，设置配置信息
        /// </summary>
        /// <param name="valueList"></param>
        public static void setChannelConfigValue(List<String> valueList)
        {
            configList = valueList;

            ADC1_CHx1 = int.Parse(valueList[0]);
            ADC1_CHx2 = int.Parse(valueList[1]);
            ADC1_CHx3 = int.Parse(valueList[2]);
            ADC1_CHx4 = int.Parse(valueList[3]);
            ADC1_CHx5 = int.Parse(valueList[4]);
            ADC1_CHx6 = int.Parse(valueList[5]);
            ADC1_CHx7 = int.Parse(valueList[6]);
            ADC1_CHx8 = int.Parse(valueList[7]);

            ADC1_CHx9 = int.Parse(valueList[8]);
            ADC1_CHx10 = int.Parse(valueList[9]);
            ADC1_CHx11 = int.Parse(valueList[10]);
            ADC1_CHx12 = int.Parse(valueList[11]);


            ADC2_CHx1 = int.Parse(valueList[12]);
            ADC2_CHx2 = int.Parse(valueList[13]);
            ADC2_CHx3 = int.Parse(valueList[14]);
            ADC2_CHx4 = int.Parse(valueList[15]);
            ADC2_CHx5 = int.Parse(valueList[16]);
            ADC2_CHx6 = int.Parse(valueList[17]);
            ADC2_CHx7 = int.Parse(valueList[18]);
            ADC2_CHx8 = int.Parse(valueList[19]);
            ADC2_CHx9 = int.Parse(valueList[20]);
            ADC2_CHx10 = int.Parse(valueList[21]);
            ADC2_CHx11 = int.Parse(valueList[22]);
            ADC2_CHx12 = int.Parse(valueList[23]);
            ADC2_CHx13 = int.Parse(valueList[24]);
            ADC2_CHx14 = int.Parse(valueList[25]);
            ADC2_CHx15 = int.Parse(valueList[26]);

            DIN_CHx1 = int.Parse(valueList[27]);
            DIN_CHx2 = int.Parse(valueList[28]);
            DIN_CHx3 = int.Parse(valueList[29]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getSendCmd(){
            byte[] cmd = new byte[38];
            //Header
            cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
            cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
            //Device_id
            cmd[2] = MainForm.EquipmentId;
            //Reserve
            cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
            //--Category
            cmd[4] = byte.Parse("02", System.Globalization.NumberStyles.HexNumber);

            //Len(2 byte)
            cmd[5] = 0;
            cmd[6] = 30;

            //--data
            for (int m = 0; m < 30; m++) {
                cmd[m + 7] = byte.Parse(configList[m], System.Globalization.NumberStyles.Integer);
            }
            


            //Verify
            byte verifyByte = 0;
            for(int i=0;i<cmd.Length;i++){
                verifyByte^=cmd[i];
            }
            cmd[37] = verifyByte;

            //转换为十六进制字符串
            String cmdStr ="";
            for (int i = 0; i < cmd.Length; i++)
            {
                StringBuilder hexStr = new StringBuilder(cmd[i].ToString("X2"));
                cmdStr += "0x"+ hexStr+" ";
            }
             
            return cmdStr;
        }
    }
}
