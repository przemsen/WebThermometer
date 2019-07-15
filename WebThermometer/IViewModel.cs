using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThermometer
{
    public interface IViewModel
    {
        string Label1 { get; set; }
        string Label2 { get; set; }
        string Label3 { get; set; }
        string Label4 { get; set; }
        string Label5 { get; set; }

        string Value1 { get; set; }
        string Value2 { get; set; }
        string Value3 { get; set; }
        string Value4 { get; set; }
        string Value5 { get; set; }

        string Status { get; set; }

        Task Refresh();
    }
}
