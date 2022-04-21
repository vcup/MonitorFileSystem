using MonitorFileSystem.Monitor;
using MonitorFileSystem.Common;
using System.Collections;

namespace MonitorFileSystem.Action;

internal class Chain : IChain
{
    private readonly List<IOperate> operates;
    private readonly List<IObserver<WatchingEventInfo>> observers;

    public Chain(string name, string? description)
    {
        Name = name;
        Description = description;

        operates = new();
        observers = new();
    }

    public string Name { get; }

    public string? Description { get; }


    public void OnCompleted()
    {
        foreach (var observer in observers)
        {
            observer.OnCompleted();
        }
    }

    public void OnError(Exception error)
    {
        foreach (var observer in observers)
        {
            observer.OnError(error);
        }
    }

    public void OnNext(WatchingEventInfo value)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(value);
        }
    }

    public void Process(WatchingEventInfo info)
    {
        foreach (var operate in operates)
        {
            operate.Process(info);
        }
    }

    public async Task ProcessAsync(WatchingEventInfo info)
    {
        foreach (var operate in operates)
        {
            await operate.ProcessAsync(info);
        }
    }

    public IDisposable Subscribe(IObserver<WatchingEventInfo> observer)
    {
        if (observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new UnSubscribe<WatchingEventInfo>(observers, observer);
    }

    public IEnumerator<IOperate> GetEnumerator()
    {
        return operates.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
