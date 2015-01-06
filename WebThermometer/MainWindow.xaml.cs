using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WebThermometer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int             _defaultRefreshIntervalSec = 300;
        private const int             _waitForNetworkAfterSleepResumeDelayMillisec = 5000;
        private       App             _app;
        private       DispatcherTimer _timer;      

        public IViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _app = (App)App.Current;
            SystemEvents.PowerModeChanged += OnPowerChange;
            ViewModel = new MeteoWawPlViewModel();
            DataContext = ViewModel;

            int refreshTime = _defaultRefreshIntervalSec;
            try { refreshTime = Int32.Parse(ConfigurationManager.AppSettings["RefreshTime"]); } catch { }
            
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(OnDispatcherTimerTick);
            _timer.Interval = new TimeSpan(0, 0, refreshTime);
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

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {                     
            ViewModel.Refresh();
        }

        private void OnCtxMenuExitClick(object sender, RoutedEventArgs e)
        {
            _app.Shutdown();
        }

        private void OnTrayIconMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Activate();
        }

        private void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }

    
}
