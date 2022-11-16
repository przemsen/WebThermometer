using System;
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

    [GeneratedRegex(@"<strong id=""PARAM_TA"">(.+?)</strong>")]
    private static partial Regex _tempRegex();

    [GeneratedRegex(@"<strong id=""PARAM_WCH"">(.+?)</strong>")]
    private static partial Regex _sensedTempRegex();

    [GeneratedRegex(@"<strong id=""PARAM_PN"">(.+?)</strong>")]
    private static partial Regex _pressRegex();

    [GeneratedRegex(@"<strong id=""PARAM_0_WV"">(.+?)</strong>")]
    private static partial Regex _windRegex();

    [GeneratedRegex(@"<strong id=""PARAM_LDATE"">(.+?)</strong>")]
    private static partial Regex _timeRegex();

    private const string _meteoWawPlUrl = "https://meteo.waw.pl";
    private static readonly string _airlyApiUrl = $"https://airapi.airly.eu/v2/measurements/installation?installationId={App.Settings.AirlyInstallationId}&apikey={App.Settings.AirlyApiKey}";
    private const string _ioErr = "Błąd połączenia";
    private const string _parseErr = "Błąd treści";

    private string _htmlSrc;
    private bool _isMeteoInValidState;
    private bool _isAirlyInValidState = true;

    private string _airlyValue = default;
    private string _airlyColor = default;

    private static HttpRequestMessage GetMeteoWawPlHttpRequestMessage() => new()
    {
        RequestUri = new Uri(_meteoWawPlUrl),
        Method = HttpMethod.Get,
        Headers = { { "Accept", "*/*" }, { "Host", "www.meteo.waw.pl" } }
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
            var response = await _httpClient.SendAsync(GetMeteoWawPlHttpRequestMessage());
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

        var textValue = ParseTargetValueImpl(_tempRegex(), appendText: null );
        double? numberValue = null;

        if (double.TryParse(textValue, out double parsedNumber))
        {
            numberValue = parsedNumber;
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

    public string GetValue4()
    {
        return ParseTargetValueImpl(_pressRegex(), " hPa");
    }

    public string GetValue5()
    {
        if (_isAirlyInValidState)
        {
            return _airlyValue;
        }
        else
        {
            return _parseErr;
        }
    }

    public string GetValue6()
    {
        if (_isAirlyInValidState)
        {
            return _airlyColor;
        }
        else
        {
            return _parseErr;
        }
    }

    public string GetStatus()
    {
        return $"Updated: {ParseTargetValueImpl(_timeRegex(), string.Empty)}";
    }

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

        return $"{match.Groups[1].Value.Trim()}{appendText}";
    }
}
