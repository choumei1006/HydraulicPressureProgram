using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreepRateApp.Core
{
    public static class ChartList
    {
        public static List<string> GetTemperature(string filePath, string maxT, string minT)
        {
            List<string> tList = new List<string>();
            if (string.IsNullOrWhiteSpace(maxT) || string.IsNullOrWhiteSpace(minT))
            {
                tList = DataDeal.DataDealFormat(ReadTxt.ReadTxtFile(filePath));
            }
            else
            {
                double maxTep = double.Parse(maxT);
                double minTep = double.Parse(minT);
                List<string> mylist = DataDeal.DataDealFilter(ReadTxt.ReadTxtFile(filePath), maxTep, minTep);
                tList = DataDeal.MyDataDealFomate(mylist);
            }
            return tList;
        }

        public static List<string> GetSlop(List<string> list)
        {
            if (list.Count > 1)
            {
                return Derivative.GetDerivate(list);
            }
            else
            {
                return null;
            }
        }
    }
}
