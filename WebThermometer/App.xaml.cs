using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace WebThermometer;

public partial class App : Application
{
    public static readonly Settings Settings;

    static App()
    {
        var settingsFileContents = File.ReadAllText("settings.json");

        var options = new JsonSerializerOptions();
        options.ReadCommentHandling = JsonCommentHandling.Skip;
        options.Converters.Add(new DataSourcesNullableEnumConverter());

        Settings = JsonSerializer.Deserialize<Settings>(settingsFileContents, options);
        if (Settings.AirlyInstallationId is not null)
        {
            Settings.DataSource ??= DataSources.Both;
        }
        else
        {
            Settings.DataSource = DataSources.WwwMeteo;
        }
    }

    void AppStartup(object sender, StartupEventArgs args)
    {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GCSettings.LatencyMode = GCLatencyMode.Batch;
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
    }

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            System.IO.File.WriteAllText("error.txt", ex.ToString());
        }
    }
}

//-------------------------------------------------------------------------

public class Settings
{
    public string AirlyApiKey { get; set; }
    public int? AirlyInstallationId { get; set; }
    public DataSources? DataSource { get; set; }
};

public enum DataSources
{
    WwwMeteo, Airly, Both
}

public class DataSourcesNullableEnumConverter : JsonConverter<DataSources?>
{
    public override DataSources? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null or JsonTokenType.None)
        {
            return null;
        }
        else
        {
            var value = reader.GetString();
            DataSources result;
            if (Enum.TryParse<DataSources>(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, DataSources? value, JsonSerializerOptions options)
    {

    }
}
