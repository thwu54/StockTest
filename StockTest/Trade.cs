using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace  My
{
    public enum TradeType
    {
        Buy = 1,
        Sell = 2,
        None = 0
    }
    public enum Perioda
    { 
        W, 
        D, 
        H, 
        H4
    }
     
    public class Trade
    {
        public static DataTable Trades2DataTable(List<Trade> Trades)
        {
            DataTable dataTable = new DataTable();

            using (var reader = ObjectReader.Create(Trades))
            {
                dataTable.Load(reader);
            }
            return dataTable;
        }
        public TradeType _Type =TradeType.None;
        public DateTime _Date1 = DateTime.Now;
        public DateTime _Date2 = DateTime.Now;
        public double _Point1 = 0;
        public double _Point2 = 0;
        public double GainRate = 0;
        public double Gain = 0;
        public bool IsClose = false;
        public int _TradeNo = 0;
        public int _TradeNo2 = 0;
        public double StopL = 100000;
        public double StopW = 0;
        public bool IsStart = false;
        public bool IsEnd = false;
        public int TradeCount = 0;
        public Trade( )
        {
         
        }

        public Trade(TradeType Type, DateTime Date,double Point,int TradeNo)
        {
            _Type = Type;
            _Date1 = Date;
            _Point1 = Point;
            _TradeNo = TradeNo;
        }

        public Trade(TradeType Type, DateTime Date, double Point, int TradeNo,int StopLose)
        {
            _Type = Type;
            _Date1 = Date;
            _Point1 = Point;
            _TradeNo = TradeNo;
            if (Type == TradeType.Buy)
                StopL = Point - StopLose;
            else
                StopL = Point + StopLose;
        }

        public Trade(TradeType Type, DateTime Date, double Point, DateTime Date2, double Point2, int TradeNo, int TradeNo2)
        {
            if(Type== TradeType.Buy)
            {
                string ss = "";
            }
            _Type = Type;
            _Date1 = Date;
            _Point1 = Point;
            _Date2 = Date2;
            _Point2 = Point2;
            _TradeNo = TradeNo;
            _TradeNo2 = TradeNo2;
            IsClose = true;
        }
        public void CloseTrade(DateTime Date, double Point,int TradeNo2)
        { 
            _Date2 = Date;
            _Point2 = Point;
            _TradeNo2 = TradeNo2;
            IsClose = true;
        }

        public double profit(double cost) {
            if(_Type== TradeType.Buy)
            {
                this.GainRate = ((_Point2 - _Point1) - cost * _Point2) / _Point1;
                return this.GainRate;
            }
            else
            {
                this.GainRate= ((_Point1 - _Point2) - cost * _Point2) / _Point1;
                return this.GainRate;
            }
        }

        public double profitTX(double cost)
        {
            if (_Type == TradeType.Buy)
            {
                this.GainRate = ((_Point2 - _Point1) - cost);
                return this.GainRate;
            }
            else
            {
                this.GainRate = ((_Point1 - _Point2) - cost);
                return this.GainRate;
            }
        }

    }
}
