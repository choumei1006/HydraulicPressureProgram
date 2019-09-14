using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreepRateApp.Core
{
    public static class FindMax
    {
        public static int FindMaxData(List<string> list)
        {
            int temp=-1;
            for(int i=list.Count-1;i>1;i--)
            {
                double a=double.Parse(list[i]);
                double b=double.Parse(list[i-1]);
                if((a-b)>0.0)
                {
                    temp=i;
                    break;
                }
            }
            return temp;
        }
    }
}
