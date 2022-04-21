namespace MonitorFileSystem.Monitor;

public interface IWatcher : IObservable<WatchingEventInfo>
{
    string Name { get; }
    string MonitorPath { get; }
    string Filter { get; }

    bool Monitoring { get; }
    bool MonitorSubDirectory { get; set; }

    WatchingEvent WatchingEvent { get; set; }
}
