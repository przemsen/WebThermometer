using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace WebThermometer
{
    public class MeteoWawPlDataService : IDataService
    {
        private static readonly Regex _numberRegex = new Regex(@"-?\s*\d+,\d+", RegexOptions.Compiled);
        private const String          _url         = "http://www.meteo.waw.pl";
        private const String          _ioErr       = "Błąd połączenia";
        private const String          _parseErr    = "Błąd treści";
        private HtmlWeb               _htmlWeb;
        private HtmlDocument          _htmlDoc;
        private bool                  _isInValidState;

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
            return ParseTargetValueImpl("msr_short_elm_ta", " °C");
        }

        public String GetValue2()
        {
            return ParseTargetValueImpl("msr_short_elm_wch", " °C");

        }

        public String GetValue3()
        {
            return ParseTargetValueImpl("msr_short_elm_rh", " %");
        }

        public String GetValue4()
        {
            return ParseTargetValueImpl("msr_short_elm_pr", " hPa");
        }

        public String GetValue5()
        {
            return ParseTargetValueImpl("msr_short_elm_wv", " m/s");
        }

        public String GetStatus()
        {
            if (!_isInValidState)
            {
                return _ioErr;
            }

            var node = _htmlDoc.GetElementbyId("PARAM_LDATE");
            return (node != null) ? "meteo.waw.pl: " + node.InnerText : null;
        }

        private string ParseTargetValueImpl(string valueName, string appendText)
        {
            if (!_isInValidState)
            {
                return _ioErr;
            }

            var node = _htmlDoc.GetElementbyId(valueName);

            if (node == null)
            {
                return _parseErr;
            }

            var target = node.ChildNodes.Where(n => n.Name == "span").FirstOrDefault();

            if (target == null)
            {
                return _parseErr;
            }

            var targetText = target.InnerText;

            if (!_numberRegex.IsMatch(targetText))
            {
                return _parseErr;
            }
            else
            {
                return _numberRegex.Match(targetText).Value + appendText;
            }
        }


    }
}
