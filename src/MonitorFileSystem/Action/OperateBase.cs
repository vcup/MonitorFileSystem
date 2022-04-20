using MonitorFileSystem.Monitor;

namespace MonitorFileSystem.Action;

public abstract class OperateBase : IOperate
{
    protected readonly ILogger<IOperate> _logger;

    public OperateBase(ILogger<IOperate> logger)
    {
        _logger = logger;
    }

    public virtual void OnCompleted()
    {
    }

    public virtual void OnError(Exception error)
    {
        _logger.LogError(error.Message);
    }

    public virtual void OnNext(WatchingEventInfo value)
    {
        Process(value);
    }

    public abstract void Process(WatchingEventInfo info);

    public abstract Task ProcessAsync(WatchingEventInfo info);
}
