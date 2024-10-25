using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace My
{
    public interface Index
    {
        string Name { get; }
        bool Show { get; }
        string PanelNo { get; }
        string[] Column { get; }
        string[] LineType { get; }
        string para { get; }
        void Init(bool Show, string PanelNo);
        
        void Draw(ref GraphPane myPane, DataTable lTable);

        double[] GetMaxMin(double _start, double _end);
    }
}
