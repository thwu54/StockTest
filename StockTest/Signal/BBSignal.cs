using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;

namespace My
{
    class BBSignal : Signal
    {
        public string Name { get; set; }

        public List<Trade> Trades { get; set; }

        private DataFrame df { get; set; }

        private int currentIndex = 0;

        void Signal.Gen()
        {
            
        }
        
        public BBSignal(string name)
        {
            this.Name = name;
        }


        public double UB(int i)
        {
            double K = (double)df.Columns["upperband"][currentIndex - i];

            return K;
        }

        public double LB(int i)
        {
            double D = (double)df.Columns["lowerband"][currentIndex - i];
            return D;
        }

        public double MB(int i)
        {
            double D = (double)df.Columns["middleband"][currentIndex - i];
            return D;
        }

        public double C(int i)
        {
            double C = (double)df.Columns["close"][currentIndex - i];
            return C;
        }

        public double L(int i)
        {
            double L = (double)df.Columns["low"][currentIndex - i];
            return L;
        }

        public DateTime date(int i)
        {
            DateTime date = DateTime.Parse(df.Columns["date"][currentIndex - i].ToString());
            return date;
        }

        public double V(int i)
        {
            return (double)df.Columns["volume"][currentIndex - i]; 
        }

        public double O(int i)
        {
            return (double)df.Columns["open"][currentIndex - i];
        }

        public double AVG(string col, int i, int length)
        {
            DataFrameColumn column = df[col];
            var columnData = Enumerable.Range(currentIndex - i - length, length)
                   .Select(row => column[row]);
            IEnumerable<double> convertedValues = columnData.Cast<double>();
            double avg = convertedValues.Average();
            return avg;
        }

        public double Min(string col, int i, int length)
        {
            DataFrameColumn column = df[col];
            var columnData = Enumerable.Range(currentIndex - i - length, length)
                   .Select(row => column[row]);
            IEnumerable<double> convertedValues = columnData.Cast<double>();
            double avg = convertedValues.Min();
            return avg;
        }

        public double Max(string col, int i, int length)
        {
            DataFrameColumn column = df[col];
            var columnData = Enumerable.Range(currentIndex - i - length, length)
                   .Select(row => column[row]);
            IEnumerable<double> convertedValues = columnData.Cast<double>();
            double avg = convertedValues.Max();
            return avg;
        }

        public ArrayList profit()
        {
            double firstPoint = 0;
            double Profit = 0;
            double WinCount = 0;
            double AllCount = 0;
            double Win = 0;
            double Lose = 0;
            int KeepTimeRange = 0;
            foreach (Trade item in Trades)
            {
                if (item.IsClose)
                {
                    if (firstPoint == 0)
                    {
                        firstPoint = item._Point1;
                    }
                    double profit1 = item.profit(0.01);
                    Profit += profit1;
                    AllCount++;
                    KeepTimeRange += item._TradeNo2 - item._TradeNo;
                    if (item.profit(0.01) > 0)
                    {
                        WinCount++;
                        Win += profit1;
                    }
                    else
                    {
                        Lose += profit1;
                    }
                }
            }
            double WinFisrtRate = Profit;
            double WinRate = WinCount / AllCount;
            if (double.IsNaN(WinRate))
                WinRate = 0;
            double Win1 = Win / WinCount;
            double Lose1 = (Lose / (AllCount - WinCount)) * -1;
            if (double.IsNaN(WinFisrtRate))
                WinFisrtRate = 0;
            if (WinCount == AllCount)
                Lose1 = 0;
            double WinLoseRate = (Win1 / Lose1);
            if (double.IsNaN(WinLoseRate))
                WinLoseRate = 0;
            if (WinCount == 0)
                WinLoseRate = 0;
            if (Lose1 == 0)
                WinLoseRate = 10;


            double Kelly = WinRate - ((1 - WinRate) / WinLoseRate);
            if (double.IsNaN(Kelly))
                Kelly = 0;
            if (double.IsNegativeInfinity(Kelly))
                Kelly = 0;
            ArrayList list = new ArrayList();
            list.Add(this.Name);
            list.Add(Trades.Count);
            list.Add(WinFisrtRate);
            list.Add(WinRate);
            list.Add(WinLoseRate);
            list.Add(Kelly);
            list.Add(KeepTimeRange);

            //
            return list;
        }

        public static ArrayList profit(List<Trade> Trades)
        {
            double firstPoint = 0;
            double Profit = 0;
            double WinCount = 0;
            double AllCount = 0;
            double Win = 0;
            double Lose = 0;
            int KeepTimeRange = 0;
            foreach (Trade item in Trades)
            {
                if (item.IsClose)
                {
                    if (firstPoint == 0)
                    {
                        firstPoint = item._Point1;
                    }
                    double profit1 = item.profit(0.01);
                    Profit += profit1;
                    AllCount++;
                    KeepTimeRange += item._TradeNo2 - item._TradeNo;
                    if (item.profit(0.01) > 0)
                    {
                        WinCount++;
                        Win += profit1;
                    }
                    else
                    {
                        Lose += profit1;
                    }
                }
            }
            double WinFisrtRate = Profit  ;
            double WinRate = WinCount / AllCount;
            if (double.IsNaN(WinRate))
                WinRate = 0;
            double Win1 = Win / WinCount;
            double Lose1 = (Lose / (AllCount - WinCount)) * -1;
            if (double.IsNaN(WinFisrtRate))
                WinFisrtRate = 0;
            if (WinCount == AllCount)
                Lose1 = 0;
            double WinLoseRate = (Win1 / Lose1);
            if (double.IsNaN(WinLoseRate))
                WinLoseRate = 0;
            if (WinCount == 0)
                WinLoseRate = 0;
            if (Lose1 == 0)
                WinLoseRate = 10;


            double Kelly = WinRate - ((1 - WinRate) / WinLoseRate);
            if (double.IsNaN(Kelly))
                Kelly = 0;
            if (double.IsNegativeInfinity(Kelly))
                Kelly = 0;
            ArrayList list = new ArrayList();
            list.Add("ALL");
            list.Add(Trades.Count);
            list.Add(WinFisrtRate);
            list.Add(WinRate);
            list.Add(WinLoseRate);
            list.Add(Kelly);
            list.Add(KeepTimeRange);

            //
            return list;
        }

        public bool IsAvgUp(int length)
        { 
            for (int i = 0; i < length; i++)
            {
                if (MB(i) < MB(i + 1))
                    return false;
            }
            return true;
        }

        public void Init(DataFrame df, string[] para)
        {
            Trades = new List<Trade>(); 
            this.df = df;
            TradeType Type = TradeType.None;
            for (int i = 31; i < df.Rows.Count; i++)
            {
                currentIndex = i;
                if (Trades.Count > 0)
                {
                    if (Trades[Trades.Count - 1].IsClose)
                        Type = TradeType.None;
                    else
                        Type = Trades[Trades.Count - 1]._Type;
                    Trade Trade1 = Trades[Trades.Count - 1];


                    ////move lose
                    //if (Type == TradeType.Buy && L(1) > Trade1._Point1 && L(0)>L(1))
                    //{
                    //    Trade1.StopL = L(1);
                    //}
                    if (C(0) > Trade1.StopW)
                    {
                        Trade1.StopL = L(1); 
                    }

                    //out
                    if ((C(0) < Trade1.StopL   || i == (df.Rows.Count - 1)) && Type != TradeType.None)
                    {
                        Trade1.CloseTrade(date(0), C(0), i); 
                    }

                }



                //in
                if (V(0) > 5*AVG("volume",1,30) && C(0) >O(0) && C(0)>UB(0)    && Type == TradeType.None)
                {
                    Trade lTrade = new Trade(TradeType.Buy, date(0), C(0), i);
                    if (i == (df.Rows.Count - 1))
                        lTrade = new Trade(TradeType.Buy, date(0), C(0), date(0), C(0), i, i);

                    Trades.Add(lTrade);
                   
                    try
                    {
                        lTrade.StopW = C(0) + C(0) - L(0);

                        lTrade.StopL = L(0);
                    }
                    catch (Exception ex)
                    {
                        string xx = i.ToString();
                    }

                }
            }
        }
    }
}
