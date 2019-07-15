using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace WebThermometer
{
    public class MeteoWawPlDataService : IDataService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = new TimeSpan(0, 0, 5),
        };
        private static readonly Regex _tempRegex = new Regex(@"<strong id=""PARAM_TA"">(.+?)</strong>", RegexOptions.Compiled);
        private static readonly Regex _humidRegex = new Regex(@"<strong id=""PARAM_RH"">(.+?)</strong>", RegexOptions.Compiled);
        private static readonly Regex _sensedTempRegex = new Regex(@"<strong id=""PARAM_WCH"">(.+?)</strong>", RegexOptions.Compiled);
        private static readonly Regex _pressRegex = new Regex(@"<strong id=""PARAM_PN"">(.+?)</strong>", RegexOptions.Compiled);
        private static readonly Regex _windRegex = new Regex(@"<strong id=""PARAM_0_WV"">(.+?)</strong>", RegexOptions.Compiled);
        private static readonly Regex _timeRegex = new Regex(@"<strong id=""PARAM_LDATE"">(.+?)</strong>", RegexOptions.Compiled);

        private readonly Uri _uri = new Uri(_url);
        private const string _url = "http://www.meteo.waw.pl";
        private const string _ioErr = "Błąd połączenia";
        private const string _parseErr = "Błąd treści";

        private string _htmlSrc;
        private bool _isInValidState;

        public async Task Refresh()
        {
            try
            {
                _htmlSrc = await _httpClient.GetStringAsync(_uri);
                _isInValidState = true;
            }
            catch
            {
                _isInValidState = false;
            }
        }

        public string GetValue1()
        {
            return ParseTargetValueImpl(_tempRegex, " °C");
        }

        public string GetValue2()
        {
            return ParseTargetValueImpl(_sensedTempRegex, " °C");
        }

        public string GetValue3()
        {
            return ParseTargetValueImpl(_humidRegex, " %");
        }

        public string GetValue4()
        {
            return ParseTargetValueImpl(_pressRegex, " hPa");
        }

        public string GetValue5()
        {
            return ParseTargetValueImpl(_windRegex, " m/s");
        }

        public string GetStatus()
        {
            return $"Updated: {ParseTargetValueImpl(_timeRegex, string.Empty)}";
        }

        private string ParseTargetValueImpl(Regex regex, string appendText)
        {
            if (!_isInValidState)
            {
                return _ioErr;
            }

            var match = regex.Match(_htmlSrc);
            if (!match.Success)
            {
                return _parseErr;
            }

            return $"{match.Groups[1].Value.Trim()} {appendText}";
        }
    }
}
