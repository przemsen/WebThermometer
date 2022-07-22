using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebThermometer;

public class MeteoWawPlDataService : IDataService
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = new TimeSpan(0, 0, 5)
    };

    private static readonly Regex _tempRegex = new (@"<strong id=""PARAM_TA"">(.+?)</strong>", RegexOptions.Compiled);
    private static readonly Regex _humidRegex = new (@"<strong id=""PARAM_RH"">(.+?)</strong>", RegexOptions.Compiled);
    private static readonly Regex _sensedTempRegex = new (@"<strong id=""PARAM_WCH"">(.+?)</strong>", RegexOptions.Compiled);
    private static readonly Regex _pressRegex = new (@"<strong id=""PARAM_PN"">(.+?)</strong>", RegexOptions.Compiled);
    private static readonly Regex _windRegex = new (@"<strong id=""PARAM_0_WV"">(.+?)</strong>", RegexOptions.Compiled);
    private static readonly Regex _timeRegex = new (@"<strong id=""PARAM_LDATE"">(.+?)</strong>", RegexOptions.Compiled);

    private const string _url = "https://meteo.waw.pl";
    private const string _ioErr = "Błąd połączenia";
    private const string _parseErr = "Błąd treści";

    private string _htmlSrc;
    private bool _isInValidState;

    private static HttpRequestMessage GetHttpRequestMessage() => new()
    {
        RequestUri = new Uri(_url),
        Method = HttpMethod.Get,
        Headers = { { "Accept", "*/*" }, { "Host", "www.meteo.waw.pl" } }
    };

    public async Task Refresh()
    {
        try
        {
            var response = await _httpClient.SendAsync(GetHttpRequestMessage());
            _htmlSrc = await response.Content.ReadAsStringAsync();
            _isInValidState = true;
        }
        catch
        {
            _isInValidState = false;
        }
    }

    public (string textValue, double? numberValue) GetValue1()
    {
        const string degreesString = " °C";

        var textValue = ParseTargetValueImpl(_tempRegex, appendText: null );
        double? numberValue = null;

        if (double.TryParse(textValue, out double parsedNumber))
        {
            numberValue = parsedNumber;
        }

        return ($"{textValue}{degreesString}", numberValue);
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

        return $"{match.Groups[1].Value.Trim()}{appendText}";
    }
}
