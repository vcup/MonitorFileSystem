namespace MonitorFileSystem.Monitor;

/// <summary>
/// a Collection of IWatcher
/// </summary>
public interface IGroup : IObservable<WatchingEventInfo>, IObserver<WatchingEventInfo>, IEnumerable<IWatcher>
{
    /// <summary>
    /// Guid of this Group
    /// </summary>
    Guid Guid { get; }
    /// <summary>
    /// A name of this Group
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// A description of this Group
    /// </summary>
    string Description { get; set; }

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
