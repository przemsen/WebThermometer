WebThermometer
==============

![Screen](/screenshot.png?raw=true)

This is WPF desktop application written in C# which serves as a client for web based weather services. Current implementation uses Polish [https://www.meteo.waw.pl](http://www.meteo.waw.pl) web site as data source along with optional Airly [Airly.org](https://airly.org/map/pl/) CAQI index. You can set up your Airly parameters in `settings.json` file. API key obtained after registering at [developer.airly.org](https://developer.airly.org) is required.

- Application uses Hardcodet.NotifyIcon.WPF to display its tray icon.
- The view tries to be decoupled from the specific data so that it should be possible to easily fork the source code and implement other data sources
- MeteoWawPlWithAilryViewModel.cs -- implementation of the view-model specific for the data source.
- MeteoWawPlWithAirlyDataService.cs -- implementation of the data retrieval used by view-model.
- .NET 7.0



