using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    


    public interface ConditionBase
    {

        ConditionType Type { get; }
        TradeType TradeType { get; }
        int SetIndex(int _Index);
        bool CheckCondition(List<int> GAList, Signal GA);
        double Checkvalue(List<int> GAList, Signal GA);

    }



}
