using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace CreepRateApp
{
    public static class GlobalValue
    {
        public static string DbConnString = "mongodb://127.0.0.1:27017";

        public static string DbName = "CreepRate";

        /// <summary>
        /// 端口
        /// </summary>
        public static string PortName = "1";
        /// <summary>
        /// 波特率
        /// </summary>
        public static string BaudRate = "9600";
        /// <summary>
        /// 数据位
        /// </summary>
        public static string DataBits = "8";
        /// <summary>
        /// 停止位
        /// </summary>
        public static string StopBits = "1";
        /// <summary>
        /// 校验
        /// </summary>
        public static Parity Parity = Parity.None;
        /// <summary>
        /// 编码方式
        /// 1-16进制
        /// 2-ASCII
        /// 3-UTF-8
        /// 4-Unicode
        /// </summary>
        //public static string CodeType = "1";

        /// <summary>
        /// 间隔时间
        /// </summary>
        public static string IntalvasTime = "250";

        //---------------------------------start---------------------------
        /// <summary>
        /// 铁水重量IronWaterWeight
        /// </summary>
        public static string IronWaterWeight = "250";
        /// <summary>
        /// 硫含量SulfurPercent
        /// </summary>
        public static string SulfurPercent = "250";
        /// <summary>
        /// 铁水温度IronWaterTemperature
        /// </summary>
        public static string IronWaterTemperature = "250";
        /// <summary>
        /// 稀土TombarthiteWeight
        /// </summary>
        public static string TombarthiteWeight = "250";
        /// <summary>
        /// 蠕化剂RuhuajiWeight
        /// </summary>
        public static string RuhuajiWeight = "250";
        /// <summary>
        /// 一次孕育剂FirstPregnant
        /// </summary>
        public static string FirstPregnant = "250";
        /// <summary>
        /// 二次孕育剂SecendPregnant
        /// </summary>
        public static string SecendPregnant = "250";
        
        //------------------------------------end-------------------------
    }
}
