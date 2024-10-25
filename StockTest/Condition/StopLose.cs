using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class StopLose_01 : ConditionBase
    {
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index + 1;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public StopLose_01()
        {
            Type = ConditionType.StopLose;
        }

        public bool CheckCondition(List<int> GAList, Signal GA )
        {
            double value = GAList[Index];
            if (value < 50)
                return true;
            else
                return false;
        }
         
        public double Checkvalue(List<int> GAList , Signal GA)
        {
            double value = GAList[Index]*2;
            GASignal da = (GASignal)GA;
            if (da.Trade1 == null)
                return 0;
            if (da.Trade1._Type == TradeType.Buy)
            {
                value = value / 5;
                return da.Min("low", 0, (int)value);
            }
            else
            {
                value = value / 5;
                return da.Max("high", 0, (int)value);
            }
        }
    }
}
