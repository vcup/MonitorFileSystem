using System.IO.Abstractions;
using MonitorFileSystem.Common;

namespace MonitorFileSystem.Monitor;

public class Watcher : InitializableBase, IWatcher
{
    private bool _watcherIsReady;
    private WatchingEvent _event;
    private readonly ILogger<Watcher> _logger;
    private readonly IFileSystemWatcher _watcher;
    private readonly List<IObserver<WatchingEventInfo>> _observers = new();

    public Watcher(IFileSystem fileSystem, ILogger<Watcher> logger)
    {
        _logger = logger;
        Name = string.Empty;
        _watcher = fileSystem.FileSystemWatcher.CreateNew();
        _watcher.Path = "./";
        _watcher.Error += OnError;
    }

    public override void Initialization()
    {
        Initialization(Guid.NewGuid());
    }

    public void Initialization(Guid guid)
    {
        CheckIsNotInitialized();
        Guid = guid;
        _watcher.EnableRaisingEvents = _watcherIsReady;
        IsInitialized = true;
    }

    public Guid Guid { get; protected set; }
    public string Name { get; set; }

    public string MonitorPath
    {
        get => _watcher.Path;
        set => _watcher.Path = value;
    }

    public string Filter
    {
        get => _watcher.Filter;
        set => _watcher.Filter = value;
    }

    public WatchingEvent WatchingEvent
    {
        get => _event;
        set
        {
            if (_event == value)
            {
                return;
            }
            else if (_watcher.EnableRaisingEvents | !((int)value >> 4 > 0 &
                                                     value.HasFlag(WatchingEvent.Created) |
                                                     value.HasFlag(WatchingEvent.Renamed) |
                                                     value.HasFlag(WatchingEvent.Deleted)))
            {
                _watcher.EnableRaisingEvents = false;
            }
            else
            {
                _watcher.EnableRaisingEvents = (_watcherIsReady = true) & IsInitialized;
            }

            _watcher.NotifyFilter = (NotifyFilters)((int)value >> 4);

            #region Setup _watcher Events

            if (value.HasFlag(WatchingEvent.Created))
            {
                _watcher.Created -= OnCreated;
                _watcher.Created += OnCreated;
            }
            else
            {
                _watcher.Created -= OnCreated;
            }

            if (value.HasFlag(WatchingEvent.Deleted))
            {
                _watcher.Deleted -= OnDeleted;
                _watcher.Deleted += OnDeleted;
            }
            else
            {
                _watcher.Deleted -= OnDeleted;
            }

            if (value.HasFlag(WatchingEvent.Renamed))
            {
                _watcher.Renamed -= OnRenamed;
                _watcher.Renamed += OnRenamed;
            }
            else
            {
                _watcher.Renamed -= OnRenamed;
            }

            if ((int)value >> 4 > 0)
            {
                _watcher.Changed -= OnChanged;
                _watcher.Changed += OnChanged;
            }
            else
            {
                _watcher.Changed -= OnChanged;
            }

            #endregion

            _event = value;
        }
    }

    public bool MonitorSubDirectory
    {
        get => _watcher.IncludeSubdirectories;
        set => _watcher.IncludeSubdirectories = value;
    }

    public bool Monitoring
    {
        get => _watcher.EnableRaisingEvents;
        set => _watcher.EnableRaisingEvents = value;
    }

    public IDisposable Subscribe(IObserver<WatchingEventInfo> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }

        return new UnSubscribe<WatchingEventInfo>(_observers, observer);
    }

    protected virtual void NotifyObservers(object sender, FileSystemEventArgs e)
    {
        var info = this.GetWatchedEventInfo(e);
        foreach (var observer in _observers)
        {
            observer.OnNext(info);
        }
    }

    protected virtual void NotifyObservers(object sender, RenamedEventArgs e)
    {
        var info = this.GetWatchedEventInfo(e);
        foreach (var observer in _observers)
        {
            observer.OnNext(info);
        }
    }

    protected virtual void NotifyObservers(object sender, ErrorEventArgs e)
    {
        foreach (var observer in _observers)
        {
            observer.OnError(e.GetException());
        }
    }

    protected virtual void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogDebug("watcher {Id} watched a Changed event, path:\n{Path}",
            GetId(), e.FullPath);
        LogDetail();
        NotifyObservers(sender, e);
    }

    protected virtual void OnCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogDebug("watcher {Id} watched a Created event, path:\n{Path}",
            GetId(), e.FullPath);
        LogDetail();
        NotifyObservers(sender, e);
    }

    protected virtual void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _logger.LogDebug("watcher {Id} watched a Deleted event path:\n{Path}",
            GetId(), e.FullPath);
        LogDetail();
        NotifyObservers(sender, e);
    }

    protected virtual void OnRenamed(object sender, RenamedEventArgs e)
    {
        _logger.LogDebug("watcher {Id} watched a Renamed event\nold: {OldPath}\n{NewPath}",
            GetId(), e.OldFullPath, e.FullPath);
        LogDetail();
        NotifyObservers(sender, e);
    }

    protected virtual void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException(), "watcher {Id} got a error",
            GetId());
        LogDetail();
        NotifyObservers(sender, e);
    }

    protected virtual string GetId()
    {
        return string.IsNullOrEmpty(Name) ? Guid.ToString() : Name;
    }

    protected virtual void LogDetail()
    {
        _logger.LogTrace("Guid:                {Guid}\n" +
                         "monitoring path:     {Path}\n" +
                         "Filter:              {Filter}\n" +
                         "Event:               {Event}\n" +
                         "MonitorSubDirectory: {MonitorSubDirectory}\n",
            Guid.ToString(), MonitorPath, Filter, WatchingEvent, MonitorSubDirectory);
    }
}