namespace MonitorFileSystem.Monitor;

public interface IMonitorManager : ICollection<IWatcher>, ICollection<IGroup>
{
    IWatcher? FindWatcher(string name);
    IGroup? FindGroup(string name);
}