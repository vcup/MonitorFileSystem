using MonitorFileSystem.Monitor;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;

namespace MonitorFileSystem.Action;

public abstract class OperateBase : IOperate
{
    protected readonly ILogger<IOperate> Logger;
    // see Initialization()
    protected IFileSystem FileSystem = null!;

    protected OperateBase(ILogger<IOperate> logger)
    {
        Logger = logger;
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
        if (parameters.Length != 0 && parameters[0] is IFileSystem)
        {
            FileSystem = (IFileSystem)parameters[0];
        }
        else
        {
            FileSystem = new FileSystem();
        }
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
}
