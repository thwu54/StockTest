using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class KD_1 : ConditionBase
    {     
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public int Index=0;
         

        public int SetIndex(int _Index)
        {
            Index = _Index;
            _Index = _Index + 1;
            return _Index;
        }
        public KD_1()
        {
            Type =  ConditionType.In;
            TradeType = TradeType.Buy; 
        }



        public bool CheckCondition(List<int> GAList, Signal GA )//(double value, Signal GA, TradeType Type)
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.K(0) > da.D(0))
                    return true;
                else
                    return false;
            else
                return true;
        }

        public double Checkvalue(List<int> GAList , Signal GA)
        {
            return 0;
        }
    }

    class KD_2 : ConditionBase
    {
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            _Index = _Index + 1;
            return _Index;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public KD_2()
        {
            Type = ConditionType.In;
            TradeType = TradeType.Sell;
        }
        public bool CheckCondition(List<int> GAList, Signal GA )
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.K(0) < da.D(0))
                    return true;
                else
                    return false;
            else
                return true;
        }

        public double Checkvalue(List<int> GAList , Signal GA)
        {
            return 0;
        }
    }

    class KD_3 : ConditionBase
    {
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index+1;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public KD_3()
        {
            Type = ConditionType.In;
            TradeType = TradeType.Buy;
        }
        public bool CheckCondition(List<int> GAList, Signal GA )
        {
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.K(1) < value)
                    return true;
                else
                    return false;
            else
                return true;
        }

        public double Checkvalue(List<int> GAList , Signal GA)
        {
            return 0;
        }
    }

    class KD_4 : ConditionBase
    {
        public int Index = 0;
        public int SetIndex(int _Index)
        {
            Index = _Index;
            return Index + 1;
        }
        public ConditionType Type { get; }
        public TradeType TradeType { get; }
        public KD_4()
        {
            Type = ConditionType.In;
            TradeType = TradeType.Sell;
        }
        public bool CheckCondition(List<int> GAList, Signal GA )
        { 
            double value = GAList[Index];
            GASignal da = (GASignal)GA;
            if (value > 50)
                if (da.K(1) > value)
                    return true;
                else
                    return false;
            else
                return true;
        }

        public double Checkvalue(List<int> GAList , Signal GA)
        {
            return 0;
        }
    }
}
