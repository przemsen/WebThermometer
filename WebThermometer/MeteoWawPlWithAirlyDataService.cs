using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

namespace WebThermometer;

public partial class MeteoWawPlWithAirlyDataService : IDataService
{
    private static readonly HttpClientHandler _httpClientHandler = new()
    {
        UseCookies = false,
        AutomaticDecompression = System.Net.DecompressionMethods.All
    };

    private static readonly HttpClient _httpClient = new(_httpClientHandler)
    {
        Timeout = new TimeSpan(0, 0, 5),
    };

    private static readonly NumberFormatInfo _numberFormatInfo = new() { NumberDecimalSeparator = "," };
    private const string _numberFormat = "0.0";

    [GeneratedRegex(@"<strong id=""PARAM_TA"">(.+?)</strong>", RegexOptions.NonBacktracking)]
    private static partial Regex _tempRegex();

    [GeneratedRegex(@"<strong id=""PARAM_WCH"">(.+?)</strong>", RegexOptions.NonBacktracking)]
    private static partial Regex _sensedTempRegex();

    [GeneratedRegex(@"<strong id=""PARAM_0_WV"">(.+?)</strong>", RegexOptions.NonBacktracking)]
    private static partial Regex _windRegex();

    [GeneratedRegex(@"<strong id=""PARAM_0_PR"">(.+?)</strong>", RegexOptions.NonBacktracking)]
    private static partial Regex _pressureRegex();

    private const string _meteoHost = "meteo.waw.pl";
    private const string _meteoUrl = $"https://{_meteoHost}";

    private const string _ioErr = "Błąd połączenia";
    private const string _parseErr = "Błąd treści";

    private string _htmlSrc;
    private bool? _isMeteoInValidState = false;
    private bool? _isAirlyInValidState = false;

    private string _airlyValue;
    private string _airlyColor;
    private string _airlyPressure;
    private double _airlyTemp;
    private double _airlyHumid;

    private const string _airlyHost = "airapi.airly.eu";
    private static readonly string _airlyApiUrl =
        $"https://{_airlyHost}/v2/measurements/installation?installationId={
            App.Settings.AirlyInstallationId}&apikey={
            App.Settings.AirlyApiKey}";

    private static HttpRequestMessage GetMeteoHttpRequestMessage() => new()
    {
        RequestUri = new Uri(_meteoUrl),
        Method = HttpMethod.Get,
        Headers = { { "Accept", "*/*" }, { "Host", _meteoHost } }
    };

    private static HttpRequestMessage GetAirlyHttpRequestMessage() => new()
    {
        RequestUri = new Uri(_airlyApiUrl),
        Method = HttpMethod.Get,
    };

    public async Task Refresh()
    {
        if (App.Settings.DataSource is DataSources.WwwMeteo or DataSources.Both)
        {
            _isMeteoInValidState = false;
            try
            {
                var response = await _httpClient.SendAsync(GetMeteoHttpRequestMessage());
                _htmlSrc = await response.Content.ReadAsStringAsync();
                _isMeteoInValidState = true;
            }
            catch
            {

            }
        }
        else
        {
            _isMeteoInValidState = null;
        }

        if (App.Settings.DataSource is DataSources.Airly or DataSources.Both)
        {
            _airlyPressure = string.Empty;
            _airlyTemp = default;
            _airlyValue = string.Empty;
            _airlyColor = string.Empty;
            _isAirlyInValidState = false;

            try
            {
                var response = await _httpClient.SendAsync(GetAirlyHttpRequestMessage());
                var data = await response.Content.ReadAsStringAsync();

                var indexOfStandards = data.IndexOf("\"standards\"");

                if (indexOfStandards == -1)
                {
                    _isAirlyInValidState = false;
                    return;
                }

                var dataShortened = data[0..indexOfStandards];
                var dataShortenedTrimmed = dataShortened.TrimEnd();
                var dataShortenedTrimmedNoComma = dataShortenedTrimmed[0..^1];
                var targetDataShortened = $"{dataShortenedTrimmedNoComma}}}}}";

                var jsonRoot                         = JsonNode.Parse(targetDataShortened);
                var jsonRootCurrent                  = jsonRoot["current"];
                var jsonRootCurrentIndexes           = jsonRootCurrent["indexes"].AsArray();
                var jsonRootCurrentIndexesFirst      = jsonRootCurrentIndexes[0];
                var jsonRootCurrentIndexesFirstValue = jsonRootCurrentIndexesFirst["value"].GetValue<double>().ToString(_numberFormat, _numberFormatInfo);
                var jsonRootCurrentIndexesFirstColor = jsonRootCurrentIndexesFirst["color"].GetValue<string>();

                var jsonRootCurrentValues            = jsonRootCurrent["values"].AsArray();
                foreach (var v in jsonRootCurrentValues)
                {
                    var name = v["name"].GetValue<string>();
                    var value = v["value"].GetValue<double>();

                    if (name == "PRESSURE")
                    {
                        _airlyPressure = value.ToString(_numberFormat, _numberFormatInfo);
                    }
                    else if (name == "TEMPERATURE")
                    {
                        _airlyTemp = value;
                        _isAirlyInValidState = true;
                    }
                    else if (name == "HUMIDITY")
                    {
                        _airlyHumid = value;
                    }
                }

                _airlyValue = jsonRootCurrentIndexesFirstValue;
                _airlyColor = jsonRootCurrentIndexesFirstColor;

            }
            catch
            {

            }
        }
        else
        {
            _isAirlyInValidState = null;
        }

    }

    public (string textValue, double? numberValue) GetValue1()
    {
        const string degreesString = " °C";

        double? numberValue = null;
        string textValue;

        if (_isAirlyInValidState is true)
        {
            numberValue = _airlyTemp;
            textValue = _airlyTemp.ToString(_numberFormat, _numberFormatInfo);
        }
        else
        {
            if (_isMeteoInValidState is true)
            {
                textValue = ParseTargetValueImpl(_tempRegex(), string.Empty);
                if (double.TryParse(textValue, out double result))
                {
                    numberValue = result;
                    textValue = result.ToString(_numberFormat, _numberFormatInfo);
                }
                else
                {
                    textValue = _parseErr;
                }
            }
            else
            {
                textValue = _parseErr;
            }

        }

        return ($"{textValue}{degreesString}", numberValue);
    }

    public string GetValue2()
    {
        return ParseTargetValueImpl(_sensedTempRegex(), " °C");
    }

    public string GetValue3()
    {
        return ParseTargetValueImpl(_windRegex(), " m/s");
    }

    public string GetValue4() => _isAirlyInValidState switch
    {
        true => $"{_airlyPressure} hPa",
        _ => _isMeteoInValidState switch
        {
            true => ParseTargetValueImpl(_pressureRegex(), " hPa"),
            _ => _parseErr
        }
    };

    public string GetValue5() => _isAirlyInValidState switch
    {
        true => _airlyValue,
        false => _parseErr,
        null => string.Empty
    };

    public string GetValue6() => _isAirlyInValidState switch
    {
        true => _airlyColor,
        _ => string.Empty
    };

    public string GetValue7() => _isAirlyInValidState switch
    {
        true => _airlyHumid.ToString(_numberFormat, _numberFormatInfo),
        _ => string.Empty
    };

    public string GetStatus() => $"Updated: {DateTime.Now}";

    private string ParseTargetValueImpl(Regex regex, string appendText)
    {
        string ret = _isMeteoInValidState switch
        {
            false => _ioErr,
            null => string.Empty,
            true => null,
        };

        if (ret is not null)
        {
            return ret;
        }

        var match = regex.Match(_htmlSrc);
        if (!match.Success)
        {
            ret = _parseErr;
        }

        if (ret is not null)
        {
            return ret;
        }

        return match.Groups[1].Value.Trim() + appendText;
    }
}
