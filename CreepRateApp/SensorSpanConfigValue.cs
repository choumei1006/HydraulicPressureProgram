using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace CreepRateApp
{
    /// <summary>
    /// 传感器量程配置信息类
    /// </summary>
    public static class SensorSpanConfigValue
    {
        //配置信息数组
        public static List<string> configList =
            new List<string> { "60", "0", "60", "0", "60", "0", "60", "0", "60", "0", "60", "0", "60", "0", "60", "0", "125", "-25", "100", "0", "60", "0", "60", "0" };

        /// <summary>
        /// 压力测点1-8最高最低值
        /// </summary>
        public static int CHx1_FS = 60;

        public static int CHx1_ZERO = 0;

        public static int CHx2_FS = 60;

        public static int CHx2_ZERO = 0;

        public static int CHx3_FS = 60;

        public static int CHx3_ZERO = 0;

        public static int CHx4_FS = 60;

        public static int CHx4_ZERO = 0;

        public static int CHx5_FS = 60;

        public static int CHx5_ZERO = 0;

        public static int CHx6_FS = 60;

        public static int CHx6_ZERO = 0;

        public static int CHx7_FS = 60;

        public static int CHx7_ZERO = 0;

        public static int CHx8_FS = 60;

        public static int CHx8_ZERO = 0;

        /// <summary>
        /// 温度测点最高最低值
        /// </summary>
        public static int CHx9_FS = 125;

        public static int CHx9_ZERO = -25;

        /// <summary>
        /// 水分测点最高最低值
        /// </summary> 
        public static int CHx10_FS = 100;

        public static int CHx10_ZERO = 0;

        /// <summary>
        /// 预留1、2最高最低值
        /// </summary> 
        public static int CHx11_FS = 60;

        public static int CHx11_ZERO = 0;

        public static int CHx12_FS = 60;

        public static int CHx12_ZERO = 0;

      
        /// <summary>
        /// 通过集合数据，设置配置信息
        /// </summary>
        /// <param name="valueList"></param>
        public static void setSpanConfigValue(List<String> valueList)
        {
            configList = valueList;
            
            //测点1-8最高最低值
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

            //温度
            CHx9_FS = int.Parse(valueList[16]);
            CHx9_ZERO = int.Parse(valueList[17]);

            //水分
            CHx10_FS = int.Parse(valueList[18]);
            CHx10_ZERO = int.Parse(valueList[19]);

            //预留1、2最高最低值
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
            cmd[2] = MainForm.EquipmentId;
            //Reserve
            cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);
            //--Category
            cmd[4] = byte.Parse("04", System.Globalization.NumberStyles.HexNumber);

            //Len
            cmd[5] = 24;
            cmd[6] = 0;

            sbyte[] scmd = new sbyte[12];   //存储最低值（有符号）

            //data
            for (int m = 0,n=0; m < 24; m+=2,n++) {

                //处理最高值(无符号) 
                cmd[m + 7] = byte.Parse(configList[m], System.Globalization.NumberStyles.Integer);
                //处理最低值(有符号)
                scmd[n] = sbyte.Parse(configList[m+1], System.Globalization.NumberStyles.Integer); 
            }
             


            //Verify
            byte verifyByte = 0;
            int sCmdIndex1 = 0;
            for(int i=0;i<cmd.Length;i++){
                if (i >= 7 && i <= 30 && (i % 2 == 0))
                {
                    verifyByte = (byte)(verifyByte ^ scmd[sCmdIndex1]);
                    sCmdIndex1++;
                }
                else {
                    verifyByte = (byte)(verifyByte ^ (sbyte)cmd[i]);
                } 
            }
            cmd[31] = verifyByte;

            //转换为十六进制字符串
            String cmdStr ="";
            int sCmdIndex2 = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                //最低值
                if (i >= 7 && i <= 30 && (i % 2 == 0))
                {
                    StringBuilder hexStr = new StringBuilder(scmd[sCmdIndex2].ToString("X2"));
                    cmdStr += "0x" + hexStr + " ";
                    sCmdIndex2++;
                }
                else
                {
                    StringBuilder hexStr = new StringBuilder(cmd[i].ToString("X2"));
                    cmdStr += "0x" + hexStr + " ";
                }  
            }  
            return cmdStr;
        }
    }
}
