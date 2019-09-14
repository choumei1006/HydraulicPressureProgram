using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreepRateApp.Core
{
    public static class FindMin
    {
        public static int FindMinData(List<string> list, int maxNum)
        {
            int temp = 0;
            int temp2 =0;
            double aaa = 100000.0;
            for (int i = 0; i < maxNum; i++)
            {
                double a = double.Parse(list[i]);
                double b = double.Parse(list[i + 1]);
                if ((double)(b - a) / 0.5 < 0.0)
                {

                }
                else
                {
                    temp = i;
                    break;
                }
            }
            for (int i = temp; i < maxNum; i++)
            {
                double a = double.Parse(list[i]);
                if (a < aaa)
                {
                    aaa = a;
                    temp2 = i;
                }
            }
            return temp2;
        }

        public static int FindNovalidMin(List<string> list, int maxNum)
        {
            int temp = 0;
            int temp2 = 0;
            int flag = 0;
            double aaa = 100000.0;
            for (int i = 0; i < maxNum; i++)
            {
                double a = double.Parse(list[i]);
                double b = double.Parse(list[i + 1]);
                if ((double)(b - a) / 0.5 < 0.0)
                {

                }
                else
                {
                    temp = i;
                    break;
                }
            }
            for (int i = 2; i < maxNum - 50; i++)
            {
                double a = double.Parse(list[i]);
                if (a < aaa)
                {
                    aaa = a;
                    temp2 = i;
                }
            }
            if (temp2 < 120 && temp2 > 50)
            {
                flag = 1;
            }
            return flag;
        }
    }
}
