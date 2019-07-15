using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WebThermometer
{
    public partial class MainWindow : Window
    {
        private const int _defaultRefreshIntervalSec = 300;
        private const int _waitForNetworkAfterSleepResumeDelayMillisec = 10000;
        private App _app;
        private DispatcherTimer _timer;

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
}
