using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IOperate : IObserver<WatchingEventInfo>
{
    bool IsInitialized { get; }

    void Initialization();
    void Process(WatchingEventInfo info);
    Task ProcessAsync(WatchingEventInfo info);
}
