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
    public static class SensorSpanConfigValue
    {
        //配置信息数组
        public static List<string> configList = null;

        /// <summary>
        /// 压力测点1-8最高最低值
        /// </summary>
        public static int CHx1_FS = -1;

        public static int CHx1_ZERO = -1;

        public static int CHx2_FS = -1;

        public static int CHx2_ZERO = -1;

        public static int CHx3_FS = -1;

        public static int CHx3_ZERO = -1;

        public static int CHx4_FS = -1;

        public static int CHx4_ZERO = -1;


        public static int CHx5_FS = -1;

        public static int CHx5_ZERO = -1;

        public static int CHx6_FS = -1;

        public static int CHx6_ZERO = -1;

        public static int CHx7_FS = -1;

        public static int CHx7_ZERO = -1;

        public static int CHx8_FS = -1;

        public static int CHx8_ZERO = -1;

        public static int CHx9_FS = -1;

        public static int CHx9_ZERO = -1;

        public static int CHx10_FS = -1;

        public static int CHx10_ZERO = -1;

        public static int CHx11_FS = -1;

        public static int CHx11_ZERO = -1;

        public static int CHx12_FS = -1;

        public static int CHx12_ZERO = -1;

      
        /// <summary>
        /// 通过集合数据，设置配置信息
        /// </summary>
        /// <param name="valueList"></param>
        public static void setChannelConfigValue(List<String> valueList)
        {
            configList = valueList;

            CHx1_FS = int.Parse(valueList[0]);
            CHx1_ZERO = int.Parse(valueList[1]);
            CHx2_FS = int.Parse(valueList[2]);
            CHx2_ZERO = int.Parse(valueList[3]);
            CHx3_FS = int.Parse(valueList[4]);
            CHx3_ZERO = int.Parse(valueList[5]);
            CHx4_FS = int.Parse(valueList[6]);
            CHx4_ZERO = int.Parse(valueList[7]);

            CHx5_FS = int.Parse(valueList[8]);
            CHx5_ZERO = int.Parse(valueList[9]);
            CHx6_FS = int.Parse(valueList[10]);
            CHx6_ZERO = int.Parse(valueList[11]);


            CHx7_FS = int.Parse(valueList[12]);
            CHx7_ZERO = int.Parse(valueList[13]);
            CHx8_FS = int.Parse(valueList[14]);
            CHx8_ZERO = int.Parse(valueList[15]);
            CHx9_FS = int.Parse(valueList[16]);
            CHx9_ZERO = int.Parse(valueList[17]);
            CHx10_FS = int.Parse(valueList[18]);
            CHx10_ZERO = int.Parse(valueList[19]);
            CHx11_FS = int.Parse(valueList[20]);
            CHx11_ZERO = int.Parse(valueList[21]);
            CHx12_FS = int.Parse(valueList[22]);
            CHx12_ZERO = int.Parse(valueList[23]);
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getSendCmd(){
            byte[] cmd = new byte[32];
            //Header
            cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
            cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
            //Device_id
            cmd[2] = Convert.ToByte(MainForm.EquipmentId);
            //Reserve
            cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
            //--Category
            cmd[4] = byte.Parse("04", System.Globalization.NumberStyles.HexNumber);

            //Len
            cmd[5] = byte.Parse("00", System.Globalization.NumberStyles.HexNumber);
            cmd[6] = byte.Parse("18", System.Globalization.NumberStyles.HexNumber);

            //--data
            for (int m = 0; m < 24; m++) {
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

            //System.Text.UnicodeEncoding unicodeEncoder = new UnicodeEncoding();




            return cmdStr;
        }
    }
}
