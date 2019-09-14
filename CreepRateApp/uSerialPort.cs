using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace SerialportSample
{
    /*  类说明：封封系统SerialPort
     */
    public class uSerialPort 
    {
       private SerialPort comm = new SerialPort();

       public uSerialPort()
       {
           comm.NewLine = "\r\n";
           comm.RtsEnable = true;//根据实际情况吧。

           comm.DataReceived += comm_DataReceived;
       }

       ~uSerialPort()
       {
           comm.Close();
           comm.Dispose();
       }

       public bool Run(string m_PortName, string m_BaudRate)
       {
           bool blResult = false;

           comm.PortName = m_PortName;
           comm.BaudRate = int.Parse(m_BaudRate);

           try
           {
               if (comm.IsOpen) comm.Close();

               comm.Open();
               blResult = true;
           }
           catch (Exception ex)
           {
               throw ex;
           }
           return blResult;
       }

       void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
       {
           throw new NotImplementedException();
       }


    }
}
