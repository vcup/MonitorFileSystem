namespace MonitorFileSystem.Monitor;

public interface IWatcher : IObservable<WatchingEventInfo>
{
    Guid Guid { get; }
    
    string Name { get; set; }
    string MonitorPath { get; set; }
    string Filter { get; set; }

    bool Monitoring { get; set; }
    bool MonitorSubDirectory { get; set; }

    WatchingEvent WatchingEvent { get; set; }
}
