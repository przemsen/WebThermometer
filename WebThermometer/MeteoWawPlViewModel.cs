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
        
        public String Label1 { get { return "Temperatura:";   } set { } }
        public String Label2 { get { return "T. odczuwalna:"; } set { } }
        public String Label3 { get { return "Wilgotność:";    } set { } }
        public String Label4 { get { return "Ciśnienie:";     } set { } }
        public String Label5 { get { return "Wiatr:";         } set { } }

        public String Value1 { get; set; }
        public String Value2 { get; set; }
        public String Value3 { get; set; }
        public String Value4 { get; set; }
        public String Value5 { get; set; }
        public String Status { get; set; }

        public MeteoWawPlViewModel()
        {
            _service = new MeteoWawPlDataService();
        }

        public void Refresh()
        {
            _service.Refresh();            
            Value1 = _service.GetValue1(); OnPropertyChanged("Value1");
            Value2 = _service.GetValue2(); OnPropertyChanged("Value2");
            Value3 = _service.GetValue3(); OnPropertyChanged("Value3");
            Value4 = _service.GetValue4(); OnPropertyChanged("Value4");
            Value5 = _service.GetValue5(); OnPropertyChanged("Value5");
            Status = _service.GetStatus(); OnPropertyChanged("Status");
        }

        protected void OnPropertyChanged(String propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
       
    }
}
