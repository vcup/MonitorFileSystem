using MonitorFileSystem.Monitor;
using MonitorFileSystem.Common;
using System.Collections;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

internal class Chain : OperateBase, IChain
{
    private readonly List<IOperate> _operates = new();
    private readonly List<IObserver<WatchingEventInfo>> _observers = new();
    private readonly Dictionary<IOperate, IDisposable> _unsubscribes = new();

    public Chain(IFileSystem fileSystem, ILogger<Chain> logger)
        : base(fileSystem, logger)
    {
    }

    // see Initialization(string, string, bool)
    public string Name { get; set; } = null!;

    public override void OnCompleted()
    {
        foreach (var observer in _observers)
        {
            observer.OnCompleted();
        }
    }

    public override void OnError(Exception error)
    {
        foreach (var observer in _observers)
        {
            observer.OnError(error);
        }
    }

    public override void OnNext(WatchingEventInfo value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }

    public override void Initialization(Guid guid)
    {
        CheckIsNotInitialized();
        throw new NotImplementedException("this Operate have not parameterless Initialization");
    }

    public void Initialization(string name, bool isReadOnly)
    {
        Initialization(Guid.NewGuid(), name, isReadOnly);
    }

    public void Initialization(Guid guid, string name, bool isReadOnly)
    {
        CheckIsNotInitialized();
        Name = name;
        IsReadOnly = isReadOnly;
    }

    public override void Process(WatchingEventInfo info)
    {
        CheckIsInitialized();
        foreach (var operate in _operates)
        {
            operate.Process(info);
        }
    }

    public override async Task ProcessAsync(WatchingEventInfo info)
    {
        CheckIsInitialized();
        foreach (var operate in _operates)
        {
            await operate.ProcessAsync(info);
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

    private IDisposable SubscribeAt(int index, IObserver<WatchingEventInfo> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Insert(index, observer);
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
        _unsubscribes.Add(item, Subscribe(item));
    }

    public void Clear()
    {
        // this method can be make async
        foreach (var operate in _operates)
        {
            _unsubscribes[operate].Dispose();
        }

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
        if (!Contains(item) || IsReadOnly)
        {
            return false;
        }

        _unsubscribes[item].Dispose();
        return _operates.Remove(item);
    }

    public int Count => _operates.Count;
    public bool IsReadOnly { get; private set; }

    public int IndexOf(IOperate item)
    {
        return _operates.IndexOf(item);
    }

    /// <summary>
    /// Insert a operate to index, if has not called <see cref="Chain.Subscribe"/>,
    /// the operate will be called index-th when observed event
    /// see also <see cref="IChain.Insert"/>
    /// </summary>
    /// <param name="index">index of item</param>
    /// <param name="item">the operate</param>
    /// <exception cref="ArgumentOutOfRangeException">index is out of range</exception>
    public void Insert(int index, IOperate item)
    {
        if ((uint)index >= (uint)Count)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (IsReadOnly)
        {
            return;
        }

        _unsubscribes.Add(item, _observers.Count == _operates.Count
            ? SubscribeAt(index, item)
            : Subscribe(item)
        );
        _operates.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)Count)
        {
            throw new IndexOutOfRangeException();
        }

        if (IsReadOnly)
        {
            return;
        }

        _unsubscribes[this[index]].Dispose();
        _operates.RemoveAt(index);
    }

    /// <summary>
    /// same of <see cref="Chain.Insert"/>
    /// see also <see cref="IChain.this"/>
    /// </summary>
    /// <param name="index">index of Operate</param>
    /// <exception cref="InvalidOperationException">this Collection is ReadOnly</exception>
    public IOperate this[int index]
    {
        get => _operates[index];
        set
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("this collection is readonly");
            }

            _unsubscribes[_operates[index]].Dispose();
            _operates[index] = value;
            Subscribe(value);
        }
    }
}