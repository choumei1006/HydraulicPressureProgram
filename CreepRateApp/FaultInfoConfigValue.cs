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

        public static long updateTime = DateTime.Now.Ticks; 

        //配置信息数组
        public static List<string> configList = null;

        /// <summary>
        /// 起始压力
        /// </summary>
        public static string Start_press = "";

        /// <summary>
        /// 开机压力
        /// </summary>
        public static string Open_press = "";

        /// <summary>
        /// 稳定持续时长
        /// </summary>
        public static string dura_stb = "";

        /// <summary>
        /// 建压超时时长
        /// </summary>
        public static string TIME_SYS = "";

        /// <summary>
        /// 系统最低压力
        /// </summary>
        public static string MINPRESS_SYS = "";

        /// <summary>
        /// 系统最高压力 
        /// </summary>
        public static string MAXPRESS_SYS = "";

        /// <summary>
        /// 仓压最低压力
        /// </summary>
        public static string MINPRESS_house = "";

        /// <summary>
        /// 系统压力降低值
        /// </summary>
        public static string Syspress_dn = "";

        /// <summary>
        /// 仓压力最高压力
        /// </summary>
        public static string MAXPRESS_house = "";

        /// <summary>
        /// 回转连接器压差
        /// </summary>
        public static string rotary_Dvalue = "";

        /// <summary>
        /// 蓄能压力时长
        /// </summary>
        public static string Infla_time = "";

        /// <summary>
        /// 蓄能最低压力
        /// </summary>
        public static string Infla_press = "";

        /// <summary>
        /// 左供油最低压力
        /// </summary>
        public static string MINPRESS_LEFT = "";

        /// <summary>
        /// 左供油最高压力
        /// </summary>
        public static string MAXPRESS_LEFT = "";

        /// <summary>
        /// 右供油最低压力
        /// </summary>
        public static string MINPRESS_RIGHT = "";

        /// <summary>
        /// 右供油最高压力
        /// </summary>
        public static string MAXPRESS_RIGHT = "";

        /// <summary>
        /// 固定供油最低压力
        /// </summary>
        public static string MINPRESS_hold = "";

        /// <summary>
        /// 固定供油最高压力
        /// </summary>
        public static string MAXPRESS_hold = "";

        /// <summary>
        /// 固定器异常压力值
        /// </summary>
        public static string fault_hold = "";

        /// <summary>
        /// 阀组压力限值
        /// </summary>
        public static string Valves_maxpress = "";

        /// <summary>
        /// 阀组压力差值
        /// </summary>
        public static string Valves_Dvalue = "";

        /// <summary>
        /// 温度最高值
        /// </summary>
        public static string Temp_max = "";

        /// <summary>
        /// 液体饱和度上限
        /// </summary>
        public static string Liquid_Max = "";

        /// <summary>
        /// Bit0：液位低
        /// </summary>
        public static string digital_bit0 = "";

        /// <summary>
        /// Bit1：过滤器堵塞
        /// </summary>
        public static string digital_bit1 = "";

        /// <summary>
        /// 通过集合数据，设置配置信息
        /// </summary>
        /// <param name="valueList"></param>
        public static void setFaultConfigValue(List<String> valueList)
        {
            //25项，此处的valueList数组中String保证合理性检测（不为空）
            configList = valueList;

            updateTime = DateTime.Now.Ticks;

            Start_press = valueList[0];
            Open_press = valueList[1];
            dura_stb = valueList[2];
            TIME_SYS = valueList[3];
            MINPRESS_SYS = valueList[4];
            MAXPRESS_SYS = valueList[5];
            MINPRESS_house = valueList[6];
            Syspress_dn = valueList[7];

            MAXPRESS_house = valueList[8];
            rotary_Dvalue = valueList[9];
            Infla_time = valueList[10];
            Infla_press = valueList[11];


            MINPRESS_LEFT = valueList[12];
            MAXPRESS_LEFT = valueList[13];
            MINPRESS_RIGHT = valueList[14];
            MAXPRESS_RIGHT = valueList[15];
            MINPRESS_hold = valueList[16];
            MAXPRESS_hold = valueList[17];
            fault_hold = valueList[18];
            Valves_maxpress = valueList[19];
            Valves_Dvalue = valueList[20];
            Temp_max = valueList[21];
            Liquid_Max = valueList[22];
            digital_bit0 = valueList[23];
            digital_bit1 = valueList[24];
            
        }

        /// <summary>
        /// 获取命令
        /// </summary>
        /// <returns></returns>
        public static string getSendCmd()
        {
            //7+(24*2-1)+1
            byte[] cmd = new byte[55];

            //Header
            cmd[0] = byte.Parse("EB", System.Globalization.NumberStyles.HexNumber);
            cmd[1] = byte.Parse("90", System.Globalization.NumberStyles.HexNumber);

            //DEVICE_ID
            cmd[2] = MainForm.EquipmentId;


            //Reserve
            cmd[3] = byte.Parse("ff", System.Globalization.NumberStyles.HexNumber);

            //Category
            cmd[4] = byte.Parse("01", System.Globalization.NumberStyles.HexNumber);

            //Len (2 byte)
            cmd[5] = 47;
            cmd[6] = 0;


            //data(下标7->52（7+2*23-1）最后一个字节digital稍后处理) 
            for (int m = 0,n = 7; m <= 22; m++,n+=2)
            {

                int[] intIndexList = { 3, 4, 11, 22, 23 };    //整数下标数组 
                int byteNum;    //当前访问到的配置信息数组元素
                 
                if (intIndexList.Contains(m + 1))
                { 
                    byteNum = (int.Parse(configList[m])) * 100;
                }
                else { 
                    byteNum = (int)(Double.Parse(configList[m]) * 100);
                }

                //分高字节与低字节存储在2个字节中
                cmd[n] = (byte)(byteNum & 255);
                cmd[n + 1] =    (byte)((byteNum >> 8) & 255); 


            }

            //data(digital  下标53)
            byte byte_bit0 = byte.Parse(configList[23], System.Globalization.NumberStyles.Integer);
            byte byte_bit1 = byte.Parse(configList[24], System.Globalization.NumberStyles.Integer);
            cmd[53] = (byte)(byte_bit0 ^ (byte_bit1<<1));
            

            //Verify
            byte verifyByte = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                verifyByte ^= cmd[i];
            }
            cmd[54] = verifyByte;

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

        /// <summary>
        /// 获取故障信息配置值List
        /// </summary>
        /// <returns></returns>
        public static List<String> getConfigList() {
            return configList;
        }
         
    }
}
