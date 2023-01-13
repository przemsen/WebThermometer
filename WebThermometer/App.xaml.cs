using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Text.Json;
using System.Windows;

namespace WebThermometer;

public partial class App : Application
{
    public static readonly Settings Settings;

    static App()
    {
        var settingsFileContents = File.ReadAllText("settings.json");
        Settings = JsonSerializer.Deserialize<Settings>(settingsFileContents);
    }

    void AppStartup(object sender, StartupEventArgs args)
    {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
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

public record Settings(
    string AirlyApiKey,
    int? AirlyInstallationId
);
