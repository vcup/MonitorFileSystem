using System.Collections;
using MonitorFileSystem.Common;

namespace MonitorFileSystem.Monitor;

public class Group : IGroup
{
    private readonly List<IObserver<WatchingEventInfo>> _observers;
    private readonly List<IWatcher> _watchers;
    private readonly Dictionary<IWatcher, IDisposable> _unsubscribes;

    public Group(Guid guid) : this(guid, guid.ToString(), String.Empty)
    {
    }

    public Group(string name) : this(Guid.NewGuid(), name, String.Empty)
    {
    }

    public Group(string name, string description) : this (Guid.NewGuid(), name, description)
    {
    }
    
    public Group(Guid guid, string name, string description)
    {
        Guid = guid;
        Name = name;
        Description = description;
        _observers = new();
        _watchers = new();
        _unsubscribes = new();
    }

    public Guid Guid { get; }
    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// <p>
    /// Add a watcher to Collection, and also Subscribe this instance for Watcher
    /// Observers subscribed to this instance will be notified when this Watcher Watched Event
    /// </p>
    /// see also <see cref="IGroup.Add"/>
    /// <param name="watcher">this instance will subscript this Watcher</param>
    /// </summary>
    public void Add(IWatcher watcher)
    {
        if (!_watchers.Contains(watcher))
        {
            _watchers.Add(watcher);
            _unsubscribes.Add(watcher, watcher.Subscribe(this));
        }
    }

    /// <summary>
    /// <p>
    /// remove the watcher for this Group, and also Unsubscribe this instance for Watcher
    /// </p>
    /// see also <see cref="IGroup.Remove"/>
    /// </summary>
    /// <param name="watcher">this instance will unsubscribe and remove for collection</param>
    public void Remove(IWatcher watcher)
    {
        if (_watchers.Contains(watcher))
        {
            _watchers.Remove(watcher);
            _unsubscribes[watcher].Dispose();
        }
    }

    public void OnCompleted()
    {
        foreach (var observer in _observers)
        {
            observer.OnCompleted();
        }
    }

    public void OnError(Exception error)
    {
        foreach (var observer in _observers)
        {
            observer.OnError(error);
        }
    }

    public void OnNext(WatchingEventInfo value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }

    public IDisposable Subscribe(IObserver<WatchingEventInfo> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
        return new UnSubscribe<WatchingEventInfo>(_observers, observer);
    }
    public IEnumerator<IWatcher> GetEnumerator()
    {
        return _watchers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
