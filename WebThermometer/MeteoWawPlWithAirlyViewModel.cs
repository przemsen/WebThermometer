using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WebThermometer;

public class MeteoWawPlWithAirlyViewModel : IViewModel, INotifyPropertyChanged
{
    private readonly IDataService _service;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Label1 => "Temperatura:";
    public string Label2 => "T. odczuwalna:";
    public string Label3 => "Wiatr:";
    public string Label4 => "Ciśnienie:";
    public string Label5 => "Airly CAQI:";
    public string Label7 => "Wilgotność:";

    public string Value1 { get; set; }
    public string Value2 { get; set; }
    public string Value3 { get; set; }
    public string Value4 { get; set; }
    public string Value5 { get; set; }
    public string Value6 { get; set; }
    public string Value7 { get; set; }
    public string Status { get; set; }

    public double? ParsedValue1 { get; set; }

    public MeteoWawPlWithAirlyViewModel()
    {
        _service = new MeteoWawPlWithAirlyDataService();
    }

    public async Task Refresh()
    {
        await _service.Refresh();
        (Value1, ParsedValue1) = _service.GetValue1(); OnPropertyChanged(nameof(Value1)); OnPropertyChanged(nameof(ParsedValue1));
        Value2 = _service.GetValue2(); OnPropertyChanged(nameof(Value2));
        Value3 = _service.GetValue3(); OnPropertyChanged(nameof(Value3));
        Value4 = _service.GetValue4(); OnPropertyChanged(nameof(Value4));
        Value5 = _service.GetValue5(); OnPropertyChanged(nameof(Value5));
        Value6 = _service.GetValue6(); OnPropertyChanged(nameof(Value6));
        Value7 = _service.GetValue7(); OnPropertyChanged(nameof(Value7));
        Status = _service.GetStatus(); OnPropertyChanged(nameof(Status));
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
    }

    public void Reset()
    {
        Value1 = null; OnPropertyChanged(nameof(Value1));
        ParsedValue1 = null; OnPropertyChanged(nameof(ParsedValue1));
        Value2 = null; OnPropertyChanged(nameof(Value2));
        Value3 = null; OnPropertyChanged(nameof(Value3));
        Value4 = null; OnPropertyChanged(nameof(Value4));
        Value5 = null; OnPropertyChanged(nameof(Value5));
        Value6 = null; OnPropertyChanged(nameof(Value6));
        Status = null; OnPropertyChanged(nameof(Status));
    }

    protected void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
