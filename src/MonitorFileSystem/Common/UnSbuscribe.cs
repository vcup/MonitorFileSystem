namespace MonitorFileSystem.Common;

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
