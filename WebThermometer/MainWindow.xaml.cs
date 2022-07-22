using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WebThermometer;

public partial class MainWindow : Window
{
    private const int _defaultRefreshIntervalSec = 300;
    private const int _waitForNetworkAfterSleepResumeDelayMillisec = 10000;
    private readonly App _app;
    private readonly DispatcherTimer _timer;

    public IViewModel ViewModel { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        _app = (App)App.Current;
        SystemEvents.PowerModeChanged += OnPowerChange;
        ViewModel = new MeteoWawPlViewModel();
        DataContext = ViewModel;

        _timer = new DispatcherTimer();
        _timer.Tick += OnDispatcherTimerTick;
        _timer.Interval = new TimeSpan(0, 0, _defaultRefreshIntervalSec);
        _timer.Start();

        var theAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        var fvi = FileVersionInfo.GetVersionInfo(theAssembly.Location);
        var inforVersion = theAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        BuildNumberRun.Text = inforVersion;

        ViewModel.Refresh();
    }

    private void OnPowerChange(object s, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Resume)
        {
            Thread.Sleep(_waitForNetworkAfterSleepResumeDelayMillisec);
            _timer.Start();
            ViewModel.Refresh();
        }
        else if (e.Mode == PowerModes.Suspend)
        {
            _timer.Stop();
        }
    }

    private async void OnDispatcherTimerTick(object sender, EventArgs e)
    {
        await ViewModel.Refresh();
    }

    private void OnCtxMenuExitClick(object sender, RoutedEventArgs e)
    {
        _app.Shutdown();
    }

    internal void ShowWindow()
    {
        if (IsVisible)
        {
            Show();
        }

        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
        Topmost = true;
        Topmost = false;
    }

    private void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}

//-------------------------------------------------------------------------

public class TemperatureToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double temperature)
        {
            return temperature switch
            {
                >= 33.0d => Color.FromRgb(0xf3, 0x1f, 0x1f),               // #f31f1f;
                >= 30.0d and < 33.0d => Color.FromRgb(0xea, 0x69, 0x0d),   // #ea690d;
                >= 27.0d and < 30.0d => Color.FromRgb(0xe6, 0xc8, 0x32),   // #e6c832;

                >= 24.0d and < 27.0d => Color.FromRgb(0xe0, 0xe0, 0x11),   // #e0e011;
                >= 21.0d and < 24.0d => Color.FromRgb(0x9d, 0xcc, 0x2f),   // #9dcc2f;
                >= 18.0d and < 21.0d => Color.FromRgb(0x61, 0xc1, 0x22),   // #61c122;

                >= 15.0d and < 18.0d => Color.FromRgb(0x19, 0xa5, 0x0f),   // #19a50f;
                >= 12.0d and < 15.0d => Color.FromRgb(0x2d, 0xb9, 0x6a),   // #2db96a;
                >= 9.0d and < 12.0d => Color.FromRgb(0x36, 0xbe, 0x8e),    // #36be8e;

                >= 6.0d and < 9.0d => Color.FromRgb(0x43, 0xbb, 0x9d),     // #43bb9d;
                >= 3.0d and < 6.0d => Color.FromRgb(0x2d, 0xab, 0x9c),     // #2dab9c;
                >= 0.0d and < 3.0d => Color.FromRgb(0x11, 0x93, 0xb3),     // #1193b3;

                >= -3.0d and < 0d => Color.FromRgb(0x11, 0x67, 0xb3),      // #1167b3;
                >= -6.0d and < -3.0d => Color.FromRgb(0x11, 0x4c, 0xb3),   // #114cb3;
                >= -9.0d and < -6.0d => Color.FromRgb(0x3e, 0x4c, 0xcc),   // #3e4ccc;

                >= -12.0d and < -9.0d => Color.FromRgb(0x38, 0x38, 0xb3),  // #3838b3;
                >= -15.0d and < -12.0d => Color.FromRgb(0x00, 0x00, 0xa8), // #0000a8;
                >= -18.0d and < -15.0d => Color.FromRgb(0x00, 0x00, 0x79), // #000079;

                < -18.0d => Color.FromRgb(0x00, 0x00, 0x36),               // #000036;

                _ => Colors.White
            };
        }

        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
