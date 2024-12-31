using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My;
namespace StockTest
{
    public class TradeManager
    { 
        public Dictionary<string, TradeManager1> Trades { get; set; } 

        public void DoTrade(string TradeNo,DateTime Date,TradeType Type, double Point)
        {
            if (!Trades.ContainsKey(TradeNo))
                Trades.Add(TradeNo, new TradeManager1()); 
            Trades[TradeNo].TradeAction(Type, Date, Point);
        }

        public double[] GetNonClosePrice(string StockNo,double Price)
        {
            return Trades[StockNo].GetNonClosePrice(Price);
        }

        public double[] GetClosePrice(string StockNo )
        {
            return Trades[StockNo].GetClosePrice();
        }

        public double GetClosePriceALL()
        {
            return Trades.Sum(x => x.Value.GetClosePrice()[0]);
        }

        public double GetNonALL(DateTime date)
        {
            double Gain = 0;
            foreach (string stockno in Trades.Keys)
            {
                double currentPrice = (double)(MyDatas.StockData.StockData[stockno].Select($"Date >= #{date.ToString("yyyy-MM-dd HH:mm:ss")}#").FirstOrDefault()?["Close"] ?? 0);
                double[] ResultNonClose = GetNonClosePrice(stockno, currentPrice);
                Gain+= ResultNonClose[0];
            }
            return Gain;
        }

        public double GetNonClosePriceALL( double Price)
        {
            return Trades.Sum(x => x.Value.GetNonClosePrice(Price)[0]);
        }

    }

    public class TradeSignal
    {
        public TradeType Type { get; set; }
        public DateTime Date { get; set; }
        public double Point { get; set; }
    }
    public class TradeManager1
    {
        public List<TradeSignal> Signals { get; set; }
        public List<Trade> TradeNow { get; set; }
        public List<Trade> TradeHis { get; set; }
        public int TradeNo = 0;
        public void TradeAction(TradeType Type,DateTime TradeDate , double Point)
        {
            TradeNo++;
            Signals.Add(new TradeSignal() { Type = Type, Date = TradeDate, Point = Point });
            if (TradeNow.Count == 0)
            {
                int TradeNo = TradeNow.Count + 1;
                Trade Trade1 = new Trade(Type, TradeDate, Point, TradeNo);
                TradeNow.Add(Trade1);
            }
            else
            {
                if (TradeNow[TradeNow.Count - 1]._Type == Type)
                {
                    int TradeNo = TradeNow.Count + 1;
                    Trade Trade1 = new Trade(Type, TradeDate, Point, TradeNo);
                    TradeNow.Add(Trade1);
                }
                else
                {
                    TradeNow[TradeNow.Count - 1].CloseTrade(TradeDate, Point, TradeNo);
                    Trade TradeH = TradeNow[TradeNow.Count - 1];
                    TradeHis.Add(TradeH);
                    TradeNow.Remove(TradeH);
                }
            }
        }
        public double[] GetNonClosePrice(double Price )
        {
            if (TradeNow.Count == 0)
                return new double[] { 0,0};
            else
            {
                if (TradeNow[TradeNow.Count - 1]._Type == TradeType.Sell)
                {
                    return new double[] { TradeNow.Sum(x => x._Point1 - Price - x._Point2 * 0.006), (double)TradeNow.Count };
                }
                else
                {
                    return new double[] { TradeNow.Sum(x => Price - x._Point1 - x._Point2 * 0.006), (double)TradeNow.Count };
                }
            } 
        }
        public double[] GetClosePrice()
        {
            if (TradeHis.Count == 0)
                return new double[] { 0, 0 };
            else
            {
                double TradeGain = 0;
                double TradeCount = 0;
                foreach (Trade item in TradeHis)
                {
                    TradeCount++;
                    if (item._Type == TradeType.Sell)
                    {
                        TradeGain+= item._Point1 - item._Point2 - item._Point2 * 0.006;
                    }
                    else
                    {
                        TradeGain+= item._Point2 - item._Point1 - item._Point2 * 0.006;
                    }
                }
                return new double[] { TradeGain, TradeCount };
            }
        }
    }

}
