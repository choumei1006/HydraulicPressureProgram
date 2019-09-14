using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.apache.commons.math3.fitting;

namespace CreepRateApp.Core
{
    public static class Derivative
    {
        public static List<string> GetDerivate(List<string> list)
        {
            List<string> tempList = new List<string>();
            List<string> list_x = new List<string>();
            List<double> result_list = new List<double>();
            WeightedObservedPoints obs = new WeightedObservedPoints();
            PolynomialCurveFitter fitter = PolynomialCurveFitter.create(9);
            for (int i = 0; i < list.Count; i++)
            {
                double a1 = double.Parse(list[i]);
                obs.add(i, a1);
            }
            double[] coeff = fitter.fit(obs.toList());
            for (int i = 0; i < coeff.Length; i++)
            {
                result_list.Add(coeff[i]);
            }
            List<string> k = new List<string>();
            double a = result_list[1];
            double b = result_list[2];
            double c = result_list[3];
            double d = result_list[4];
            double e = result_list[5];
            double f = result_list[6];
            double g = result_list[7];
            double h = result_list[8];
            double z = result_list[9];
            //for (int i = 0; i < list.Count - 100; i++)//去除后100个不稳定的点
            for (int i = 0; i < list.Count; i++)
            {
                double temp_y = (9 * z) * Math.Pow(i, 8)
                    + (8 * h) * Math.Pow(i, 7)
                    + (7 * g) * Math.Pow(i, 6)
                    + (6 * f) * Math.Pow(i, 5)
                    + (5 * e) * Math.Pow(i, 4)
                    + (4 * d) * Math.Pow(i, 3)
                    + (3 * c) * Math.Pow(i, 2)
                    + (2 * b) * Math.Pow(i, 1)
                    + a;
                k.Add(temp_y.ToString());
            }
            return k;
        }
    }
}
