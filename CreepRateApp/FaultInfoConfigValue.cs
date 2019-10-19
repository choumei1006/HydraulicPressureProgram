using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace CreepRateApp
{
    /// <summary>
    /// 故障配置信息类
    /// </summary>
    public static class FaultInfoConfigValue
    {
        //配置信息数组
        public static List<string> configList = null;

        /// <summary>
        /// 系统欠压（压力测点2）：建压时长（单位:S）
        /// </summary>
        public static string TIME_SYS = "";

        /// <summary>
        /// 系统欠压：最低压力（单位:MPa）
        /// </summary>
        public static string MINPRESS_SYS = "";

        /// <summary>
        /// 系统超压：最高压力（单位:MPa）
        /// </summary>
        public static string MAXPRESS_SYS = "";

        /// <summary>
        /// 仓压力（压力测点3）：最低压力（单位:MPa）
        /// </summary>
        public static string MINPRESS_house = "";

        /// <summary>
        /// 仓压力（压力测点3）：最高压力（单位:MPa）
        /// </summary>
        public static string MAXPRESS_house = "";

        /// <summary>
        /// 回转连接器漏油：压差（单位:MPa）
        /// </summary>
        public static string Oil_rotary_inc = "";

        /// <summary>
        /// 蓄能器充气压力（压力测点？）：压力（单位:MPa）
        /// </summary>
        public static string Infla_press = "";

        /// <summary>
        /// 左供油压力（压力测点5）：最低压力（单位:MPa）
        /// </summary>
        public static string MINPRESS_LEFT = "";

        /// <summary>
        /// 左供油压力（压力测点5）：最高压力（单位:MPa）
        /// </summary>
        public static string MAXPRESS_LEFT = "";

        /// <summary>
        /// 右供油压力（压力测点7）：最低压力（单位:MPa）
        /// </summary>
        public static string MINPRESS_RIGHT = "";

        /// <summary>
        /// 右供油压力（压力测点7）：最高压力（单位:MPa）
        /// </summary>
        public static string MAXPRESS_RIGHT = "";

        /// <summary>
        /// 固定供油压力（压力测点8）：最低压力（单位:MPa）
        /// </summary>
        public static string MINPRESS_hold = "";

        /// <summary>
        /// 固定供油压力（压力测点8）：最高压力（单位:MPa）
        /// </summary>
        public static string MAXPRESS_hold = "";

        /// <summary>
        /// 固定器异常：压力测点8）异常压力阈值（单位:MPa）
        /// </summary>
        public static string Pressfault_hold = "";

        /// <summary>
        /// 温度过高：温度阈值（单位:°）
        /// </summary>
        public static string Temp = "";

        /// <summary>
        /// 液位：下限值
        /// </summary>
        public static string Minliquid = "";

        /// <summary>
        /// 液位：上限值
        /// </summary>
        public static string Maxliquid = "";

        /// <summary>
        /// 含水量：上限值
        /// </summary>
        public static string Maxwater = "";

        /// <summary>
        /// 过滤器两端压差：差值
        /// </summary>
        public static string Filterpress = "";

        /// <summary>
        /// 通过集合数据，设置配置信息
        /// </summary>
        /// <param name="valueList"></param>
        public static void setChannelConfigValue(List<String> valueList)
        {
            configList = valueList;

            TIME_SYS = valueList[0];
            MINPRESS_SYS = valueList[1];
            MAXPRESS_SYS = valueList[2];
            MINPRESS_house = valueList[3];
            MAXPRESS_house = valueList[4];
            Oil_rotary_inc = valueList[5];
            Infla_press = valueList[6];
            MINPRESS_LEFT = valueList[7];

            MAXPRESS_LEFT = valueList[8];
            MINPRESS_RIGHT = valueList[9];
            MAXPRESS_RIGHT = valueList[10];
            MINPRESS_hold = valueList[11];


            MAXPRESS_hold = valueList[12];
            Pressfault_hold = valueList[13];
            Temp = valueList[14];
            Minliquid = valueList[15];
            Maxliquid = valueList[16];
            Maxwater = valueList[17];
            Filterpress = valueList[18];
            
        }

        /// <summary>
        /// 获取命令
        /// </summary>
        /// <returns></returns>
        public static string getSendCmd()
        {
            byte[] cmd = new byte[24];

            //Header
            cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
            cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);
            //DEVICE_ID
            cmd[3] = 1;


            //Reserve
            cmd[4] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);;

            //Len
            cmd[3] = 0;
            cmd[4] = 47; 

            //data
            //--Category
            cmd[3] = byte.Parse("01", System.Globalization.NumberStyles.HexNumber);

            //--data
            for (int m = 0; m < 19; m++)
            {
                cmd[m + 4] = byte.Parse(configList[m], System.Globalization.NumberStyles.Integer);
            }

            

            //Verify
            byte verifyByte = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                verifyByte ^= cmd[i];
            }
            cmd[23] = verifyByte;

            //转换为十六进制字符串
            String cmdStr = "";
            for (int i = 0; i < cmd.Length; i++)
            {
                StringBuilder hexStr = new StringBuilder(cmd[i].ToString("X2")); 
                cmdStr += "0x" + hexStr + " ";
            }

            //System.Text.UnicodeEncoding unicodeEncoder = new UnicodeEncoding();




            return cmdStr;
        }
         
    }
}
