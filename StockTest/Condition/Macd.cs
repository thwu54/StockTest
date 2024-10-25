using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class Macd_01 : ConditionBase
    {
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index + 1;
        }
        public Macd_01( ) {
            Type =  ConditionType.In;
            TradeType = TradeType.Buy;
        }
        public bool CheckCondition(List<int> GAList, Signal GA)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.macdhist(0) > 0 && da.macdhist(1) < 0)
                    return true;
                else
                    return false;
            else
                return true;
        }

        public double Checkvalue(List<int> GAList, Signal GA)
        {
            return 0;
        }
    }
    class Macd_02 : ConditionBase
    {
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index + 1;
        }
        public Macd_02()
        {
            Type = ConditionType.In;
            TradeType = TradeType.Sell;
        }
        public bool CheckCondition(List<int> GAList, Signal GA)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.macdhist(0) < 0 && da.macdhist(1) > 0)
                    return true;
                else
                    return false;
            else
                return true;
        }

        public double Checkvalue(List<int> GAList, Signal GA)
        {
            return 0;
        }
    }
}
