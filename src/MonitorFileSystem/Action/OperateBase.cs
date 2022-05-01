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

    public bool IsInitialized { get; private set; }
    public virtual void Initialization(params object[] parameters)
    {
        IsInitialized = true;
    }

    public virtual void Process(WatchingEventInfo info)
    {
        CheckIsInitialized();
    }

    public virtual Task ProcessAsync(WatchingEventInfo info)
    {
        CheckIsInitialized();
        return Task.Run(() => Process(info));
    }

    protected void CheckIsInitialized()
    {
        if (!IsInitialized)
        {
            throw new ArgumentException("Instance is not Initialized");
        }
    }
}
