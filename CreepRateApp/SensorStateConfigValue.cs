using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace CreepRateApp
{
    /// <summary>
    /// 传感器状态配置信息类
    /// </summary>
    public static class SensorStateConfigValue
    {
        private static List<string> configList = new List<string>();
        /// <summary>
        /// 传感器通道状态_阀组状态
        /// </summary>
        private static Byte ValveGroupSate = 0;


        /// <summary>
        /// 电磁阀1-5
        /// </summary>
        private static Byte Valve1 = 0;
        private static Byte Valve2 = 0;
        private static Byte Valve3 = 0;
        private static Byte Valve4 = 0;
        private static Byte Valve5 = 0;

        public static void setSensorStateConfigValue(List<string> valueList) {
            configList = valueList;
            int rst = valueList[0].CompareTo("True") ;

            Valve1 = (byte)(valueList[0].CompareTo("True") == 0 ? 1 : 0);
            Valve2 = (byte)(valueList[1].CompareTo("True") == 0 ? 1 : 0);
            Valve3 = (byte)(valueList[2].CompareTo("True") == 0 ? 1 : 0);
            Valve4 = (byte)(valueList[3].CompareTo("True") == 0 ? 1 : 0);
            Valve5 = (byte)(valueList[4].CompareTo("True") == 0 ? 1 : 0);
        }

        /// <summary>
        /// 获取传感器通道组状态
        /// </summary>
        /// <returns></returns>
        public static byte getValveGroupState() {
            ValveGroupSate =(byte)(Valve1 ^ (Valve2 <<1) ^ (Valve3 << 2) ^ (Valve4 << 3) ^ (Valve5 << 4));
            return ValveGroupSate;
        }

        /// <summary>
        /// 获取传感器状态配置List
        /// </summary>
        /// <returns></returns>
        public static List<string> getSensorStateConfigList() {
            return configList;
        }

        /// <summary>
        /// 获取命令
        /// </summary>
        /// <returns></returns>
        public static string getSendCmd()
        {
            byte[] cmd = new byte[6];

            //Header
            cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
            cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);

            //Len
            cmd[2] = 2;

            //data
            //--Category
            cmd[3] = byte.Parse("04", System.Globalization.NumberStyles.HexNumber);

            //--data
            /*for (int m = 0; m < 19; m++)
            {
                cmd[m + 4] = byte.Parse(configList[m], System.Globalization.NumberStyles.Integer);
            }*/
            cmd[4] = getValveGroupState();



            //Verify
            byte verifyByte = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                verifyByte ^= cmd[i];
            }
            cmd[5] = verifyByte;

            //转换为十六进制字符串
            String cmdStr = "";
            for (int i = 0; i < cmd.Length; i++)
            {
                StringBuilder hexStr = new StringBuilder(cmd[i].ToString("X2"));
                cmdStr +="0x"+ hexStr + " ";
            }

            //System.Text.UnicodeEncoding unicodeEncoder = new UnicodeEncoding();




            return cmdStr;
        }
         

        
    }
}
