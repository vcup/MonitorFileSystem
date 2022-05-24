using MonitorFileSystem.Common;
using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public interface IOperate : IObserver<WatchingEventInfo>, IInitializable
{
    Guid Guid { get; }

    string Description { get; set; }

    void Initialization(Guid guid);

    void Process(WatchingEventInfo info);

    Task ProcessAsync(WatchingEventInfo info);
}