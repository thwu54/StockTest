using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class RSI_01 : ConditionBase
    {
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index + 1;
        }
        public RSI_01()
        {
            Type = ConditionType.In;
            TradeType = TradeType.Buy;
        }
        public bool CheckCondition(List<int> GAList, Signal GA)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value < 50)
                if (da.RSI(0) > value * 2)
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

    class RSI_02 : ConditionBase
    {
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index++;
        }
        public RSI_02()
        {
            Type = ConditionType.In;
            TradeType = TradeType.Sell;
        }
        public bool CheckCondition(List<int> GAList, Signal GA)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value < 50)
                if (da.RSI(0) < value * 2)
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
