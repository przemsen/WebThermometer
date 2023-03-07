using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

    private static readonly NumberFormatInfo _numberFormat = new() { NumberDecimalSeparator = "," };

    [GeneratedRegex(@">(.+?)<sup>&#176;C</sup></dl>", RegexOptions.NonBacktracking)]
    private static partial Regex _tempRegex();

    [GeneratedRegex(@"<dt>temp. odczuwalna</dt><dd>(.+?)<sup>", RegexOptions.NonBacktracking)]
    private static partial Regex _sensedTempRegex();

    [GeneratedRegex(@"</dt><dd>(.+?)<sup>km/h</sup>", RegexOptions.NonBacktracking)]
    private static partial Regex _windRegex();

    [GeneratedRegex(@"<label>czas pomiaru aktualnych warunków: (.+?)</label>")]
    private static partial Regex _timeRegex();

    private const string _meteoHost = "meteo.org.pl";
    private const string _meteoUrl = $"https://{_meteoHost}/warszawa";

    private const string _ioErr = "Błąd połączenia";
    private const string _parseErr = "Błąd treści";

    private string _htmlSrc;
    private bool _isMeteoInValidState;
    private bool _isAirlyInValidState = true;

    private string _airlyValue;
    private string _airlyColor;
    private string _airlyPressure;
    private double _airlyTemp;

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
        try
        {
            var response = await _httpClient.SendAsync(GetMeteoHttpRequestMessage());
            _htmlSrc = await response.Content.ReadAsStringAsync();
            _isMeteoInValidState = true;
        }
        catch
        {
            _isMeteoInValidState = false;
        }

        if (App.Settings is { AirlyApiKey: null } or { AirlyInstallationId: null })
        {
            _isAirlyInValidState = true;
            return;
        }

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
            var jsonRootCurrentIndexesFirstValue = jsonRootCurrentIndexesFirst["value"].GetValue<double>().ToString();
            var jsonRootCurrentIndexesFirstColor = jsonRootCurrentIndexesFirst["color"].GetValue<string>();

            var jsonRootCurrentValues            = jsonRootCurrent["values"].AsArray();
            foreach (var v in jsonRootCurrentValues)
            {
                if (v["name"].GetValue<string>() == "PRESSURE")
                {
                    _airlyPressure = v["value"].GetValue<double>().ToString();
                }
                else if (v["name"].GetValue<string>() == "TEMPERATURE")
                {
                    _airlyTemp = v["value"].GetValue<double>();
                }
            }

            _airlyValue = jsonRootCurrentIndexesFirstValue;
            _airlyColor = jsonRootCurrentIndexesFirstColor;

            _isAirlyInValidState = true;
        }
        catch
        {
            _isAirlyInValidState = false;
        }
    }

    public (string textValue, double? numberValue) GetValue1()
    {
        const string degreesString = " °C";
        const string format = "0.0";

        double? numberValue = null;
        string textValue = null;

        if (_isAirlyInValidState)
        {
            numberValue = _airlyTemp;
            textValue = _airlyTemp.ToString(format, _numberFormat);
        }
        else
        {
            if (_isMeteoInValidState)
            {
                textValue = ParseTargetValueImpl(_tempRegex(), string.Empty);
                if (double.TryParse(textValue, out double result))
                {
                    numberValue = result;
                    textValue = result.ToString(format, _numberFormat);
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
        return ParseTargetValueImpl(_windRegex(), " km/h");
    }

    public string GetValue4() => _isAirlyInValidState switch
    {
        true => $"{_airlyPressure} hPa",
        _ => _parseErr
    };

    public string GetValue5() => _isAirlyInValidState switch
    {
        true => _airlyValue,
        _ => _parseErr
    };

    public string GetValue6() => _isAirlyInValidState switch
    {
        true => _airlyColor,
        _ => _parseErr
    };

    public string GetStatus() =>
        $"Updated: {ParseTargetValueImpl(_timeRegex(), string.Empty)}";

    private string ParseTargetValueImpl(Regex regex, string appendText)
    {
        if (!_isMeteoInValidState)
        {
            return _ioErr;
        }

        var match = regex.Match(_htmlSrc);
        if (!match.Success)
        {
            return _parseErr;
        }

        return match.Groups[1].Value.Trim() + appendText;
    }
}
