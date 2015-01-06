using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThermometer
{
    public class MeteoWawPlDataService : IDataService
    {
        private const String _url = "http://www.meteo.waw.pl";
        private const String _err = "Błąd połączenia";
        private HtmlWeb      _htmlWeb;
        private HtmlDocument _htmlDoc;
        private bool         _isInValidState;

        public void Refresh()
        {
            _isInValidState = true;
            try
            {
                _htmlDoc = _htmlWeb.Load(_url);              
            }
            catch
            {
                _isInValidState = false;
            }
        }
        
        public MeteoWawPlDataService()
        {
            _htmlWeb = new HtmlWeb();
            Refresh();
        }
        
        public String GetValue1()
        {  
            if (!_isInValidState)
            {
                return _err;
            }

            var node = _htmlDoc.GetElementbyId("PARAM_0_TA") ;  
            return (node != null) ? node.InnerText + " °C" : null;
            
        }

        public String GetValue2()
        {
            if (!_isInValidState)
            {
                return _err;
            }

            var node = _htmlDoc.GetElementbyId("PARAM_0_WCH");
            return (node != null) ? node.InnerText + " °C" : null;

        }

        public String GetValue3()
        {
            if (!_isInValidState)
            {
                return _err;
            }

            var node = _htmlDoc.GetElementbyId("PARAM_0_RH");
            return (node != null) ? node.InnerText + " %" : null;
        }

        public String GetValue4()
        {
            if (!_isInValidState)
            {
                return _err;
            }
            
            var node = _htmlDoc.GetElementbyId("PARAM_0_PR");
            return (node != null) ? node.InnerText + " hPa" : null;

        }

        public String GetValue5()
        {
            if (!_isInValidState)
            {
                return _err;
            }
            
            var node = _htmlDoc.GetElementbyId("PARAM_0_WV");
            return (node != null) ? node.InnerText + " m/s" : null;
        }

        public String GetStatus()
        {
            if (!_isInValidState)
            {
                return _err;
            }

            var node = _htmlDoc.GetElementbyId("PARAM_LDATE");
            return (node != null) ? "meteo.waw.pl: " + node.InnerText : null;            
        }


    }
}
