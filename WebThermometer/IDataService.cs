using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThermometer
{
    public interface IDataService
    {
        String GetValue1();
        String GetValue2();
        String GetValue3();
        String GetValue4();
        String GetValue5();
        String GetStatus();
        void Refresh();
    }
}
