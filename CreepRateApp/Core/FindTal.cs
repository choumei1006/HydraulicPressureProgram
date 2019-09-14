using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreepRateApp.Core
{
    public static class FindTal
    {
        public static List<Object> ComputeTal(List<string> groupTxt)
        {
            int num = 0;
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            List<Object> list3 = new List<object>();
            double result = -99.99;
            for (int i = 0; i < groupTxt.Count; i++)
            {
                int a = (int.Parse(groupTxt[i]) - 32) * 5 / 9;
                if (a > 1120 && a < 1190)
                {
                    list.Add(a.ToString());
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                int a = int.Parse(list[i]);
                int b = int.Parse(list[i + 1]);
                double temp = (b - a) / 0.5;
            }
            for (int i = 0; i < list2.Count; i++)
            {
                double temp = double.Parse(list[i + 1]) - double.Parse(list[i]);
                if (temp == 0.0 || temp == 0)
                {
                    result = double.Parse(list[i]);
                    num = i;
                }
            }
            list3.Add(num);
            list3.Add(result);
            return list3;
        }
    }
}
