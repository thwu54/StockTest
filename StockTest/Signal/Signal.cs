using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;
namespace My
{
    public interface Signal
    {
        string Name { get; }
        List<Trade> Trades { get; set; }   
        
        void Init(DataFrame _IndexData,string[] para);
        void Gen();

        
    }

     


}
