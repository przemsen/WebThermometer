WebThermometer
==============

![Screen](/screenshot.png?raw=true)

This is WPF desktop application written in C# which serves as a client for web based weather services. Current implementation uses Polish [https://www.meteo.waw.pl](http://www.meteo.waw.pl) web site as data source.

- Application uses Hardcodet.NotifyIcon.WPF to display its tray icon.
- The view tries to be decoupled from the specific data so that it should be possible to easily fork the source code and implement other data sources
- MeteoWawPlViewModel.cs -- implementation of the view-model specific for the specific data source.
- MeteoWawPlDataService.cs -- implementation of the data retrieval used by view-model.
- .NET 6.0



