﻿using System.Collections;

namespace MonitorFileSystem.Monitor;

public class Group : IGroup
{
    private readonly List<IObserver<WatchingEventInfo>> _observers;
    private readonly List<IWatcher > _watchers;

    public Group(string name, string? description)
    {
        Name = name;
        Description = description;
        _observers = new();
        _watchers = new();
    }

    public string Name { get; }

    public string? Description { get; }

    public void Add(IWatcher watcher)
    {
        if (!_watchers.Contains(watcher))
        {
            _watchers.Add(watcher);
            watcher.Subscribe(this);
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
            observer?.OnError(error);
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
