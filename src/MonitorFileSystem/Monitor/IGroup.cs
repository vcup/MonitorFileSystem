namespace MonitorFileSystem.Monitor;

/// <summary>
/// a Collection of IWatcher
/// </summary>
public interface IGroup : IObservable<WatchingEventInfo>, IObserver<WatchingEventInfo>, IEnumerable<IWatcher>
{
    /// <summary>
    /// A name of this Group, should be unique
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// A description of this Group
    /// </summary>
    string Description { get; }

    /// <summary>
    /// add a Watcher to this Group
    /// </summary>
    /// <param name="watcher">the watcher what will be added to Collection </param>
    void Add(IWatcher watcher);

    /// <summary>
    /// remove a Watcher for this Group
    /// </summary>
    /// <param name="watcher">will removed for this Group</param>
    void Remove(IWatcher watcher);
}
