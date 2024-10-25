using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{

    public class IndexClass
    {
        public string _name = "";
        private string _desc = "";
        public string _para = "";
        private IndexClass(string name)
        {
            _name = name;
            if (name == "BBANDS")
            {
                _desc = "布林通道 參數 20,2.1,2.1 ";
                _para = "20,2.1,2.1";
            }
            if (name == "SMA")
            {
                _desc = "移動平均 參數 20  ";
                _para = "20";
            }
        }
        public static readonly IndexClass BBANDS = new IndexClass("BBANDS");
        public static readonly IndexClass SMA = new IndexClass("SMA");
        public static readonly IndexClass MAX = new IndexClass("MAX");
    }

     
 
}
