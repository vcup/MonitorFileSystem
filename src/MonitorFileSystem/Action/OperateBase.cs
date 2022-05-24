using MonitorFileSystem.Monitor;
using System.IO.Abstractions;
using MonitorFileSystem.Common;

namespace MonitorFileSystem.Action;

public abstract class OperateBase : InitializableBase, IOperate
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
        Logger.LogError(exception: error, "An exception occurred when {Name}", GetType().ToString());
    }

    public virtual void OnNext(WatchingEventInfo value)
    {
        Process(value);
    }

    public Guid Guid { get; protected set; }
    public string Description { get; set; } = string.Empty;

    public override void Initialization()
    {
        Initialization(Guid.NewGuid());
    }

    public virtual void Initialization(Guid guid)
    {
        CheckIsNotInitialized();
        Guid = guid;
        IsInitialized = true;
    }

    public virtual void Process(WatchingEventInfo info)
    {
        CheckIsInitialized();
    }

    public virtual Task ProcessAsync(WatchingEventInfo info)
    {
        return Task.Run(() => Process(info));
    }
}