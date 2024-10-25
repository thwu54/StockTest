using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class StopWin_01 : ConditionBase
    {
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index + 1;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public StopWin_01()
        {
            Type = ConditionType.StopWin;
        }

        public bool CheckCondition(List<int> GAList, Signal GA )
        {
            GASignal da = (GASignal)GA;
            double value = GAList[Index];
            if (value < 50 && da.Trade1.StopL != 100000)
                return true;
            else
                return false;
        }

        public double Checkvalue(List<int> GAList,  Signal GA)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (da.Trade1._Type == TradeType.Buy)
            {
                double Win= (da.Trade1._Point1 - da.Trade1.StopL)* value/10;
                return da.Trade1._Point1 + Win; 
            }
            else
            {
                double Win = (da.Trade1.StopL-da.Trade1._Point1 ) * value / 10;
                return da.Trade1._Point1 - Win;
            }
        }
    }
    
}
