using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThermometer
{
    public interface IDataService
    {
        string GetValue1();
        string GetValue2();
        string GetValue3();
        string GetValue4();
        string GetValue5();
        string GetStatus();
        Task Refresh();
    }
}
