using System.Threading.Tasks;

namespace WebThermometer;

public interface IDataService
{
    (string textValue, double? numberValue) GetValue1();
    string GetValue2();
    string GetValue3();
    string GetValue4();
    string GetValue5();
    string GetValue6();
    string GetValue7();
    string GetStatus();
    Task Refresh();
}
