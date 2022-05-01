using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IOperate : IObserver<WatchingEventInfo>
{
    bool IsInitialized { get; }

    void Initialization(params object[] parameters);
    void Process(WatchingEventInfo info);
    Task ProcessAsync(WatchingEventInfo info);
}
