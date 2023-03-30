using System.Threading.Tasks;

namespace WebThermometer;

public interface IViewModel
{
    string Label1 { get; }
    string Label2 { get; }
    string Label3 { get; }
    string Label4 { get; }
    string Label5 { get; }
    string Label7 { get; }

    string Value1 { get; set; }
    string Value2 { get; set; }
    string Value3 { get; set; }
    string Value4 { get; set; }
    string Value5 { get; set; }
    string Value6 { get; set; }
    string Value7 { get; set; }

    double? ParsedValue1 { get; set; }

    string Status { get; set; }

    Task Refresh();

    void Reset();
}
