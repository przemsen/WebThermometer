using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThermometer
{
    public interface IViewModel
    {
        String Label1 { get; set; }
        String Label2 { get; set; }
        String Label3 { get; set; }
        String Label4 { get; set; }
        String Label5 { get; set; }

        String Value1 { get; set; }
        String Value2 { get; set; }
        String Value3 { get; set; }
        String Value4 { get; set; }
        String Value5 { get; set; }
        
        String Status { get; set; }
        
        void Refresh();
    }
}
