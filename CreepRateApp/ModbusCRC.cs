
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialportSample
{
    /*    Moudbus Crc校验通用类
     **/

    public class ModbusCRC
    {
        private string _crcvalue = "";

        public string CrcValue
        {
            get { return _crcvalue; }
            set { _crcvalue = value; }
        }

        ///<summary>
        ///例如十六进制字符串转化为字节数组
        ///例如：54 21 28 22 E5 F3转换为字节数组
        ///</summary>
        ///<param name="m_HexStr">十六进制字符串</param>
        ///<returns></returns>
        public byte[] HexToBytes(string m_HexStr)
        {
            string strTemp = m_HexStr.Replace(" ", "");  //兼容带空格与不带空格字符串;

            byte[] byResult = new byte[strTemp.Length / 2];
            for (int i = 0; i < byResult.Length; i++)
                byResult[i] = Convert.ToByte(strTemp.Substring(i * 2, 2), 16);

            return byResult;
        }

        ///<summary>
        ///将字节数据转换为十六进制字符串，中间用 “ ”分割如:F3 16 BB BB BB BB
        ///</summary>
        ///<param name="m_Data">要转换的字节数组</param>
        ///<returns></returns>
        public String BytesToHex(byte[] m_Data)
        {
            return BitConverter.ToString(m_Data).Replace("-", " ").Trim();
        }

        /// <summary>
        /// 计算对应输入字节数组的CRC校验值（CRC16位）
        /// </summary>
        /// <param name="m_ByteData">字节数组</param>
        /// <returns>返回计算出来crc值</returns>
        public byte[] CRC16(byte[] m_ByteData)
        {
            ushort CRC = 0xffff;
            ushort POLYNOMIAL = 0xa001;
            byte[] byResultList = null;

            try
            {
                CrcValue = "";

                for (int i = 0; i < m_ByteData.Length; i++)
                {
                    CRC ^= m_ByteData[i];
                    for (int j = 0; j < 8; j++)
                    {
                        if ((CRC & 0x0001) != 0)
                        {
                            CRC >>= 1;
                            CRC ^= POLYNOMIAL;
                        }
                        else
                            CRC >>= 1;
                    }
                }
                byResultList = BitConverter.GetBytes(CRC);

                if (byResultList != null)
                {
                    StringBuilder tpResult = new StringBuilder();
                    for (int i = 0; i < byResultList.Length; i++)
                        tpResult.Append(byResultList[i].ToString("X2")); //转化为十六进制

                    _crcvalue = tpResult.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return byResultList;
        }

        /// <summary>
        /// 计算对应输入字节数组的CRC校验值（CRC16位）
        /// </summary>
        /// <param name="m_ByteData">十六进制组合的命令字符串</param>
        /// <returns>返回计算出来crc值</returns>
        public byte[] CRC16(string m_HexStr)
        {
            byte[] byResultList = null;
            try
            {
                byte[] bySource = HexToBytes(m_HexStr);

                byResultList = CRC16(bySource);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return byResultList;
        }

        /// <summary>
        /// 计算命令串CRC校验值并且生成命令串
        /// </summary>
        /// <param name="m_HexStr">十六进制组合的命令字符串</param>
        /// <returns>最终发送的命令串</returns>
        public byte[] FillCRC16(string m_HexStr)
        {
            byte[] byResultList = null;
            try
            {
                byte[] byTempList = HexToBytes(m_HexStr);

                byResultList = new byte[byTempList.Length + 2];
                Array.Copy(byTempList, byResultList, byTempList.Length);   //赋值前半截命令到返回数组

                //计算CRC16校验值
                byte[] strArrResult = CRC16(byTempList);
                if (strArrResult != null)
                {
                    byResultList[byResultList.Length - 2] = strArrResult[0];
                    byResultList[byResultList.Length - 1] = strArrResult[1];
                }
                return byResultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
