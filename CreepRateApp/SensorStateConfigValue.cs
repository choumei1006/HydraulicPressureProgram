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
        /// <summary>
        /// 传感器通道状态_阀组状态
        /// </summary>
        public static Byte ValveGroupSate = 0;


        /// <summary>
        /// 电磁阀1-5
        /// </summary>
        public static Byte Valve1 = 0;
        public static Byte Valve2 = 0;
        public static Byte Valve3 = 0;
        public static Byte Valve4 = 0;
        public static Byte Valve5 = 0;

        /// <summary>
        /// 获取传感器通道组状态
        /// </summary>
        /// <returns></returns>
        public static byte getValveGroupState() {
            ValveGroupSate =(byte)(Valve1 ^ (Valve2 <<1) ^ (Valve3 << 2) ^ (Valve4 << 3) ^ (Valve5 << 4));
            return ValveGroupSate;
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
