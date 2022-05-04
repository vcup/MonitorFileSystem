namespace MonitorFileSystem.Monitor;

public interface IMonitorManager : ICollection<IWatcher>, ICollection<IGroup>
{
    IWatcher? FindWatcher(string name);
    IGroup? FindGroup(string name);

    void ClearUp();

    IEnumerable<IWatcher> Watchers { get; }
    IEnumerable<IGroup> Groups { get; }
}