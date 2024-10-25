using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZedGraph;
namespace My
{
    class MACD : Index
    {
        public string Name { get; }

        public bool Show { get; set; }

        public string PanelNo { get; set; }

        public string[] Column { get; set; }

        public string[] LineType => throw new NotImplementedException();

        public string para { get; set; }

        public MACD()
        {
            this.Name = "MACD";
            this.Show = false;
            this.PanelNo = "3";
            this.Column = new string[] { "macd", "macdsignal", "macdhist" };
            this.para = "9,3,3";
        }
        private DataTable _lTable = new DataTable();
        public double[] GetMaxMin(double _start, double _end)
        {
            int start = (int)_start;
            int end = (int)_end;
            double Max = 0;
            double Min = 1000000;
            foreach (string item in Column)
            { 
                for (int i = start; i <= end; i++)
                {
                    if (_lTable.Rows.Count <= i)
                        break;
                    double y = _lTable.Rows[i][item] is DBNull ? double.NaN : double.Parse(_lTable.Rows[i][item].ToString());
                    if (y > Max)
                    {
                        Max = y;
                    }
                    if (y < Min)
                    {
                        Min = y;
                    }
                }
            }
            return new double[] { Max, Min };
        }
        public void Draw(ref GraphPane myPane, DataTable lTable)
        {
            _lTable = lTable;
            foreach (string item in Column)
            {
                PointPairList lList = new PointPairList();
                foreach (DataRow lRow in lTable.Rows)
                {
                    XDate xDate = new XDate((DateTime.Parse(lRow[0].ToString())).ToOADate());
                    double x = xDate.XLDate;
                    double y = lRow[item] is DBNull ? double.NaN : double.Parse(lRow[item].ToString());
                    lList.Add(x, y);
                }
                if(item== "macdhist")
                {
                    BarItem macdhist = myPane.AddBar(item, lList, Color.Blue); 
                    macdhist.IsY2Axis = true;
                }
                else
                {
                    LineItem curve = myPane.AddCurve(item, lList, Color.Blue, SymbolType.None);
                    curve.IsY2Axis = true;
                }
                
            }

        }

        public void Init(bool Show, string PanelNo)
        {
            this.Show = Show;
            this.PanelNo = PanelNo;
        }
    }
}
