using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public enum ConditionType
    {
        In = 1,
        Out = 2,
        StopLose = 3,
        StopWin = 4,
        MoveLoseCondition = 5,
        MoveLose = 6,
        Direction = 7
    }
    public class Condition
    {
        public List<int> In = new List<int>();
        public List<int> Out = new List<int>();
        public List<int> StopLose = new List<int>();
        public List<int> StopWin = new List<int>();
        public List<int> MoveLoseCondition = new List<int>();
        public List<int> MoveLoseP = new List<int>();

        public List<ConditionType> ConditionTypes = new List<ConditionType>();
        public List<ConditionBase> ConditionList = new List<ConditionBase>();
        public Condition()
        {
             
        }

        public void add(ConditionType lConditionType)
        {
            ConditionTypes.Add(lConditionType);
        }

        

    }
    //public static class MyPublic
    //{
    //    public static Condition MyCondition = new Condition();
    //}
}
