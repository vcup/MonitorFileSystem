namespace MonitorFileSystem.Monitor;

public sealed class WatchingEventInfo
{
    public IWatcher Watcher = null!;
    public string Path = null!;
    public string? OldPath;
    public WatchingEvent WatchedEvent;
}