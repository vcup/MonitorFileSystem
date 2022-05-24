namespace MonitorFileSystem.Common;

public interface IInitializable
{
    bool IsInitialized { get; }

    void Initialization();
}