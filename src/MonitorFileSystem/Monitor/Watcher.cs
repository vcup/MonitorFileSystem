namespace MonitorFileSystem.Monitor;

internal class Watcher : IWatcher
{
    private WatchingEvent _event;
    private FileSystemWatcher _watcher;
    private List<IObserver<WatchingEventInfo>> _observers = new();

    public Watcher(string name, string path, string filter)
    {
        Name = name;
        _watcher = new FileSystemWatcher(path, filter);
        _event = WatchingEvent.None;

        _watcher.Error += OnError;
    }

    public Watcher(string name, string path, string filter, WatchingEvent @event)
    {
        Name = name;
        _watcher = new FileSystemWatcher(path, filter);
        WatchingEvent = @event;

        _watcher.Error += OnError;
    }

    public string Name { get; }

    public string MonitorPath => _watcher.Path;

    public string Filter => _watcher.Filter;

    public WatchingEvent WatchingEvent {
        get => _event;
        set
        {
            if (_event == value)
            {
                return;
            }
            else if (value is WatchingEvent.None & _watcher.EnableRaisingEvents)
            {
                _watcher.EnableRaisingEvents = false;
            }
            else
            {
                _watcher.EnableRaisingEvents = true;
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

            #endregion

            _event = value;
        }
    }

    public bool MonitorSubDirectory {
        get => _watcher.IncludeSubdirectories;
        set => _watcher.IncludeSubdirectories = value;
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
            observer?.OnError(e.GetException());
        }
    }

    protected virtual void OnChanged(object sender, FileSystemEventArgs e)
    {
        NotifyObservers(sender, e);
    }

    protected virtual void OnCreated(object sender, FileSystemEventArgs e)
    {
        NotifyObservers(sender, e);
    }
    protected virtual void OnDeleted(object sender, FileSystemEventArgs e)
    {
        NotifyObservers(sender, e);
    }

    protected virtual void OnRenamed(object sender, RenamedEventArgs e)
    {
        NotifyObservers(sender, e);
    }

    protected virtual void OnError(object sender, ErrorEventArgs e)
    {
        NotifyObservers(sender, e);
    }


    internal sealed class UnSubscribe<T> : IDisposable
    {
        private readonly IList<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        internal UnSubscribe(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;

        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }
}
