using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class MoveLose_01 : ConditionBase
    {
        public int Index = 0;
        public int Index2 = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            Index2 = _Index + 1;
            Index = Index + 2;
            return Index;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public MoveLose_01() {
            Type = ConditionType.MoveLose;
        }

        public bool CheckCondition(List<int> GAList, Signal GA)
        {
            double value = GAList[Index];
            if (value > 50)
                return true;
            else
            {
                GASignal da = (GASignal)GA;
                value = value * 2;
                int OverBar =(int) value / 5;
                if (da.Trade1._Type== TradeType.Buy)
                {
                    if (da.C(0) > da.Max("high", 1, (int)OverBar))
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (da.C(0) < da.Max("low", 1, (int)OverBar))
                        return true;
                    else
                        return false;
                } 
            }
        }

        public double Checkvalue(List<int> GAList, Signal GA)
        {
            double value = GAList[Index2];
            GASignal da = (GASignal)GA; 
            if (da.Trade1._Type == TradeType.Buy)
            {
                value = value / 5;
                return da.Min("low",0,(int)value);
            }
            else
            {
                value = value / 5;
                return da.Min("high", 0, (int)value);
            }
        }
    }

    class MoveLose_02 : ConditionBase
    {
        public int Index = 0;
        public int Index2 = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            Index2 = _Index + 1;
            return Index + 2;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public MoveLose_02()
        {
            Type = ConditionType.MoveLose;
        }

        public bool CheckCondition(List<int> GAList, Signal GA)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.L(1) > da.Trade1._Point1)
                    return true;
                else
                    return false;
            return true;
        }

        public double Checkvalue(List<int> GAList, Signal GA)
        {
            double value = GAList[Index2];
            GASignal da = (GASignal)GA;
            if (da.Trade1._Type == TradeType.Buy)
            {
                value = value / 10;
                return da.L((int)value);
            }
            else
            {
                value = value / 10;
                return da.H((int)value);
            } 
        }
    }
}
