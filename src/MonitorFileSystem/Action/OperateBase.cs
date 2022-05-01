using MonitorFileSystem.Monitor;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public abstract class OperateBase : IOperate
{
    protected readonly ILogger<IOperate> Logger;
    protected readonly IFileSystem FileSystem;

    protected OperateBase(ILogger<IOperate> logger) : this(logger, new FileSystem())
    {
    }

    // only used when testing
    protected OperateBase(ILogger<IOperate> logger, IFileSystem fileSystem)
    {
        Logger = logger;
        FileSystem = fileSystem;
    }

    public virtual void OnCompleted()
    {
    }

    public virtual void OnError(Exception error)
    {
        Logger.LogError(error.Message);
    }

    public virtual void OnNext(WatchingEventInfo value)
    {
        Process(value);
    }

    public abstract void Process(WatchingEventInfo info);

    public virtual Task ProcessAsync(WatchingEventInfo info)
    {
        return Task.Run(() => Process(info));
    }
}
