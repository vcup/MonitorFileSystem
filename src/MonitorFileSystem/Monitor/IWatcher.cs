using MonitorFileSystem.Common;

namespace MonitorFileSystem.Monitor;

public interface IWatcher : IInitializable, IObservable<WatchingEventInfo>
{
    void Initialization(Guid guid);
    
    Guid Guid { get; }

    string Name { get; set; }
    string MonitorPath { get; set; }
    string Filter { get; set; }

    bool Monitoring { get; set; }
    bool MonitorSubDirectory { get; set; }

    WatchingEvent WatchingEvent { get; set; }
}