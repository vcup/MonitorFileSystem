using MonitorFileSystem.Monitor;
using MonitorFileSystem.Common;
using System.Collections;

namespace MonitorFileSystem.Action;

internal class Chain : IChain
{
    private readonly List<IOperate> _operates;
    private readonly List<IObserver<WatchingEventInfo>> _observers;

    public Chain(string name, string? description, bool? isReadOnly)
    {
        Name = name;
        Description = description;
        IsReadOnly = isReadOnly ?? false;

        _operates = new();
        _observers = new();
    }

    public string Name { get; }

    public string? Description { get; }


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

    public void Process(WatchingEventInfo info)
    {
        foreach (var operate in _operates)
        {
            operate.Process(info);
        }
    }

    public async Task ProcessAsync(WatchingEventInfo info)
    {
        foreach (var operate in _operates)
        {
            await operate.ProcessAsync(info);
        }
    }

    public IDisposable Subscribe(IObserver<WatchingEventInfo> observer)
    {
        if (_observers.Contains(observer))
        {
            _observers.Add(observer);
        }

        return new UnSubscribe<WatchingEventInfo>(_observers, observer);
    }

    public IEnumerator<IOperate> GetEnumerator()
    {
        return _operates.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(IOperate item)
    {
        if (IsReadOnly)
        {
            return;
        }
        _operates.Add(item);
    }

    public void Clear()
    {
        _operates.Clear();
    }

    public bool Contains(IOperate item)
    {
        return _operates.Contains(item);
    }

    public void CopyTo(IOperate[] array, int arrayIndex)
    {
        _operates.CopyTo(array, arrayIndex);
    }

    public bool Remove(IOperate item)
    {
        if (IsReadOnly)
        {
            return false;
        }
        return _operates.Remove(item);
    }

    public int Count => _operates.Count;
    public bool IsReadOnly { get; }
    public int IndexOf(IOperate item)
    {
        return _operates.IndexOf(item);
    }

    public void Insert(int index, IOperate item)
    {
        if (IsReadOnly)
        {
            return;
        }
        _operates.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        if (IsReadOnly)
        {
            return;
        }
        _operates.RemoveAt(index);
    }

    public IOperate this[int index]
    {
        get => _operates[index];
        set => _operates[index] = value;
    }
}
