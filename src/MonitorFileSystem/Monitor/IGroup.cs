namespace MonitorFileSystem.Monitor;

public interface IGroup : IObservable<WatchingEventInfo>, IObserver<WatchingEventInfo>, IEnumerable<IWatcher>
{
    string Name { get; }
    string? Description { get; }

    void Add(IWatcher watcher);
}
