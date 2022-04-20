using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IOperate : IObserver<WatchingEventInfo>
{
    void Process(WatchingEventInfo info);
    Task ProcessAsync(WatchingEventInfo info);
}
