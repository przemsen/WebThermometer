using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThermometer
{
    public class MeteoWawPlViewModel : IViewModel, INotifyPropertyChanged
    {
        private IDataService _service;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Label1 { get { return "Temperatura:";   } set { } }
        public string Label2 { get { return "T. odczuwalna:"; } set { } }
        public string Label3 { get { return "Wilgotność:";    } set { } }
        public string Label4 { get { return "Ciśnienie:";     } set { } }
        public string Label5 { get { return "Wiatr:";         } set { } }

        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public string Status { get; set; }

        public MeteoWawPlViewModel()
        {
            _service = new MeteoWawPlDataService();
        }

        public async Task Refresh()
        {
            await _service.Refresh();
            Value1 = _service.GetValue1(); OnPropertyChanged("Value1");
            Value2 = _service.GetValue2(); OnPropertyChanged("Value2");
            Value3 = _service.GetValue3(); OnPropertyChanged("Value3");
            Value4 = _service.GetValue4(); OnPropertyChanged("Value4");
            Value5 = _service.GetValue5(); OnPropertyChanged("Value5");
            Status = _service.GetStatus(); OnPropertyChanged("Status");
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
