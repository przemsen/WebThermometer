WebThermometer
==============

This is WPF desktop application written in C# which serves as a client for web based weather services. Current implementation uses Polish [http://www.meteo.waw.pl](http://www.meteo.waw.pl) web site as data source.

- Application uses Hardcodet.NotifyIcon.Wpf to display tray icon.
- The design features some of MVV* concepts and tries to adhere to SOLID principles. Main point of interest are:
- MainWindow.xaml.cs -- implementation of the view.
- MeteoWawPlViewModel.cs -- implementation of the view-model specific for the arbitrary data source.
- MeteoWawPlDataService.cs -- implementation of the data retrieval used by view-model.
- Compiled for .NET 4.8



