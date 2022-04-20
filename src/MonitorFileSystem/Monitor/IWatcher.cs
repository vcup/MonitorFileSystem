namespace MonitorFileSystem.Monitor;

internal interface IWatcher : IObservable<WatchingEventInfo>
{
    string Name { get; }
    string MonitorPath { get; }
    string Filter { get; }

    bool MonitorSubDirectory { get; set; }

    WatchingEvent WatchingEvent { get; set; }
}
