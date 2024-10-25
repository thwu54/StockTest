using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public enum In
    {
        K_Up_D = 1,
        K_Down = 2,
        None = 0
    }
    class InCondition
    {
        public static int currentIndex = 0;
        public static DataFrame df { get; set; }
         

        public static double Min(string col, int i, int length)
        {
            DataFrameColumn column = df[col];
            var columnData = Enumerable.Range(currentIndex - i - length, length)
                   .Select(row => column[row]);
            IEnumerable<double> convertedValues = columnData.Cast<double>();
            double avg = convertedValues.Min();
            return avg;
        }

        public static double K(int i)
        {
            double K = (double)df.Columns["slowk"][currentIndex - i]; 
            return K;
        }

        public static double D(int i)
        {
            double D = (double)df.Columns["slowd"][currentIndex - i];
            return D;
        }

        public static double C(int i)
        {
            double C = (double)df.Columns["close"][currentIndex - i];
            return C;
        }

        public static double L(int i)
        {
            double L = (double)df.Columns["low"][currentIndex - i];
            return L;
        }
    }
}
