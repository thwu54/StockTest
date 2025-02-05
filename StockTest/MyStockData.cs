using My;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZedGraph;

namespace StockTest
{
    public class MyStockData
    {
        public Dictionary<string, DataTable> StockData = new Dictionary<string, DataTable>();
        public MyStockData()
        {
            //計算程式執行時間
            DateTime startTime = DateTime.Now;

            List<string> List = GetStockList();
            foreach (string StockNo in List)
            {
                string filename = StockNo + "_W-SAT_20200101_20250101"  ; 
                if (FunLog.checkFileExist2("database\\fix\\", filename))
                {
                    DataTable Table = MyData.JsonToDataTable("database\\fix\\" + filename);
                    if (!StockData.ContainsKey(StockNo))
                        StockData.Add(StockNo, Table);  
                }
            }
            double ss= DateTime.Now.Subtract(startTime).TotalSeconds;
            string sss = ss.ToString();

        }

        public static List<string> GetStockList()
        {
            string[] files = FunLog.GetFolderFile("database");
            List<string> stockList = new List<string>();
            foreach (string item in files)
            {
                string[] parts = item.Split('_');
                if (parts.Length > 0)
                {
                    string stockno = parts[0];
                    if (!stockList.Contains(stockno))
                        stockList.Add(stockno);
                }
            }
            return stockList;
        }
    }
}
