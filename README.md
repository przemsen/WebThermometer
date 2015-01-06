WebThermometer
==============

This is WPF desktop application written in C# which serves as a client for web based weather services. Current implementation uses Polish [http://www.meteo.waw.pl](http://www.meteo.waw.pl) web site as data source.

- Application uses HtmlAgilityPack library to do the html parsing. 
- Application uses Hardcodet.NotifyIcon.Wpf to display tray icon.
- The design features some of MVV* concepts and tries to adhere to SOLID principles. Main point of interest are:
- MainWindow.xaml.cs -- implementation of view.
- MeteoWawPlViewModel.cs -- implementation of view-model specific for arbitrary data source.
- MeteoWawPlDataService.cs -- implementation of data retrieval used by view-model.




