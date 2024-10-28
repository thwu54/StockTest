using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace My
{
    public class ConditionALL
    {
        public static List<string> GetConditionList()
        {
            List<string> ConditionList = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            try
            {
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes()
                        .Where(t => typeof(ConditionBase).IsAssignableFrom(t) && t.IsClass);
                    bool IsCreate = false;
                    // 建立類別實例並加入清單
                    foreach (var type in types)
                    {
                        var instance = Activator.CreateInstance(type) as ConditionBase;
                        if (instance != null)
                        {
                            ConditionList.Add( instance.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string xx = ex.Message;

            }
            return ConditionList;
        }
        public List<ConditionBase> ConditionList = new List<ConditionBase>();
        public int index = 0;
        public ConditionALL()
        {
            //List<ConditionBase> instances = new List<ConditionBase>();

            // 取得所有載入的組件
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            try
            {
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes()
                        .Where(t => typeof(ConditionBase).IsAssignableFrom(t) && t.IsClass);
                    bool IsCreate = false;
                    // 建立類別實例並加入清單
                    foreach (var type in types)
                    {
                        var instance = Activator.CreateInstance(type) as ConditionBase;
                        if (instance != null)
                        {
                            index = instance.SetIndex(index);
                            ConditionList.Add(instance);
                            IsCreate = true;
                        }
                    }
                    if (IsCreate)
                        break;
                }
            }
            catch (Exception ex)
            {
                string xx = ex.Message;
               
            }
            // 逐一檢查每個組件中的類別
            
            string ss = "";
            //KD_1 KD_1 = new KD_1(); index = KD_1.SetIndex(index); ConditionList.Add(KD_1);
            //KD_2 KD_2 = new KD_2(); index = KD_2.SetIndex(index); ConditionList.Add(KD_2);
            //KD_3 KD_3 = new KD_3(); index = KD_3.SetIndex(index); ConditionList.Add(KD_3);
            //KD_4 KD_4 = new KD_4(); index = KD_4.SetIndex(index); ConditionList.Add(KD_4);
            //Macd_01 Macd_01 = new Macd_01(); index = Macd_01.SetIndex(index); ConditionList.Add(Macd_01);
            //Macd_02 Macd_02 = new Macd_02(); index = Macd_02.SetIndex(index); ConditionList.Add(Macd_02);
            //RSI_01 RSI_01 = new RSI_01(); index = RSI_01.SetIndex(index); ConditionList.Add(RSI_01);
            //RSI_02 RSI_02 = new RSI_02(); index = RSI_02.SetIndex(index); ConditionList.Add(RSI_02);
            //StopLose_01 StopLose_01 = new StopLose_01(); index = StopLose_01.SetIndex(index); ConditionList.Add(StopLose_01);
            //StopWin_01 StopWin_01 = new StopWin_01(); index = StopWin_01.SetIndex(index); ConditionList.Add(StopWin_01);
            //MoveLose_01 MoveLose_01 = new MoveLose_01(); index = MoveLose_01.SetIndex(index); ConditionList.Add(MoveLose_01);
            //MoveLose_02 MoveLose_02 = new MoveLose_02(); index = MoveLose_02.SetIndex(index); ConditionList.Add(MoveLose_02);
        }
    }

    public static class MyPublic
    {
        public static ConditionALL MyCondition = new ConditionALL();
    }
}
