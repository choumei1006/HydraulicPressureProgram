using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreepRateApp.Core
{
    public static class DataDeal
    {
        public static List<string> DataDealFormat(string[] groupTxt)
        {
            int maxTxt = -1;
            int maxTxtNum = -1;
            for (int i = 0; i < groupTxt.Length; i++)
            {
                if (CreepRateApp.Core.CheckData.IsNumeric(groupTxt[i]))
                {
                    int value = (int)(double.Parse(groupTxt[i])) / 1;
                    if (value > maxTxt)
                    {
                        //maxTxt = int.Parse((double.Parse(groupTxt[i]))/1.0);
                        maxTxt = value;
                        maxTxtNum = i;
                    }
                }
            }
            List<string> list = new List<string>();
            for (int i = maxTxtNum; i < groupTxt.Length; i++)
            {
                list.Add(groupTxt[i]);
            }
            return list;
        }

        public static List<string> SerialDataDealFormat(List<string> groupTxt)
        {
            int maxTxt = -1;
            int maxTxtNum = -1;
            for (int i = 0; i < groupTxt.Count; i++)
            {
                if (CreepRateApp.Core.CheckData.IsNumeric(groupTxt[i]))
                {
                    if (int.Parse(groupTxt[i]) > maxTxt)
                    {
                        maxTxt = int.Parse(groupTxt[i]);
                        maxTxtNum = i;
                    }
                }
            }
            List<string> list = new List<string>();
            for (int i = maxTxtNum; i < groupTxt.Count; i++)
            {
                list.Add(groupTxt[i]);
            }
            return list;
        }

        public static List<String> MyDataDealFomate(List<String> list)
        {
            int maxTxt = -1;
            int maxTxtNum = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (CreepRateApp.Core.CheckData.IsNumeric(list[i]))
                {
                    if (int.Parse(list[i]) > maxTxt)
                    {
                        maxTxt = int.Parse(list[i]);
                        maxTxtNum = i;
                    }
                }
            }
            List<String> listdata = new List<string>();
            for (int i = maxTxtNum; i < list.Count; i++)
            {
                listdata.Add(list[i]);
            }
            return listdata;
        }

        public static List<string> DataDealFilter(string[] groupTxt, double max, double min)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < groupTxt.Length; i++)
            {
                if (CreepRateApp.Core.CheckData.IsNumeric(groupTxt[i]))
                {
                    if (int.Parse(groupTxt[i]) < max && int.Parse(groupTxt[i]) > min)
                    {
                        list.Add(groupTxt[i]);
                    }
                }
            }
            return list;
        }

        public static List<double> BeforeMaxData(string[] groupTxt)
        {
            List<double> listData = new List<double>();
            for (int i = 0; i < groupTxt.Length; i++)
            {
                if (CreepRateApp.Core.CheckData.IsNumeric(groupTxt[i]))
                {
                    listData.Add(double.Parse(groupTxt[i]));
                }
            }
            return listData;
        }
    }
}
