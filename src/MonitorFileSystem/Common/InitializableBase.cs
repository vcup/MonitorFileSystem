namespace MonitorFileSystem.Common;

public abstract class InitializableBase : IInitializable
{
    public bool IsInitialized { get; protected set; }
    
    public abstract void Initialization();
    
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