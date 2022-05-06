using MonitorFileSystem.Monitor;
using System.IO.Abstractions;

namespace MonitorFileSystem.Action;

public abstract class OperateBase : IOperate
{
    protected readonly ILogger<IOperate> Logger;
    // see Initialization()
    protected readonly IFileSystem FileSystem;

    protected OperateBase(IFileSystem fileSystem, ILogger<OperateBase> logger)
    {
        FileSystem = fileSystem;
        Logger = logger;
    }

    public virtual void OnCompleted()
    {
    }

    public virtual void OnError(Exception error)
    {
        Logger.LogError(exception:error, "An exception occurred when {Name}", GetType().ToString());
    }

    public virtual void OnNext(WatchingEventInfo value)
    {
        Process(value);
    }
    public bool IsInitialized { get; protected set; }

    public virtual void Initialization()
    {
        CheckIsNotInitialized();
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
            throw new InvalidOperationException("Instance is not Initialized");
        }
    }
    
    protected void CheckIsNotInitialized()
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Instance already Initialized");
        }
    }
}
