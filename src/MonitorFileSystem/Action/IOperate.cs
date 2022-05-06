using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IOperate : IObserver<WatchingEventInfo>
{
    Guid Guid { get; }
    string Description { get; set; }
    
    bool IsInitialized { get; }

    void Initialization();
    
    void Initialization(Guid guid);
    void Process(WatchingEventInfo info);
    Task ProcessAsync(WatchingEventInfo info);
}
