using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;

namespace My
{
    class GASignal : Signal
    {
        public string Name { get; set; }

        public List<Trade> Trades { get; set; }
        public double DropDown = 0;
        public double LastMoney = 0;
        public double LastGain = 0;
        public double GainRate = 0;
        public double kelly = 0;
        public double Profit = 0;
        public double TradeCount = 0;
        private DataFrame df { get; set; }
        public int CurrentIndex { get => CurrentIndex1; set => CurrentIndex1 = value; }
        public string ProfitType { get => ProfitType1; set => ProfitType1 = value; }
        public Trade Trade1 { get => Trade11; set => Trade11 = value; }
        public int CurrentIndex1 { get => CurrentIndex2; set => CurrentIndex2 = value; }
        public string ProfitType1 { get => ProfitType2; set => ProfitType2 = value; }
        public Trade Trade11 { get => Trade12; set => Trade12 = value; }
        public int CurrentIndex2 { get => currentIndex; set => currentIndex = value; }
        public string ProfitType2 { get => profitType; set => profitType = value; }
        public Trade Trade12 { get => trade1; set => trade1 = value; }

        private int currentIndex = 0;
        public void Gen()
        {

        }

        public GASignal()
        {

        }

        public GASignal(string name)
        {
            this.Name = name;
            if (name == "TX")
                this.ProfitType = "TX";
        }

        public double RSI(int i)
        {
            double rsi = (double)df.Columns["rsi"][CurrentIndex - i];

            return rsi;
        }
        public double K(int i)
        {
            double K = (double)df.Columns["slowk"][CurrentIndex - i];

            return K;
        }

        public double SMA5(int i)
        {
            double sma5 = (double)df.Columns["sma5"][CurrentIndex - i];
            return sma5;
        }

        public double SMA10(int i)
        {
            double sma5 = (double)df.Columns["sma10"][CurrentIndex - i];
            return sma5;
        }

        public double SMA20(int i)
        {
            double sma5 = (double)df.Columns["sma20"][CurrentIndex - i];
            return sma5;
        }

        public double D(int i)
        {
            double D = (double)df.Columns["slowd"][CurrentIndex - i];
            return D;
        }

        public double UB(int i)
        {
            double K = (double)df.Columns["upperband"][CurrentIndex - i];

            return K;
        }

        public double LB(int i)
        {
            double D = (double)df.Columns["lowerband"][CurrentIndex - i];
            return D;
        }

        public double MB(int i)
        {
            double D = (double)df.Columns["middleband"][CurrentIndex - i];
            return D;
        }

        public double O(int i)
        {
            return (double)df.Columns["open"][CurrentIndex - i];
        }

        public double C(int i)
        {
            double C = (double)df.Columns["close"][CurrentIndex - i];
            return C;
        }

        public double L(int i)
        {
            int current = CurrentIndex - i;
            if (current < 0)
                current = 0;
            double L = (double)df.Columns["low"][current];
            return L;
        }

        public double H(int i)
        {
            int current = CurrentIndex - i;
            if (current < 0)
                current = 0;
            double H = (double)df.Columns["high"][current];
            return H;
        }

        public double V(int i)
        {
            int current = CurrentIndex - i;
            if (current < 0)
                current = 0;
            return (double)df.Columns["volume"][current];
        }
        // "macd", "macdsignal", "macdhist"
        public double macd(int i)
        {
            return (double)df.Columns["macd"][CurrentIndex - i];
        }
        public double macdsignal(int i)
        {
            return (double)df.Columns["macdsignal"][CurrentIndex - i];
        }
        public double macdhist(int i)
        {
            return (double)df.Columns["macdhist"][CurrentIndex - i];
        }

        public double AVG(string col, int i, int length)
        {
            DataFrameColumn column = df[col];
            int current = CurrentIndex - i - length;
            if (current < 0)
                current = 0;
            var columnData = Enumerable.Range(current, length)
                   .Select(row => column[row]);
            IEnumerable<double> convertedValues = columnData.Cast<double>();
            double avg = convertedValues.Average();
            return avg;
        }

        public double Min(string col, int i, int length)
        {
            if (length == 0)
                length = 1;
            DataFrameColumn column = df[col];
            int current = CurrentIndex - i - length;
            if (current < 0)
                current = 0;
            var columnData = Enumerable.Range(current, length)
                   .Select(row => column[row]);
            IEnumerable<double> convertedValues = columnData.Cast<double>();
            double avg = convertedValues.Min();
            return avg;
        }

        public double Max(string col, int i, int length)
        {
            try
            {
                if (length == 0)
                    length = 1;
                if (col == "high")
                {
                    string x = "";
                }
                DataFrameColumn column = df[col];
                int current = CurrentIndex - i - length;
                if (current < 0)
                    current = 0;
                var columnData = Enumerable.Range(current, length)
                       .Select(row => column[row]);
                
                IEnumerable<double> convertedValues = columnData.Cast<double>();
                double avg = convertedValues.Max();
                return avg;
            }
            catch (Exception ex)
            {
                string xx = ex.Message;
            }
            return 0;
        }
        //create function
            
        //建立多執行續


        public DateTime date(int i)
        {
            DateTime date = DateTime.Parse(df.Columns["date"][CurrentIndex - i].ToString());
            return date;
        }
        private string profitType = "";
        public ArrayList profit()
        {
            double firstPoint = 0;
            double Profit = 0;
            double WinCount = 0;
            double AllCount = 0;
            double Win = 0;
            double Lose = 0;
            int KeepTimeRange = 0;
            double MaxProfit = 0;
            double MaxDropDown = 0;
            foreach (Trade item in Trades)
            {
                if (item.IsClose)
                {
                    if (firstPoint == 0)
                    {
                        firstPoint = item._Point1;
                    }
                    double profit1 = 0;
                    if (this.ProfitType == "TX")
                        profit1 = item.profitTX(2);
                    else
                        profit1 = item.profit(0.01);
                    Profit += profit1;
                    AllCount++;
                    KeepTimeRange += item._TradeNo2 - item._TradeNo;
                    if (profit1 > 0)
                    {
                        WinCount++;
                        Win += profit1;
                    }
                    else
                    {
                        Lose += profit1;
                    }

                    //
                    if (Profit > MaxProfit)
                        MaxProfit = Profit;
                    if ((MaxProfit - Profit) > MaxDropDown)
                        MaxDropDown = (MaxProfit - Profit);

                }
            }

            double WinRate = WinCount / AllCount;
            if (double.IsNaN(WinRate))
                WinRate = 0;
            double Win1 = Win / WinCount;
            double Lose1 = (Lose / (AllCount - WinCount)) * -1;
            double GainRate = Win*-1 / Lose;
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
            if (Trades.Count < df.Rows.Count/40  || MaxDropDown > 1000)
            {
                Kelly = 0;
                GainRate = 0;
            }
            if (GainRate > 10)
                GainRate = AllCount;


            ArrayList list = new ArrayList();
            list.Add(this.Name);
            list.Add(Trades.Count);
            list.Add(Profit);
            list.Add(WinRate);
            list.Add(WinLoseRate);
            list.Add(Kelly);
            list.Add(KeepTimeRange);
            list.Add(MaxDropDown);
            list.Add(GainRate);
            //
            return list;
        }

        public static double GetLastProfit(string ProfitType, double StartProfit,double LoseRate,double DropDown,List<Trade> TradeLog, int Cost)
        {
            double _PointMoney = 50;
            //DateTime sDate = new DateTime(2019, 3, 1);
            //DateTime eDate = new DateTime(2023, 1, 1);
            DateTime sDate = new DateTime(2019, 3, 1);
            DateTime eDate = new DateTime(2023, 1, 1);
            DataFrame df = MyGraphic.GetDateData( ProfitType,"",  sDate, eDate);
            
            double MaxLose = StartProfit * LoseRate;
            double Profit = StartProfit;
            
            int Count = 0;
            double Gains = 0;
            double RealTimeProfit = Profit ;
            for (int i = 0; i < df.Rows.Count; i++)
            {
                double C = (double)df.Columns["close"][i];
                double H = (double)df.Columns["high"][i];
                double L = (double)df.Columns["low"][i];
                DateTime date = DateTime.Parse(df.Columns["date"][i].ToString()); 
                int CloseTrade = 0;
                double NonProfit = 0;
                foreach (Trade Trade1 in TradeLog)
                {
                    RealTimeProfit = Profit + NonProfit;
                    //開始交易
                    if (!Trade1.IsStart && date >= Trade1._Date1 && date < Trade1._Date2)
                    {
                        Trade1.TradeCount = (int)((RealTimeProfit * LoseRate) / (DropDown * _PointMoney));

                        Trade1.IsStart = true;
                       
                    }
                    //結束交易
                    if (!Trade1.IsEnd && (date >= Trade1._Date2 || i == df.Rows.Count - 1))
                    {
                        if (!Trade1.IsStart)
                        {
                            Trade1.TradeCount = (int)((RealTimeProfit * LoseRate) / (DropDown * _PointMoney));
                            Trade1.IsStart = true;
                        }
                        if (Trade1.GainRate < 0)
                        {
                            string xx = "";
                        }

                        Profit +=  (Trade1.GainRate* _PointMoney) * Trade1.TradeCount;
                        Trade1.IsStart = true;
                        Trade1.IsEnd = true;
                        Count++;
                        Gains += (Trade1.GainRate * _PointMoney) * Trade1.TradeCount;
                    }
                    if (Trade1.IsEnd)
                        CloseTrade++;
                    if (Trade1.IsStart && !Trade1.IsEnd)
                    {
                        double Gain1 = 0;
                        if (Trade1._Type == TradeType.Buy)
                        {
                            Gain1 = ((L - Trade1._Point1) - Cost);
                        }
                        else
                        {
                            Gain1 = ((Trade1._Point1 - H) - Cost);
                        }
                        if (Gain1 < -1000)
                        {
                            string ss = "";
                        }
                        NonProfit += Gain1* _PointMoney * Trade1.TradeCount;
                    }
                }
                if (CloseTrade == TradeLog.Count)
                    break;

            }
            if (DropDown < 1000)
            {
                bool trace = true;
            }
            return Profit;
        }
        /// <summary>
        /// 取得GetDropDown double[] Gain, DropDown, LastMoney
        /// </summary>
        /// <param name="a">第一个整数。</param>
        /// <param name="b">第二个整数。</param>
        /// <returns>double[] Gain, DropDown, LastMoney</returns>
        public static double[] GetDropDown(string ProfitType, List<Trade> TradeLog,  int Cost,DataFrame df)
        {
            
            double Gain = 0;

            double RealTimeGain = 0;
            double MaxGain = 0;
            double DropDown = 0;
            for (int i = 0; i < df.Rows.Count; i++)
            {
                double C = (double)df.Columns["close"][i];
                double H = (double)df.Columns["high"][i];
                double L = (double)df.Columns["low"][i];
                DateTime date = DateTime.Parse(df.Columns["date"][i].ToString());
                double NonCloseGain = 0;
                int CloseTrade = 0;
                foreach (Trade Trade1 in TradeLog)
                {
                    if (!Trade1.IsStart && date >= Trade1._Date1 && date < Trade1._Date2)
                        Trade1.IsStart = true;
                    if (!Trade1.IsEnd && (date >= Trade1._Date2 || i == df.Rows.Count - 1))
                    {
                        Trade1.IsStart = true;
                        Trade1.IsEnd = true;
                        Gain += Trade1.GainRate;
                    }
                    if (Trade1.IsEnd)
                        CloseTrade++;
                    if (Trade1.IsStart && !Trade1.IsEnd)
                    {
                        double Gain1 = 0;
                        if (Trade1._Type == TradeType.Buy)
                        {
                            Gain1 = ((L - Trade1._Point1) - Cost);
                        }
                        else
                        {
                            Gain1 = ((Trade1._Point1 - H) - Cost);
                        }
                        NonCloseGain += Gain1;
                    }
                }

                RealTimeGain = Gain + NonCloseGain;
                if (RealTimeGain > MaxGain)
                    MaxGain = RealTimeGain;
                if ((MaxGain - RealTimeGain) > DropDown)
                    DropDown = MaxGain - RealTimeGain;
                if (CloseTrade == TradeLog.Count)
                    break;
            }
            foreach (Trade Trade1 in TradeLog)
            {
                Trade1.IsStart = false;
                Trade1.IsEnd = false;
            }
            
            double LastMoney=GetLastProfit(ProfitType, 5000000, 0.2, DropDown, TradeLog, 6);
            //
            
            return new double[] { Gain, DropDown, LastMoney };
        }
        
        //GA 60 20 100 60 10
        public void Init(DataFrame df, string[] para)
        {
            Trades = new List<Trade>();
            this.df = df;
            TradeType Type = TradeType.None;
            for (int i = 1; i < df.Rows.Count; i++)
            {
                CurrentIndex = i;
                if (Trades.Count > 0)
                {
                    if (Trades[Trades.Count - 1].IsClose)
                        Type = TradeType.None;
                    else
                        Type = Trades[Trades.Count - 1]._Type;
                    Trade Trade1 = Trades[Trades.Count - 1];

                    //move lose
                    if (Type == TradeType.Buy && L(1) > Trade1._Point1)
                    {
                        Trade1.StopL = L(1);
                    }

                    //out
                    if ((C(0) < Trade1.StopL || i == (df.Rows.Count - 1)) && Type != TradeType.None)
                    {
                        Trade1.CloseTrade(date(0), C(0), i);

                        if (i == (df.Rows.Count - 1))
                        {
                            string ss = "";
                        }
                    }

                }


                //in
                if (K(0) > D(0) && K(1) < 20 && Type == TradeType.None)
                {
                    Trade lTrade = new Trade(TradeType.Buy, date(0), C(0), i);
                    if (i == (df.Rows.Count - 1))
                        lTrade = new Trade(TradeType.Buy, date(0), C(0), date(0), C(0), i, i);

                    Trades.Add(lTrade);
                    DataFrameColumn column = df["close"];
                    try
                    {
                        lTrade.StopL = this.Min("close", 0, 10);
                    }
                    catch (Exception ex)
                    {
                        string xx = i.ToString();
                    }

                }
            }

        }
        public bool CheckConditionOld(int i, double value)
        {

            switch (i)
            {
                case 0:// ｖａｌｕｅ>50 K>D  60 20 100 60 10
                    if (value > 50)
                        if (K(0) > D(0))
                            return true;
                        else
                            return false;
                    else
                        return true;
                case 1:// K(1) <　ｖａｌｕｅ
                    if (value > 0)
                        if (K(1) < value)
                            return true;
                        else
                            return false;
                    return true;
                case 3://move lose
                    if (value > 50)
                        if (L(1) > Trade1._Point1)
                            return true;
                        else
                            return false;
                    return true;
                case 5://  量偵測
                    if (value <= 50)
                        return V(0) > (value / 10) * AVG("volume", 1, 30);
                    else
                        return true;

                case 6://  紅
                    if (value > 50)
                        return C(0) > O(0);
                    else
                        return true;
                case 7://  UB
                    if (value > 50)
                        return C(0) > UB(0);
                    else
                        return true;
                case 8://  macd >0
                    if (value > 50)
                        if (macdhist(0) > 0 && macdhist(1) < 0)
                            return true;
                        else
                            return false;
                    else
                        return true;



            }
            return false;
        }
        public bool CheckCondition(int i, double value)
        {

            switch (i)
            {
                case 0:// ｖａｌｕｅ>50 K>D  60 20 100 60 10
                    if (value > 50)
                        return false;
                    else
                        return true;
                case 14:// ｖａｌｕｅ>50 K>D  60 20 100 60 10
                    if (value > 50)
                        if (K(0) > D(0))
                            return true;
                        else
                            return false;
                    else
                        return true;
                case 1:// ｖａｌｕｅ>50 K>D  60 20 100 60 10
                    if (value > 50)
                        if (K(0) < D(0))
                            return true;
                        else
                            return false;
                    else
                        return true;
                case 6:// K(1) <　ｖａｌｕｅ
                    if (value > 50)
                        if (K(1) < value)
                            return true;
                        else
                            return false;
                    return true;
                case 7:// K(1) <　ｖａｌｕｅ
                    if (value > 50)
                        if (K(1) > value)
                            return true;
                        else
                            return false;
                    return true;
                case 3://move lose
                    if (value > 50)
                        if (L(1) > Trade1._Point1)
                            return true;
                        else
                            return false;
                    return true;
                case 5://  量偵測
                    if (value <= 50)
                        return V(0) > (value / 10) * AVG("volume", 1, 30);
                    else
                        return true;

                case 8://  紅
                    if (value > 50)
                        return C(0) > O(0);
                    else
                        return true;
                case 9://  UB
                    if (value > 50)
                        return C(0) > UB(0);
                    else
                        return true;
                case 10://  macd >0
                    if (value > 50)
                        if (macdhist(0) > 0 && macdhist(1) < 0)
                            return true;
                        else
                            return false;
                    else
                        return true;
                case 11://  macd >0
                    if (value > 50)
                        if (macdhist(0) < 0 && macdhist(1) > 0)
                            return true;
                        else
                            return false;
                    else
                        return true;
                case 12:
                    if (value > 50)
                        return true;
                    else
                        if (RSI(0) > value * 2)
                        return true;
                    else
                        return false;
                case 13:
                    if (value > 50)
                        return true;
                    else
                        if (RSI(0) < value * 2)
                        return true;
                    else
                        return false;



            }
            return false;
        }

        public double CheckValue(int i, double value,TradeType type)
        {
            switch (i)
            {

                case 4:
                    value = value / 10;
                    if(type== TradeType.Buy)
                        return L((int)value);
                    else
                        return H((int)value);
                case 2:
                    value = value / 10;
                    if (type == TradeType.Buy)
                        return this.Min("close", 0, (int)value);
                    else
                        return this.Max("close", 0, (int)value);


            }
            return 0;
        }

        private Trade trade1;
        public double GenProfit(List<int> GA, DataFrame df)
        {
            if (GA == null)
            {
                string ss = "";
                return 1;
            }

            Trades = new List<Trade>();
            this.df = df;
            TradeType Type = TradeType.None;

            for (int i = 1; i < df.Rows.Count; i++)
            {
                CurrentIndex = i;
                if (Trades.Count > 0)
                {
                    if (Trades[Trades.Count - 1].IsClose)
                        Type = TradeType.None;
                    else
                        Type = Trades[Trades.Count - 1]._Type;
                    Trade1 = Trades[Trades.Count - 1];
                    //MoveLose
                    foreach (ConditionBase item in My.MyPublic.MyCondition.ConditionList)
                    {
                        if (item.Type == ConditionType.MoveLose)
                        {
                            if (item.CheckCondition(GA, this ))
                            {
                                Trade1.StopL = item.Checkvalue(GA,   this);
                                break;
                            }                             
                        }
                    }
                    //StopWin
                    foreach (ConditionBase item in My.MyPublic.MyCondition.ConditionList)
                    {
                        if (item.Type == ConditionType.StopWin)
                        {
                            if (item.CheckCondition(GA, this ))
                            {
                                Trade1.StopW = item.Checkvalue(GA,  this);
                                break;
                            }
                        }
                    }

                    //out
                    if ((C(0) > Trade1.StopW || C(0) < Trade1.StopL || i == (df.Rows.Count - 1)) && Type != TradeType.None)
                    {
                        Trade1.CloseTrade(date(0), C(0), i);

                        if (i == (df.Rows.Count - 1))
                        {
                            string ss = "";
                        }
                    }

                }


                //in
                bool BuyIn = true;
                TradeType InType = TradeType.Buy;
                foreach (ConditionBase item in My.MyPublic.MyCondition.ConditionList)
                {
                    if (item.Type == ConditionType.In)
                    {
                        if (item.TradeType == TradeType.Buy)
                        {
                            if (!item.CheckCondition(GA, this))
                            {
                                BuyIn = false;
                                break;
                            }
                        }
                    }
                }

                bool SellIn = true;
                foreach (ConditionBase item in My.MyPublic.MyCondition.ConditionList)
                {
                    if (item.Type == ConditionType.In)
                    {
                        if (item.TradeType == TradeType.Sell)
                        {
                            if (!item.CheckCondition(GA, this))
                            {
                                SellIn = false;
                                break;
                            }
                        } 
                    }
                }

                if (BuyIn && !SellIn)
                    InType = TradeType.Buy;
                else if (SellIn && !BuyIn)
                    InType = TradeType.Sell;
                else
                    InType = TradeType.None;

                    //StopLose
                double StopLose = 0;
                foreach (ConditionBase item in My.MyPublic.MyCondition.ConditionList)
                {
                    if(item.Type == ConditionType.StopLose)
                    {
                        StopLose= item.Checkvalue(GA,   this);
                        break;
                    } 
                }
                 

                if (InType!= TradeType.None && Type != InType )
                {
                    if (Type != TradeType.None)
                        Trades[Trades.Count - 1].CloseTrade(date(0), C(0),i);
                    Trade lTrade = new Trade(InType, date(0), C(0), i,1000);
                    if (i == (df.Rows.Count - 1)  )
                        lTrade = new Trade(InType, date(0), C(0), date(0), C(0), i, i);

                    Trades.Add(lTrade);
                    DataFrameColumn column = df["close"];
                    try
                    {
                        lTrade.StopL = StopLose;
                    }
                    catch (Exception ex)
                    {
                        string xx = i.ToString();
                    }
                }


            }
             
            ArrayList profiles = this.profit();
            DateTime st = DateTime.Now;
            this.Profit = (double)profiles[2];
            double[] Drop= GetDropDown("TX", Trades, 2,df);
            double tt = (DateTime.Now - st).TotalMilliseconds;
            double GainRate = Drop[0] / Drop[1];
            if (GainRate < 0)
                GainRate = 0; 
            if (double.IsNaN(GainRate))
                GainRate = 0;
            this.LastMoney = Drop[2];
            this.LastGain = Drop[0];
            this.DropDown = Drop[1];
            this.TradeCount = Trades.Count;
            this.GainRate = GainRate;
            return GainRate;



        }

        public static double GetProfits(List<Trade> TradeLog)
        {
            List<Trade> sortedTradeLog = TradeLog.OrderBy(Trade => Trade._Date1).ToList();

            return 0;
        }

        public List<Trade> GetTradeLog(DataFrame df, List<int> GA)
        {
            GenProfit(GA, df);
            return Trades;
        }



        public ArrayList GetGAOptimal(DataFrame df, int GACount)
        {
            ArrayList lList = new ArrayList();
            GA ga = new GA(MyPublic.MyCondition.index, GACount, 0.8, 0.3);
            ga.InitPopulation();
            ga.Evaluation(GenProfit, df);
            lList.Add(ga.MaxFetness);
            lList.Add(ga.MaxUnit);
            lList.Add(ga.MaxGeneration);
            lList.Add(GetTradeLog(df, ga.MaxUnit));

            return lList;

        }
    }
}
