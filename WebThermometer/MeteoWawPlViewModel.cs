using System.ComponentModel;
using System.Threading.Tasks;

namespace WebThermometer;

public class MeteoWawPlViewModel : IViewModel, INotifyPropertyChanged
{
    private readonly IDataService _service;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Label1 => "Temperatura:";
    public string Label2 => "T. odczuwalna:";
    public string Label3 => "Wilgotność:";
    public string Label4 => "Ciśnienie:";
    public string Label5 => "Wiatr:";

    public string Value1 { get; set; }
    public string Value2 { get; set; }
    public string Value3 { get; set; }
    public string Value4 { get; set; }
    public string Value5 { get; set; }
    public string Status { get; set; }

    public double? ParsedValue1 { get; set; }

    public MeteoWawPlViewModel()
    {
        _service = new MeteoWawPlDataService();
    }

    public async Task Refresh()
    {
        await _service.Refresh();
        (Value1, ParsedValue1) = _service.GetValue1(); OnPropertyChanged(nameof(Value1)); OnPropertyChanged(nameof(ParsedValue1));
        Value2 = _service.GetValue2(); OnPropertyChanged(nameof(Value2));
        Value3 = _service.GetValue3(); OnPropertyChanged(nameof(Value3));
        Value4 = _service.GetValue4(); OnPropertyChanged(nameof(Value4));
        Value5 = _service.GetValue5(); OnPropertyChanged(nameof(Value5));
        Status = _service.GetStatus(); OnPropertyChanged(nameof(Status));
    }

    protected void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
